using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class ItemDto
  {
    public ItemDto()
    {
      Effects = Array.Empty<ItemEffectDto>();
      ExpireTime = DateTimeOffset.MinValue;
    }

     public ulong Id { get; set; }

     public ItemNumber ItemNumber { get; set; }

     public ItemPriceType PriceType { get; set; }

     public ItemPeriodType PeriodType { get; set; }

     public ushort Period { get; set; }

     public uint Color { get; set; }

    
    public DateTimeOffset ExpireTime { get; set; }

     public int Durability { get; set; }

    
    public ItemEffectDto[] Effects { get; set; }

     public uint EnchantMP { get; set; }

     public uint EnchantLevel { get; set; }
#if LATESTS4
        
        public uint EsperID { get; set; } 
#endif
  }
}
