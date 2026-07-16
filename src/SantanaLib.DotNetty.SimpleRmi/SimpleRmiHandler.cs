using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SantanaLib.DotNetty.Handlers.MessageHandling;
using SantanaLib.DotNetty.SimpleRmi.Codecs;
using SantanaLib.DotNetty.SimpleRmi.CodeGeneration;
using SantanaLib.DotNetty.SimpleRmi.Reflection;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi
{
    public class SimpleRmiHandler : ChannelHandlerAdapter
    {
        private const string ProxyStateKey = "__SimpleRmi-Proxy__521d53a2-feb4-40cc-bcaf-0a32ab998b3f";
        private static readonly string s_lengthFieldEncoderName = $"SimpleRmi-{nameof(LengthFieldPrepender)}#494e3d93-3d07-459c-b6b2-30c5a498521f";
        private static readonly string s_lengthFieldDecoderName = $"SimpleRmi-{nameof(LengthFieldBasedFrameDecoder)}#494e3d93-3d07-459c-b6b2-30c5a498521f";
        private static readonly string s_messageEncoderName = $"SimpleRmi-{nameof(MessageEncoder)}#494e3d93-3d07-459c-b6b2-30c5a498521f";
        private static readonly string s_messageDecoderName = $"SimpleRmi-{nameof(MessageDecoder)}#494e3d93-3d07-459c-b6b2-30c5a498521f";
        private static readonly string s_messageHandlerName = $"SimpleRmi-{nameof(SimpleMessageHandler)}#494e3d93-3d07-459c-b6b2-30c5a498521f";

        private const int MaxRmiFrameLength = 16 * 1024 * 1024;

        private readonly SimpleMessageHandler _messageHandler = new SimpleMessageHandler();

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            context.Channel.Pipeline
                .AddBefore(context.Name, s_lengthFieldEncoderName, new LengthFieldPrepender(4))
                .AddBefore(context.Name, s_lengthFieldDecoderName, new LengthFieldBasedFrameDecoder(MaxRmiFrameLength, 0, 4, 0, 4))
                .AddBefore(context.Name, s_messageEncoderName, new MessageEncoder())
                .AddBefore(context.Name, s_messageDecoderName, new MessageDecoder())
                .AddAfter(context.Name, s_messageHandlerName, _messageHandler);
            base.HandlerAdded(context);
        }

        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            context.Channel.Pipeline.Remove(s_lengthFieldEncoderName);
            context.Channel.Pipeline.Remove(s_lengthFieldDecoderName);
            context.Channel.Pipeline.Remove(s_messageEncoderName);
            context.Channel.Pipeline.Remove(s_messageDecoderName);
            base.HandlerRemoved(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            context.Channel.GetAttribute(ChannelAttributes.ResponseQueue)
                .Set(new ConcurrentDictionary<Guid, TaskCompletionSource<RmiMessage>>());
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var responseQueue = context.Channel.GetAttribute(ChannelAttributes.ResponseQueue).Get();
            foreach (var tcs in responseQueue.Values)
                tcs.TrySetCanceled();
            responseQueue.Clear();
            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var rmiMessage = message as RmiMessage;
            if (rmiMessage == null)
                throw new ArgumentException($"{nameof(SimpleRmiHandler)} only accepts {nameof(RmiMessage)} messages");

            var responseQueue = context.Channel.GetAttribute(ChannelAttributes.ResponseQueue).Get();
            if (responseQueue.TryRemove(rmiMessage.Guid, out var tcs))
                tcs.TrySetResult(rmiMessage);
            base.ChannelRead(context, message);
        }

        public void AddService(RmiService service)
        {
            var val = (IEnumerable<InterfaceInfo>)typeof(ServiceInfo<>).MakeGenericType(service.GetType())
                .GetProperty(nameof(ServiceInfo<RmiService>.Interfaces))
                .GetValue(null);

            if (!val.Any())
                throw new ArgumentException("The service does not implement any rmi interfaces", nameof(service));

            var messageHandler = (IMessageHandler)typeof(MessageHandlerFactory<>).MakeGenericType(service.GetType())
                .GetMethod("Get")
                .Invoke(null, new object[] { service });
            _messageHandler.Add(messageHandler);
        }

        internal T GetProxy<T>(IChannel channel)
            where T : class
        {
            var type = typeof(T);
            var key = ProxyStateKey + type.FullName;

            var attribute = channel.GetAttribute(AttributeKey<ProxyBase>.ValueOf(key));
            var proxy = attribute.Get();
            if (proxy == null)
            {
                proxy = ProxyFactory<T>.Create(channel);
                attribute.Set(proxy);
            }
            return DynamicCast<T>.From(proxy);
        }
    }
}
