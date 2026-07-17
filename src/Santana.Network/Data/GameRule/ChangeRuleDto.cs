using ProudNetSrc.Serialization;
namespace Santana.Network.Data.GameRule
{

  [Dto]
  public class ChangeRuleDto
  {
    public ChangeRuleDto()
    {
      Name = "";
      Password = "";
    }

     public Santana.GameRule GameRule { get; set; }

     public byte MapId { get; set; }

     public byte PlayerLimit { get; set; }

     public ushort Points { get; set; }

     public int Unk1 { get; set; }

     public byte Time { get; set; }

     public int ItemLimit { get; set; }

    public string Password { get; set; }

    public string Name { get; set; }

     public bool HasSpectator { get; set; }

     public byte SpectatorLimit { get; set; }

     public int Unk3 { get; set; }

     public int Unk4 { get; set; }
  }

  [Dto]
  public class ChangeRuleDto2
  {
    public ChangeRuleDto2()
    {
      Name = "";
      Password = "";
    }

     public Santana.GameRule GameRule { get; set; }

     public byte MapId { get; set; }

     public byte PlayerLimit { get; set; }

     public ushort Points { get; set; }

     public int Unk1 { get; set; }

     public byte Time { get; set; }

     public int ItemLimit { get; set; }

    public string Password { get; set; }

    public string Name { get; set; }

     public bool HasSpectator { get; set; }

     public byte SpectatorLimit { get; set; }

     public int Unk3 { get; set; }

     public int Unk4 { get; set; }

     public byte Unk5 { get; set; }

     public byte ChangeRuleId { get; set; }

     public int IsRandom { get; set; }

     public int FMBurnMode { get; set; }

     public int Unk8 { get; set; }

     public byte Unk9 { get; set; }
  }
}
