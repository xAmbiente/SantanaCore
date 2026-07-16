using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ChangeAvatarUnk1Dto
  {
    public ChangeAvatarUnk1Dto()
    {
      Costumes = Array.Empty<ItemNumber>();
      Skills = Array.Empty<ItemNumber>();
      Weapons = Array.Empty<ItemNumber>();
      Unk5 = Array.Empty<int>();
      Unk6 = Array.Empty<int>();
      Unk7 = Array.Empty<int>();
    }

     public ulong AccountId { get; set; }

    
    public ItemNumber[] Costumes { get; set; }

    
    public ItemNumber[] Skills { get; set; }

    
    public ItemNumber[] Weapons { get; set; }

    
    public int[] Unk5 { get; set; }

    
    public int[] Unk6 { get; set; }

    
    public int[] Unk7 { get; set; }

     public int Unk8 { get; set; }

     public CharacterGender Gender { get; set; }

     public float HP { get; set; }
  }
}
