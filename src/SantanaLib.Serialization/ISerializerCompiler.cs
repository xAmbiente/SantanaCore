using System;
using Sigil;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization
{
    public interface ISerializerCompiler
    {
        bool CanHandle(Type type);
        void EmitSerialize(Emit emiter, Local value);
        void EmitDeserialize(Emit emiter, Local value);
    }
}
