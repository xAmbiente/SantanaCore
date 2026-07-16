using System.Reflection;
using SantanaLib.Reflection;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi.Reflection
{
    internal static class ProxyBaseMembers
    {
        public static readonly ConstructorInfo Ctor =
            typeof(ProxyBase).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null, new[] { typeof(IChannel) }, null);

        public static readonly MethodInfo Send = ReflectionHelper.GetMethod((ProxyBase p) => p.Send(default(RmiMessage)));
        public static readonly MethodInfo SendAsync = ReflectionHelper.GetMethod((ProxyBase p) => p.SendAsync(default(RmiMessage)));
        public static readonly MethodInfo SendWithResponse = ReflectionHelper.GetMethod((ProxyBase p) => p.SendWithResponse(default(RmiMessage)));
        public static readonly MethodInfo SendWithResponseAsync = typeof(ProxyBase).GetMethod(nameof(ProxyBase.SendWithResponseAsync));
    }
}
