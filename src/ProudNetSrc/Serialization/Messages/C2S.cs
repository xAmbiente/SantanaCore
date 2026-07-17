using System.Net;
using SantanaLib.Serialization;
using ProudNetSrc.Serialization.Serializers;

namespace ProudNetSrc.Serialization.Messages
{
  [SantanaContract]
  internal class ReliablePingMessage : IMessage
  {
    [SantanaMember(0)] public int RecentFrameRate { get; set; }
  }

  [SantanaContract]
  internal class P2P_NotifyDirectP2PDisconnectedMessage : IMessage
  {
    [SantanaMember(0)] public uint RemotePeerHostId { get; set; }

    [SantanaMember(1)] public uint Reason { get; set; }
  }

  [SantanaContract]
  internal class NotifyUdpToTcpFallbackByClientMessage : IMessage
  {
  }

  [SantanaContract]
  internal class P2PGroup_MemberJoin_AckMessage : IMessage
  {
    [SantanaMember(0)] public uint GroupHostId { get; set; }

    [SantanaMember(1)] public uint AddedMemberHostId { get; set; }

    [SantanaMember(2)] public uint EventId { get; set; }

    [SantanaMember(3)] public bool LocalPortReuseSuccess { get; set; }
  }

  [SantanaContract]
  internal class NotifyP2PHolepunchSuccessMessage : IMessage
  {
    public NotifyP2PHolepunchSuccessMessage()
    {
      ABSendAddr = new IPEndPoint(0, 0);
      ABRecvAddr = ABRecvAddr;
      BASendAddr = ABRecvAddr;
      BARecvAddr = ABRecvAddr;
    }

    [SantanaMember(0)] public uint A { get; set; }

    [SantanaMember(1)] public uint B { get; set; }

    [SantanaMember(2, typeof(IPEndPointSerializer))]
    public IPEndPoint ABSendAddr { get; set; }

    [SantanaMember(3, typeof(IPEndPointSerializer))]
    public IPEndPoint ABRecvAddr { get; set; }

    [SantanaMember(4, typeof(IPEndPointSerializer))]
    public IPEndPoint BASendAddr { get; set; }

    [SantanaMember(5, typeof(IPEndPointSerializer))]
    public IPEndPoint BARecvAddr { get; set; }
  }

  [SantanaContract]
  internal class ShutdownTcpMessage : IMessage
  {
    [SantanaMember(0)] public short Unk { get; set; }
  }

  [SantanaContract]
  internal class ShutdownTcpHandshakeMessage : IMessage
  {
  }

  [SantanaContract]
  internal class NotifyLogMessage : IMessage
  {
    [SantanaMember(0)] public TraceId TraceId { get; set; }

    [SantanaMember(1, typeof(StringSerializer))]
    public string Message { get; set; }
  }

  [SantanaContract]
  internal class NotifyJitDirectP2PTriggeredMessage : IMessage
  {
    [SantanaMember(0)] public uint HostId { get; set; }
  }

  [SantanaContract]
  internal class NotifyNatDeviceNameDetectedMessage : IMessage
  {
    [SantanaMember(0, typeof(StringSerializer))]
    public string Name { get; set; }
  }

  [SantanaContract]
  internal class C2S_RequestCreateUdpSocketMessage : IMessage
  {
  }

  [SantanaContract]
  internal class C2S_CreateUdpSocketAckMessage : IMessage
  {
    [SantanaMember(0)] public bool Success { get; set; }
  }

  [SantanaContract]
  internal class ReportC2CUdpMessageCountMessage : IMessage
  {
    [SantanaMember(0)] public uint HostId { get; set; }

    [SantanaMember(1)] public uint UdpMessageTrialCount { get; set; }

    [SantanaMember(2)] public uint UdpMessageSuccessCount { get; set; }
  }

  [SantanaContract]
  internal class ReportC2SUdpMessageTrialCountMessage : IMessage
  {
    [SantanaMember(0)] public int TrialCount { get; set; }
  }
}
