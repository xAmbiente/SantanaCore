using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Relay
{
  [Packet(11002, PacketType.Relay)]
  public class SEnterLoginPlayerMessage
  {
    public uint HostId;
    public ulong AccountId;
    public string Nickname = "";

    public SEnterLoginPlayerMessage() { }

    public SEnterLoginPlayerMessage(uint hostId, ulong accountId, string nickname)
    {
      HostId = hostId;
      AccountId = accountId;
      Nickname = nickname;
    }

  }

  [Packet(11001, PacketType.Relay)]
  public class SNotifyLoginResultMessage
  {
    public int Result;

    public SNotifyLoginResultMessage() { }

    public SNotifyLoginResultMessage(int result) => Result = result;

  }
}
