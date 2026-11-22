using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.Core.Network;

/// <summary>
/// Managed port of KUdpEcho (KncWX2Server/Common/Socket/UdpEcho.{h,cpp}).
/// Receives 10-byte packed KUdpToServer ([8B LE UserUID][2B LE Port]) and emits E_UDP_PORT_NOT
/// containing the observed NAT/source UDP port back to the client's TCP session.
/// </summary>
public sealed class KUdpEchoServer : IAsyncDisposable
{
    private readonly InProcessRouter _router;
    private readonly uint _sourcePerformerId;
    private UdpClient? _udp;
    private CancellationTokenSource? _cts;
    private Task? _recvTask;

    public ushort BoundPort { get; private set; }
    public bool IsRunning => _udp != null && _cts is { IsCancellationRequested: false };
    public long EchoCount { get; private set; }

    public KUdpEchoServer(InProcessRouter router, uint sourcePerformerId = PerformerId.PI_GS_SERVER)
    {
        _router = router;
        _sourcePerformerId = sourcePerformerId;
    }

    public ushort Start(ushort port = 9302)
    {
        if (_udp != null) return BoundPort;
        _udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        BoundPort = (ushort)((IPEndPoint)_udp.Client.LocalEndPoint!).Port;
        _cts = new CancellationTokenSource();
        _recvTask = Task.Run(() => ReceiveLoopAsync(_cts.Token));
        KLog.Cout($"KUdpEchoServer listening on UDP :{BoundPort}");
        return BoundPort;
    }

    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _udp != null)
        {
            try
            {
                var result = await _udp.ReceiveAsync(ct);
                if (result.Buffer.Length != 10)
                {
                    KLog.Cwarn($"KUdpEchoServer: invalid KUdpToServer size {result.Buffer.Length} from {result.RemoteEndPoint}");
                    continue;
                }

                long userUid = BinaryPrimitives.ReadInt64LittleEndian(result.Buffer.AsSpan(0, 8));
                ushort declaredPort = BinaryPrimitives.ReadUInt16LittleEndian(result.Buffer.AsSpan(8, 2));
                ushort observedPort = (ushort)result.RemoteEndPoint.Port;

                EchoCount++;
                KLog.Clog($"KUdpEcho UID={userUid} declared={declaredPort} observed={result.RemoteEndPoint.Address}:{observedPort}");

                var ev = new KEvent();
                ev.SetInProcess(PerformerId.PI_GS_PROXY, [userUid, -1], EventId.E_UDP_PORT_NOT, observedPort);
                ev.DestPerformer.AddUID(userUid);
                _router.Send(_sourcePerformerId, 0, ev);
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
                KLog.Cwarn($"KUdpEchoServer recv error: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _udp?.Dispose();
        _udp = null;
        if (_recvTask != null)
        {
            try { await _recvTask; } catch { /* ignore */ }
        }
        _cts?.Dispose();
        _cts = null;
    }
}
