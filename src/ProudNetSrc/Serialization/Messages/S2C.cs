using System;
using System.Net;
using SantanaLib.Serialization;
using ProudNetSrc.Serialization.Serializers;
namespace ProudNetSrc.Serialization.Messages
{
  [SantanaContract]
  internal class ReliablePongMessage : IMessage
  {
  }
  [SantanaContract]
  internal class NotifyUdpToTcpFallbackByServerMessage : IMessage
  {
  }
  [SantanaContract]
  internal class ShutdownTcpAckMessage : IMessage
  {
  }
  [SantanaContract]
  internal class P2PGroup_MemberJoinMessage : IMessage
  {
    public P2PGroup_MemberJoinMessage()
    {
      UserData = Array.Empty<byte>();
      SecureKey = Array.Empty<byte>();
      ConnectionMagicNumber = Guid.Empty;
    }
    public P2PGroup_MemberJoinMessage(uint groupId, uint memberId, uint eventId, byte[] secureKey,
        bool enableDirectP2P)
        : this()
    {
      GroupId = groupId;
      MemberId = memberId;
      EventId = eventId;
      EnableDirectP2P = enableDirectP2P;
      SecureKey = secureKey;
    }
    [SantanaMember(0)] public uint GroupId { get; set; }
    [SantanaMember(1)] public uint MemberId { get; set; }
    [SantanaMember(2, typeof(ArrayWithScalarSerializer))]
    public byte[] UserData { get; set; }
    [SantanaMember(3)] public uint EventId { get; set; }
    [SantanaMember(4, typeof(ArrayWithScalarSerializer))]
    public byte[] SecureKey { get; set; }
    [SantanaMember(5)] public uint P2PFirstFrameNumber { get; set; }
    [SantanaMember(6)] public Guid ConnectionMagicNumber { get; set; }
    [SantanaMember(7)] public bool EnableDirectP2P { get; set; }
    [SantanaMember(8)] public ushort BindPort { get; set; }
  }
  [SantanaContract]
  internal class P2PGroup_MemberJoin_UnencryptedMessage : IMessage
  {
    public P2PGroup_MemberJoin_UnencryptedMessage()
    {
      UserData = Array.Empty<byte>();
      ConnectionMagicNumber = Guid.Empty;
    }
    public P2PGroup_MemberJoin_UnencryptedMessage(uint groupId, uint memberId, uint eventId, bool enableDirectP2P)
        : this()
    {
      GroupId = groupId;
      MemberId = memberId;
      EventId = eventId;
      EnableDirectP2P = enableDirectP2P;
    }
    [SantanaMember(0)] public uint GroupId { get; set; }
    [SantanaMember(1)] public uint MemberId { get; set; }
    [SantanaMember(2, typeof(ArrayWithScalarSerializer))]
    public byte[] UserData { get; set; }
    [SantanaMember(3)] public uint EventId { get; set; }
    [SantanaMember(4)] public uint P2PFirstFrameNumber { get; set; }
    [SantanaMember(5)] public Guid ConnectionMagicNumber { get; set; }
    [SantanaMember(6)] public bool EnableDirectP2P { get; set; }
    [SantanaMember(7)] public ushort BindPort { get; set; }
  }
  [SantanaContract]
  internal class P2PRecycleCompleteMessage : IMessage
  {
    public P2PRecycleCompleteMessage()
    {
      InternalAddress = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 65535);
      ExternalAddress = InternalAddress;
      SendAddress = InternalAddress;
      RecvAddress = InternalAddress;
    }
    public P2PRecycleCompleteMessage(uint hostId)
        : this()
    {
      HostId = hostId;
    }
    [SantanaMember(0)] public uint HostId { get; set; }
    [SantanaMember(1)] public bool Recycled { get; set; }
    [SantanaMember(2, typeof(IPEndPointSerializer))]
    public IPEndPoint InternalAddress { get; set; }
    [SantanaMember(3, typeof(IPEndPointSerializer))]
    public IPEndPoint ExternalAddress { get; set; }
    [SantanaMember(4, typeof(IPEndPointSerializer))]
    public IPEndPoint SendAddress { get; set; }
    [SantanaMember(5, typeof(IPEndPointSerializer))]
    public IPEndPoint RecvAddress { get; set; }
  }
  [SantanaContract]
  internal class RequestP2PHolepunchMessage : IMessage
  {
    public RequestP2PHolepunchMessage()
    {
      LocalEndPoint = new IPEndPoint(0, 0);
      EndPoint = new IPEndPoint(0, 0);
    }
    public RequestP2PHolepunchMessage(uint hostId, IPEndPoint localEndPoint, IPEndPoint endPoint)
    {
      HostId = hostId;
      LocalEndPoint = localEndPoint;
      EndPoint = endPoint;
    }
    [SantanaMember(0)] public uint HostId { get; set; }
    [SantanaMember(1, typeof(IPEndPointSerializer))]
    public IPEndPoint LocalEndPoint { get; set; }
    [SantanaMember(2, typeof(IPEndPointSerializer))]
    public IPEndPoint EndPoint { get; set; }
  }
  [SantanaContract]
  internal class P2P_NotifyDirectP2PDisconnected2Message : IMessage
  {
    public P2P_NotifyDirectP2PDisconnected2Message()
    {
    }
    public P2P_NotifyDirectP2PDisconnected2Message(uint remotePeerHostId, uint reason)
    {
      RemotePeerHostId = remotePeerHostId;
      Reason = reason;
    }
    [SantanaMember(0)] public uint RemotePeerHostId { get; set; }
    [SantanaMember(1)] public uint Reason { get; set; }
  }
  [SantanaContract]
  internal class P2PGroup_MemberLeaveMessage : IMessage
  {
    public P2PGroup_MemberLeaveMessage()
    {
    }
    public P2PGroup_MemberLeaveMessage(uint memberId, uint groupId)
    {
      MemberId = memberId;
      GroupId = groupId;
    }
    [SantanaMember(0)] public uint MemberId { get; set; }
    [SantanaMember(1)] public uint GroupId { get; set; }
  }
  [SantanaContract]
  internal class NotifyDirectP2PEstablishMessage : IMessage
  {
    public NotifyDirectP2PEstablishMessage()
    {
      ABSendAddr = new IPEndPoint(0, 0);
      ABRecvAddr = ABSendAddr;
      BASendAddr = ABSendAddr;
      BARecvAddr = ABSendAddr;
    }
    public NotifyDirectP2PEstablishMessage(uint a, uint b, IPEndPoint abSendAddr, IPEndPoint abRecvAddr,
        IPEndPoint baSendAddr, IPEndPoint baRecvAddr)
    {
      A = a;
      B = b;
      ABSendAddr = abSendAddr;
      ABRecvAddr = abRecvAddr;
      BASendAddr = baSendAddr;
      BARecvAddr = baRecvAddr;
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
  internal class RenewP2PConnectionStateMessage : IMessage
  {
    public RenewP2PConnectionStateMessage()
    {
    }
    public RenewP2PConnectionStateMessage(uint hostId)
    {
      HostId = hostId;
    }
    [SantanaMember(0)] public uint HostId { get; set; }
  }
  [SantanaContract]
  internal class NewDirectP2PConnectionMessage : IMessage
  {
    public NewDirectP2PConnectionMessage()
    {
    }
    public NewDirectP2PConnectionMessage(uint hostId)
    {
      HostId = hostId;
    }
    [SantanaMember(0)] public uint HostId { get; set; }
  }
  [SantanaContract]
  internal class S2C_RequestCreateUdpSocketMessage : IMessage
  {
    public S2C_RequestCreateUdpSocketMessage()
    {
    }
    public S2C_RequestCreateUdpSocketMessage(IPEndPoint endPoint)
    {
      EndPoint = endPoint;
    }
    [SantanaMember(0, typeof(IPEndPointAddressStringSerializer))]
    public IPEndPoint EndPoint { get; set; }
  }
  [SantanaContract]
  internal class S2C_CreateUdpSocketAckMessage : IMessage
  {
    public S2C_CreateUdpSocketAckMessage()
    {
    }
    public S2C_CreateUdpSocketAckMessage(bool success, IPEndPoint endPoint)
    {
      Success = success;
      EndPoint = endPoint;
    }
    [SantanaMember(0)] public bool Success { get; set; }
    [SantanaMember(1, typeof(IPEndPointAddressStringSerializer))]
    public IPEndPoint EndPoint { get; set; }
  }
}
