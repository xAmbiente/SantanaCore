using SantanaLib.Collections.Concurrent;
﻿using System;
using System.Collections.Concurrent;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.Handlers.MessageHandling
{
    public class SimpleMessageHandler : ChannelHandlerAdapter
    {
        private readonly ConcurrentDictionary<Type, IMessageHandler> _handlers = new ConcurrentDictionary<Type, IMessageHandler>();

        public override async void ChannelRead(IChannelHandlerContext context, object message)
        {
            var release = true;
            try
            {
                var handled = false;
                foreach (var entry in _handlers.Values)
                {
                    var processed = await entry.OnMessageReceived(context, message);
                    if (processed)
                        handled = true;
                }
                if (!handled)
                {
                    release = false;
                    context.FireChannelRead(message);
                }
            }
            catch (Exception ex)
            {
                context.Channel.Pipeline.FireExceptionCaught(ex);
            }
            finally
            {
                if (release)
                    ReferenceCountUtil.Release(message);
            }
        }

        public SimpleMessageHandler Add(IMessageHandler handler)
        {
            if (!_handlers.TryAdd(handler.GetType(), handler))
                throw new ArgumentException("Type already exists");
            return this;
        }

        public T Get<T>() where T : IMessageHandler
        {
            IMessageHandler service;
            _handlers.TryGetValue(typeof(T), out service);
            return (T)service;
        }

        public void Remove<T>() where T : IMessageHandler
        {
            _handlers.Remove(typeof(T));
        }
    }
}
