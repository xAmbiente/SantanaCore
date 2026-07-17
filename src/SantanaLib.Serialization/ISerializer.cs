using System;
using System.IO;

namespace SantanaLib.Serialization
{
    public interface ISerializer
    {
        bool CanHandle(Type type);
    }

    public interface ISerializer<T> : ISerializer
    {
        void Serialize(BinaryWriter writer, T value);
        T Deserialize(BinaryReader reader);
    }
}
