
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Game
{
  [Dto]
  public class ItemDurabilityInfoDto
  {
    public ItemDurabilityInfoDto()
    {
      Durabilityloss = 0;
      Unk1 = 1;
    }

    public ItemDurabilityInfoDto(ulong itemId, int durabilityloss, int unk1)
    {
      ItemId = itemId;
      Durabilityloss = durabilityloss;
      Unk1 = unk1;
    }

    public ulong ItemId { get; set; }

    public int Durabilityloss { get; set; }

    public int Unk1 { get; set; }
  }
}
