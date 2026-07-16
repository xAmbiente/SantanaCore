using System;
using System.Net;
using SantanaLib.Serialization;
using ProudNetSrc.Serialization.Serializers;

namespace ProudNetSrc.Serialization.Messages.Core
{
  [SantanaContract]
  internal class NotifyCSEncryptedSessionKeyMessage : ICoreMessage
  {
    [SantanaMember(0, typeof(ArrayWithScalarSerializer))]
    public byte[] SecureKey { get; set; }

    [SantanaMember(1, typeof(ArrayWithScalarSerializer))]
    public byte[] FastKey { get; set; }
  }

  [SantanaContract]
  internal class NotifyServerConnectionRequestDataMessage : ICoreMessage
  {
    public NotifyServerConnectionRequestDataMessage()
    {
      Version = Guid.Empty;
      InternalNetVersion = Constants.NetVersion;
    }

    [SantanaMember(0, typeof(ArrayWithScalarSerializer))]
    public byte[] UserData { get; set; }

    [SantanaMember(1)] public Guid Version { get; set; }

    [SantanaMember(2)] public uint InternalNetVersion { get; set; }
  }

  [SantanaContract]
  internal class ServerHolepunchMessage : ICoreMessage
  {
    public ServerHolepunchMessage()
    {
      MagicNumber = Guid.Empty;
    }

    [SantanaMember(0)] public Guid MagicNumber { get; set; }
  }

  [SantanaContract]
  internal class NotifyHolepunchSuccessMessage : ICoreMessage
  {
    public NotifyHolepunchSuccessMessage()
    {
      MagicNumber = Guid.Empty;
      LocalEndPoint = new IPEndPoint(0, 0);
      EndPoint = LocalEndPoint;
    }

    [SantanaMember(0)] public Guid MagicNumber { get; set; }

    [SantanaMember(1, typeof(IPEndPointSerializer))]
    public IPEndPoint LocalEndPoint { get; set; }

    [SantanaMember(2, typeof(IPEndPointSerializer))]
    public IPEndPoint EndPoint { get; set; }
  }

  [SantanaContract]
  internal class PeerUdp_ServerHolepunchMessage : ICoreMessage
  {
    public PeerUdp_ServerHolepunchMessage()
    {
      MagicNumber = Guid.Empty;
    }

    [SantanaMember(0)] public Guid MagicNumber { get; set; }

    [SantanaMember(1)] public uint HostId { get; set; }
  }

  [SantanaContract]
  internal class PeerUdp_NotifyHolepunchSuccessMessage : ICoreMessage
  {
    public PeerUdp_NotifyHolepunchSuccessMessage()
    {
      LocalEndPoint = new IPEndPoint(0, 0);
      EndPoint = new IPEndPoint(0, 0);
    }

    [SantanaMember(0, typeof(IPEndPointSerializer))]
    public IPEndPoint LocalEndPoint { get; set; }

    [SantanaMember(1, typeof(IPEndPointSerializer))]
    public IPEndPoint EndPoint { get; set; }

    [SantanaMember(2)] public uint HostId { get; set; }
  }

  [SantanaContract]
  internal class UnreliablePingMessage : ICoreMessage
  {
    [SantanaMember(0)] public double ClientTime { get; set; }

    [SantanaMember(1)] public double Ping { get; set; }
  }

  [SantanaContract]
  internal class SpeedHackDetectorPingMessage : ICoreMessage
  {
  }

  [SantanaContract]
  internal class ReliableRelay1Message : ICoreMessage
  {
    public ReliableRelay1Message()
    {
      Destination = Array.Empty<RelayDestinationDto>();
      Data = Array.Empty<byte>();
    }

    [SantanaMember(0, typeof(ArrayWithScalarSerializer))]
    public RelayDestinationDto[] Destination { get; set; }

    [SantanaMember(1, typeof(ArrayWithScalarSerializer))]
    public byte[] Data { get; set; }
  }

  [SantanaContract]
  internal class UnreliableRelay1Message : ICoreMessage
  {
    public UnreliableRelay1Message()
    {
      Destination = Array.Empty<uint>();
      Data = Array.Empty<byte>();
    }

    [SantanaMember(0)]
    public MessagePriority Priority { get; set; }

    [SantanaMember(1, typeof(ScalarSerializer))]
    public int UniqueId { get; set; }

    [SantanaMember(2, typeof(ArrayWithScalarSerializer))]
    public uint[] Destination { get; set; }

    [SantanaMember(3, typeof(ArrayWithScalarSerializer))]
    public byte[] Data { get; set; }
  }


  [SantanaContract]
  internal class ReliableRelay1UnkMessage : ICoreMessage
  {
    public ReliableRelay1UnkMessage()
    {
      Destination = new RelayDestinationDto();
      Data = Array.Empty<byte>();
    }

    [SantanaMember(0)] public RelayDestinationDto Destination { get; set; }

    [SantanaMember(1, typeof(ArrayWithScalarSerializer))]
    public byte[] Data { get; set; }
  }
}
