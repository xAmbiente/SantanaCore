using Santana.Network.Data.Relay;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Relay
{
  [Packet(10001, PacketType.Relay)]
  public class CRequestLoginMessage
  {
    public ulong AccountId;
    public string Nickname = "";
    public RoomLocation RoomLocation;
    public bool CreatedRoom;

  }

  [Packet(10002, PacketType.Relay)]
  public class CNotifyP2PLogMessage
  {
    public int Unk1;
    public short Unk2;
    public byte Unk3;

  }
}
