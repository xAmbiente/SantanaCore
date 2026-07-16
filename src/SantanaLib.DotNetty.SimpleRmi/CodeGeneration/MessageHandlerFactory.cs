using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using SantanaLib.DotNetty.Handlers.MessageHandling;
using SantanaLib.DotNetty.SimpleRmi.Reflection;
using DotNetty.Transport.Channels;
using Sigil;
using Sigil.NonGeneric;

namespace SantanaLib.DotNetty.SimpleRmi.CodeGeneration
{
    using Factory = Func<RmiService, IMessageHandler>;

    internal static class MessageHandlerFactory<T>
        where T : RmiService
    {
        private static readonly Lazy<Factory> s_instance = new Lazy<Factory>(Build, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IMessageHandler Get(RmiService service) => s_instance.Value(service);

        private static Factory Build()
        {
            var typeName = $"{typeof(T).FullName.Replace("+", ".")}MessageHandler";
            var typeBuilder = TypeBuilderFactory.Create(typeName, typeof(MessageHandlerBase));
            var serviceField = typeBuilder.DefineField("_rmiService", typeof(T), FieldAttributes.Private | FieldAttributes.InitOnly);

            BuildConstructor(typeBuilder, serviceField);
            foreach (var i in ServiceInfo<T>.Interfaces)
            {
                foreach (var method in i.Methods)
                {
                    if (typeof(Task).IsAssignableFrom(method.Return.Type))
                        BuildAsyncMethod(typeBuilder, serviceField, i, method);
                    else
                        BuildMethod(typeBuilder, serviceField, i, method);
                }
            }

            var handlerType = typeBuilder.CreateTypeInfo();
            var constructor = handlerType.GetConstructor(new[] { typeof(RmiService) });

            var emiter = Emit<Factory>.NewDynamicMethod();
            emiter.LoadArgument(0);
            emiter.NewObject(constructor);
            emiter.Return();

            return emiter.CreateDelegate();
        }

        private static void BuildConstructor(TypeBuilder typeBuilder, FieldInfo serviceField)
        {
            var emiter = Emit<Action<RmiService>>.BuildConstructor(typeBuilder, MethodAttributes.Public);

            emiter.LoadArgument(0);
            emiter.Call(typeof(MessageHandlerBase).GetConstructor(Type.EmptyTypes));

            emiter.LoadArgument(0);
            emiter.LoadArgument(1);
            emiter.CastClass(typeof(T));
            emiter.StoreField(serviceField);
            emiter.Return();
            emiter.CreateConstructor();
        }

        private static void BuildMethod(TypeBuilder typeBuilder, FieldInfo serviceField, InterfaceInfo interfaceInfo, RmiMethod method)
        {
            var messageFactoryType = typeof(MessageFactory<>).MakeGenericType(interfaceInfo.Type);

            var requestType = (Type)messageFactoryType
                .GetMethod(nameof(MessageFactory<object>.GetRequestType))
                .Invoke(null, new object[] { method.Info });

            var responseType = (Type)messageFactoryType
                .GetMethod(nameof(MessageFactory<object>.GetResponseType))
                .Invoke(null, new object[] { method.Info });

            var emiter = Emit.BuildInstanceMethod(typeof(void), new[] { typeof(IChannelHandlerContext), requestType },
                typeBuilder, requestType.Name, MethodAttributes.Public | MethodAttributes.HideBySig);

            SetServiceAndSession(emiter, serviceField, false);

            emiter.LoadArgument(0);
            emiter.LoadField(serviceField);
            foreach (var parameter in method.Parameters)
            {
                emiter.LoadArgument(2);
                emiter.Call(requestType.GetProperty(parameter.Name).GetMethod);
            }
            emiter.CallVirtual(method.Info);

            SetServiceAndSession(emiter, serviceField, true);

            if (method.Return.Type != typeof(void))
            {
                using (var returnValue = emiter.DeclareLocal(method.Return.Type))
                using (var responseMessage = emiter.DeclareLocal(responseType))
                {
                    emiter.StoreLocal(returnValue);

                    emiter.NewObject(responseMessage.LocalType);
                    emiter.StoreLocal(responseMessage);

                    emiter.LoadLocal(responseMessage);
                    emiter.LoadArgument(2);
                    emiter.Call(typeof(RmiMessage).GetProperty(nameof(RmiMessage.Guid)).GetMethod);
                    emiter.Call(typeof(RmiMessage).GetProperty(nameof(RmiMessage.Guid)).SetMethod);

                    emiter.LoadLocal(responseMessage);
                    emiter.LoadLocal(returnValue);
                    emiter.Call(responseMessage.LocalType.GetProperty("ReturnValue").SetMethod);

                    emiter.LoadArgument(1);
                    emiter.LoadLocal(responseMessage);
                    emiter.Call(MessageHandlerBaseMembers.Send);
                }
            }
            emiter.Return();

            var methodBuilder = emiter.CreateMethod();
            var attributeConstructor = typeof(MessageHandlerAttribute).GetConstructor(new[] { typeof(object) });
            Debug.Assert(attributeConstructor != null);

            var attributeBuilder = new CustomAttributeBuilder(attributeConstructor, new object[] { requestType });
            methodBuilder.SetCustomAttribute(attributeBuilder);
        }

        private static void BuildAsyncMethod(TypeBuilder typeBuilder, FieldBuilder serviceField, InterfaceInfo interfaceInfo, RmiMethod method)
        {
            var messageFactory = typeof(MessageFactory<>)
                .MakeGenericType(interfaceInfo.Type);

            var requestType = (Type)messageFactory
                .GetMethod(nameof(MessageFactory<object>.GetRequestType), BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { method.Info });

            var responseType = (Type)messageFactory
                .GetMethod(nameof(MessageFactory<object>.GetResponseType), BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { method.Info });

            var emiter = Emit.BuildInstanceMethod(typeof(Task), new[] { typeof(IChannelHandlerContext), requestType },
                typeBuilder, requestType.Name, MethodAttributes.Public | MethodAttributes.HideBySig);

            SetServiceAndSession(emiter, serviceField, false);

            if (method.Return.Type == typeof(Task))
            {
                #region No response

                emiter.LoadArgument(0);
                emiter.LoadField(serviceField);

                emiter.LoadArgument(0);
                emiter.LoadField(serviceField);
                foreach (var parameter in method.Parameters)
                {
                    emiter.LoadArgument(2);
                    emiter.Call(requestType.GetProperty(parameter.Name).GetMethod);
                }

                emiter.CallVirtual(method.Info);
                emiter.Call(MessageHandlerBaseMembers.HandleAsync);

                #endregion
            }
            else
            {
                #region Response

                emiter.LoadArgument(0);
                emiter.LoadField(serviceField);
                emiter.LoadArgument(1);
                emiter.LoadArgument(2);

                emiter.LoadArgument(0);
                emiter.LoadField(serviceField);
                foreach (var parameter in method.Parameters)
                {
                    emiter.LoadArgument(2);
                    emiter.Call(requestType.GetProperty(parameter.Name).GetMethod);
                }
                emiter.CallVirtual(method.Info);

                var returnType = method.Return.Type.GenericTypeArguments[0];
                var callbackMethod = BuildCallbackMethod(typeBuilder, method.Info.Name + "Callback", returnType, responseType);

                emiter.LoadArgument(0);
                emiter.LoadFunctionPointer(callbackMethod, new[] { returnType, typeof(RmiMessage) });
                emiter.NewObject(typeof(Func<,,>).MakeGenericType(returnType, typeof(RmiMessage), typeof(RmiMessage)), typeof(object), typeof(IntPtr));
                emiter.Call(MessageHandlerBaseMembers.HandleWithResponseAsync.MakeGenericMethod(returnType));

                #endregion
            }
            emiter.Return();

            var methodBuilder = emiter.CreateMethod();
            var attributeConstructor = typeof(MessageHandlerAttribute).GetConstructor(new[] { typeof(object) });
            Debug.Assert(attributeConstructor != null);

            var attributeBuilder = new CustomAttributeBuilder(attributeConstructor, new object[] { requestType });
            methodBuilder.SetCustomAttribute(attributeBuilder);
        }

        private static void SetServiceAndSession(Emit emiter, FieldInfo serviceField, bool setNull)
        {
            emiter.LoadArgument(0);
            emiter.LoadField(serviceField);
            if (setNull)
                emiter.LoadNull();
            else
                emiter.LoadArgument(1);
            emiter.Call(typeof(RmiService).GetProperty(nameof(RmiService.CurrentContext)).SetMethod);
        }

        private static MethodBuilder BuildCallbackMethod(TypeBuilder typeBuilder, string name, Type parameterType, Type responseType)
        {
            var emiter = Emit.BuildInstanceMethod(typeof(RmiMessage), new[] { parameterType, typeof(RmiMessage) },
                typeBuilder, name, MethodAttributes.Private | MethodAttributes.HideBySig);

            using (var message = emiter.DeclareLocal(responseType))
            {
                emiter.NewObject(message.LocalType);
                emiter.StoreLocal(message);

                emiter.LoadLocal(message);
                emiter.LoadArgument(2);
                emiter.Call(typeof(RmiMessage).GetProperty(nameof(RmiMessage.Guid)).GetMethod);
                emiter.Call(typeof(RmiMessage).GetProperty(nameof(RmiMessage.Guid)).SetMethod);

                emiter.LoadLocal(message);
                emiter.LoadArgument(1);
                emiter.Call(message.LocalType.GetProperty("ReturnValue").SetMethod);

                emiter.LoadLocal(message);
                emiter.Return();
            }

            return emiter.CreateMethod();
        }
    }
}
