using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/Square.{h,cpp}:
/// manages town/trade square rooms (max 60 concurrent users), unit entry/exit,
/// position/sync state caching, and KSquareInfo snapshots.
/// </summary>
public sealed class KCnSquare
{
    public const int SquareMaxSlot = 60;

    public sealed class SquareMember
    {
        public long UnitUid { get; init; }
        public long GsUid { get; init; }
        public string NickName { get; init; } = string.Empty;
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public bool IsOpeningPersonalShop { get; set; }
    }

    private readonly Dictionary<long, SquareMember> _members = new();
    private readonly object _sync = new();

    public long SquareUid { get; init; }
    public sbyte SquareType { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string RelayIp { get; set; } = "127.0.0.1";
    public int RelayPort { get; set; } = 9303;

    public int NumMember
    {
        get { lock (_sync) return _members.Count; }
    }

    public bool IsEmpty => NumMember == 0;

    public int JoinSquare(SquareMember member)
    {
        lock (_sync)
        {
            if (_members.ContainsKey(member.UnitUid))
                return NetError.NET_OK;
            if (_members.Count >= SquareMaxSlot)
                return NetError.ERR_ROOM_09;
            _members[member.UnitUid] = member;
            return NetError.NET_OK;
        }
    }

    public bool LeaveSquare(long unitUid)
    {
        lock (_sync) return _members.Remove(unitUid);
    }

    public bool SyncPosition(long unitUid, float x, float y, float z)
    {
        lock (_sync)
        {
            if (!_members.TryGetValue(unitUid, out var m)) return false;
            m.PosX = x;
            m.PosY = y;
            m.PosZ = z;
            return true;
        }
    }

    public int DeleteByGsUid(long gsUid)
    {
        lock (_sync)
        {
            var targets = _members.Values.Where(m => m.GsUid == gsUid).Select(m => m.UnitUid).ToList();
            foreach (long uid in targets)
                _members.Remove(uid);
            return targets.Count;
        }
    }

    public KSquareInfo GetSquareInfo()
    {
        lock (_sync)
        {
            return new KSquareInfo
            {
                SquareType = SquareType,
                SquareUid = SquareUid,
                RoomName = RoomName,
                MaxSlot = SquareMaxSlot,
                JoinSlot = (sbyte)Math.Clamp(_members.Count, 0, sbyte.MaxValue),
                RelayIp = RelayIp,
                Port = RelayPort,
            };
        }
    }

    public List<SquareMember> SnapshotMembers()
    {
        lock (_sync) return _members.Values.ToList();
    }
}
