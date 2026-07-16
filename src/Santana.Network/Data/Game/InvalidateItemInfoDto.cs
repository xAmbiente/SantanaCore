
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class InvalidateItemInfoDto
  {
     public ulong ItemId { get; set; }

     public uint Unk1 { get; set; }

     public uint Unk2 { get; set; }

     public byte Unk3 { get; set; }
  }
}
