
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Chat
{
  [Dto]
  public class BRUserDataDto
  {
     public uint CountFirstPlaceKilled { get; set; }

     public uint CountFirstPlace { get; set; }
  }

    [Dto]
    public class BRUserDataScoreDto
    {
         public float TotalScore { get; set; }
    }
}
