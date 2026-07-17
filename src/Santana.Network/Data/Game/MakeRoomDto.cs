
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class MakeRoomDto
  {
    public MakeRoomDto()
    {
      rName = "";
      rPassword = "";
    }

     public int GameRule { get; set; }

     public byte Map_ID { get; set; }

     public byte Player_Limit { get; set; }

     public short Points { get; set; }

     public byte Time { get; set; }

     public int Weapon_Limit { get; set; }

    public string rName { get; set; }

    public string rPassword { get; set; }

     public byte Spectator { get; set; }

     public byte SpectatorCount { get; set; }

     public long mUnknow01 { get; set; }

     public short mUnknow02 { get; set; }

     public int mUnknow03 { get; set; }

     public int FMBURNMode { get; set; }

#if LATESTS4

        public int ServerKey { get; set; }
#endif
  }
}
