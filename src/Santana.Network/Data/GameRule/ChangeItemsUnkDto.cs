using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class ChangeItemsUnkDto
  {
    public ChangeItemsUnkDto()
    {
      Skills = Array.Empty<ulong>();
      Weapons = Array.Empty<ulong>();
      Unk4 = Array.Empty<int>();
      Unk5 = Array.Empty<int>();
    }


    public ulong AccountId { get; set; }

    // op 2025/3038: Skills/Weapons son u64 en el wire (el cliente los manda de 8 bytes), NO uint32.
    // Antes eran ItemNumber[] (uint32) -> desalineaba el payload -> armas basura en el visual [0xa6] -> crash 2v2.
    public ulong[] Skills { get; set; }


    public ulong[] Weapons { get; set; }

    
    public int[] Unk4 { get; set; }

    
    public int[] Unk5 { get; set; }

    
    public int Unk6 { get; set; }

    
    public float HP { get; set; }
  }
}
