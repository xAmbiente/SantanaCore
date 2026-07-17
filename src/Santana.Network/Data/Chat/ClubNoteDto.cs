
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class ClubNoteDto
  {
    public ClubNoteDto()
    {
      Unk6 = "";
      Unk7 = "";
    }

     public int Unk1 { get; set; }

     public byte Unk2 { get; set; }

     public byte Unk3 { get; set; }

     public byte Unk4 { get; set; }

     public byte Unk5 { get; set; }

    public string Unk6 { get; set; }

    public string Unk7 { get; set; }
  }
}
