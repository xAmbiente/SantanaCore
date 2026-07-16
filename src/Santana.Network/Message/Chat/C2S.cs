namespace Santana.Network.Message.Chat
{
  using Santana.Network.Data.Chat;
  using ProudNetSrc.Serialization;

  [Packet(15001, PacketType.Chat)]
  public class LoginReqMessage
  {
    public ulong AccountId { get; set; }
    public string Nickname { get; set; }
    public string SessionId { get; set; }

  }

  [Packet(15002, PacketType.Chat)]
  public class DenyActionReqMessage
  {
    public DenyAction Action { get; set; }
    public DenyDto Deny { get; set; }
  }

  [Packet(15003, PacketType.Chat)]
  public class FriendActionReqMessage
  {
    public FriendAction Action { get; set; }
    public ulong AccountId { get; set; }
    public string Nickname { get; set; }

  }

  [Packet(15004, PacketType.Chat)]
  public class CombiCheckNameReqMessage
  {
    public string Name { get; set; }

  }

  [Packet(15005, PacketType.Chat)]
  public class CombiActionReqMessage
  {
    public uint Action { get; set; }

    public ulong TargetAccountId { get; set; }

    public string CombiName { get; set; }

    public string CombiMate { get; set; }

    [Skip] public int Unk1 { get; set; }

  }

  [Packet(15006, PacketType.Chat)]
  public class UserDataOneReqMessage
  {
    public ulong AccountId { get; set; }

  }

  [Packet(15007, PacketType.Chat)]
  public class UserDataThreeAckMessage
  {
    public ulong AccountId { get; set; }
    public int Unk { get; set; }
    public UserDataDto UserData { get; set; }

  }

  [Packet(15008, PacketType.Chat)]
  public class MessageChatReqMessage
  {
    public ChatType ChatType { get; set; }
    public string Message { get; set; }

  }

  [Packet(15009, PacketType.Chat)]
  public class MessageWhisperChatReqMessage
  {
    public string ToNickname { get; set; }
    public string Message { get; set; }

  }

  [Packet(15010, PacketType.Chat)]
  public class RoomInvitationPlayerReqMessage
  {
    public ulong AccountId { get; set; }

  }

  [Packet(15011, PacketType.Chat)]
  public class NoteListReqMessage
  {
    public byte Page { get; set; }
    public int MessageType { get; set; }

  }

  [Packet(15012, PacketType.Chat)]
  public class NoteSendReqMessage
  {
    public string Receiver { get; set; }
    public ulong Unk1 { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public int Unk2 { get; set; }

  }

  [Packet(15013, PacketType.Chat)]
  public class NoteReadReqMessage
  {
    public ulong Id { get; set; }

  }

  [Packet(15014, PacketType.Chat)]
  public class NoteDeleteReqMessage
  {
    public ulong[] Notes { get; set; }

  }

  [Packet(15015, PacketType.Chat)]
  public class NoteCountReqMessage
  {
    public ulong Unk { get; set; }

  }

  [Packet(15016, PacketType.Chat)]
  public class OptionSaveCommunityReqMessage
  {
    public int AllowCombi { get; set; }
    public int AllowFriendReq { get; set; }
    public int AllowInvite { get; set; }
    public int RevealInfo { get; set; }

  }

  [Packet(15017, PacketType.Chat)]
  public class OptionSaveBinaryReqMessage
  {
    public uint Checksum { get; set; }
    [Scalar] public byte[] Data { get; set; }

  }

  [Packet(15018, PacketType.Chat)]
  public class NoteRejectImportuneReqMessage
  {
    public int Unk1 { get; set; }
    public long Unk2 { get; set; }

  }

  [Packet(15019, PacketType.Chat)]
  public class ClubNoteSendReqMessage
  {
    public ClubNoteDto Note { get; set; }

  }

  [Packet(15020, PacketType.Chat)]
  public class ClubMemberListReqMessage
  {
    public uint ClanId { get; set; }

  }

  [Packet(15021, PacketType.Chat)]
  public class ClubClubMemberInfoReq2Message
  {
    public uint ClanId { get; set; }
    public ulong AccountId { get; set; }

  }

  [Packet(15022, PacketType.Chat)]
  public class ClubMemberListReq2Message
  {
  }

  [Packet(15023, PacketType.Chat)]
  public class ClubNoteSendReq2Message
  {
  }

  [Packet(15024, PacketType.Chat)]
  public class ChannellistReqMessage
  {
  }
}
