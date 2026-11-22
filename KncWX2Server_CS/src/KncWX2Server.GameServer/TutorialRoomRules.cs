namespace KncWX2Server.GameServer;

/// <summary>Single-player dungeon IDs from CXSLDungeon::IsSinglePlayDungeon.</summary>
public static class TutorialRoomRules
{
    public static bool IsSinglePlayDungeon(int dungeonId) =>
        dungeonId == 30040 || dungeonId is >= 30030 and <= 30035 ||
        dungeonId is >= 39600 and <= 39608;
}
