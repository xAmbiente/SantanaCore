
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class CPTUserDataDto
    {
         public uint Kills { get; set; }

         public uint Domination { get; set; }
    }


    [Dto]
    public class CPTUserDataScoreDto
    {
         public float TotalScore { get; set; }
    }
}
