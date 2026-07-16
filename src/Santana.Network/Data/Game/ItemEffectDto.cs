
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class ItemEffectDto
  {
    public ItemEffectDto()
    {
      Unk1 = -1;
    }

     public uint Effect { get; set; }

     public int Unk1 { get; set; }

     public long Unk2 { get; set; }

     public int Unk3 { get; set; }
  }
}
