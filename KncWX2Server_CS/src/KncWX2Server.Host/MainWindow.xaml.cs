using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using KncWX2Server.CenterServer;
using KncWX2Server.ChannelServer;
using KncWX2Server.ChannelServer.Managers;
using KncWX2Server.Core.Framework;
using KncWX2Server.Core.Logging;
using KncWX2Server.Core.Packets;
using KncWX2Server.Core.Region;
using KncWX2Server.GameServer;
using KncWX2Server.GlobalServer;
using KncWX2Server.LoginServer;
using Microsoft.Win32;

namespace KncWX2Server.Host;

public partial class MainWindow : Window
{
    private readonly InProcessRouter _sharedRouter = new();
    private readonly KChannelServer _channelServer;
    private readonly KLoginServer _loginServer;
    private readonly KCenterServer _centerServer;
    private readonly KGlobalServer _globalServer;
    private readonly KGameServer _gameServer;
    private readonly ObservableCollection<string> _logs = [];
    private readonly DispatcherTimer _uiTimer;

    public MainWindow()
    {
        InitializeComponent();

        string dbPath = Path.Combine(AppContext.BaseDirectory, "KncWX2Server.db");
        _channelServer = new KChannelServer(RegionCode.TW, dbPath, _sharedRouter);
        _loginServer = new KLoginServer(RegionCode.TW, dbPath, _sharedRouter);
        _centerServer = new KCenterServer(RegionCode.TW, _sharedRouter);
        _globalServer = new KGlobalServer(RegionCode.TW, _sharedRouter);
        _gameServer = new KGameServer(RegionCode.TW, dbPath, _sharedRouter, _centerServer, _globalServer);
        _gameServer.GuildAdListProvider = (sort, page) =>
        {
            var (entries, total) = _loginServer.GuildBoardManager.GetGuildAdList((KGuildBoardManager.AdSortType)sort, page);
            var mapped = entries.Select(e => new KGuildAdInfo
            {
                GuildUid = e.GuildUid,
                GuildName = e.GuildName,
                GuildLevel = e.GuildLevel,
                GuildExp = e.GuildExp,
                MasterNickName = e.MasterNickName,
                CurNumMember = e.CurMemberCount,
                AdMessage = e.AdMessage,
                AdRegDate = e.RegDateUtc.ToString("yyyy-MM-dd"),
                AdEndDate = e.EndDateUtc.ToString("yyyy-MM-dd"),
            }).ToList();
            return (mapped, total);
        };
        _gameServer.WeddingHallGetProvider = wuid =>
        {
            if (!_loginServer.WeddingHallManager.TryGetWeddingInfo(wuid, out var e) || e == null)
                return null;
            return new KWeddingHallInfo
            {
                WeddingUid = e.WeddingUid,
                WeddingHallType = e.WeddingHallType,
                OfficiantNpc = e.OfficiantNpc,
                Groom = e.GroomUnitUid,
                Bride = e.BrideUnitUid,
                WeddingDateStr = e.WeddingDateUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                WeddingDate = new DateTimeOffset(e.WeddingDateUtc).ToUnixTimeSeconds(),
                RoomUid = e.RoomUid,
            };
        };
        _gameServer.WeddingHallAddProvider = info => _loginServer.WeddingHallManager.AddWeddingInfo(new KWeddingHallManager.WeddingHallEntry
        {
            WeddingUid = info.WeddingUid,
            RoomUid = info.RoomUid,
            GroomUnitUid = info.Groom,
            BrideUnitUid = info.Bride,
            WeddingHallType = info.WeddingHallType,
            OfficiantNpc = info.OfficiantNpc,
            WeddingDateUtc = DateTimeOffset.FromUnixTimeSeconds(Math.Max(0, info.WeddingDate)).UtcDateTime,
        });
        _gameServer.WeddingHallRoomUpdateProvider = (wuid, ruid) => _loginServer.WeddingHallManager.UpdateWeddingInfo(wuid, ruid);

        RegionCombo.ItemsSource = new[]
        {
            RegionProfile.Taiwan,
            RegionProfile.HongKong,
            RegionProfile.China,
            RegionProfile.Japan,
        };
        RegionCombo.DisplayMemberPath = nameof(RegionProfile.DisplayName);
        RegionCombo.SelectedIndex = 0;

        LogLevelCombo.ItemsSource = new[] { -1, 0, 1, 2 };
        LogLevelCombo.SelectedItem = 2;

        WaitQueueModeCombo.ItemsSource = Enum.GetValues<WaitQueueMode>();
        WaitQueueModeCombo.SelectedItem = WaitQueueMode.WQ_NONE;

        LogListBox.ItemsSource = _logs;

        KLog.OnLog += entry =>
        {
            Dispatcher.BeginInvoke(() =>
            {
                string line = $"[{entry.Timestamp:HH:mm:ss}] [{entry.Category}] {entry.Message}";
                _logs.Add(line);
                if (_logs.Count > 1000)
                    _logs.RemoveAt(0);
                if (LogListBox.Items.Count > 0)
                    LogListBox.ScrollIntoView(LogListBox.Items[^1]);
            });
        };

        _channelServer.OnStateChanged += () => Dispatcher.BeginInvoke(RefreshUiState);

        _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _uiTimer.Tick += (_, _) => RefreshMonitorGrids();
        _uiTimer.Start();
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ushort.TryParse(PortTextBox.Text, out ushort port))
            port = _channelServer.Region.DefaultChannelPort;
        if (!int.TryParse(GroupIdTextBox.Text, out int groupId))
            groupId = _channelServer.Region.Region == RegionCode.CN ? 1 : 0;

        string? luaPath = string.IsNullOrWhiteSpace(LuaPathTextBox.Text) ? null : LuaPathTextBox.Text.Trim();
        _loginServer.LoadServerResources();
        // Game resource loading also loads the shared Center and Global managers.
        _gameServer.LoadServerResources(includeHeavyTables: true);
        _centerServer.Start(serverGroupId: groupId);
        _globalServer.Start(serverGroupId: groupId);
        _loginServer.Start(serverGroupId: groupId);
        _gameServer.Start(serverGroupId: groupId);
        if (_channelServer.Start(luaPath, port, groupId))
        {
            RefreshUiState();
            RefreshMonitorGrids();
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _gameServer.ShutDown();
        _loginServer.ShutDown();
        _globalServer.ShutDown();
        _centerServer.ShutDown();
        _channelServer.ShutDown();
        RefreshUiState();
        RefreshMonitorGrids();
    }

    private void RegionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RegionCombo.SelectedItem is not RegionProfile profile)
            return;

        if (_channelServer.Region.Region != profile.Region)
        {
            string? luaPath = string.IsNullOrWhiteSpace(LuaPathTextBox.Text) ? null : LuaPathTextBox.Text.Trim();
            _channelServer.SwitchRegion(profile.Region, luaPath);
            _centerServer.SwitchRegion(profile.Region);
            _globalServer.SwitchRegion(profile.Region);
            _loginServer.SwitchRegion(profile.Region);
            _gameServer.SwitchRegion(profile.Region);
            PortTextBox.Text = _channelServer.Region.DefaultChannelPort.ToString();
            GroupIdTextBox.Text = _channelServer.ServerGroupId.ToString();
            RefreshUiState();
            RefreshMonitorGrids();
        }
    }

    private void LogLevelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LogLevelCombo.SelectedItem is int lvl)
            KLog.SetLevel(lvl);
    }

    private void ClearLogs_Click(object sender, RoutedEventArgs e) => _logs.Clear();

    private void BrowseLua_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog { Filter = "Lua Scripts (*.lua)|*.lua|All Files (*.*)|*.*" };
        if (dlg.ShowDialog() == true)
            LuaPathTextBox.Text = dlg.FileName;
    }

    private void ExecLuaFile_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(LuaPathTextBox.Text))
            _channelServer.ScriptManager.DoFile(LuaPathTextBox.Text.Trim());
    }

    private void EvalLua_Click(object sender, RoutedEventArgs e) => ExecuteLuaCommand();

    private void LuaCmdTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            ExecuteLuaCommand();
    }

    private void ExecuteLuaCommand()
    {
        string cmd = LuaCmdTextBox.Text.Trim();
        if (string.IsNullOrEmpty(cmd)) return;
        KLog.Cout($"> {cmd}");
        _channelServer.ScriptManager.DoString(cmd, "console");
        LuaCmdTextBox.Clear();
    }

    private void RefreshMonitor_Click(object sender, RoutedEventArgs e) => RefreshMonitorGrids();

    private void KickSelected_Click(object sender, RoutedEventArgs e)
    {
        if (UsersGrid.SelectedItem is UserRow row)
        {
            if (_channelServer.SimLayer.ActorManager.GetByUid(row.Uid) is KChannelUser user)
                user.ReserveDestroy();
        }
    }

    private void CreateAccount_Click(object sender, RoutedEventArgs e)
    {
        string uid = NewUserIdBox.Text.Trim();
        string pw = NewPasswordBox.Text;
        if (!int.TryParse(NewAuthLevelBox.Text, out int authLevel))
            authLevel = 0;

        if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(pw))
        {
            MessageBox.Show("UserID and Password are required.");
            return;
        }

        bool ok = _channelServer.DbLayer.Sqlite.CreateAccount(_channelServer.Region.Code, uid, pw, authLevel);
        KLog.Cout(ok
            ? $"Created SQLite account '{uid}' in region {_channelServer.Region.Code}."
            : $"Account '{uid}' already exists in region {_channelServer.Region.Code}.");
    }

    private void ApplyWaitQueue_Click(object sender, RoutedEventArgs e)
    {
        if (WaitQueueModeCombo.SelectedItem is WaitQueueMode mode)
            _channelServer.WaitQueueManager.SetMode((int)mode);
        if (int.TryParse(WaitQueueLimitBox.Text, out int limit))
            _channelServer.WaitQueueManager.SetTotalCCULimit(_channelServer.ServerGroupId, limit);
    }

    private void RefreshUiState()
    {
        bool running = _channelServer.IsRunning;
        StartButton.IsEnabled = !running;
        StopButton.IsEnabled = running;
        StatusText.Text = running ? (_channelServer.ServerReady ? "SUITE RUNNING" : "STARTING...") : "STOPPED";
        StatusBadge.Background = new SolidColorBrush(running ? Color.FromRgb(39, 103, 73) : Color.FromRgb(116, 42, 42));
    }

    private void RefreshMonitorGrids()
    {
        var actors = _channelServer.SimLayer.ActorManager.GetSnapshot()
            .OfType<KChannelUser>()
            .Select(u => new UserRow(u.Uid, u.UserId, u.IpStr, u.StateIdString))
            .ToList();
        UsersGrid.ItemsSource = actors;

        var channels = _channelServer.GetChannelListSnapshot().Values
            .Select(c => new ChannelRow(c.ChannelId, c.ChannelName, c.Ip, c.MasterPort, $"{c.CurrentUser}/{c.MaxUser}"))
            .ToList();
        ChannelsGrid.ItemsSource = channels;

        SuiteGrid.ItemsSource = new[]
        {
            new SuiteRow("ChannelServer", $"TCP :{_channelServer.NetLayer.Port}", _channelServer.IsRunning ? "RUNNING" : "STOPPED", $"Sessions: {actors.Count}"),
            new SuiteRow("LoginServer", $"TCP :{_loginServer.ListenPort}", _loginServer.IsRunning ? "RUNNING" : "STOPPED", $"Authed: {_loginServer.AuthenticatedUserCount}"),
            new SuiteRow("CenterServer", $"TCP :{_centerServer.ListenPort}", _centerServer.IsRunning ? "RUNNING" : "STOPPED", $"Rooms: {_centerServer.RoomManager.RoomCount}, Parties: {_centerServer.PartyManager.PartyCount}"),
            new SuiteRow("GlobalServer", $"TCP :{_globalServer.ListenPort}", _globalServer.IsRunning ? "RUNNING" : "STOPPED", $"PvPQueue: {_globalServer.MatchMakingManager.QueueCount}, Matches: {_globalServer.MatchMakingManager.TotalMatchesFormed}"),
            new SuiteRow("GameServer", $"TCP :{_gameServer.ListenPort}", _gameServer.IsRunning ? "RUNNING" : "STOPPED", $"Users: {_gameServer.ConnectedUserCount}, ActiveUnits: {_gameServer.ActiveUnitCount}, Zones: {_gameServer.FieldManager.ActiveZoneCount}"),
            new SuiteRow("TRServer", $"TCP :{_gameServer.TrPort}", _gameServer.IsRunning ? "RUNNING" : "STOPPED", $"Peers: {_gameServer.TrServer.RegisteredUserCount}, Relayed: {_gameServer.TrServer.RelayedEventCount}"),
            new SuiteRow("KUdpRelay (KTDNUDP)", $"UDP :{_gameServer.UdpRelayPort}", _gameServer.UdpRelay.IsRunning ? "RUNNING" : "STOPPED", $"Peers: {_gameServer.UdpRelay.PeerCount}, Relayed: {_gameServer.UdpRelay.RelayedPackets}"),
            new SuiteRow("KUdpEcho", $"UDP :{_gameServer.UdpEchoPort}", _gameServer.UdpEcho.IsRunning ? "RUNNING" : "STOPPED", $"Echoed: {_gameServer.UdpEcho.EchoCount}"),
        };

        RoomsGrid.ItemsSource = _centerServer.RoomManager.GetRoomList(1, 100)
            .Select(r => new RoomRow(r.RoomUid, r.RoomName, ((RoomState)r.RoomState).ToString(), $"{r.JoinSlot}/{r.MaxSlot}", $"{r.UdpRelayIp}:{r.UdpRelayPort}"))
            .ToList();

        FooterSummary.Text = $"Region: {_channelServer.Region.Code} | Group: {_channelServer.ServerGroupId} | CH:{actors.Count} GS:{_gameServer.ActiveUnitCount} Rooms:{_centerServer.RoomManager.RoomCount} UDP:{_gameServer.UdpRelay.RelayedPackets} | Tick: {_channelServer.TickCount}";
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _uiTimer.Stop();
        StopButton_Click(sender, new RoutedEventArgs());
    }

    private sealed record UserRow(long Uid, string UserId, string Ip, string State);
    private sealed record ChannelRow(int ChannelId, string ChannelName, string Ip, ushort Port, string Users);
    private sealed record SuiteRow(string Service, string Endpoint, string Status, string Metrics);
    private sealed record RoomRow(long RoomUid, string RoomName, string State, string Slots, string UdpRelay);
}
