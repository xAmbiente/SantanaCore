
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class FriendDto
  {
    public FriendDto()
    {
      Nickname = "";
    }

     public ulong AccountId { get; set; }

    
    public string Nickname { get; set; }

     public uint State { get; set; }

     public uint Unk { get; set; }
  }
}
