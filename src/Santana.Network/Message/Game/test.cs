using System;
using System.IO;
using SantanaLib.Serialization;

namespace Santana.Network.Message.Game
{
    public class RawByteArraySerializer : ISerializer
    {
        public bool CanHandle(Type type)
        {
            return type == typeof(byte[]);
        }

        public void Serialize(BinaryWriter writer, object value)
        {
            var data = (byte[])value ?? Array.Empty<byte>();

            writer.Write(data.Length);
            writer.Write(data);
        }

        public object Deserialize(BinaryReader reader, Type type)
        {
            int length = reader.ReadInt32();
            return reader.ReadBytes(length);
        }
    }
}
