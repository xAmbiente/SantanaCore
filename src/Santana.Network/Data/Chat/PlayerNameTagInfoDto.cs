
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class PlayerNameTagInfoDto
    {
         public uint AccountId { get; set; }

         public uint Unk1 { get; set; }

         public uint TagId { get; set; }
    }
}
