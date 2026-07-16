using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class AlchemyDto
    {
        public AlchemyDto()
        {

        }

         public int GearId { get; set; }
         public int GearCount { get; set; }
    }

    [Dto]
    public class AlchemyItemDto
    {
        public AlchemyItemDto()
        {

        }

         public int Unk { get; set; }
         public ItemNumber itemNumber { get; set; }
         public ItemPriceType itemPriceType { get; set; }
         public ItemPeriodType itemPeriodType { get; set; }
         public uint Period { get; set; }
         public short Unk3 { get; set; }
         public byte Color { get; set; }
         public uint Effect { get; set; }
    }
}
