using KncWX2Server.Core.Packets;

namespace KncWX2Server.CenterServer;

/// <summary>
/// Faithful C# port of CenterServer/PvpMonsterManager.{h,cpp}:
/// allocates pre-registered PvP NPC UIDs and creates PvP room monsters.
/// </summary>
public sealed class KPvpMonsterManager
{
    private readonly KRoomMonsterManager _roomMonsters;
    private readonly Dictionary<long, KPvpNpcInfo> _pvpNpcInfo = new();

    public KPvpMonsterManager(KRoomMonsterManager roomMonsters)
    {
        _roomMonsters = roomMonsters;
    }

    public void StartGame() => _roomMonsters.StartGame();

    public void EndGame()
    {
        _roomMonsters.EndGame();
        _pvpNpcInfo.Clear();
    }

    public void OnCloseRoom()
    {
        _roomMonsters.OnCloseRoom();
        _pvpNpcInfo.Clear();
    }

    public void SetPvpNpcInfo(IDictionary<long, KPvpNpcInfo> pvpNpcInfo)
    {
        _pvpNpcInfo.Clear();
        foreach (var kv in pvpNpcInfo)
            _pvpNpcInfo[kv.Key] = kv.Value;
    }

    public Dictionary<long, KPvpNpcInfo> GetPvpNpcInfo() => new(_pvpNpcInfo);

    public bool CreatePvpMonster(KNPCUnitReq createNpcInfo, out int npcUid)
    {
        npcUid = 0;
        long matchedKey = 0;
        bool found = false;
        foreach (var kv in _pvpNpcInfo)
        {
            if (kv.Value.NpcId == createNpcInfo.NpcId)
            {
                matchedKey = kv.Key;
                found = true;
                break;
            }
        }
        if (!found)
            return false;

        int assignedUid = (int)matchedKey;
        _pvpNpcInfo.Remove(matchedKey);
        _roomMonsters.CreateMonster(
            createNpcInfo.NpcId,
            createNpcInfo.Level,
            (KRoomMonsterManager.MonsterGrade)createNpcInfo.MonsterGrade,
            createNpcInfo.NoDrop,
            createNpcInfo.GroupId,
            assignedUid);
        npcUid = assignedUid;
        return true;
    }
}
