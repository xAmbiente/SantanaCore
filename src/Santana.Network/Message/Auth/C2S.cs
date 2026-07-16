using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Auth
{
  [Packet(5001, PacketType.Auth)]
  public class LoginKRReqMessage
  {
    public string Unk1 { get; set; }
    public string Unk2 { get; set; }
    public string Unk3 { get; set; }
    public string Unk4 { get; set; }
    public int Unk5 { get; set; }
    public int Unk6 { get; set; }
    public int Unk7 { get; set; }
    public string Unk8 { get; set; }
    public int Unk9 { get; set; }
    public string Unk10 { get; set; }
    public string Unk11 { get; set; }
    public string token { get; set; }
    public string AuthToken { get; set; }
    public string NewToken { get; set; }
    public string DataTime { get; set; }

  }

  [Packet(5003, PacketType.Auth)]
  public class LoginJPReqMessage
  {
  }

  [Packet(5002, PacketType.Auth)]
  public class LoginEUReqMessage
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public string Unk1 { get; set; }
    public string Unk2 { get; set; }
    public int Unk3 { get; set; }
    public int Unk4 { get; set; }
    public int Unk5 { get; set; }
    public string Unk6 { get; set; }
    public int Unk7 { get; set; }
    public string Unk8 { get; set; }
    public string Unk9 { get; set; }
    public string token { get; set; }
    public string AuthToken { get; set; }
    public string NewToken { get; set; }
    public string DataTime { get; set; }

  }

  [Packet(5004, PacketType.Auth)]
  public class ServerListReqMessage
  {
  }

  [Packet(5007, PacketType.Auth)]
  public class GameDataReqMessage
  {
  }

  [Packet(5005, PacketType.Auth)]
  public class OptionVersionCheckReqMessage
  {
    public ulong AccountId { get; set; }
    public uint Checksum { get; set; }

  }
}
