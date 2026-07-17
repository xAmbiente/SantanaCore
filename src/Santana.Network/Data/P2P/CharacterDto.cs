using System;
using System.Numerics;

using ProudNetSrc.Serialization;
namespace Santana.Network.Data.P2P
{
  [Dto]
  public class CharacterDto
  {
    public CharacterDto()
    {
      Id = 0;
      CurrentWeapon = WeaponSlot.None;
      Position = Vector3.Zero;
      Costumes = Array.Empty<ItemDto>();
      Skills = Array.Empty<ItemDto>();
      Weapons = Array.Empty<ItemDto>();
      Name = "";
      Unk2 = "";
      Values = Array.Empty<ValueDto>();
    }

     public LongPeerId Id { get; set; }

     public Team Team { get; set; }

    public Vector3 Position { get; set; }

     public byte Rotation1 { get; set; }

     public byte Rotation2 { get; set; }

    public ItemDto[] Costumes { get; set; }

    public ItemDto[] Skills { get; set; }

    public ItemDto[] Weapons { get; set; }

    [Wire(Kind.UInt)]
    public WeaponSlot CurrentWeapon { get; set; }

     public CharacterGender Gender { get; set; }

    public string Name { get; set; }

     public byte Unk1 { get; set; }

    public string Unk2 { get; set; }

    [Compressed]
    public float CurrentHP { get; set; }

    [Compressed]
    public float MaxHP { get; set; }

    [Compressed]
    public float Unk3 { get; set; }

    public ValueDto[] Values { get; set; }
  }
}
