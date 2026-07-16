using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;

namespace Santana.Ipc
{
    public static class IpcExtensions
    {
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(30);

        public static Task SubscribeToRequestAsync<TRequest, TResponse>(this IMessageBus bus,
            Func<TRequest, Task<TResponse>> handler, CancellationToken cancellationToken = default)
            where TRequest : MessageWithGuid
            where TResponse : MessageWithGuid
        {
            return bus.SubscribeAsync((Func<TRequest, Task>)Handle, cancellationToken);

            async Task Handle(TRequest request)
            {
                var response = await handler(request);
                response.Guid = request.Guid;
                await bus.PublishAsync(response);
            }
        }

        public static async Task<TResponse> PublishRequestAsync<TRequest, TResponse>(this IMessageBus bus, TRequest request)
            where TRequest : MessageWithGuid
            where TResponse : MessageWithGuid
        {
            var tcs = new TaskCompletionSource<TResponse>();
            var cts = new CancellationTokenSource();
            request.Guid = Guid.NewGuid();

            await bus.SubscribeAsync<TResponse>(response =>
            {
                if (response.Guid == request.Guid)
                {
                    tcs.TrySetResult(response);
                    cts.Cancel();
                }
            }, cts.Token);

            await bus.PublishAsync(request);

            var timeout = Task.Delay(RequestTimeout);
            if (await Task.WhenAny(timeout, tcs.Task) == timeout)
            {
                cts.Cancel();
                throw new TimeoutException($"Sin respuesta para {typeof(TRequest).Name} en {RequestTimeout.TotalSeconds}s");
            }

            return tcs.Task.Result;
        }
    }
}
