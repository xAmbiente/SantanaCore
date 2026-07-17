
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class DenyDto
  {
    public DenyDto()
    {
      Nickname = "";
    }

     public ulong AccountId { get; set; }

    public string Nickname { get; set; }
  }
}
