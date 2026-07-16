using SantanaLib.Collections.Generic;
using SantanaLib.Collections.Concurrent;
using Sigil;
using System;
using System.Collections.Generic;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization
{
    public static class EmitExtensions
    {
        public static void CallSerializer(this Emit @this, ISerializer serializer, Local value)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var properties = @this.GetAttachedProperties();
            var generator = (SerializerGenerator)properties.GetValueOrDefault(SerializerGenerator.Key);
            if (generator == null)
                throw new ArgumentException("Emiter does not belong to a serializer");

            var field = generator.CreateField(serializer);
            @this.LoadField(field.FieldBuilder);
            @this.LoadArgument(1);
            @this.LoadLocal(value);

            @this.CallVirtual(serializer.GetType().GetMethod(nameof(ISerializer<object>.Serialize)));
        }

        public static void CallSerializerForType(this Emit @this, Type type, Local value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var properties = @this.GetAttachedProperties();
            var generator = (SerializerGenerator)properties.GetValueOrDefault(SerializerGenerator.Key);
            if (generator == null)
                throw new ArgumentException("Emiter does not belong to a serializer");

            var compiler = Serializer.GetCompilerForType(type);
            if (compiler != null)
            {
                compiler.EmitSerialize(@this, value);
                return;
            }

            var serializer = Serializer.GetOrCreateSerializer(type);
            if (serializer == null)
                throw new ArgumentException($"No serializer for {type.FullName} available", nameof(type));

            CallSerializer(@this, serializer, value);
        }

        public static void CallDeserializer(this Emit @this, ISerializer serializer, Local value)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var properties = @this.GetAttachedProperties();
            var generator = (SerializerGenerator)properties.GetValueOrDefault(SerializerGenerator.Key);
            if (generator == null)
                throw new ArgumentException("Emiter does not belong to a serializer");

            var field = generator.CreateField(serializer);
            @this.LoadField(field.FieldBuilder);
            @this.LoadArgument(1);
            @this.CallVirtual(serializer.GetType().GetMethod(nameof(ISerializer<object>.Deserialize)));

            @this.StoreLocal(value);
        }

        public static void CallDeserializerForType(this Emit @this, Type type, Local value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var properties = @this.GetAttachedProperties();
            var generator = (SerializerGenerator)properties.GetValueOrDefault(SerializerGenerator.Key);
            if (generator == null)
                throw new ArgumentException("Emiter does not belong to a serializer");

            var compiler = Serializer.GetCompilerForType(type);
            if (compiler != null)
            {
                compiler.EmitDeserialize(@this, value);
                return;
            }

            var serializer = Serializer.GetOrCreateSerializer(type);
            if (serializer == null)
                throw new ArgumentException($"No serializer for {type.FullName} available", nameof(type));

            CallDeserializer(@this, serializer, value);
        }
    }
}