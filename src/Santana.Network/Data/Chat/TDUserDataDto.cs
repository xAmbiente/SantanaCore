
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class TDUserDataDto
    {
         public float WinRate { get; set; }

         public float TDScore { get; set; }

         public float TDSucc { get; set; }

         public float DefenseScore { get; set; }

         public float OffenseScore { get; set; }

         public float KillScore { get; set; }

         public float RecoveryScore { get; set; }
    }

    [Dto]
    public class TDUserDataScoreDto
    {
         public float TotalScore { get; set; }
    }
}
