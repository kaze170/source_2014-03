using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Network;

/// <summary>
/// Optional TCP inter-server event transport bridge for InProcessRouter.
/// Enables multi-process KncWX2Server deployments (Channel/Login/Center/Game/Global across separate processes/hosts)
/// by serializing KEvent frames using KSerializer while keeping single-process calls zero-overhead in-memory.
/// </summary>
public sealed class KTcpEventBridge : IDisposable
{
    private const int MaxFrameBytes = 16 * 1024 * 1024;

    private readonly InProcessRouter _router;
    private readonly RegionProfile _region;
    private readonly ConcurrentDictionary<uint, (string Host, int Port)> _remoteTargets = new();
    private readonly ConcurrentDictionary<string, TcpClient> _clientPool = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _sendLocks = new(StringComparer.OrdinalIgnoreCase);
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _acceptTask;

    public int ListenPort { get; private set; }
    public bool IsListening => _listener != null;
    public long SentEventCount { get; private set; }
    public long ReceivedEventCount { get; private set; }

    public KTcpEventBridge(InProcessRouter router, RegionProfile? region = null)
    {
        _router = router;
        _region = region ?? RegionProfile.Current;
    }

    public int StartListener(int port = 0, IPAddress? bindAddress = null)
    {
        if (_listener != null) return ListenPort;
        _cts = new CancellationTokenSource();
        _listener = new TcpListener(bindAddress ?? IPAddress.Loopback, port);
        _listener.Start();
        ListenPort = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _acceptTask = Task.Run(() => AcceptLoopAsync(_cts.Token));
        return ListenPort;
    }

    public void RegisterRemotePerformer(uint performerId, string host, int port)
    {
        _remoteTargets[performerId] = (host, port);
        _router.RegisterRoute(performerId, ev => SendRemote(performerId, ev));
    }

    public bool SendRemote(uint performerId, KEvent ev)
    {
        if (!_remoteTargets.TryGetValue(performerId, out var target))
            return false;

        try
        {
            return SendRemoteAsync(target.Host, target.Port, ev).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            KLog.Cwarn($"[KTcpEventBridge] Failed sending {ev.EventId} to {target.Host}:{target.Port}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendRemoteAsync(string host, int port, KEvent ev, CancellationToken ct = default)
    {
        byte[] frame = EncodeEventFrame(ev);
        string key = $"{host}:{port}";
        var gate = _sendLocks.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            if (!_clientPool.TryGetValue(key, out var client) || !client.Connected)
            {
                client?.Dispose();
                client = new TcpClient { NoDelay = true };
                await client.ConnectAsync(host, port, ct).ConfigureAwait(false);
                _clientPool[key] = client;
            }

            var stream = client.GetStream();
            await stream.WriteAsync(frame, ct).ConfigureAwait(false);
            await stream.FlushAsync(ct).ConfigureAwait(false);
            SentEventCount++;
            return true;
        }
        catch
        {
            if (_clientPool.TryRemove(key, out var dead))
                dead.Dispose();
            throw;
        }
        finally
        {
            gate.Release();
        }
    }

    private byte[] EncodeEventFrame(KEvent ev)
    {
        var wireEvent = ev.Clone();
        if (wireEvent.Buffer.Length == 0 && wireEvent.InProcessPayload is IKSerializable ser)
        {
            var payloadKs = new KSerializer(_region);
            payloadKs.BeginWriting(wireEvent.Buffer);
            ser.Put(payloadKs, _region);
            payloadKs.EndWriting();
        }

        var buf = new KSerBuffer();
        var ks = new KSerializer(_region);
        ks.BeginWriting(buf);
        wireEvent.Put(ks, _region);
        ks.EndWriting();

        byte[] payload = buf.ToArray();
        byte[] frame = new byte[4 + payload.Length];
        BinaryPrimitives.WriteInt32LittleEndian(frame.AsSpan(0, 4), payload.Length);
        Buffer.BlockCopy(payload, 0, frame, 4, payload.Length);
        return frame;
    }

    private async Task AcceptLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _listener != null)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(ct).ConfigureAwait(false);
                client.NoDelay = true;
                _ = Task.Run(() => HandleClientAsync(client, ct), ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                KLog.Cwarn($"[KTcpEventBridge] Accept error: {ex.Message}");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken ct)
    {
        using (client)
        {
            var stream = client.GetStream();
            byte[] lenBuf = new byte[4];
            while (!ct.IsCancellationRequested)
            {
                if (!await ReadExactAsync(stream, lenBuf, ct).ConfigureAwait(false))
                    break;

                int length = BinaryPrimitives.ReadInt32LittleEndian(lenBuf);
                if (length <= 0 || length > MaxFrameBytes)
                    break;

                byte[] payload = new byte[length];
                if (!await ReadExactAsync(stream, payload, ct).ConfigureAwait(false))
                    break;

                var ev = new KEvent();
                var ks = new KSerializer(_region);
                ks.BeginReading(new KSerBuffer(payload));
                bool ok = ev.Get(ks, _region);
                ks.EndReading();
                if (ok)
                {
                    ReceivedEventCount++;
                    _router.Send(0, 0, ev);
                }
            }
        }
    }

    private static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, CancellationToken ct)
    {
        int read = 0;
        while (read < buffer.Length)
        {
            int n = await stream.ReadAsync(buffer.AsMemory(read, buffer.Length - read), ct).ConfigureAwait(false);
            if (n <= 0) return false;
            read += n;
        }
        return true;
    }

    public void Dispose()
    {
        _cts?.Cancel();
        try { _listener?.Stop(); } catch { }
        _listener = null;
        foreach (var kv in _clientPool)
        {
            try { kv.Value.Dispose(); } catch { }
        }
        _clientPool.Clear();
        foreach (var kv in _sendLocks)
            kv.Value.Dispose();
        _sendLocks.Clear();
        _cts?.Dispose();
        _cts = null;
    }
}
