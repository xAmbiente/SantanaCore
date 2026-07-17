using Sigil;
using System;
using System.IO;
using System.Reflection;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization.Serializers
{
    internal class CharSerializer : ISerializerCompiler
    {
        private static readonly MethodInfo WriteMethod = typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(char) });
        private static readonly MethodInfo ReadMethod = typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadChar));

        public bool CanHandle(Type type)
        {
            return type == typeof(char);
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
