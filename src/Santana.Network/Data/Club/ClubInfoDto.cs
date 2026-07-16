
using ProudNetSrc.Serialization;
namespace Santana.Network.Data.Club
{
  [Dto]
  public class ClubInfoDto
  {
    public ClubInfoDto()
    {
      Type = "";
      Name = "";
      MasterName = "";
      CreationDate = "";
      Unk7 = 1;
      Motto = " ";
      Announce = "";
    }

     public uint Id { get; set; }

    
    public string Type { get; set; }

    
    public string Name { get; set; }

     public int MemberCount { get; set; }

    
    public string MasterName { get; set; }

    
    public string CreationDate { get; set; }

     public int Unk1 { get; set; }

     public int Unk2 { get; set; }

     public int Unk3 { get; set; }

     public int Unk4 { get; set; }

     public int Unk5 { get; set; }

     public int Unk6 { get; set; }

    
    public string Motto { get; set; }

    
    public string Announce { get; set; }

     public int Unk7 { get; set; }

     public int Unk8 { get; set; }

     public int Unk9 { get; set; }

     public int Unk10 { get; set; }
  }
}
