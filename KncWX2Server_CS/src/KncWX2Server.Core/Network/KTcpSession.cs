using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Security;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Network;

/// <summary>
/// Wire-exact TCP session implementing KSession (Common/Socket/Session.{h,cpp}):
/// - 2-byte Little-Endian total frame length prefix (including the 2 length bytes)
/// - KSecureBuffer DES-CBC + 10-byte HMAC-MD5 ICV
/// - E_ACCEPT_CONNECTION_NOT handshake switching from SPI=0 to per-session random SPI
/// - Heartbeat, ReserveDestroy, sequence number verification, and auth-fail kick threshold.
/// </summary>
public sealed class KTcpSession : IAsyncDisposable
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly KSADatabase _saDb = new();
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly CancellationTokenSource _cts = new();

    private ushort _currentSpi;
    private int _authFailCount;
    private volatile bool _destroyReserved;
    private volatile bool _isConnected = true;

    public uint PacketAuthFailLimit { get; set; } = 100;
    public bool CheckSequenceNum { get; set; } = true;
    public RegionProfile Region { get; set; } = RegionProfile.Current;

    public string RemoteIp { get; }
    public ushort RemotePort { get; }
    public uint RemoteIpNumeric { get; }
    public DateTime ConnectedAt { get; } = DateTime.UtcNow;
    public DateTime LastHeartbeatAt { get; private set; } = DateTime.UtcNow;
    public bool IsConnected => _isConnected && !_destroyReserved && _client.Connected;

    public event Action<KEvent>? OnEventReceived;
    public event Action<KTcpSession>? OnDisconnected;

    public KTcpSession(TcpClient client, RegionProfile region)
    {
        _client = client;
        _client.NoDelay = true;
        _stream = client.GetStream();
        Region = region;

        if (client.Client.RemoteEndPoint is IPEndPoint ep)
        {
            var ipv4 = ep.Address.IsIPv4MappedToIPv6 ? ep.Address.MapToIPv4() : ep.Address;
            RemoteIp = ipv4.ToString();
            RemotePort = (ushort)ep.Port;
            byte[] addrBytes = ipv4.GetAddressBytes();
            if (addrBytes.Length == 4)
                RemoteIpNumeric = BitConverter.ToUInt32(addrBytes, 0);
        }
        else
        {
            RemoteIp = "0.0.0.0";
        }
    }

    public async Task StartAsync()
    {
        // Send E_ACCEPT_CONNECTION_NOT encrypted with default SPI=0, then switch to new random SPI.
        await SendAcceptConnectionAsync();
        _ = Task.Run(ReceiveLoopAsync);
    }

    private async Task SendAcceptConnectionAsync()
    {
        ushort newSpi = _saDb.CreateNewSA(out var newSa);
        var ev = new KEvent
        {
            EventId = EventId.E_ACCEPT_CONNECTION_NOT,
        };
        var ks = new KSerializer(Region);
        ks.BeginWriting(ev.Buffer);
        ks.Put(newSpi);
        newSa.Put(ks, Region);
        ks.EndWriting();

        await SendEventAsync(ev);
        _currentSpi = newSpi;
    }

    public bool SendPacket(uint performerId, long destUid, EventId eventId, IKSerializable packet, RegionProfile? region = null)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = performerId;
        ev.DestPerformer.AddUID(destUid);
        ev.EventId = eventId;

        var r = region ?? Region;
        var ks = new KSerializer(r);
        ks.BeginWriting(ev.Buffer);
        packet.Put(ks, r);
        ks.EndWriting();

        _ = SendEventAsync(ev);
        return true;
    }

    public bool SendID(uint performerId, long destUid, EventId eventId)
    {
        var ev = new KEvent();
        ev.DestPerformer.PerformerId = performerId;
        ev.DestPerformer.AddUID(destUid);
        ev.EventId = eventId;

        var ks = new KSerializer(Region);
        ks.BeginWriting(ev.Buffer);
        ks.Put((sbyte)0);
        ks.EndWriting();

        _ = SendEventAsync(ev);
        return true;
    }

    public async Task SendEventAsync(KEvent ev)
    {
        if (!_isConnected) return;

        var ks = new KSerializer(Region);
        var eventBuf = new KSerBuffer();
        ks.BeginWriting(eventBuf);
        ev.Put(ks, Region);
        ks.EndWriting();

        byte[] frame;
        await _sendLock.WaitAsync(_cts.Token).ConfigureAwait(false);
        try
        {
            if (!_isConnected) return;
            var sec = new KSecureBuffer(_saDb, _currentSpi);
            if (!sec.Create(eventBuf.AsSpan()))
                return;

            ReadOnlySpan<byte> secBytes = sec.WireBytes;
            ushort totalLen = checked((ushort)(secBytes.Length + sizeof(ushort)));
            frame = new byte[totalLen];
            BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
            secBytes.CopyTo(frame.AsSpan(2));

            await _stream.WriteAsync(frame, _cts.Token).ConfigureAwait(false);
            await _stream.FlushAsync(_cts.Token).ConfigureAwait(false);
        }
        catch (Exception)
        {
            CloseInternal();
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public void ReserveDestroy()
    {
        _destroyReserved = true;
        CloseInternal();
    }

    private async Task ReceiveLoopAsync()
    {
        byte[] lenBuf = new byte[2];
        try
        {
            while (!_cts.IsCancellationRequested && _isConnected)
            {
                if (!await ReadExactAsync(lenBuf, _cts.Token).ConfigureAwait(false))
                    break;

                ushort totalLen = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
                if (totalLen < 4 || totalLen > 65000)
                {
                    KLog.Cwarn($"Invalid packet length {totalLen} from {RemoteIp}:{RemotePort}");
                    break;
                }

                byte[] secData = new byte[totalLen - 2];
                if (!await ReadExactAsync(secData, _cts.Token).ConfigureAwait(false))
                    break;

                var sec = new KSecureBuffer(_saDb, _currentSpi, secData);
                if (!sec.TryGetPayload(out byte[] payload, out bool sequenceMismatch, out string failureReason, authentic: true, checkSequenceNum: CheckSequenceNum))
                {
                    if (sequenceMismatch)
                    {
                        OnEventReceived?.Invoke(new KEvent
                        {
                            EventId = EventId.E_CHECK_SEQUENCE_COUNT_NOT,
                            FromType = EventFromType.Client,
                        });
                    }
                    _authFailCount++;
                    KLog.Cwarn($"Packet auth/decrypt failed ({_authFailCount}/{PacketAuthFailLimit}) [{failureReason}] from {RemoteIp}:{RemotePort}");
                    if (PacketAuthFailLimit > 0 && _authFailCount >= PacketAuthFailLimit)
                    {
                        KLog.Cerr($"AuthFailLimit exceeded for {RemoteIp}:{RemotePort}, disconnecting.");
                        break;
                    }
                    continue;
                }

                var kbuff = new KSerBuffer(payload);
                var ev = new KEvent { FromType = EventFromType.Client };
                var ks = new KSerializer(Region);
                ks.BeginReading(kbuff);
                bool parsed = ev.Get(ks, Region);
                ks.EndReading();

                if (!parsed)
                {
                    KLog.Cwarn($"Failed to deserialize KEvent from {RemoteIp}:{RemotePort}");
                    continue;
                }

                if (ev.Buffer.IsCompressed)
                    ev.Buffer.UnCompress();

                switch (ev.EventId)
                {
                    case EventId.E_HEART_BEAT:
                        LastHeartbeatAt = DateTime.UtcNow;
                        break;
                    case EventId.E_RESERVE_DESTROY:
                        ReserveDestroy();
                        return;
                    default:
                        OnEventReceived?.Invoke(ev);
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            KLog.Clog($"TCP session ended ({RemoteIp}:{RemotePort}): {ex.Message}");
        }
        finally
        {
            CloseInternal();
        }
    }

    private async Task<bool> ReadExactAsync(byte[] buffer, CancellationToken ct)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int read = await _stream.ReadAsync(buffer.AsMemory(offset), ct).ConfigureAwait(false);
            if (read == 0) return false;
            offset += read;
        }
        return true;
    }

    private void CloseInternal()
    {
        if (!_isConnected) return;
        _isConnected = false;
        try { _cts.Cancel(); } catch { }
        try { _client.Close(); } catch { }
        OnDisconnected?.Invoke(this);
    }

    public ValueTask DisposeAsync()
    {
        CloseInternal();
        _sendLock.Dispose();
        _cts.Dispose();
        return ValueTask.CompletedTask;
    }
}
