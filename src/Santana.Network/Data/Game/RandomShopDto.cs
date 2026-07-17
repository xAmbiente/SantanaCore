using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class RandomShop_ItemDto
    {
        public RandomShop_ItemDto()
        {
        }

        public int unk { get; set; }

        public byte unk1 { get; set; }

        public int categorie_id { get; set; }

        public string categorie_name { get; set; }

        public int unk2 { get; set; }

        public int item_id { get; set; }

        public string ColorGroup_string { get; set; }

        public string None_string { get; set; }

        public string AP_2_string { get; set; }

        public int Color { get; set; }
    }

    [Dto]
    public class RandomShop_CategorieDto
    {
        public RandomShop_CategorieDto()
        {

        }

        public int categorie_id { get; set; }

        public byte unk1 { get; set; }

        public short unk2 { get; set; }

        public string unk3 { get; set; }

        public string unk4 { get; set; }

        public string available { get; set; }

        public string pen_string { get; set; }

        public int price { get; set; }

        public string gender { get; set; }

    }

    [Dto]
    public class RandomShopItemsDto
    {
        public RandomShopItemsDto()
        {
        }

        public ItemNumber ItemID { get; set; }

        public ItemPeriodType ItemPeriodType { get; set; }

        public uint Period { get; set; }

        public byte Color { get; set; }

        public uint Effect { get; set; }

        public RandomShopBoxColor BoxColor { get; set; }

        public uint Unk { get; set; }
    }
}
