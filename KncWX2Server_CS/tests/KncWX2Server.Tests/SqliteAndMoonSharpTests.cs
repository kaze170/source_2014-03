using KncWX2Server.ChannelServer;
using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Database;
using KncWX2Server.Core.Region;
using Xunit;

namespace KncWX2Server.Tests;

public sealed class SqliteAndMoonSharpTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"knc_test_{Guid.NewGuid():N}.db");

    [Fact]
    public void SqliteDatabase_InitializesSeedsAndVerifiesAccounts()
    {
        var db = new SqliteDatabaseManager(_dbPath);
        db.InitializeAndSeed();

        foreach (var code in new[] { "TW", "HK", "CN", "JP" })
        {
            var groups = db.GetServerGroupList(code);
            Assert.NotEmpty(groups);

            var channels = db.GetChannelList(code);
            Assert.NotEmpty(channels);

            var ok = db.VerifyOrRegisterAccount(code, "test01", "test01");
            Assert.Equal(NetError.NET_OK, ok.NetErr);

            var badPw = db.VerifyOrRegisterAccount(code, "test01", "wrong");
            Assert.Equal(NetError.ERR_VERIFY_03, badPw.NetErr);
        }

        Assert.True(db.CreateAccount("TW", "newuser", "pw123", 1));
        Assert.Equal(NetError.NET_OK, db.VerifyOrRegisterAccount("TW", "newuser", "pw123").NetErr);
    }

    [Fact]
    public void MoonSharp_ExecutesChannelLuaConfig_And_WaitQueueScript()
    {
        using var server = new ChannelServerTestHarness(_dbPath, RegionCode.TW);
        bool ok = server.Server.ScriptManager.DoString("""
            Server:SetMailingList("ops@example.com")
            Server:SetShutDownWait(30000)
            NetLayer:SetCheckSequenceNum(true)
            NetLayer:SetPacketAuthFailLimit(25)
            SimLayer:SetUseKogOTP(true)
            SimLayer:SetNationFlag(NF_TW)
            SimLayer:SetAuthFlag(AF_GLOBAL_SERVICE)
            GASHAuthManager:InitGASHAuthValue('300148', 'AF')
            GASHAuthManager:InitGASHAuthAddress('10.9.11.2', 5411, 5412, 5413)
            WaitQueueManager:SetMode(WQ_CCU_LIMIT)
            WaitQueueManager:SetTotalCCULimit(SERVER_GROUP_ID["SGI_SOLES"], 5000)
            log(2)
            """, "test_tw_config");

        Assert.True(ok);
        Assert.Equal("ops@example.com", server.Server.MailingList);
        Assert.Equal(25u, server.Server.NetLayer.PacketAuthFailLimit);
        Assert.Equal("300148", server.Server.GashAuthManager.ServiceCode);
        Assert.Equal(WaitQueueMode.WQ_CCU_LIMIT, server.Server.WaitQueueManager.Mode);
        Assert.Equal(5000, server.Server.WaitQueueManager.GetTotalCCULimit(0));
    }

    public void Dispose()
    {
        try { if (File.Exists(_dbPath)) File.Delete(_dbPath); } catch { }
    }

    private sealed class ChannelServerTestHarness : IDisposable
    {
        public KChannelServer Server { get; }
        public ChannelServerTestHarness(string dbPath, RegionCode code)
        {
            Server = new KChannelServer(code, dbPath);
            Server.RegToLua();
        }
        public void Dispose() => Server.ShutDown();
    }
}
