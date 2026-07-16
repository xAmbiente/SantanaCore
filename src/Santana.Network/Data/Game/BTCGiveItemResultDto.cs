using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class BTCGiveItemResultDto
    {
        public BTCGiveItemResultDto()
        {
        }

        public BTCGiveItemResultDto(int unk, ItemNumber itemNumber)
        {
            Unk = unk;
            ItemNumber = itemNumber;
        }

         public int Unk { get; set; }
         public ItemNumber ItemNumber { get; set; }
    }
}
