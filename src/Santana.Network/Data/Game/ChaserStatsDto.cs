
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
    [Dto]
    public class ChaserStatsDto
    {
         public uint ChasedWon { get; set; }

         public uint ChasedRounds { get; set; }

         public uint ChaserWon { get; set; }

         public uint ChaserRounds { get; set; }

         public uint Kills { get; set; }
    }
}
