using KncWX2Server.Core.Events;
using KncWX2Server.Core.Network;
using KncWX2Server.Core.Region;

namespace KncWX2Server.Core.Framework;

/// <summary>
/// Port of Common/Actor.{h,cpp} combining KPerformer event queue, FSM state, and TCP session ownership.
/// </summary>
public abstract class KActor : KPerformer
{
    protected readonly FSMClass Fsm;
    public KTcpSession Session { get; }
    public RegionProfile Region { get; }

    public int StateId => Fsm.CurrentState;
    public string StateIdString => Fsm.GetStateIDString(Fsm.CurrentState);
    public bool IsDestroyed { get; private set; }
    public string IpStr => Session.RemoteIp;
    public uint IpNumeric => Session.RemoteIpNumeric;

    protected KActor(KTcpSession session, FSMClass fsm, RegionProfile region)
    {
        Session = session;
        Fsm = fsm;
        Region = region;
        Uid = KncUid.GetTempUID(region);
        Name = $"Actor_{Uid}";
    }

    public virtual void OnConnected()
    {
    }

    public virtual void OnDestroy()
    {
        IsDestroyed = true;
    }

    public void ReserveDestroy()
    {
        IsDestroyed = true;
        Session.ReserveDestroy();
    }

    public bool SendPacket(EventId eventId, Serialization.IKSerializable packet)
        => Session.SendPacket(PerformerId.PI_GS_PROXY, Uid, eventId, packet, Region);

    public bool SendID(EventId eventId)
        => Session.SendID(PerformerId.PI_GS_PROXY, Uid, eventId);

    public override void Tick()
    {
        if (!Session.IsConnected)
        {
            OnDestroy();
            return;
        }

        base.Tick();
    }
}
