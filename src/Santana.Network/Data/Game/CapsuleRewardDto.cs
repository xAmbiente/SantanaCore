
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class CapsuleRewardDto
  {
    public CapsuleRewardDto()
    {
    }

    public CapsuleRewardDto(CapsuleRewardType _type, uint _pen, ItemNumber _itemnumber, ItemPriceType _priceType,
        ItemPeriodType _periodType, uint _period)
    {
      RewardType = _type;
      PEN = _pen;
      ItemNumber = _itemnumber;
      PriceType = _priceType;
      PeriodType = _periodType;
      Period = _period;
      Unk2 = 1;
    }

     public CapsuleRewardType RewardType { get; set; }

     public uint PEN { get; set; }

     public ItemNumber ItemNumber { get; set; }

     public ItemPriceType PriceType { get; set; }

     public ItemPeriodType PeriodType { get; set; }

     public uint Period { get; set; }

     public byte Color { get; set; }

     public uint Effect { get; set; }

     public byte Unk2 { get; set; }
  }
}
