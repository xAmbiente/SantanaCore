
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class ClubHistoryDto
  {
    public ClubHistoryDto()
    {
      Unk3 = "";
      Unk4 = "";
      Unk5 = "";
      Unk6 = "";
      Unk7 = "";
      Unk8 = "";
    }

     public uint Unk1 { get; set; }

     public uint Unk2 { get; set; }

    public string Unk3 { get; set; }

    public string Unk4 { get; set; }

    public string Unk5 { get; set; }

    public string Unk6 { get; set; }

    public string Unk7 { get; set; }

    public string Unk8 { get; set; }
  }
}
