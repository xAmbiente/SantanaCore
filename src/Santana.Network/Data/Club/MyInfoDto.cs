
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Club
{
  [Dto]
  public class ClubMyInfoDto
  {
    public ClubMyInfoDto()
    {
      Type = "";
      Id = 0;
      State = 0;
      Unk5 = -1;
    }

     public uint Id { get; set; }

    public string Type { get; set; }

    public string Name { get; set; }

     public ClubState State { get; set; }

     public int Unk1 { get; set; }

     public ClubRank Rank { get; set; }

     public int Unk2 { get; set; }

     public int Unk3 { get; set; }

     public int Unk4 { get; set; }

     public long Unk5 { get; set; }

     public int Unk6 { get; set; }

     public byte Unk7 { get; set; }
  }
}
