using SantanaLib.Collections.Generic;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SantanaLib.DotNetty.SimpleRmi.Reflection
{
    internal class InterfaceInfo
    {
        public TypeInfo Type { get; }

        public bool IsRmiInterface => Type.GetCustomAttribute<RmiContractAttribute>() != null;
        public RmiMethod[] Methods => IsRmiInterface
            ? Type.GetMethodsFlattenHierarchy()
                .Where(info => info.GetCustomAttribute<RmiAttribute>() != null)
                .Select(info => new RmiMethod(info))
                .ToArray()
            : null;

        public InterfaceInfo(TypeInfo type)
        {
            Type = type;
        }
    }

    internal static class InterfaceInfo<T>
    {
        private static readonly InterfaceInfo s_info = new InterfaceInfo(typeof(T).GetTypeInfo());

        public static TypeInfo Type => s_info.Type;
        public static bool IsRmiInterface => s_info.IsRmiInterface;
        public static RmiMethod[] Methods => s_info.Methods;
    }

    internal class RmiMethod
    {
        public MethodInfo Info { get; set; }
        public RmiReturn Return { get; set; }
        public RmiParameter[] Parameters { get; set; }

        public RmiMethod(MethodInfo methodInfo)
        {
            Info = methodInfo;
            Return = new RmiReturn(Info);
            Parameters = methodInfo.GetParameters()
                .Select(info => new { param = info, attribute = info.GetCustomAttribute<RmiSerializerAttribute>() })
                .Select(info => new RmiParameter(info.param))
                .ToArray();
        }
    }

    internal class RmiReturn
    {
        public Type Type { get; }
        public Type SerializerType { get; }
        public object[] SerializerParameters { get; }

        public RmiReturn(MethodInfo info)
        {
            Type = info.ReturnType;
            var attribute = info.ReturnTypeCustomAttributes.GetCustomAttributes(false)
                .FirstOfTypeOrDefault<RmiSerializerAttribute>();

            SerializerType = attribute?.SerializerType;
            SerializerParameters = attribute?.SerializerParameters;
        }
    }

    internal class RmiParameter
    {
        public ParameterInfo Info { get; }
        public string Name => Info.Name;
        public Type Type => Info.ParameterType;
        public Type SerializerType => Info.GetCustomAttribute<RmiSerializerAttribute>()?.SerializerType;
        public object[] SerializerParameters => Info.GetCustomAttribute<RmiSerializerAttribute>()?.SerializerParameters;

        public RmiParameter(ParameterInfo info)
        {
            Info = info;
        }
    }
}
