
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class RandomShopRollingDto
  {

    public ItemNumber ItemId { get; set; }

    public ItemPeriodType PeriodType { get; set; }

    public int Period { get; set; }

    public byte Color { get; set; }

    public uint EffectGroupId { get; set; }

    public byte Unk1 { get; set; }

    public uint ShopItemId { get; set; }
  }
}
