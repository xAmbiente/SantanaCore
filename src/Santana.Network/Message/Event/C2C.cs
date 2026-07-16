using System;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Event
{
  [Packet(12001, PacketType.Game)]
  public class ChatMessage
  {
    public string Message { get; set; }

  }

  [Packet(20002, PacketType.Game)]
  public class EventMessageMessage
  {
    public EventMessageMessage()
    {
      String = "";
    }

    public EventMessageMessage(GameEventMessage @event, ulong accountId, uint unk, ushort value, string @string)
    {
      Event = @event;
      AccountId = accountId;
      Unk = unk;
      Value = value;
      String = @string;
    }

    [Wire(Kind.UInt)] public GameEventMessage Event { get; set; }

    public ulong AccountId { get; set; }

    public uint Unk { get; set; }

    public ushort Value { get; set; }

    public string String { get; set; }

  }

  [Packet(20003, PacketType.Game)]
  public class ChangeTargetMessage
  {
    public short Unk1 { get; set; }

    public int Unk2 { get; set; }

  }

  [Packet(20004, PacketType.Game)]
  public class ArcadeSyncMessage
  {
    public ArcadeSyncMessage()
    {
      Unk3 = Array.Empty<byte>();
    }

    public byte Unk1 { get; set; }

    public int Unk2 { get; set; }

    [Scalar] public byte[] Unk3 { get; set; }

  }

  [Packet(20005, PacketType.Game)]
  public class ArcadeSyncReqMessage
  {
    public ArcadeSyncReqMessage()
    {
      Unk2 = Array.Empty<byte>();
    }

    public int Unk1 { get; set; }

    [Scalar] public byte[] Unk2 { get; set; }

  }

  [Packet(20006, PacketType.Game)]
  public class PacketMessage
  {
    public PacketMessage()
    {
      Data = Array.Empty<byte>();
    }

    public PacketMessage(bool isCompressed, byte[] data)
    {
      IsCompressed = isCompressed;
      Data = data;
    }

    public bool IsCompressed { get; set; }

    [Scalar] public byte[] Data { get; set; }

  }
}
