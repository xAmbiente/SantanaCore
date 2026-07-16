using SantanaLib.Collections.Generic;
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SantanaLib.Serialization;

namespace SantanaLib.DotNetty.SimpleRmi
{
    internal static class MessageFactory
    {
        private static readonly IDictionary<string, Type> s_typeLookup = new Dictionary<string, Type>();
        private static readonly IDictionary<Type, string> s_opCodeLookup = new Dictionary<Type, string>();

        static MessageFactory()
        {
            Register(typeof(KeepAliveMessage));
        }

        public static void Register(Type type)
        {
            Debug.Assert(typeof(RmiMessage).IsAssignableFrom(type));
            Debug.Assert(type.GetConstructor(Type.EmptyTypes) != null);

            var opCode = type.FullName;
            s_opCodeLookup.Add(type, opCode);
            s_typeLookup.Add(opCode, type);
        }

        public static RmiMessage GetMessage(string opCode, BinaryReader r)
        {
            var type = s_typeLookup.GetValueOrDefault(opCode);
            if (type == null)
                throw new InvalidMessageException(opCode);

            return (RmiMessage)Serializer.Deserialize(r, type);
        }

        public static string GetOpCode(Type type)
        {
            Debug.Assert(s_opCodeLookup.ContainsKey(type), $"No opcode for {type.FullName} found");
            return s_opCodeLookup[type];
        }
    }
}
