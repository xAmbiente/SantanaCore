using SantanaLib.Threading.Tasks;
﻿using System;
using System.Threading.Tasks;
using SantanaLib.DotNetty.Handlers.MessageHandling;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi
{
    public class MessageHandlerBase : MessageHandler
    {
        public static void Send(IChannelHandlerContext context, object message)
        {
            context.Channel.WriteAndFlushAsync(message).WaitEx();
        }

        public static async Task HandleAsync(RmiService rmiService, Task task)
        {
            await task.ConfigureAwait(false);
            rmiService.CurrentContext = null;
        }

        public static async Task HandleWithResponseAsync<TReturn>(RmiService rmiService, IChannelHandlerContext context, RmiMessage message, Task task, Delegate callback)
        {
            var result = await ((Task<TReturn>)task)
                .ConfigureAwait(false);

            rmiService.CurrentContext = null;

            var response = ((Func<TReturn, RmiMessage, RmiMessage>)callback)(result, message);
            await context.Channel.WriteAndFlushAsync(response)
                .ConfigureAwait(false);
        }
    }
}
