
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class ShopItemDto
    {
         public ItemNumber ItemNumber { get; set; }

         public ItemPriceType PriceType { get; set; }

         public ItemPeriodType PeriodType { get; set; }

         public ushort Period { get; set; }

         public byte Color { get; set; }

         public uint Effect { get; set; }
    }
}
