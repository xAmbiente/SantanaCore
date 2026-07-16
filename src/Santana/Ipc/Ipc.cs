using System;
using Foundatio.Messaging;
using Foundatio.Serializer;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Santana.Ipc
{
    public static class Ipc
    {
        public static IMessageBus Bus { get; private set; }
        public static bool IsEnabled => Bus != null;

        private static ConnectionMultiplexer _muxer;

        public static void Initialize(string connectionString, string topic = "santana")
        {
            if (Bus != null)
                throw new InvalidOperationException("Ipc ya esta inicializado");

            _muxer = ConnectionMultiplexer.Connect(connectionString);
            var serializer = new JsonNetSerializer(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            });

            Bus = new RedisMessageBus(new RedisMessageBusOptions
            {
                Subscriber = _muxer.GetSubscriber(),
                Serializer = serializer,
                Topic = topic
            });
        }

        public static void Shutdown()
        {
            (Bus as IDisposable)?.Dispose();
            Bus = null;
            _muxer?.Dispose();
            _muxer = null;
        }
    }
}
