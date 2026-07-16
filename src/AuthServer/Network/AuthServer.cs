using System;
using System.Threading.Tasks;
using SantanaLib.DotNetty.Handlers.MessageHandling;
using SantanaLib.Threading;
using SantanaLib.Threading.Tasks;
using Santana.Network.Message.Auth;
using Santana.Network.Service;
using ProudNetSrc;
using ProudNetSrc.Serialization;
using Serilog;
using Serilog.Core;

namespace Santana.Network
{
    internal class AuthServer : ProudServer
    {
        private static readonly ILogger
            Logger = Log.ForContext(Constants.SourceContextPropertyName, nameof(AuthServer));

        private readonly ILoop _flushLoop;

        private AuthServer(Configuration config)
            : base(config)
        {
            _flushLoop = new TaskLoop(TimeSpan.FromSeconds(10), Tick);
            ServerManager = new ServerManager();
        }

        public static AuthServer Instance { get; private set; }

        public ServerManager ServerManager { get; }

        public static void Initialize(Configuration config)
        {
            if (Instance != null)
                throw new InvalidOperationException("Server is already initialized");

            config.Version = new Guid("{9be73c0b-3b10-403e-be7d-9f222702a38c}");
            config.MessageFactories = new MessageFactory[] { new AuthMessageFactory() };
            config.MessageHandlers = new IMessageHandler[] { new AuthService() };
#if DEBUG
      config.Logger = Logger;
#endif
            Instance = new AuthServer(config);
        }

        protected override void OnStarted()
        {
            _flushLoop.Start();
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            _flushLoop.Stop();
            base.OnStopping();
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Logger.Error(e.Exception, "Fault reached the top of the service without being handled anywhere below");
            base.OnError(e);
        }

        private Task Tick(TimeSpan elapsed)
        {
            ServerManager.Flush();
            return Task.CompletedTask;
        }
    }
}
