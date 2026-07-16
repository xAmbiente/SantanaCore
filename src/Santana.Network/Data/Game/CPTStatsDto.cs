
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class CPTStatsDto
    {
         public uint Won { get; set; }

         public uint Lost { get; set; }

         public float TotalScore { get; set; }

         public uint CaptainKilled { get; set; }

         public uint Captain { get; set; }
    }
}
