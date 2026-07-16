using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SantanaLib.DotNetty.SimpleRmi.Reflection
{
    internal static class ServiceInfo<T>
        where T : RmiService
    {
        public static IEnumerable<InterfaceInfo> Interfaces
            => typeof(T).GetInterfaces().Select(type => new InterfaceInfo(type.GetTypeInfo()));

        public static IEnumerable<RmiMethod> Methods => Interfaces.SelectMany(i => i.Methods);
    }
}
