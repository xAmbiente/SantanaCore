using System;
using System.IO;
using System.Net;
using DotNetty.Transport.Channels;
using Newtonsoft.Json;
using ProudNetSrc;
using Serilog;

namespace Santana.Relay
{
    internal static class Program
    {
        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] |Relay| {Message}{NewLine}{Exception}")
                .MinimumLevel.Verbose()
                .CreateLogger();

            var cfg = RelayConfig.Load();

            Log.Information("--------------------------------------------");
            Log.Information("Forwarding node coming online");
            Log.Information("--------------------------------------------");

            Ipc.Ipc.Initialize(cfg.Redis);
            Log.Information("Message bus backend reachable at {r}", cfg.Redis);

            var acceptGroup = new MultithreadEventLoopGroup(1);
            RelayHost.Initialize(new Configuration
            {
                SocketListenerThreads = acceptGroup,
                SocketWorkerThreads = new MultithreadEventLoopGroup(cfg.WorkerThreads),
                WorkerThread = new SingleThreadEventLoop()
            });

            RelayIpc.StartAsync().GetAwaiter().GetResult();

            RelayHost.Instance.Listen(cfg.Listener, IPAddress.Parse(cfg.IP), cfg.UdpPorts);
            Log.Information("Accepting forwarding traffic on {ep}, datagram ports: {ports}", cfg.Listener,
                cfg.UdpPorts == null || cfg.UdpPorts.Length == 0 ? "none, peer traffic goes through the forwarder" : string.Join(",", cfg.UdpPorts));
            Log.Information("Startup complete. Type 'exit' at this prompt to shut the node down.");

            while (true)
            {
                var line = Console.ReadLine();
                if (line == null) break;
                if (line.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    line.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                    line.Equals("stop", StringComparison.OrdinalIgnoreCase))
                    break;
            }

            RelayIpc.Stop();
            RelayHost.Instance.Dispose();
            Ipc.Ipc.Shutdown();
        }
    }

    internal class RelayConfig
    {
        [JsonProperty("server_ip")] public string IP { get; set; } = "127.0.0.1";
        [JsonProperty("listener_relay")] public string ListenerRaw { get; set; } = "127.0.0.1:28005";
        [JsonProperty("listener_relay_udp_ports")] public int[] UdpPorts { get; set; } = null;
        [JsonProperty("worker_threads")] public int WorkerThreads { get; set; } = 3;
        [JsonProperty("redis")] public string Redis { get; set; } = "127.0.0.1:6379";

        [JsonIgnore]
        public IPEndPoint Listener
        {
            get
            {
                var parts = ListenerRaw.Split(':');
                return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
            }
        }

        public static RelayConfig Load()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relay.json");
            if (!File.Exists(path))
            {
                var fresh = new RelayConfig();
                File.WriteAllText(path, JsonConvert.SerializeObject(fresh, Formatting.Indented));
                Log.Warning("Config file absent, wrote a stock one to {p} and continuing with built-in settings", path);
                return fresh;
            }

            return JsonConvert.DeserializeObject<RelayConfig>(File.ReadAllText(path)) ?? new RelayConfig();
        }
    }
}
