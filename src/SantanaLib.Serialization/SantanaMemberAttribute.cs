using System;

namespace SantanaLib.Serialization
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class SantanaMemberAttribute : Attribute
    {
        public int Order { get; set; }
        public Type SerializerType { get; set; }
        public object[] SerializerParameters { get; set; }

        public SantanaMemberAttribute(int order)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order));

            Order = order;
        }

        public SantanaMemberAttribute(int order, Type serializerType, params object[] serializerParameters)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof(order));

            Order = order;
            SerializerType = serializerType;
            SerializerParameters = serializerParameters;
        }
    }
}
