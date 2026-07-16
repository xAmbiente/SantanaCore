using SantanaLib.Collections.Generic;
﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using SantanaLib.DotNetty.SimpleRmi.Reflection;
using SantanaLib.Serialization;
using Sigil.NonGeneric;

namespace SantanaLib.DotNetty.SimpleRmi.CodeGeneration
{
    internal class MessageFactory<TInterface>
    {
        private static readonly Lazy<IReadOnlyDictionary<MethodInfo, Type>> s_requestTypes = new Lazy<IReadOnlyDictionary<MethodInfo, Type>>(BuildRequests,
            LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly Lazy<IReadOnlyDictionary<MethodInfo, Type>> s_responseTypes = new Lazy<IReadOnlyDictionary<MethodInfo, Type>>(BuildResponses,
            LazyThreadSafetyMode.ExecutionAndPublication);

        public static Type GetRequestType(MethodInfo methodInfo) => s_requestTypes.Value.GetValueOrDefault(methodInfo);
        public static Type GetResponseType(MethodInfo methodInfo) => s_responseTypes.Value.GetValueOrDefault(methodInfo);

        private static IReadOnlyDictionary<MethodInfo, Type> BuildRequests()
        {
            var requests = new Dictionary<MethodInfo, Type>();
            foreach (var info in InterfaceInfo<TInterface>.Methods)
                requests[info.Info] = BuildRequest(info);
            return requests;
        }

        private static IReadOnlyDictionary<MethodInfo, Type> BuildResponses()
        {
            var responses = new Dictionary<MethodInfo, Type>();
            foreach (var info in InterfaceInfo<TInterface>.Methods)
                responses[info.Info] = BuildResponse(info);
            return responses;
        }

        private static Type BuildRequest(RmiMethod method)
        {
            var typeName = $"{typeof(TInterface).FullName.Replace("+", ".")}{method.Info.Name}RequestMessage";
            var typeBuilder = TypeBuilderFactory.Create(typeName, typeof(RmiMessage));

            var contractConstructor = typeof(SantanaContractAttribute).GetConstructor(Type.EmptyTypes);
            var contractAttribute = new CustomAttributeBuilder(contractConstructor, Array.Empty<object>());
            typeBuilder.SetCustomAttribute(contractAttribute);

            var order = 0;
            foreach (var parameter in method.Parameters)
                BuildProperty(typeBuilder, parameter.Info.Name, parameter.Info.ParameterType,
                    order++, parameter.SerializerType, parameter.SerializerParameters);

            var type = typeBuilder.CreateTypeInfo();
            MessageFactory.Register(type);
            return type;
        }

        private static Type BuildResponse(RmiMethod method)
        {
            var returnType = method.Return.Type;
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.GenericTypeArguments.Length == 0)
                    return null;
                returnType = returnType.GenericTypeArguments[0];
            }

            if (returnType == typeof(void))
                return null;

            var typeName = $"{typeof(TInterface).FullName.Replace("+", ".")}{method.Info.Name}ResponseMessage";
            var typeBuilder = TypeBuilderFactory.Create(typeName, typeof(RmiMessage));

            var contractConstructor = typeof(SantanaContractAttribute).GetConstructor(Type.EmptyTypes);
            var contractAttribute = new CustomAttributeBuilder(contractConstructor, Array.Empty<object>());
            typeBuilder.SetCustomAttribute(contractAttribute);

            BuildProperty(typeBuilder, "ReturnValue", returnType,
                0, method.Return.SerializerType, method.Return.SerializerParameters);

            var type = typeBuilder.CreateTypeInfo();
            MessageFactory.Register(type);
            return type;
        }

        private static void BuildProperty(TypeBuilder typeBuilder, string name, Type type, int order, Type serializerType, object[] serializerParameters)
        {
            var memberConstructor = typeof(SantanaMemberAttribute)
                .GetConstructor(new[] { typeof(int), typeof(Type), typeof(object[]) });
            var property = typeBuilder.DefineProperty(name,
                PropertyAttributes.HasDefault, type, null);
            var memberAttribute = new CustomAttributeBuilder(memberConstructor,
                new object[] { order, serializerType, serializerParameters });

            property.SetCustomAttribute(memberAttribute);
            var field = typeBuilder.DefineField("_" + name, type,
                FieldAttributes.Private);

            const MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            var getter = Emit.BuildInstanceMethod(type, Type.EmptyTypes,
                typeBuilder, $"get_{property.Name}", attributes);
            getter.LoadArgument(0);
            getter.LoadField(field);
            getter.Return();
            property.SetGetMethod(getter.CreateMethod());

            var setter = Emit.BuildInstanceMethod(typeof(void), new[] { type },
                typeBuilder, $"set_{property.Name}", attributes);
            setter.LoadArgument(0);
            setter.LoadArgument(1);
            setter.StoreField(field);
            setter.Return();
            setter.CreateMethod();
            property.SetSetMethod(setter.CreateMethod());
        }
    }
}
