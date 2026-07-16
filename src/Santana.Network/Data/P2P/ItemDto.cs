
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.P2P
{
  [Dto]
  public class ItemDto
  {
    public ItemDto()
    {
      ItemNumber = 0;
    }

    public ItemDto(ItemNumber itemNumber, int unk2)
    {
      ItemNumber = itemNumber;
      Unk2 = unk2;
    }

     public ItemNumber ItemNumber { get; set; }

     public int Unk2 { get; set; }

    public override string ToString()
    {
      return ItemNumber.ToString();
    }
  }
}
