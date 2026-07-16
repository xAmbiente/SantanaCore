
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class NameTagDto
  {
    public NameTagDto()
    {
      AccountId = 0;
      TagId = 0;
    }

    public NameTagDto(ulong accountId, uint tagId)
    {
      AccountId = accountId;
      TagId = tagId;
    }

     public ulong AccountId { get; set; }

     public uint TagId { get; set; }
  }
}
