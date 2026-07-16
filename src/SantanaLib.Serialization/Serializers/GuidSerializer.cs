using System;
using System.IO;
using Sigil;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization.Serializers
{
    public class GuidSerializer : ISerializerCompiler
    {
        public bool CanHandle(Type type) => type == typeof(Guid);

        public void EmitDeserialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadConstant(16);
            emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
            emiter.NewObject<Guid, byte[]>();
            emiter.StoreLocal(value);
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            emiter.LoadArgument(1);
            emiter.LoadLocalAddress(value);
            emiter.Call(typeof(Guid).GetMethod(nameof(Guid.ToByteArray), Type.EmptyTypes));
            emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(byte[]) }));
        }
    }
}
