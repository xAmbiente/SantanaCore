using System;

namespace SantanaLib.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        Inherited = false, AllowMultiple = false)]
    public class SantanaContractAttribute : Attribute
    {
        public Type SerializerType { get; set; }
        public object[] SerializerParameters { get; set; }

        public SantanaContractAttribute()
        { }

        public SantanaContractAttribute(Type serializerType, params object[] serializerParameters)
        {
            SerializerType = serializerType;
            SerializerParameters = serializerParameters;
        }
    }
}
