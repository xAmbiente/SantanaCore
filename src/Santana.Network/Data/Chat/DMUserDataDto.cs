
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class DMUserDataDto
    {
         public float WinRate { get; set; }

         public float KillDeathRate { get; set; }

         public float KillDeath { get; set; }

         public float KillScore { get; set; }

         public float KillAssistScore { get; set; }

         public float RecoveryScore { get; set; }
    }

    [Dto]
    public class DMUserDataScoreDto
    {
         public float TotalScore { get; set; }
    }
}
