using SantanaLib.Collections.Generic;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.Handlers.MessageHandling
{
    public interface IMessageHandler
    {
        Task<bool> OnMessageReceived(IChannelHandlerContext context, object message);
    }

    public abstract class MessageHandler<TKey> : IMessageHandler
    {
        protected delegate void Handler(MessageHandler<TKey> handler, IChannelHandlerContext context, object message);
        protected delegate Task AsyncHandler(MessageHandler<TKey> handler, IChannelHandlerContext context, object message);

        protected IDictionary<TKey, Handler> Handlers;
        protected IDictionary<TKey, AsyncHandler> AsyncHandlers;

        protected MessageHandler()
        {
            RegisterFromAttribute();
        }

        public virtual Task<bool> OnMessageReceived(IChannelHandlerContext context, object message)
        {
            var handler = GetHandler(context, message);
            if (handler != null)
            {
                handler(this, context, message);
                return Task.FromResult(true);
            }

            var asyncHandler = GetAsyncHandler(context, message);
            if (asyncHandler == null)
                return Task.FromResult(false);

            return asyncHandler(this, context, message)
                .ContinueWith((task, _) =>
                {
                    task.Exception?.Rethrow();
                    return true;
                }, null, TaskContinuationOptions.ExecuteSynchronously);
        }

        protected virtual void RegisterHandler(object key, Handler handler)
        {
            if (Handlers == null)
                Handlers = new Dictionary<TKey, Handler>();
            Handlers.Add((TKey)key, handler);
        }

        protected virtual void RegisterAsyncHandler(object key, AsyncHandler handler)
        {
            if (AsyncHandlers == null)
                AsyncHandlers = new Dictionary<TKey, AsyncHandler>();
            AsyncHandlers.Add((TKey)key, handler);
        }

        protected abstract Handler GetHandler(IChannelHandlerContext context, object message);

        protected abstract AsyncHandler GetAsyncHandler(IChannelHandlerContext context, object message);

        protected virtual object GetMessageObject(object message) => message;

        protected virtual bool GetParameter<T>(IChannelHandlerContext context, object message, out T value)
        {
            value = default(T);
            return false;
        }

        private void RegisterFromAttribute()
        {
            var type = GetType();
            var methods = type.GetMethods();
            if (methods.Length == 0)
                return;

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<MessageHandlerAttribute>();
                if (attribute == null)
                    continue;

                var isAsync = typeof(Task).IsAssignableFrom(method.ReturnType);
                var expression = BuildFromMethod(method);
                if (isAsync)
                {
                    var del = (Func<MessageHandler<TKey>, IChannelHandlerContext, object, Task>)expression.Compile();
                    RegisterAsyncHandler(attribute.MessageId, new AsyncHandler(del));
                }
                else
                {
                    var del = (Action<MessageHandler<TKey>, IChannelHandlerContext, object>)expression.Compile();
                    RegisterHandler(attribute.MessageId, new Handler(del));
                }
            }
        }

        private LambdaExpression BuildFromMethod(MethodInfo method)
        {
            var handlerParam = Expression.Parameter(typeof(MessageHandler<TKey>), "messageHandler");
            var contextParam = Expression.Parameter(typeof(IChannelHandlerContext), "channelHandlerContext");
            var messageParam = Expression.Parameter(typeof(object), "message");
            var isAsync = typeof(Task).IsAssignableFrom(method.ReturnType);
            var @this = Expression.Variable(GetType(), "@this");

            IEnumerable<Expression> HandlerParameters(IList<Expression> parameters, IList<ParameterExpression> outVariables)
            {
                var @params = method.GetParameters();
                for (var i = 0; i < @params.Length; i++)
                {
                    var param = @params[i];
                    if (typeof(IChannelHandlerContext).IsAssignableFrom(param.ParameterType))
                    {
                        parameters.Add(Expression.Convert(contextParam, param.ParameterType));
                    }
                    else if (typeof(IChannel).IsAssignableFrom(param.ParameterType))
                    {
                        var prop = Expression.Property(contextParam, nameof(IChannelHandlerContext.Channel));
                        parameters.Add(Expression.Convert(prop, param.ParameterType));
                    }
                    else
                    {
                        var getParameterMethod = GetType()
                            .GetMethod(nameof(GetParameter), BindingFlags.NonPublic | BindingFlags.Instance)
                            .MakeGenericMethod(param.ParameterType);
                        var getMessageObjectMethod = GetType()
                            .GetMethod(nameof(GetMessageObject), BindingFlags.NonPublic | BindingFlags.Instance);

                        var value = Expression.Variable(param.ParameterType, $"value{i}");
                        outVariables.Add(value);

                        var result = Expression.Call(@this, getParameterMethod, contextParam, messageParam, value);
                        parameters.Add(value);

                        yield return Expression.IfThen(Expression.IsFalse(result),
                            Expression.Assign(
                                value,
                                Expression.Convert(
                                    Expression.Call(@this, getMessageObjectMethod, messageParam),
                                    param.ParameterType)
                            )
                        );
                    }
                }
            }

            IEnumerable<Expression> GenerateBody(IList<ParameterExpression> outVariables)
            {
                yield return Expression.Assign(@this, Expression.Convert(handlerParam, GetType()));
                var parameters = new List<Expression>();
                foreach (var expression in HandlerParameters(parameters, outVariables))
                    yield return expression;

                Expression handlerCall = Expression.Call(@this, method, parameters);
                if (isAsync)
                {
                    var returnTarget = Expression.Label(typeof(Task));
                    var returnLabel = Expression.Label(returnTarget, Expression.Constant(default(Task), typeof(Task)));
                    yield return Expression.Block(Expression.Return(returnTarget, handlerCall), returnLabel);
                }
                else
                {
                    yield return handlerCall;
                }
            }

            var variables = new List<ParameterExpression> { @this };
            var body = GenerateBody(variables).ToArray();
            return Expression.Lambda(Expression.Block(variables, body), method.Name, new[] { handlerParam, contextParam, messageParam });
        }
    }

    public class MessageHandler : MessageHandler<Type>
    {
        protected override Handler GetHandler(IChannelHandlerContext context, object message)
        {
            return Handlers?.GetValueOrDefault(message.GetType());
        }

        protected override AsyncHandler GetAsyncHandler(IChannelHandlerContext context, object message)
        {
            return AsyncHandlers?.GetValueOrDefault(message.GetType());
        }
    }
}
