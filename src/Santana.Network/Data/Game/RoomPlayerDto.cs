
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class RoomPlayerDto
  {
    public RoomPlayerDto()
    {
      Nickname = "";
    }

     public uint ClanId { get; set; }

     public ulong AccountId { get; set; }

     public byte Unk1 { get; set; }

    
    public string Nickname { get; set; }

     public byte Pos { get; set; }

     public bool IsGM { get; set; }

#if LATESTS4
        
        public byte Unk3 { get; set; }
#endif
  }
}
