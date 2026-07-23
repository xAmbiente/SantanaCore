namespace ProudNetSrc
{
    using System.Collections.Concurrent;

    public static class RelayFrameTracker
    {
        private static readonly ConcurrentDictionary<(uint From, uint To), uint> Last =
            new ConcurrentDictionary<(uint, uint), uint>();

        public static void Observe(uint from, uint to, uint frameNumber) => Last[(from, to)] = frameNumber;

        public static uint Next(uint from, uint to) => Last.AddOrUpdate((from, to), 1u, (_, v) => v + 1);

        // GameTime = reloj P2P sincronizado del grupo; lo tomamos de los golpes reales
        // que pasan por el relay y lo extrapolamos con el reloj de pared.
        private static uint _gameTime;
        private static int _gameTimeTick;

        public static void ObserveGameTime(uint gameTime)
        {
            _gameTime = gameTime;
            _gameTimeTick = System.Environment.TickCount;
        }

        // El byte Id del PeerId sube cada vez que el jugador re-entra a la sala; el server lo
        // tiene hardcodeado en 0 (Room.cs), asi que el real se aprende del trafico del propio peer.
        private static readonly ConcurrentDictionary<uint, ushort> Peers =
            new ConcurrentDictionary<uint, ushort>();

        public static void ObservePeer(uint hostId, ushort peerId) => Peers[hostId] = peerId;

        public static ushort PeerOf(uint hostId, ushort fallback) =>
            Peers.TryGetValue(hostId, out var p) ? p : fallback;

        private static readonly ConcurrentDictionary<uint, uint> Accounts =
            new ConcurrentDictionary<uint, uint>();

        public static void ObserveAccount(uint hostId, uint accountId) => Accounts[hostId] = accountId;

        public static uint AccountOf(uint hostId) =>
            Accounts.TryGetValue(hostId, out var a) ? a : 0;

        private static readonly ConcurrentDictionary<ulong, ushort> RefereePeer =
            new ConcurrentDictionary<ulong, ushort>();

        public static void SetRefereePeer(ulong accountId, ushort peerId) => RefereePeer[accountId] = peerId;

        public static ushort RefereePeerOf(uint accountId) =>
            RefereePeer.TryGetValue(accountId, out var p) ? p : (ushort)0;

        // Los HostId se reasignan al reconectar; sin esto devolvemos el peer del inquilino anterior.
        public static void Forget(uint hostId)
        {
            Peers.TryRemove(hostId, out _);
            Accounts.TryRemove(hostId, out _);
            foreach (var key in Last.Keys)
            {
                if (key.From == hostId || key.To == hostId)
                    Last.TryRemove(key, out _);
            }
        }

        public static uint GameTimeNow(uint fallback) =>
            _gameTime == 0 ? fallback : _gameTime + (uint)(System.Environment.TickCount - _gameTimeTick);
    }
}
