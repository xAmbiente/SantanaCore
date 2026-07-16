using System.Net;
using SantanaLib.Serialization;
using ProudNetSrc.Serialization.Serializers;

namespace AuthServer.ServiceModel
{
  [SantanaContract]
  public class ServerInfoDto
  {
    [SantanaMember(0)] public string ApiKey { get; set; }

    [SantanaMember(1)] public byte Id { get; set; }

    [SantanaMember(2)] public string Name { get; set; }

    [SantanaMember(3)] public ushort PlayerLimit { get; set; }

    [SantanaMember(4)] public ushort PlayerOnline { get; set; }

    [SantanaMember(5, typeof(IPEndPointSerializer))]
    public IPEndPoint EndPoint { get; set; }

    [SantanaMember(6, typeof(IPEndPointSerializer))]
    public IPEndPoint ChatEndPoint { get; set; }
  }
}
