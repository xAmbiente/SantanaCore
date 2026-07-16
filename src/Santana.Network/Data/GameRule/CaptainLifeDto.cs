
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class CaptainLifeDto
  {
     public ulong AccountId { get; set; }

     public float HP { get; set; }

    public CaptainLifeDto()
    {
    }

    public CaptainLifeDto(ulong accountId, float hp)
    {
      AccountId = accountId;
      HP = hp;
    }
  }
}
