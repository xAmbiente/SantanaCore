using System;

namespace Santana.Ipc
{

    public class MessageWithGuid
    {
        public Guid Guid { get; set; }
    }

    public class RelayAccountInfo
    {
        public ulong Id { get; set; }
        public string Nickname { get; set; }

        public RelayAccountInfo() { }
        public RelayAccountInfo(ulong id, string nickname)
        {
            Id = id;
            Nickname = nickname;
        }
    }

    public class RelayLoginRequest : MessageWithGuid
    {
        public ulong AccountId { get; set; }
        public string Nickname { get; set; }
        public string Address { get; set; }
        public uint ServerId { get; set; }
        public uint ChannelId { get; set; }
        public uint RoomId { get; set; }
    }

    public class RelayLoginResponse : MessageWithGuid
    {
        public bool OK { get; set; }
        public RelayAccountInfo Account { get; set; }

        public RelayLoginResponse() { }
        public RelayLoginResponse(bool ok, RelayAccountInfo account)
        {
            OK = ok;
            Account = account;
        }
    }

    public class PlayerDisconnectedMessage
    {
        public ulong AccountId { get; set; }

        public PlayerDisconnectedMessage() { }
        public PlayerDisconnectedMessage(ulong accountId) { AccountId = accountId; }
    }

    public class PlayerLeftRoomMessage
    {
        public ulong AccountId { get; set; }

        public PlayerLeftRoomMessage() { }
        public PlayerLeftRoomMessage(ulong accountId) { AccountId = accountId; }
    }

    public class RelayServerNotifyP2PMessage
    {
        public uint RoomId { get; set; }

        public RelayServerNotifyP2PMessage() { }
        public RelayServerNotifyP2PMessage(uint roomId) { RoomId = roomId; }
    }

    public class WarfareRefereeMessage
    {
        public ulong AccountId { get; set; }
        public ushort PeerId { get; set; }

        public WarfareRefereeMessage() { }
        public WarfareRefereeMessage(ulong accountId, ushort peerId) { AccountId = accountId; PeerId = peerId; }
    }

    public class RelayKillPlayerMessage
    {
        public ulong TargetAccountId { get; set; }
        public ulong SourceAccountId { get; set; }
        public ushort TargetPeerId { get; set; }
        public ushort SourcePeerId { get; set; }
        public int Hits { get; set; }
        public byte Icon { get; set; }

        public RelayKillPlayerMessage() { }
        public RelayKillPlayerMessage(ulong targetAccountId, ulong sourceAccountId, ushort targetPeerId, ushort sourcePeerId, int hits, byte icon)
        {
            Icon = icon;
            TargetAccountId = targetAccountId;
            SourceAccountId = sourceAccountId;
            TargetPeerId = targetPeerId;
            SourcePeerId = sourcePeerId;
            Hits = hits;
        }
    }
}
