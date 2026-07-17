
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ItemDropDto
  {
    public ItemDropDto()
    {
      Position = new byte[6];
    }

     public int Type { get; set; }

     public int EntityId { get; set; }

    [Fixed(6)]
    public byte[] Position { get; set; }

     public int Ammo { get; set; }

     public byte Unk5 { get; set; }

     public short Unk6 { get; set; }

     public short Unk7 { get; set; }
  }
}
