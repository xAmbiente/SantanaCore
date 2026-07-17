using System;
using Sigil;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization.Serializers
{
    public class EnumSerializer : ISerializerCompiler
    {
        private readonly Type _serializeAsType;

        public bool CanHandle(Type type)
        {
            return type.IsEnum;
        }

        public EnumSerializer()
        { }

        public EnumSerializer(Type serializeAsType)
        {
            if(Serializer.GetCompilerForType(serializeAsType) == null)
                throw new ArgumentException($"{serializeAsType.FullName} is not valid for enums", nameof(serializeAsType));

            _serializeAsType = serializeAsType;
        }

        public void EmitDeserialize(Emit emiter, Local value)
        {
            var underlyingType = value.LocalType.GetEnumUnderlyingType();
            var typeToUse = _serializeAsType ?? underlyingType;

            using (var tmp = emiter.DeclareLocal(typeToUse))
            {
                emiter.CallDeserializerForType(typeToUse, tmp);
                emiter.LoadLocal(tmp);
                if (underlyingType != typeToUse)
                    emiter.Convert(underlyingType);
                emiter.StoreLocal(value);
            }
        }

        public void EmitSerialize(Emit emiter, Local value)
        {
            var underlyingType = value.LocalType.GetEnumUnderlyingType();
            var typeToUse = _serializeAsType ?? underlyingType;

            using (var tmp = emiter.DeclareLocal(typeToUse))
            {
                emiter.LoadLocal(value);
                if (underlyingType != typeToUse)
                    emiter.Convert(underlyingType);
                emiter.StoreLocal(tmp);
                emiter.CallSerializerForType(typeToUse, tmp);
            }
        }
    }
}
