using Sigil;
using System;
using System.IO;
using System.Reflection;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization.Serializers
{
    internal class ByteSerializer : ISerializerCompiler
    {
        private static readonly MethodInfo WriteMethod = typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new [] { typeof(byte) });
        private static readonly MethodInfo ReadMethod = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadByte));

        public bool CanHandle(Type type)
        {
            return type == typeof(byte);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocal(value);
            emiter.CallVirtual(WriteMethod);
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.CallVirtual(ReadMethod);
            emiter.StoreLocal(value);
        }
    }
}