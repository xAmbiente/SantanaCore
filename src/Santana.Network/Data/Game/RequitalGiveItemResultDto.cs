
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class RequitalGiveItemResultDto
  {
    public RequitalGiveItemResultDto()
    {
    }

    public RequitalGiveItemResultDto(ItemNumber itemNumber, int unk2)
    {
      ItemNumber = itemNumber;
      Unk2 = unk2;
    }

     public ItemNumber ItemNumber { get; set; }

     public int Unk2 { get; set; }
  }
}
