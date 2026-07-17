
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
    [Dto]
    public class ChaserUserDataDto
    {

        public float KillProbability { get; set; }

        public float Kills { get; set; }
    }

    [Dto]
    public class ChaserUserDataScoreDto
    {

        public float SurvivalProbability { get; set; }
    }
}
