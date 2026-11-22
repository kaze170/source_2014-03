using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Region;

namespace KncWX2Server.Core.Network;

/// <summary>
/// Modern async Socket server replacing Common/Socket/IOCP.{h,cpp} and NetCommon Win32 accept loop.
/// </summary>
public sealed class KIocpServer : IAsyncDisposable
{
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _acceptTask;

    public ushort Port { get; private set; }
    public bool IsListening { get; private set; }

    public bool Start(ushort port, Func<KTcpSession, Task> onClientAccepted, Func<RegionProfile> getRegion)
    {
        if (IsListening) return false;

        try
        {
            Port = port;
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            IsListening = true;

            KLog.Cout($"TCP listener started on 0.0.0.0:{port}");

            _acceptTask = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        var tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token).ConfigureAwait(false);
                        var session = new KTcpSession(tcpClient, getRegion());
                        _ = onClientAccepted(session);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        KLog.Cerr($"TCP Accept error on port {Port}: {ex.Message}");
                    }
                }
            });

            return true;
        }
        catch (Exception ex)
        {
            KLog.Cerr($"Failed to start TCP listener on port {port}: {ex.Message}");
            IsListening = false;
            return false;
        }
    }

    public async Task StopAsync()
    {
        if (!IsListening) return;
        IsListening = false;

        try { _cts?.Cancel(); } catch { }
        try { _listener?.Stop(); } catch { }

        if (_acceptTask != null)
        {
            try { await _acceptTask.ConfigureAwait(false); } catch { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        _cts?.Dispose();
    }
}
