using SantanaLib.IO;
﻿using System;
using System.IO;
using System.Reflection;
using Sigil;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization.Serializers
{
    public class CStringSerializer : ISerializerCompiler
    {
        private static readonly MethodInfo s_writeMethod = typeof(BinaryWriterExtensions)
            .GetMethod(nameof(BinaryWriterExtensions.WriteCString), new[] { typeof(BinaryWriter), typeof(string) });

        private static readonly MethodInfo s_writeMethod2 = typeof(BinaryWriterExtensions)
            .GetMethod(nameof(BinaryWriterExtensions.WriteCString), new[] { typeof(BinaryWriter), typeof(string), typeof(int) });

        private static readonly MethodInfo s_readMethod = typeof(BinaryReaderExtensions)
                .GetMethod(nameof(BinaryReaderExtensions.ReadCString), new[] { typeof(BinaryReader) });

        private static readonly MethodInfo s_readMethod2 = typeof(BinaryReaderExtensions)
                .GetMethod(nameof(BinaryReaderExtensions.ReadCString), new[] { typeof(BinaryReader), typeof(int) });

        private readonly int _length;

        public CStringSerializer()
        { }

        public CStringSerializer(int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            _length = length;
        }

        public bool CanHandle(Type type) => type == typeof(string);

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);

            if (_length > 0)
            {
                emiter.LoadConstant(_length);
                emiter.Call(s_writeMethod2);
            }
            else
            {
                emiter.Call(s_writeMethod);
            }
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);

            if (_length > 0)
            {
                emiter.LoadConstant(_length);
                emiter.Call(s_readMethod2);
            }
            else
            {
                emiter.Call(s_readMethod);
            }

            emiter.StoreLocal(value);
        }
    }
}
