using System;
using Santana.Network.Data.Auth;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Auth
{
  [Packet(5101, PacketType.Auth)]
  public class LoginKRAckMessage
  {
    public ulong AccountId { get; set; }
    public uint SessionId { get; set; }
    public string Unk1 { get; set; }
    public string SessionId2 { get; set; }
    public AuthLoginResult Result { get; set; }
    public string Unk2 { get; set; }
    public string BannedUntil { get; set; }
    public string Unk3 { get; set; }
    public string AuthToken { get; set; }
    public string NewToken { get; set; }
    public string Datetime { get; set; }

    public LoginKRAckMessage()
    {
      Unk1 = "9";
      SessionId2 = "";
      Unk2 = "";
      BannedUntil = "";
    }

    public LoginKRAckMessage(DateTimeOffset bannedUntil)
        : this()
    {
      Result = AuthLoginResult.Banned;
      BannedUntil = bannedUntil.ToString("yyyyMMddHHmmss");
    }

    public LoginKRAckMessage(AuthLoginResult result)
        : this()
    {
      Result = result;
    }

    public LoginKRAckMessage(AuthLoginResult result, ulong accountId, uint sessionId, string authsession,
        string newsession, string datetime)
        : this()
    {
      Result = result;
      AccountId = accountId;
      SessionId = (uint)accountId;
      SessionId2 = sessionId.ToString();
      AuthToken = authsession;
      NewToken = newsession;
      Datetime = datetime;
    }

  }

  [Packet(5102, PacketType.Auth)]
  public class LoginEUAckMessage
  {
    public ulong AccountId { get; set; }
    public uint SessionId { get; set; }
    public string Unk1 { get; set; }
    public string SessionId2 { get; set; }
    public AuthLoginResult Result { get; set; }
    public string Unk2 { get; set; }
    public string BannedUntil { get; set; }
    public string mUnknow04 { get; set; }
    public string AuthToken { get; set; }
    public string NewToken { get; set; }
    public string Datetime { get; set; }

    public LoginEUAckMessage()
    {
      Unk1 = "";
      SessionId2 = "";
      Unk2 = "";
      BannedUntil = "";
    }

    public LoginEUAckMessage(DateTimeOffset bannedUntil)
        : this()
    {
      Result = AuthLoginResult.Banned;
      BannedUntil = bannedUntil.ToString("yyyyMMddHHmmss");
    }

    public LoginEUAckMessage(AuthLoginResult result)
        : this()
    {
      Result = result;
    }

    public LoginEUAckMessage(AuthLoginResult result, ulong accountId, uint sessionId, string authsession,
        string newsession, string datetime)
        : this()
    {
      Result = result;
      AccountId = accountId;
      SessionId = (uint)accountId;
      SessionId2 = sessionId.ToString();
      AuthToken = authsession;
      NewToken = newsession;
      Datetime = datetime;
    }

  }

  [Packet(5104, PacketType.Auth)]
  public class ServerListAckMessage
  {
    public ServerInfoDto[] ServerList { get; set; }

    public ServerListAckMessage()
        : this(Array.Empty<ServerInfoDto>())
    {
    }

    public ServerListAckMessage(ServerInfoDto[] serverList)
    {
      ServerList = serverList;
    }

  }

  [Packet(5105, PacketType.Auth)]
  public class OptionVersionCheckAckMessage
  {
    [Scalar] public byte[] Data { get; set; }

    public OptionVersionCheckAckMessage()
    {
      Data = Array.Empty<byte>();
    }

    public OptionVersionCheckAckMessage(byte[] data)
    {
      Data = data;
    }

  }

  [Packet(5107, PacketType.Auth)]
  public class GameDataAckMessage
  {
    public uint Type { get; set; }
    [Scalar] public byte[] Data { get; set; }
    public uint TotalLength { get; set; }

    public GameDataAckMessage()
    {
    }

    public GameDataAckMessage(uint type, byte[] data, uint totalLength)
    {
      Type = type;
      Data = data;
      TotalLength = totalLength;
    }

  }
}
