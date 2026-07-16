using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DotNetty.Transport.Channels;
using ProudNetSrc;
using Santana.Ipc;
using Santana.Network.Message.Relay;

namespace Santana.Relay
{
    internal class RelaySession : ProudSession
    {
        public RelaySession(uint hostId, IChannel channel, ProudServer server)
            : base(hostId, channel, server)
        {
        }

        public RelayPlayer Player { get; set; }
    }

    internal class RelaySessionFactory : ISessionFactory
    {
        public ProudSession Create(uint hostId, IChannel channel, ProudServer server)
        {
            return new RelaySession(hostId, channel, server);
        }
    }

    internal class RelayPlayer
    {
        public RelaySession Session { get; }
        public RelayAccountInfo Account { get; }
        public RelayRoom Room { get; set; }

        public RelayPlayer(RelaySession session, RelayAccountInfo account)
        {
            Session = session;
            Account = account;
        }

        public void Disconnect()
        {
            try { Session?.Dispose(); } catch { }
        }
    }

    internal class RelayRoom
    {
        private readonly RelayRoomManager _manager;
        private readonly ConcurrentDictionary<ulong, RelayPlayer> _players =
            new ConcurrentDictionary<ulong, RelayPlayer>();

        public uint Id { get; }
        public P2PGroup Group { get; }
        public IReadOnlyDictionary<ulong, RelayPlayer> Players => _players;

        public RelayRoom(RelayRoomManager manager, uint id, P2PGroup group)
        {
            _manager = manager;
            Id = id;
            Group = group;
        }

        public void Join(RelayPlayer plr)
        {
            if (!_players.TryAdd(plr.Account.Id, plr))
                return;

            plr.Session.SendAsync(new SEnterLoginPlayerMessage(
                plr.Session.HostId, plr.Account.Id, plr.Account.Nickname));

            foreach (var other in _players.Values.Where(x => x.Account.Id != plr.Account.Id))
            {
                other.Session.SendAsync(new SEnterLoginPlayerMessage(
                    plr.Session.HostId, plr.Account.Id, plr.Account.Nickname));

                plr.Session.SendAsync(new SEnterLoginPlayerMessage(
                    other.Session.HostId, other.Account.Id, other.Account.Nickname));
            }

            Group.Join(plr.Session.HostId);
            plr.Room = this;
        }

        public void Leave(RelayPlayer plr)
        {
            if (!_players.TryRemove(plr.Account.Id, out _))
                return;

            try { Group.Leave(plr.Session.HostId); } catch { }
            plr.Room = null;

            if (_players.IsEmpty)
                _manager.Remove(this);
        }
    }

    internal class RelayRoomManager
    {
        private readonly ProudServer _server;
        private readonly ConcurrentDictionary<uint, RelayRoom> _rooms =
            new ConcurrentDictionary<uint, RelayRoom>();

        public RelayRoomManager(ProudServer server)
        {
            _server = server;
        }

        public RelayRoom this[uint id] => _rooms.TryGetValue(id, out var r) ? r : null;

        public RelayRoom GetOrCreate(uint id)
        {
            return _rooms.GetOrAdd(id, roomId =>
            {
                var group = _server.P2PGroupManager.Create(true);
                return new RelayRoom(this, roomId, group);
            });
        }

        public void Remove(RelayRoom room)
        {
            if (_rooms.TryRemove(room.Id, out _))
            {
                try { _server.P2PGroupManager.Remove(room.Group); } catch { }
            }
        }
    }

    internal class RelayPlayerManager
    {
        private readonly ConcurrentDictionary<ulong, RelayPlayer> _players =
            new ConcurrentDictionary<ulong, RelayPlayer>();

        public RelayPlayer this[ulong accountId] => _players.TryGetValue(accountId, out var p) ? p : null;

        public bool Add(RelayPlayer plr) => _players.TryAdd(plr.Account.Id, plr);
        public void Remove(ulong accountId) => _players.TryRemove(accountId, out _);
    }
}
