
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class SiegeUserDataDto
  {
     public float WinRate { get; set; }

     public float CaptureScore { get; set; }

     public float BattleScore { get; set; }

     public float MainCoreCaptureScore { get; set; }

     public float ItemObtainScore { get; set; }
  }
}
