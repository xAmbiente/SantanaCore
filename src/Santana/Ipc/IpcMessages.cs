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
}
