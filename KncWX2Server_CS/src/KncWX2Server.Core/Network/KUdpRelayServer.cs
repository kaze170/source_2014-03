using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Logging;

namespace KncWX2Server.Core.Network;

/// <summary>
/// Managed port of CKTDNUDP (KncWX2Server/Common/Socket/KTDNUDP.{h,cpp}) acting as the
/// Elsword UDP P2P relay server. Handles SP_* system packets and transparently relays
/// opaque XPT_* P2P payloads between registered peer endpoints.
/// </summary>
public sealed class KUdpRelayServer : IAsyncDisposable
{
    public enum SystemPacket : byte
    {
        SP_CONNECT_TEST_REQ = 1,
        SP_CONNECT_TEST_ACK = 2,
        SP_DEFENCE_PORT = 3,
        SP_RETRY_INTERNAL_CONNECT_REQ = 4,
        SP_RETRY_INTERNAL_CONNECT_ACK = 5,
        SP_CONNECT_RELAY_REQ = 6,
        SP_CONNECT_RELAY_ACK = 7,
        SP_RELAY_SET_UID_LIST_REQ = 8,
        SP_RELAY_SET_UID_LIST_ACK = 9,
        SP_RELAY_PRECONFIG = 10,
        SP_RELAY = 11,
        SP_CONNECT_CHECK_REQ = 12,
        SP_CONNECT_CHECK_ACK = 13,
        SP_HEART_BEAT_REQ = 14,
        SP_HEART_BEAT_ACK = 15,
    }

    public sealed class RelayPeer
    {
        public long Uid { get; init; }
        public IPEndPoint EndPoint { get; set; } = new(IPAddress.Loopback, 0);
        public uint RelayUidsStamp { get; set; }
        public long[] RelayUids { get; set; } = [];
        public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;
    }

    private readonly ConcurrentDictionary<long, RelayPeer> _peers = new();
    private UdpClient? _udp;
    private CancellationTokenSource? _cts;
    private Task? _recvTask;

    public ushort BoundPort { get; private set; }
    public bool IsRunning => _udp != null && _cts is { IsCancellationRequested: false };
    public int PeerCount => _peers.Count;
    public long RelayedPackets { get; private set; }
    public long RelayedBytes { get; private set; }

    public IReadOnlyCollection<RelayPeer> GetPeersSnapshot() => _peers.Values.ToArray();

    public ushort Start(ushort port = 9303)
    {
        if (_udp != null) return BoundPort;

        _udp = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        BoundPort = (ushort)((IPEndPoint)_udp.Client.LocalEndPoint!).Port;
        _cts = new CancellationTokenSource();
        _recvTask = Task.Run(() => ReceiveLoopAsync(_cts.Token));
        KLog.Cout($"KUdpRelayServer listening on UDP :{BoundPort}");
        return BoundPort;
    }

    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _udp != null)
        {
            try
            {
                var result = await _udp.ReceiveAsync(ct);
                HandleDatagram(result.Buffer, result.RemoteEndPoint);
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
                KLog.Cwarn($"KUdpRelayServer recv error: {ex.Message}");
            }
        }
    }

    internal void HandleDatagram(ReadOnlySpan<byte> packet, IPEndPoint remoteEp)
    {
        if (packet.Length < 1) return;

        var sp = (SystemPacket)packet[0];
        var body = packet[1..];

        switch (sp)
        {
            case SystemPacket.SP_CONNECT_TEST_REQ:
            {
                var ack = new byte[1 + body.Length];
                ack[0] = (byte)SystemPacket.SP_CONNECT_TEST_ACK;
                body.CopyTo(ack.AsSpan(1));
                SendRaw(remoteEp, ack);
                break;
            }

            case SystemPacket.SP_CONNECT_RELAY_REQ:
            {
                // [8B UID][4B dwStamp][optional 4B dwTimestamp][optional piggybacked XPT payload]
                if (body.Length < 12) return;
                long uid = BinaryPrimitives.ReadInt64LittleEndian(body[..8]);
                uint stamp = BinaryPrimitives.ReadUInt32LittleEndian(body[8..12]);
                bool hasTimestamp = body.Length >= 16;
                uint timestamp = hasTimestamp ? BinaryPrimitives.ReadUInt32LittleEndian(body[12..16]) : 0;
                int headerLen = hasTimestamp ? 16 : 12;

                var peer = _peers.AddOrUpdate(
                    uid,
                    _ => new RelayPeer
                    {
                        Uid = uid,
                        EndPoint = remoteEp,
                        RelayUidsStamp = 0,
                        RelayUids = [],
                        LastSeenUtc = DateTime.UtcNow,
                    },
                    (_, p) =>
                    {
                        p.EndPoint = remoteEp;
                        p.LastSeenUtc = DateTime.UtcNow;
                        if (p.RelayUidsStamp != stamp)
                        {
                            p.RelayUidsStamp = 0;
                            p.RelayUids = [];
                        }
                        return p;
                    });

                var ack = new byte[1 + headerLen];
                ack[0] = (byte)SystemPacket.SP_CONNECT_RELAY_ACK;
                BinaryPrimitives.WriteInt64LittleEndian(ack.AsSpan(1, 8), uid);
                BinaryPrimitives.WriteUInt32LittleEndian(ack.AsSpan(9, 4), peer.RelayUidsStamp);
                if (hasTimestamp)
                    BinaryPrimitives.WriteUInt32LittleEndian(ack.AsSpan(13, 4), timestamp);
                SendRaw(remoteEp, ack);

                if (body.Length > headerLen && peer.RelayUidsStamp != 0 && peer.RelayUids.Length > 0)
                {
                    RelayPayload(peer.RelayUids, body[headerLen..]);
                }
                break;
            }

            case SystemPacket.SP_RELAY_SET_UID_LIST_REQ:
            {
                // Support both 13-byte header (no timestamp) and 17-byte header (with dwTimestamp)
                // Standard SERV_KTDX_OPTIMIZE_NEW_UDP_CONNECTION_STRATEGY uses 17 bytes:
                // [8B UID][4B dwStamp][4B dwTimestamp][1B byNumUIDs][N*8B UIDs][optional XPT payload]
                if (body.Length < 13) return;
                long uid = BinaryPrimitives.ReadInt64LittleEndian(body[..8]);
                uint stamp = BinaryPrimitives.ReadUInt32LittleEndian(body[8..12]);

                int countOffset = body.Length >= 17 && (17 + body[16] * 8) <= body.Length ? 16 : 12;
                bool hasTimestamp = countOffset == 16;
                uint timestamp = hasTimestamp ? BinaryPrimitives.ReadUInt32LittleEndian(body[12..16]) : 0;

                int numUids = body[countOffset];
                int headerLen = countOffset + 1 + numUids * 8;
                if (body.Length < headerLen) return;

                if (!_peers.TryGetValue(uid, out var peer))
                {
                    peer = new RelayPeer { Uid = uid, EndPoint = remoteEp };
                    _peers[uid] = peer;
                }
                else
                {
                    peer.EndPoint = remoteEp;
                    peer.LastSeenUtc = DateTime.UtcNow;
                }

                peer.RelayUidsStamp = stamp;
                if (stamp == 0 || numUids == 0)
                {
                    peer.RelayUids = [];
                }
                else
                {
                    var targets = new long[numUids];
                    for (int i = 0; i < numUids; i++)
                        targets[i] = BinaryPrimitives.ReadInt64LittleEndian(body.Slice(countOffset + 1 + i * 8, 8));
                    peer.RelayUids = targets;
                }

                var ack = new byte[1 + (hasTimestamp ? 16 : 12)];
                ack[0] = (byte)SystemPacket.SP_RELAY_SET_UID_LIST_ACK;
                BinaryPrimitives.WriteInt64LittleEndian(ack.AsSpan(1, 8), uid);
                BinaryPrimitives.WriteUInt32LittleEndian(ack.AsSpan(9, 4), stamp);
                if (hasTimestamp)
                    BinaryPrimitives.WriteUInt32LittleEndian(ack.AsSpan(13, 4), timestamp);
                SendRaw(remoteEp, ack);

                if (body.Length > headerLen && stamp != 0 && peer.RelayUids.Length > 0)
                {
                    RelayPayload(peer.RelayUids, body[headerLen..]);
                }
                break;
            }

            case SystemPacket.SP_RELAY_PRECONFIG:
            {
                if (body.Length <= 8) return;
                long uid = BinaryPrimitives.ReadInt64LittleEndian(body[..8]);
                if (!_peers.TryGetValue(uid, out var peer)) return;
                peer.LastSeenUtc = DateTime.UtcNow;
                if (peer.RelayUidsStamp != 0 && peer.RelayUids.Length > 0)
                    RelayPayload(peer.RelayUids, body[8..]);
                break;
            }

            case SystemPacket.SP_RELAY:
            {
                // [8B UID][1B byNumUIDs][N*8B UIDs][opaque XPT_* payload]
                if (body.Length <= 9) return;
                long uid = BinaryPrimitives.ReadInt64LittleEndian(body[..8]);
                int numUids = body[8];
                int headerLen = 9 + numUids * 8;
                if (numUids <= 0 || body.Length <= headerLen) return;
                if (!_peers.TryGetValue(uid, out var peer)) return;
                peer.LastSeenUtc = DateTime.UtcNow;

                var targets = new long[numUids];
                for (int i = 0; i < numUids; i++)
                    targets[i] = BinaryPrimitives.ReadInt64LittleEndian(body.Slice(9 + i * 8, 8));

                RelayPayload(targets, body[headerLen..]);
                break;
            }

            case SystemPacket.SP_HEART_BEAT_REQ:
            case SystemPacket.SP_CONNECT_CHECK_REQ:
            {
                if (body.Length >= 4)
                {
                    var ackType = sp == SystemPacket.SP_HEART_BEAT_REQ
                        ? SystemPacket.SP_HEART_BEAT_ACK
                        : SystemPacket.SP_CONNECT_CHECK_ACK;
                    var ack = new byte[1 + 8 + 4];
                    ack[0] = (byte)ackType;
                    BinaryPrimitives.WriteInt32LittleEndian(ack.AsSpan(9, 4), BinaryPrimitives.ReadInt32LittleEndian(body[..4]));
                    SendRaw(remoteEp, ack);
                }
                break;
            }
        }
    }

    private void RelayPayload(long[] targets, ReadOnlySpan<byte> payload)
    {
        if (payload.Length == 0) return;
        byte[] copy = payload.ToArray();
        foreach (long targetUid in targets)
        {
            if (_peers.TryGetValue(targetUid, out var targetPeer))
            {
                SendRaw(targetPeer.EndPoint, copy);
                RelayedPackets++;
                RelayedBytes += copy.Length;
            }
        }
    }

    private void SendRaw(IPEndPoint ep, byte[] data)
    {
        try
        {
            _udp?.Send(data, data.Length, ep);
        }
        catch (Exception ex)
        {
            KLog.Clog($"KUdpRelayServer send error to {ep}: {ex.Message}");
        }
    }

    public void RemovePeer(long uid) => _peers.TryRemove(uid, out _);

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
