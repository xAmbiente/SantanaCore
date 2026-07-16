using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class UserDataItemDto
    {

        
        public ItemNumber itemNumber { get; set; }

        
        public ItemPriceType priceType { get; set; }

        
        public int Unk2 { get; set; }

        
        public short Period { get; set; }

        
        public byte Color { get; set; }

        
        public uint[] Effects { get; set; }

        
        public int EnchantLv { get; set; }

        
        public int Unk3 { get; set; }

        public UserDataItemDto()
        {
            Effects = Array.Empty<uint>();
        }
    }
}
