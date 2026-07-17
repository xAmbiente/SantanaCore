
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Club
{
  [Dto]
  public class ClubInfoDto2
  {
    public ClubInfoDto2()
    {
      Type = "";
      Name = "";

      Unk1 = 1;
      Unk2 = 1;
      Unk3 = 1;

      MasterName = "";
      CreationDate = "";
    }

     public uint Id { get; set; }

     public uint Id2 { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

     public byte Unk1 { get; set; }

     public byte Unk2 { get; set; }

     public byte Unk3 { get; set; }

     public uint Unk4 { get; set; }

     public uint Unk5 { get; set; }

     public byte Unk6 { get; set; }

     public uint Unk7 { get; set; }

    public string MasterName { get; set; }

     public uint Unk8 { get; set; }

     public uint MemberCount { get; set; }

    public string Motto { get; set; }

    public string CreationDate { get; set; }

    public string Unk10 { get; set; }

    public string Unk11 { get; set; }

     public uint Unk12 { get; set; }

     public uint Unk13 { get; set; }

     public uint Unk14 { get; set; }

     public uint Unk15 { get; set; }

     public uint Unk16 { get; set; }

     public uint Unk17 { get; set; }

     public uint Unk18 { get; set; }

     public uint Unk19 { get; set; }

     public uint Unk20 { get; set; }

     public uint Unk21 { get; set; }

     public ushort Unk22 { get; set; }
  }
}
