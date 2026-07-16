using System.Reflection;
using SantanaLib.Reflection;

namespace SantanaLib.DotNetty.SimpleRmi.Reflection
{
    internal static class MessageHandlerBaseMembers
    {
        public static readonly MethodInfo Send = ReflectionHelper.GetMethod(() => MessageHandlerBase.Send(null, null));
        public static readonly MethodInfo HandleAsync = ReflectionHelper.GetMethod(() => MessageHandlerBase.HandleAsync(null, null));
        public static readonly MethodInfo HandleWithResponseAsync =
            typeof(MessageHandlerBase).GetMethod(nameof(MessageHandlerBase.HandleWithResponseAsync), BindingFlags.Public | BindingFlags.Static);
    }
}
