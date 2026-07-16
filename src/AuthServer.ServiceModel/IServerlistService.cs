using System.Threading.Tasks;
using SantanaLib.DotNetty.SimpleRmi;

namespace AuthServer.ServiceModel
{
  [RmiContract]
  public interface IServerlistService
  {
    [Rmi]
    Task<RegisterResult> Register(ServerInfoDto serverInfo);

    [Rmi]
    Task<bool> Update(ServerInfoDto serverInfo);

    [Rmi]
    Task<bool> Remove(byte id);
  }
}
