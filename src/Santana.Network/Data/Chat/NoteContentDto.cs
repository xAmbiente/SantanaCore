
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class NoteContentDto
  {
    public NoteContentDto()
    {
      Message = "";
      Item = new NoteItemDto();
    }

     public ulong Unk1 { get; set; }

    public string Message { get; set; }

     public NoteItemDto Item { get; set; }

     public byte Unk2 { get; set; }

     public byte Unk3 { get; set; }
  }

  [Dto]
  public class NoteItemDto
  {
     public int Unk0 { get; set; }

     public int Unk1 { get; set; }

     public int Unk2 { get; set; }

     public short Unk3 { get; set; }

     public byte Unk4 { get; set; }

     public int Unk5 { get; set; }
  }
}
