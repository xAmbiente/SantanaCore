using System;
using System.Collections.Generic;
using System.Text;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class NameTagEffectDto
    {



        public byte active { get; set; }

        
        public short Unk3 { get; set; }

        
        public int Unk4 { get; set; }

        
        public uint nametagid { get; set; }

        
        public int Unk5 { get; set; }

        
        public int Unk6 { get; set; }

        
        public string days { get; set; }

        
        public string nametag { get; set; }

        
        public string zero { get; set; }

        
        public string zero2 { get; set; }

        
        public string zero3 { get; set; }
    }
}
