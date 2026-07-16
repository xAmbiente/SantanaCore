
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class DMStatsDto
  {
     public uint Kills { get; set; }

     public uint Lost { get; set; }

     public uint Won { get; set; }

     public uint KillAssists { get; set; }

     public uint Unk5 { get; set; }

     public uint Deaths { get; set; }

     public uint Unk7 { get; set; }

     public uint Unk8 { get; set; }

     public uint Unk9 { get; set; }

     public uint Unk10 { get; set; }
  }
}
