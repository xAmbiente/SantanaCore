using SantanaLib.Threading.Tasks;
﻿using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi
{
    public abstract class ProxyBase
    {
        private readonly IChannel _channel;

        protected ProxyBase(IChannel channel)
        {
            _channel = channel;
        }

        public void Send(RmiMessage request)
        {
            SendAsync(request).WaitEx();
        }

        public Task SendAsync(RmiMessage request)
        {
            return _channel.WriteAndFlushAsync(request);
        }

        public RmiMessage SendWithResponse(RmiMessage request)
        {
            return SendWithResponseAsync(request).WaitEx();
        }

        public Task<T> SendWithResponseAsync<T>(RmiMessage request, Delegate callback)
        {
            return SendWithResponseAsync(request).ContinueWith((task, state) =>
            {
                task.Exception?.Rethrow();

                var c = (Func<Task<RmiMessage>, T>)state;
                return c(task);
            }, callback, TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task<RmiMessage> SendWithResponseAsync(RmiMessage request)
        {
            var responseQueue = _channel.GetAttribute(ChannelAttributes.ResponseQueue).Get();
            var tcs = new TaskCompletionSource<RmiMessage>();
            responseQueue.TryAdd(request.Guid, tcs);
            await _channel.WriteAndFlushAsync(request);
            return await tcs.Task;
        }
    }
}