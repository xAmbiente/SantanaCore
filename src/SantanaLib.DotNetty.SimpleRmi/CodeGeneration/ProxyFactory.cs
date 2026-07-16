using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using SantanaLib.DotNetty.SimpleRmi.Reflection;
using DotNetty.Transport.Channels;
using Sigil;
using Sigil.NonGeneric;

namespace SantanaLib.DotNetty.SimpleRmi.CodeGeneration
{
    using Factory = Func<IChannel, ProxyBase>;

    internal static class ProxyFactory<T>
    {
        private static readonly Lazy<Factory> s_instance = new Lazy<Factory>(Build, LazyThreadSafetyMode.ExecutionAndPublication);

        public static ProxyBase Create(IChannel channel) => s_instance.Value(channel);

        private static Factory Build()
        {
            var typeName = $"{InterfaceInfo<T>.Type.FullName}Proxy";
            var typeBuilder = TypeBuilderFactory.Create(typeName, typeof(ProxyBase));
            typeBuilder.AddInterfaceImplementation(InterfaceInfo<T>.Type);

            foreach (var interfaceType in InterfaceInfo<T>.Type.GetInterfaces())
                typeBuilder.AddInterfaceImplementation(interfaceType);

            BuildConstructor(typeBuilder);
            foreach (var method in InterfaceInfo<T>.Methods)
                BuildMethod(typeBuilder, method);

            var proxyType = typeBuilder.CreateTypeInfo();
            var constructor = proxyType.GetConstructor(new[] { typeof(IChannel) });

            var emiter = Emit<Factory>.NewDynamicMethod();
            emiter.LoadArgument(0);
            emiter.NewObject(constructor);
            emiter.Return();

            return emiter.CreateDelegate();
        }

        private static void BuildConstructor(TypeBuilder typeBuilder)
        {
            var emiter = Emit<Action<IChannel>>.BuildConstructor(typeBuilder, MethodAttributes.Public);

            emiter.LoadArgument(0);
            emiter.LoadArgument(1);
            emiter.Call(ProxyBaseMembers.Ctor);
            emiter.Return();
            emiter.CreateConstructor();
        }

        private static void BuildMethod(TypeBuilder typeBuilder, RmiMethod method)
        {
            var parameterTypes = method.Parameters.Select(parameter => parameter.Info.ParameterType).ToArray();
            var emiter = Emit.BuildInstanceMethod(method.Return.Type, parameterTypes,
                typeBuilder, method.Info.Name,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual |
                MethodAttributes.NewSlot);

            var requestType = MessageFactory<T>.GetRequestType(method.Info);
            var responseType = MessageFactory<T>.GetResponseType(method.Info);
            using (var message = emiter.DeclareLocal(requestType))
            {
                emiter.NewObject(requestType);
                emiter.StoreLocal(message);

                emiter.LoadLocal(message);
                emiter.Call(typeof(Guid).GetMethod(nameof(Guid.NewGuid)));
                emiter.Call(typeof(RmiMessage).GetProperty(nameof(RmiMessage.Guid)).SetMethod);

                for (var i = 0; i < method.Parameters.Length; i++)
                {
                    var parameter = method.Parameters[i];

                    emiter.LoadLocal(message);
                    emiter.LoadArgument((ushort)(i + 1));
                    emiter.Call(message.LocalType.GetProperty(parameter.Info.Name).SetMethod);
                }

                emiter.LoadArgument(0);
                emiter.LoadLocal(message);

                if (typeof(Task).IsAssignableFrom(method.Return.Type))
                {
                    if (method.Return.Type == typeof(Task))
                    {
                        emiter.Call(ProxyBaseMembers.SendAsync);
                    }
                    else
                    {
                        var returnType = method.Return.Type.GenericTypeArguments[0];
                        var callbackMethod = BuildCallbackMethod(typeBuilder, method.Info.Name + "Callback", returnType, responseType);

                        emiter.LoadNull();
                        emiter.LoadFunctionPointer(callbackMethod, new[] { typeof(Task<RmiMessage>) });
                        emiter.NewObject(typeof(Func<,>).MakeGenericType(typeof(Task<RmiMessage>), returnType), typeof(object), typeof(IntPtr));
                        emiter.Call(ProxyBaseMembers.SendWithResponseAsync.MakeGenericMethod(returnType));
                    }
                }
                else
                {
                    if (method.Return.Type == typeof(void))
                    {
                        emiter.Call(ProxyBaseMembers.Send);
                    }
                    else
                    {
                        emiter.Call(ProxyBaseMembers.SendWithResponse);
                        emiter.CastClass(responseType);
                        emiter.Call(responseType.GetProperty("ReturnValue").GetMethod);
                    }
                }
            }
            emiter.Return();

            var methodBuilder = emiter.CreateMethod();
            typeBuilder.DefineMethodOverride(methodBuilder, method.Info);
        }

        private static MethodBuilder BuildCallbackMethod(TypeBuilder typeBuilder, string name, Type returnType, Type responseType)
        {
            var emiter = Emit.BuildStaticMethod(returnType, new[] { typeof(Task<RmiMessage>) }, typeBuilder, name,
                MethodAttributes.Private | MethodAttributes.HideBySig);

            emiter.LoadArgument(0);
            emiter.Call(typeof(Task<RmiMessage>).GetProperty(nameof(Task<RmiMessage>.Result)).GetMethod);
            emiter.CastClass(responseType);
            emiter.Call(responseType.GetProperty("ReturnValue").GetMethod);

            emiter.Return();
            return emiter.CreateMethod();
        }
    }
}
