
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class DeleteNoteDto
  {
    public DeleteNoteDto()
    {
    }

    public DeleteNoteDto(ulong unk1, int unk2)
    {
      Unk1 = unk1;
      Unk2 = unk2;
    }

     public ulong Unk1 { get; set; }

     public int Unk2 { get; set; }
  }
}
