
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class ShoppingBasketDto
  {
     public ulong ItemId { get; set; }

     public ShopItemDto ShopItem { get; set; }
  }
}
