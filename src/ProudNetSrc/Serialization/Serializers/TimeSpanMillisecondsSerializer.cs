using SantanaLib.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProudNetSrc.Serialization.Serializers
{
    public class TimeSpanMillisecondsSerializer : ISerializer<TimeSpan>
    {
        public bool CanHandle(Type type)
        {
            return typeof(TimeSpan) == type;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize(BinaryWriter writer, TimeSpan value)
        {
            writer.Write((uint)value.TotalMilliseconds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TimeSpan Deserialize(BinaryReader reader)
        {
            return TimeSpan.FromMilliseconds(reader.ReadUInt32());
        }
    }
}
