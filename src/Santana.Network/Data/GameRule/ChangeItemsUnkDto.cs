using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ChangeItemsUnkDto
  {
    public ChangeItemsUnkDto()
    {
      Skills = Array.Empty<ItemNumber>();
      Weapons = Array.Empty<ItemNumber>();
      Unk4 = Array.Empty<int>();
      Unk5 = Array.Empty<int>();
    }

    
    public ulong AccountId { get; set; }

    
    public ItemNumber[] Skills { get; set; }

    
    public ItemNumber[] Weapons { get; set; }

    
    public int[] Unk4 { get; set; }

    
    public int[] Unk5 { get; set; }

    
    public int Unk6 { get; set; }

    
    public float HP { get; set; }
  }
}
