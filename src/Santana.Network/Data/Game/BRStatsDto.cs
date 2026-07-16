
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class BRStatsDto
    {
         public uint Won { get; set; }

         public uint Lost { get; set; }

         public float TotalScore { get; set; }

         public uint FirstKilled { get; set; }

         public uint FirstPlace { get; set; }
    }
}
