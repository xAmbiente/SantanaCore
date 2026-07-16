using System;
using Santana.Network.Data.Chat;
using ProudNetSrc.Serialization;
namespace Santana.Network.Message.Chat
{
    [Packet(16001, PacketType.Chat)]
    public class LoginAckMessage
    {
        public LoginAckMessage()
        {
        }
        public LoginAckMessage(uint result)
        {
            Result = result;
        }
        public uint Result { get; set; }
    }
    [Packet(16002, PacketType.Chat)]
    public class FriendActionAckMessage
    {
        public FriendActionAckMessage()
        {
            Friend = new FriendDto();
        }
        public FriendActionAckMessage(FriendResult result)
            : this()
        {
            Result = result;
        }
        public FriendActionAckMessage(FriendResult result, int unk, FriendDto friend)
        {
            Result = result;
            Unk = unk;
            Friend = friend;
        }
        public FriendResult Result { get; set; }
        public int Unk { get; set; }
        public FriendDto Friend { get; set; }
    }
    [Packet(16003, PacketType.Chat)]
    public class FriendListAckMessage
    {
        public FriendListAckMessage()
        {
            Friends = Array.Empty<FriendDto>();
        }
        public FriendListAckMessage(FriendDto[] friends)
        {
            Friends = friends;
        }
        public FriendDto[] Friends { get; set; }
    }
    [Packet(16004, PacketType.Chat)]
    public class CombiActionAckMessage
    {
        public CombiActionAckMessage()
        {
            Combi = new CombiDto();
        }
        public CombiActionAckMessage(int result, int unk, CombiDto combi)
        {
            Result = result;
            Unk = unk;
            Combi = combi;
        }
        public int Result { get; set; }
        public int Unk { get; set; }
        public CombiDto Combi { get; set; }
    }
    [Packet(16005, PacketType.Chat)]
    public class CombiListAckMessage
    {
        public CombiListAckMessage()
        {
            Combies = Array.Empty<CombiDto>();
        }
        public CombiListAckMessage(CombiDto[] combies)
        {
            Combies = combies;
        }
        public CombiDto[] Combies { get; set; }
    }
    [Packet(16006, PacketType.Chat)]
    public class CombiCheckNameAckMessage
    {
        public CombiCheckNameAckMessage()
        {
            Unk2 = "";
        }
        public CombiCheckNameAckMessage(int unk1, string unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
        public int Unk1 { get; set; }
        public string Unk2 { get; set; }
    }
    [Packet(16007, PacketType.Chat)]
    public class DenyActionAckMessage
    {
        public DenyActionAckMessage()
        {
            Deny = new DenyDto();
        }
        public DenyActionAckMessage(int result, DenyAction action, DenyDto deny)
        {
            Result = result;
            Action = action;
            Deny = deny;
        }
        public int Result { get; set; }
        public DenyAction Action { get; set; }
        public DenyDto Deny { get; set; }
    }
    [Packet(16008, PacketType.Chat)]
    public class DenyListAckMessage
    {
        public DenyListAckMessage()
        {
            Denies = Array.Empty<DenyDto>();
        }
        public DenyListAckMessage(DenyDto[] denies)
        {
            Denies = denies;
        }
        public DenyDto[] Denies { get; set; }
    }
    [Packet(16009, PacketType.Chat)]
    public class ChannelPlayerListAckMessage
    {
        public ChannelPlayerListAckMessage()
        {
            UserData = Array.Empty<PlayerInfoShortDto>();
        }
        public ChannelPlayerListAckMessage(PlayerInfoShortDto[] userData)
        {
            UserData = userData;
        }
        public PlayerInfoShortDto[] UserData { get; set; }
    }
    [Packet(16010, PacketType.Chat)]
    public class ChannelEnterPlayerAckMessage
    {
        public ChannelEnterPlayerAckMessage()
        {
            UserData = new PlayerInfoShortDto();
        }
        public ChannelEnterPlayerAckMessage(PlayerInfoShortDto userData)
        {
            UserData = userData;
        }
        public PlayerInfoShortDto UserData { get; set; }
    }
    [Packet(16011, PacketType.Chat)]
    public class ChannelLeavePlayerAckMessage
    {
        public ChannelLeavePlayerAckMessage()
        {
        }
        public ChannelLeavePlayerAckMessage(ulong accountId)
        {
            AccountId = accountId;
        }
        public ulong AccountId { get; set; }
    }
    [Packet(16012, PacketType.Chat)]
    public class MessageChatAckMessage
    {
        public MessageChatAckMessage()
        {
            Nickname = "";
            Message = "";
        }
        public MessageChatAckMessage(ChatType chatType, ulong accountId, string nick, string message)
        {
            ChatType = chatType;
            AccountId = accountId;
            Nickname = nick;
            Message = message;
        }
        public ChatType ChatType { get; set; }
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }
    }
    [Packet(16013, PacketType.Chat)]
    public class MessageWhisperChatAckMessage
    {
        public MessageWhisperChatAckMessage()
        {
            ToNickname = "";
            Nickname = "";
            Message = "";
        }
        public MessageWhisperChatAckMessage(uint unk, string toNickname, ulong accountId, string nick, string message)
        {
            Unk = unk;
            ToNickname = toNickname;
            AccountId = accountId;
            Nickname = nick;
            Message = message;
        }
        public uint Unk { get; set; }
        public string ToNickname { get; set; }
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }
    }
    [Packet(16014, PacketType.Chat)]
    public class RoomInvitationPlayerAckMessage
    {
        public RoomInvitationPlayerAckMessage()
        {
            Unk2 = "";
            Location = new PlayerLocationDto();
            Unk3 = 0;
        }
        public RoomInvitationPlayerAckMessage(ulong unk1, string unk2, PlayerLocationDto location, int unk3 = 0)
        {
            Unk1 = unk1;
            Unk2 = unk2;
            Location = location;
            Unk3 = unk3;
        }
        public ulong Unk1 { get; set; }
        public string Unk2 { get; set; }
        public PlayerLocationDto Location { get; set; }
        public int Unk3 { get; set; }
    }
    [Packet(16015, PacketType.Chat)]
    public class ClanMemberListAckMessage
    {
        public ClanMemberListAckMessage()
        {
            Players = Array.Empty<PlayerInfoDto>();
        }
        public ClanMemberListAckMessage(PlayerInfoDto[] players)
        {
            Players = players;
        }
        public PlayerInfoDto[] Players { get; set; }
    }
    [Packet(16016, PacketType.Chat)]
    public class NoteListAckMessage
    {
        public NoteListAckMessage()
        {
            Notes = Array.Empty<NoteDto>();
        }
        public NoteListAckMessage(int pageCount, byte currentPage, NoteDto[] notes)
        {
            PageCount = pageCount;
            CurrentPage = currentPage;
            Unk3 = 6;
            Notes = notes;
        }
        public int PageCount { get; set; }
        public byte CurrentPage { get; set; }
        public int Unk3 { get; set; }
        public NoteDto[] Notes { get; set; }
    }
    [Packet(16017, PacketType.Chat)]
    public class NoteSendAckMessage
    {
        public NoteSendAckMessage()
        {
        }
        public NoteSendAckMessage(int result)
        {
            Result = result;
        }
        public int Result { get; set; }
    }
    [Packet(16018, PacketType.Chat)]
    public class NoteReadAckMessage
    {
        public NoteReadAckMessage()
        {
            Note = new NoteContentDto();
        }
        public NoteReadAckMessage(ulong id, NoteContentDto note, int unk)
        {
            Id = id;
            Note = note;
            Unk = unk;
        }
        public ulong Id { get; set; }
        public NoteContentDto Note { get; set; }
        public int Unk { get; set; }
    }
    [Packet(16019, PacketType.Chat)]
    public class NoteDeleteAckMessage
    {
        public NoteDeleteAckMessage()
        {
            Notes = Array.Empty<DeleteNoteDto>();
        }
        public NoteDeleteAckMessage(DeleteNoteDto[] notes)
        {
            Notes = notes;
        }
        public DeleteNoteDto[] Notes { get; set; }
    }
    [Packet(16020, PacketType.Chat)]
    public class NoteErrorAckMessage
    {
        public NoteErrorAckMessage()
        {
        }
        public NoteErrorAckMessage(int unk)
        {
            Unk = unk;
        }
        public int Unk { get; set; }
    }
    [Packet(16021, PacketType.Chat)]
    public class NoteCountAckMessage
    {
        public NoteCountAckMessage()
        {
        }
        public NoteCountAckMessage(byte noteCount, byte unk2, byte unk3)
        {
            NoteCount = noteCount;
            Unk2 = unk2;
            Unk3 = unk3;
        }
        public byte NoteCount { get; set; }
        public byte Unk2 { get; set; }
        public byte Unk3 { get; set; }
    }
    [Packet(16022, PacketType.Chat)]
    public class ChatPlayerInfoAckMessage
    {
        public ChatPlayerInfoAckMessage()
        {
        }
        public ChatPlayerInfoAckMessage(PlayerInfoDto player)
        {
            Player = player;
        }
        public PlayerInfoDto Player { get; set; }
    }
    [Packet(16023, PacketType.Chat)]
    public class PlayerPositionAckMessage
    {
        public PlayerPositionAckMessage()
        {
            Location = new PlayerLocationDto();
        }
        public PlayerPositionAckMessage(ulong accountId, PlayerLocationDto location)
        {
            AccountId = accountId;
            Location = location ?? new PlayerLocationDto();
        }
        public ulong AccountId { get; set; }
        public PlayerLocationDto Location { get; set; }
    }
    [Packet(16024, PacketType.Chat)]
    public class ChatPlayerInfoListAckMessage
    {
        public ChatPlayerInfoListAckMessage()
        {
            Players = Array.Empty<PlayerInfoDto>();
        }
        public ChatPlayerInfoListAckMessage(PlayerInfoDto[] players)
        {
            Players = players;
        }
        public PlayerInfoDto[] Players { get; set; }
    }
    [Packet(16025, PacketType.Chat)]
    public class UserDataTwoReqMessage
    {
        public ulong AccountId { get; set; }
        public int Unk { get; set; }
    }
    [Packet(16026, PacketType.Chat)]
    public class UserDataFourAckMessage
    {
        public UserDataFourAckMessage()
        {
        }
        public UserDataFourAckMessage(int unk, UserDataDto userData)
        {
            Unk = unk;
            UserData = userData;
        }
        public int Unk { get; set; }
        public UserDataDto UserData { get; set; }
    }
    [Packet(16027, PacketType.Chat)]
    public class ClanChangeNoticeAckMessage
    {
        public string Unk { get; set; }
    }
    [Packet(16028, PacketType.Chat)]
    public class NoteRejectImportuneAckMessage
    {
        public int Unk { get; set; }
    }
    [Packet(16029, PacketType.Chat)]
    public class ClubSystemMessageMessage
    {
        public ClubSystemMessageMessage()
        {
            AccountId = 0;
            Message = "";
        }
        public ClubSystemMessageMessage(ulong accountId, string message)
        {
            AccountId = accountId;
            Message = message;
        }
        public ulong AccountId { get; set; }
        public string Message { get; set; }
    }
    [Packet(16030, PacketType.Chat)]
    public class ClubNewsRemindMessage
    {
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public ClubNewsRemindMessage()
        {
        }
        public ClubNewsRemindMessage(int unk1, int unk2)
        {
            Unk1 = unk1;
            Unk2 = unk2;
        }
    }
    [Packet(16031, PacketType.Chat)]
    public class ClubNoteSendAckMessage
    {
        public int Unk { get; set; }
    }
    [Packet(16032, PacketType.Chat)]
    public class ClubMemberListAckMessage
    {
        public ClubMemberListAckMessage()
        {
            Members = Array.Empty<ClubMemberDto>();
        }
        public ClubMemberListAckMessage(ClubMemberDto[] members)
        {
            Members = members;
        }
        public ClubMemberDto[] Members { get; set; }
    }
    [Packet(16036, PacketType.Chat)]
    public class ClubMemberListAck2Message
    {
        public ClubMemberListAck2Message()
        {
            Members = Array.Empty<ClubMemberDto2>();
        }
        public ClubMemberListAck2Message(uint clanId, ClubMemberDto2[] members)
        {
            ClanId = clanId;
            Members = members;
        }
        public uint ClanId { get; set; }
        public ClubMemberDto2[] Members { get; set; }
    }
    [Packet(16033, PacketType.Chat)]
    public class ClubMemberLoginStateAckMessage
    {
        public ClubMemberLoginStateAckMessage()
        {
            State = 0;
            AccountId = 0;
        }
        public ClubMemberLoginStateAckMessage(int state, ulong accountId)
        {
            State = state;
            AccountId = accountId;
        }
        public int State { get; set; }
        public ulong AccountId { get; set; }
    }
    [Packet(16034, PacketType.Chat)]
    public class Chennel_PlayerNameTagList_AckMessage
    {
        public Chennel_PlayerNameTagList_AckMessage()
        {
            UserData = Array.Empty<PlayerNameTagInfoDto>();
        }
        public Chennel_PlayerNameTagList_AckMessage(PlayerNameTagInfoDto[] userData)
        {
            UserData = userData;
        }
        public PlayerNameTagInfoDto[] UserData { get; set; }
    }
    [Packet(16035, PacketType.Chat)]
    public class ClubClubMemberInfoAck2Message
    {
        public ClubClubMemberInfoAck2Message()
        {
            Unk1 = 0;
            IsModerator = 0;
            Unk6 = 0;
            Nickname = "";
            JoinDate = "";
            Unk7 = "";
            Unk8 = -1;
        }
        public uint ClanId { get; set; }
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public int IsModerator { get; set; }
        public int Unk4 { get; set; }
        public int Unk5 { get; set; }
        public string JoinDate { get; set; }
        public int Unk6 { get; set; }
        public string Unk7 { get; set; }
        public long Unk8 { get; set; }
        public int Unk9 { get; set; }
        public int Unk10 { get; set; }
    }
}
