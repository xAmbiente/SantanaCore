using System.Net;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Auth
{
  [Dto]
  public class ServerInfoDto
  {
    public ServerInfoDto()
    {
      IsEnabled = true;
      Name = "";
    }

     public bool IsEnabled { get; set; }

     public uint Id { get; set; }

     public ServerType Type { get; set; }

    
    public string Name { get; set; }

     public ushort PlayerLimit { get; set; }

     public ushort PlayerOnline { get; set; }

    
    public IPEndPoint EndPoint { get; set; }

     public ushort GroupId { get; set; }
  }
}
