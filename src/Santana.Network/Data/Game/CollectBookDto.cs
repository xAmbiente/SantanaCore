using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class CollectBookDto
    {
         public ulong Unk1 { get; set; }
         public int Unk2 { get; set; }
         public int Unk3 { get; set; }

        
        public int[] Unk4 { get; set; } = new int[6];
    }
}
