
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class RoomDto
  {
    public RoomDto()
    {
      Name = "";
      Password = "";
    }

     public byte RoomId { get; set; }

     public byte State { get; set; }

     public int GameRule { get; set; }

     public byte Map { get; set; }

     public byte PlayerCount { get; set; }

     public byte PlayerLimit { get; set; }

     public int WeaponLimit { get; set; }

    
    public string Password { get; set; }

    
    public string Name { get; set; }

     public bool HasSpectator { get; set; }

     public byte CreationId { get; set; }

     public int IsRandom { get; set; }

     public int FMBURNMode { get; set; }

     public int Unk4 { get; set; }

     public int Unk5 { get; set; }
  }
}
