
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class PlayerLocationDto
  {
    public PlayerLocationDto()
    {
      ServerGroupId = -1;
      RoomId = -1;
      Unk = -1;
      ChannelId = -1;
      GameServerId = -1;
      ChatServerId = -1;
    }

     public int ServerGroupId { get; set; }

     public int ChannelId { get; set; }

     public int RoomId { get; set; }

     public int Unk { get; set; }

     public int GameServerId { get; set; }

     public int ChatServerId { get; set; }
  }
}
