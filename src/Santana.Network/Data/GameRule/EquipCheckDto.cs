using System;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{
  [Dto]
  public class EquipCheckDto
  {
    public EquipCheckDto()
    {
      Weapons = Array.Empty<ItemCheckDto>();
      Skill = new ItemCheckDto();
      Costumes = Array.Empty<ItemCheckDto>();
      MovementSpeed = 1100;
    }

    [Fixed(3)]
    public ItemCheckDto[] Weapons { get; set; }

     public ItemCheckDto Skill { get; set; }

    [Fixed(8)]
    public ItemCheckDto[] Costumes { get; set; }

     public uint MovementSpeed { get; set; }
  }
}
