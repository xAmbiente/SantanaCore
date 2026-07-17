using System;
using System.IO;
using Sigil;
using System.Reflection;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization.Serializers
{
    internal class UInt64Serializer : ISerializerCompiler
    {
        private static readonly MethodInfo WriteMethod = typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(ulong) });
        private static readonly MethodInfo ReadMethod = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadUInt64));

        public bool CanHandle(Type type)
        {
            return type == typeof (ulong);
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
