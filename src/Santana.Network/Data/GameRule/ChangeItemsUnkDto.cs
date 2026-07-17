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

    public ulong[] Skills { get; set; }

    public ulong[] Weapons { get; set; }

    public int[] Unk4 { get; set; }

    public int[] Unk5 { get; set; }

    public int Unk6 { get; set; }

    public float HP { get; set; }
  }
}
