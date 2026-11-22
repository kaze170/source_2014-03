using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using KncWX2Server.Core.Events;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.Core.Serialization;

namespace KncWX2Server.GameServer;

/// <summary>
/// Managed port of KTRServer / KTRUser / KSkTRUser (GameServer/TRServer.{h,cpp}, TRUser.{h,cpp}).
/// Uses raw 2-byte LE length framing (without KSecurityAssociation encryption) to serialize/relay
/// KEvent frames for ETR_* and opaque P2P payloads by UnitUID.
/// </summary>
public sealed class KTRServer : IAsyncDisposable
{
    public sealed class KTRUser : IDisposable
    {
        private readonly KTRServer _server;
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private int _disposed;

        public KTRUserFSM Fsm { get; } = new();
        public long UnitUid { get; private set; } = -1;
        public string RemoteIp { get; }

        public KTRUser(KTRServer server, TcpClient client)
        {
            _server = server;
            _client = client;
            _client.NoDelay = true;
            _stream = client.GetStream();
            RemoteIp = (client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "0.0.0.0";
            Fsm.TryStateTransition(KTRUserFSM.INPUT_CONNECTED, out _);
        }

        public async Task RunAsync(CancellationToken ct)
        {
            byte[] lenBuf = new byte[2];
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    if (!await ReadExactAsync(_stream, lenBuf, ct)) break;
                    ushort packetLen = BinaryPrimitives.ReadUInt16LittleEndian(lenBuf);
                    if (packetLen < 3) break;

                    byte[] body = new byte[packetLen - 2];
                    if (!await ReadExactAsync(_stream, body, ct)) break;

                    var ks = new KSerializer(_server.Region);
                    var rawBuf = new KSerBuffer(body);
                    var ev = new KEvent();
                    ks.BeginReading(rawBuf);
                    bool ok = ev.Get(ks, _server.Region);
                    ks.EndReading();
                    if (!ok) continue;

                    ev.Buffer.UnCompress();
                    await HandleEventAsync(ev, ct);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                KLog.Clog($"KTRUser[{UnitUid}] error: {ex.Message}");
            }
            finally
            {
                _server.RemoveUser(this);
                Dispose();
            }
        }

        private async Task HandleEventAsync(KEvent ev, CancellationToken ct)
        {
            switch (ev.EventId)
            {
                case EventId.E_HEART_BEAT:
                    break;

                case EventId.ETR_REG_UID:
                {
                    var req = ev.GetPayload<KETR_REG_UID>(_server.Region);
                    if (req.UnitUid > 0 && UnitUid < 0)
                    {
                        Fsm.TryStateTransition(KTRUserFSM.INPUT_REG_UID_REQ, out _);
                        UnitUid = req.UnitUid;
                        _server.RegisterUser(this);
                        Fsm.TryStateTransition(KTRUserFSM.INPUT_REG_UID_ACK, out _);
                        KLog.Clog($"KTRServer registered UnitUID={UnitUid}");
                    }
                    break;
                }

                case EventId.ETR_UNIT_DISCONNECT:
                    Fsm.TryStateTransition(KTRUserFSM.INPUT_EXIT, out _);
                    Dispose();
                    break;

                case EventId.ETR_FIELD_UNIT_SYNC_DATA_BROAD_NOT:
                    ev.EventId = EventId.EGS_FIELD_UNIT_SYNC_DATA_BROAD_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_CHANGE_USER_LIST_IN_SECTOR_NOT:
                    ev.EventId = EventId.EGS_CHANGE_USER_LIST_IN_SECTOR_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_CHAT_NOT:
                    ev.EventId = EventId.EGS_CHAT_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_UPDATE_PARTY_NOT:
                    ev.EventId = EventId.EGS_UPDATE_PARTY_UID_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT:
                    ev.EventId = EventId.EGS_CHANGE_EQUIPPED_ITEM_IN_FIELD_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_SUMMON_PET_NOT:
                    ev.EventId = EventId.EGS_SUMMON_PET_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_SUMMON_RIDING_PET_NOT:
                    ev.EventId = EventId.EGS_SUMMON_RIDING_PET_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_UNSUMMON_RIDING_PET_NOT:
                    ev.EventId = EventId.EGS_UNSUMMON_RIDING_PET_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_CHANGE_FIELD_USER_INFO_NOT:
                    ev.EventId = EventId.EGS_CHANGE_USER_INFO_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_PET_ACTION_NOT:
                    ev.EventId = EventId.EGS_PET_ACTION_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_PET_EVOLUTION_NOT:
                    ev.EventId = EventId.EGS_PET_EVOLUTION_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_TOGGLE_INVISIBLE_NOT:
                    ev.EventId = EventId.EGS_TOGGLE_INVISIBLE_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_COUPLE_MAKING_SUCCESS_NOT:
                    ev.EventId = EventId.EGS_COUPLE_MAKING_SUCCESS_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.ETR_RELATIONSHIP_EFFECT_NOT:
                    ev.EventId = EventId.EGS_RELATIONSHIP_EFFECT_NOT;
                    await _server.RelayEventAsync(this, ev, ct);
                    break;

                case EventId.E_CHECK_DDOS_GUARD_REQ:
                {
                    var evAck = new KEvent { EventId = EventId.E_CHECK_DDOS_GUARD_ACK };
                    evAck.DestPerformer.PerformerId = PerformerId.PI_TR_USER;
                    var ks = new KSerializer(_server.Region);
                    ks.BeginWriting(evAck.Buffer);
                    ks.Put((sbyte)0);
                    ks.EndWriting();
                    await SendEventAsync(evAck, ct);
                    break;
                }

                default:
                    await _server.RelayEventAsync(this, ev, ct);
                    break;
            }
        }

        public async Task SendEventAsync(KEvent ev, CancellationToken ct)
        {
            var wire = new KSerBuffer();
            var ks = new KSerializer(_server.Region);
            ks.BeginWriting(wire);
            ev.Put(ks, _server.Region);
            ks.EndWriting();

            ushort totalLen = (ushort)(wire.Length + 2);
            byte[] frame = new byte[totalLen];
            BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(0, 2), totalLen);
            wire.AsSpan().CopyTo(frame.AsSpan(2));

            await _sendLock.WaitAsync(ct);
            try
            {
                await _stream.WriteAsync(frame, ct);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buf, CancellationToken ct)
        {
            int offset = 0;
            while (offset < buf.Length)
            {
                int n = await stream.ReadAsync(buf.AsMemory(offset, buf.Length - offset), ct);
                if (n == 0) return false;
                offset += n;
            }
            return true;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1) return;
            try { _stream.Dispose(); } catch { }
            try { _client.Dispose(); } catch { }
        }
    }

    private readonly ConcurrentDictionary<long, KTRUser> _users = new();
    private TcpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _acceptTask;
    private long _relayedEventCount;

    public int WorkerThreadCount { get; private set; } = 4;
    public void TRWorkerThreadCount(int count) => WorkerThreadCount = Math.Max(1, count);

    public RegionProfile Region { get; set; }
    public ushort BoundPort { get; private set; }
    public int RegisteredUserCount => _users.Count;
    public long RelayedEventCount => Interlocked.Read(ref _relayedEventCount);

    public KTRServer(RegionProfile region)
    {
        Region = region;
    }

    public ushort Start(ushort port)
    {
        if (_listener != null) return BoundPort;
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        BoundPort = (ushort)((IPEndPoint)_listener.LocalEndpoint).Port;
        _cts = new CancellationTokenSource();
        _acceptTask = Task.Run(() => AcceptLoopAsync(_cts.Token));
        KLog.Cout($"[KTRServer] Listening on TCP :{BoundPort}");
        return BoundPort;
    }

    private async Task AcceptLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested && _listener != null)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(ct);
                var user = new KTRUser(this, client);
                _ = Task.Run(() => user.RunAsync(ct), ct);
            }
            catch (OperationCanceledException) { break; }
            catch (ObjectDisposedException) { break; }
            catch (Exception ex)
            {
                KLog.Cwarn($"[KTRServer] Accept error: {ex.Message}");
            }
        }
    }

    internal void RegisterUser(KTRUser user) => _users[user.UnitUid] = user;

    internal void RemoveUser(KTRUser user)
    {
        if (user.UnitUid > 0)
            _users.TryRemove(user.UnitUid, out _);
    }

    public bool TryRelayToTargets(IEnumerable<long> unitUids, EventId eventId, IKSerializable payload)
    {
        bool anySent = false;
        var ev = new KEvent
        {
            EventId = eventId,
            InProcessPayload = payload,
        };
        ev.DestPerformer.PerformerId = PerformerId.PI_TR_USER;
        var ks = new KSerializer(Region);
        ks.BeginWriting(ev.Buffer);
        payload.Put(ks, Region);
        ks.EndWriting();

        foreach (long uid in unitUids)
        {
            if (_users.TryGetValue(uid, out var target))
            {
                _ = target.SendEventAsync(ev, CancellationToken.None);
                Interlocked.Increment(ref _relayedEventCount);
                anySent = true;
            }
        }
        return anySent;
    }

    internal async Task RelayEventAsync(KTRUser sender, KEvent ev, CancellationToken ct)
    {
        IEnumerable<long> targets = ev.DestPerformer.Uids.Count > 0
            ? ev.DestPerformer.Uids
            : _users.Keys.Where(uid => uid != sender.UnitUid);

        foreach (long targetUid in targets)
        {
            if (_users.TryGetValue(targetUid, out var target))
            {
                await target.SendEventAsync(ev, ct);
                Interlocked.Increment(ref _relayedEventCount);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _listener?.Stop();
        _listener = null;
        foreach (var u in _users.Values)
            u.Dispose();
        _users.Clear();
        if (_acceptTask != null)
        {
            try { await _acceptTask; } catch { }
        }
        _cts?.Dispose();
        _cts = null;
    }
}
