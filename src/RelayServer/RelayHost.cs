using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using ProudNetSrc;
using ProudNetSrc.Handlers;
using ProudNetSrc.Serialization;
using SantanaLib.DotNetty.Handlers.MessageHandling;
using Santana.Ipc;
using Santana.Network.Message.Relay;
using Serilog;
using Serilog.Core;

namespace Santana.Relay
{
    internal class RelayHost : ProudServer
    {
        private static readonly ILogger Log_ =
            Log.ForContext(Constants.SourceContextPropertyName, "Relay");

        private RelayHost(Configuration config) : base(config) { }

        public static RelayHost Instance { get; private set; }
        public static RelayRoomManager Rooms { get; private set; }
        public static RelayPlayerManager Players { get; private set; }

        public static void Initialize(Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("El relay ya esta inicializado");

            config.Version = new Guid("{a43a97d1-9ec7-495e-ad5f-8fe45fde1151}");
            config.MessageFactories = new MessageFactory[] { new RelayMessageFactory() };
            config.SessionFactory = new RelaySessionFactory();

            bool NotYetLoggedIn(RelaySession session) => session.Player == null;

            config.MessageHandlers = new IMessageHandler[]
            {
                new RelayMessageRouter<RelaySession>()
                    .AddHandler(new RelayAuthHandler())
                    .RegisterRule<CRequestLoginMessage>(NotYetLoggedIn)
            };

            Instance = new RelayHost(config);
            Players = new RelayPlayerManager();
            Rooms = new RelayRoomManager(Instance);
        }

        protected override void OnDisconnected(ProudSession session)
        {
            try
            {
                if (session is RelaySession relay && relay.Player != null)
                {
                    var plr = relay.Player;
                    plr.Room?.Leave(plr);
                    Players.Remove(plr.Account.Id);
                    Log_.Information("Dropped {nick} (account {id}) from the forwarding roster", plr.Account.Nickname, plr.Account.Id);
                    relay.Player = null;
                }
            }
            catch (Exception ex)
            {
                Log_.Error(ex, "Could not fully release the resources of a departing peer");
            }

            base.OnDisconnected(session);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Log_.Error(e.Exception, "Forwarding node reported a fault");
            base.OnError(e);
        }
    }

    internal class RelayAuthHandler : ProudMessageHandler
    {
        private static readonly ILogger Log_ =
            Log.ForContext(Constants.SourceContextPropertyName, "Relay");

        [MessageHandler(typeof(CRequestLoginMessage))]
        public async Task LoginHandler(RelaySession session, CRequestLoginMessage message)
        {
            RelayLoginResponse answer;
            try
            {
                answer = await Ipc.Ipc.Bus.PublishRequestAsync<RelayLoginRequest, RelayLoginResponse>(
                    new RelayLoginRequest
                    {
                        AccountId = message.AccountId,
                        Address = session.RemoteEndPoint?.Address?.ToString(),
                        ServerId = message.RoomLocation.ServerId,
                        ChannelId = message.RoomLocation.ChannelId,
                        RoomId = message.RoomLocation.RoomId
                    });
            }
            catch (Exception ex)
            {
                Log_.Error(ex, "No reply came back from the game host while validating account {acc}", message.AccountId);
                await session.SendAsync(new SNotifyLoginResultMessage(1));
                return;
            }

            if (answer?.OK != true || answer.Account == null)
            {
                Log_.Warning("Game host refused the credentials presented for account {acc}", message.AccountId);
                await session.SendAsync(new SNotifyLoginResultMessage(1));
                return;
            }

            if (RelayHost.Players[answer.Account.Id] != null)
            {
                Log_.Warning("Account {acc} already holds an open forwarding slot, rejecting the duplicate", answer.Account.Id);
                await session.SendAsync(new SNotifyLoginResultMessage(2));
                return;
            }

            var plr = new RelayPlayer(session, answer.Account);
            session.Player = plr;
            RelayHost.Players.Add(plr);

            RelayHost.Rooms.GetOrCreate(message.RoomLocation.RoomId).Join(plr);

            Log_.Information("Admitted {nick} (account {id}), now forwarding for match {room}",
                plr.Account.Nickname, plr.Account.Id, message.RoomLocation.RoomId);

            await session.SendAsync(new SNotifyLoginResultMessage(0));
        }
    }

    internal static class RelayIpc
    {
        private static CancellationTokenSource _shutdown;

        public static async Task StartAsync()
        {
            _shutdown = new CancellationTokenSource();
            await Ipc.Ipc.Bus.SubscribeAsync<PlayerDisconnectedMessage>(OnPlayerDisconnected, _shutdown.Token);
            await Ipc.Ipc.Bus.SubscribeAsync<PlayerLeftRoomMessage>(OnPlayerLeftRoom, _shutdown.Token);
        }

        public static void Stop() => _shutdown?.Cancel();

        private static Task OnPlayerLeftRoom(PlayerLeftRoomMessage message)
        {
            var plr = RelayHost.Players[message.AccountId];
            if (plr == null)
                return Task.CompletedTask;

            plr.Room?.Leave(plr);
            plr.Room = null;
            RelayHost.Players.Remove(message.AccountId);

            return Task.CompletedTask;
        }

        private static Task OnPlayerDisconnected(PlayerDisconnectedMessage message)
        {
            RelayHost.Players[message.AccountId]?.Disconnect();
            return Task.CompletedTask;
        }
    }
}
