
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ItemDropAckDto
  {
    public ItemDropAckDto()
    {
      Position = new byte[6];
      Unk11 = new int[0];
    }

     public uint Counter { get; set; }

     public int Unk2 { get; set; }

     public int Unk3 { get; set; }

     public int Unk4 { get; set; }

    [Fixed(6)]
    public byte[] Position { get; set; }

     public long Unk6 { get; set; }

     public long Unk7 { get; set; }

     public int Unk8 { get; set; }

     public int Unk9 { get; set; }

     public int Unk10 { get; set; }

    public int[] Unk11 { get; set; }

     public short Unk12 { get; set; }

     public short Unk13 { get; set; }
  }
}
