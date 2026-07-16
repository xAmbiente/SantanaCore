using System;
using System.Threading.Tasks;
using SantanaLib.DotNetty.SimpleRmi;
using SantanaLib.Threading;
using SantanaLib.Threading.Tasks;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Serilog;
using Serilog.Core;

namespace Santana.API
{
    internal class APIServerHandler : ChannelHandlerAdapter
    {
        private static readonly ILogger Logger =
            Log.ForContext(Constants.SourceContextPropertyName, nameof(APIServerHandler));

        private readonly object _gate = new object();
        private readonly ILoop _timeoutLoop;
        private IChannelGroup _openChannels;

        public APIServerHandler()
        {
            _timeoutLoop = new TaskLoop(Config.Instance.API.Timeout, Sweep);
        }

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            _openChannels = new DefaultChannelGroup(context.Executor);

            var rmiHandler = new SimpleRmiHandler();
            rmiHandler.AddService(new ServerlistService());
            context.Channel.Pipeline.AddBefore(context.Name, null, rmiHandler);
            base.HandlerAdded(context);
        }

        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            var rmiContext = context.Channel.Pipeline.Context<SimpleRmiHandler>();
            context.Channel.Pipeline.Remove(rmiContext.Name);
            base.HandlerRemoved(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            lock (_gate)
            {
                _openChannels.Add(context.Channel);
                if (_openChannels.Count == 1)
                    _timeoutLoop.Start();
            }
            context.Channel.GetAttribute(ChannelAttributes.State).Set(new ChannelState());
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            lock (_gate)
            {
                _openChannels.Remove(context.Channel);
                if (_openChannels.Count == 0)
                    _timeoutLoop.Stop();
            }
            var channelState = context.Channel.GetAttribute(ChannelAttributes.State).Get();
            if (channelState.ServerId != null)
                Network.AuthServer.Instance.ServerManager.Remove(channelState.ServerId.Value);
            base.ChannelInactive(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Logger.Warning(exception, "Fault surfaced on a management link; tearing the link down");
            context.CloseAsync();
        }

        private async Task Sweep(TimeSpan diff)
        {
            try
            {
                foreach (var openChannel in _openChannels)
                {
                    var channelState = openChannel.GetAttribute(ChannelAttributes.State).Get();
                    var stamp = DateTimeOffset.Now;

                    if (channelState.ServerId == null &&
                        stamp - channelState.ConnectionTime >= Config.Instance.API.Timeout)
                        await openChannel.CloseAsync();

                    if (stamp - channelState.LastActivity >= Config.Instance.API.Timeout)
                        await openChannel.CloseAsync();
                }
            }
            catch { }
        }
    }
}
