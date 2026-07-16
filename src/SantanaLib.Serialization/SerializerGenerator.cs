using Sigil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sigil.NonGeneric;

namespace SantanaLib.Serialization
{
    internal class SerializerGenerator
    {
        public const string Key = "SerializerGenerator";
        private static readonly MethodInfo s_getTypeFromHandleMethod = typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), BindingFlags.Public | BindingFlags.Static);

        private readonly Descriptor _descriptor;
        private TypeBuilder _typeBuilder;

        public IList<GeneratedField> Fields { get; }

        public SerializerGenerator(Descriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            _descriptor = descriptor;
            Fields = new List<GeneratedField>();
        }

        public ISerializer Generate()
        {
            if (_descriptor.Serializer != null)
                return _descriptor.Serializer;

            _typeBuilder = Serializer.ModuleBuilder.DefineType($"{_descriptor.Type.FullName}Serializer_{Guid.NewGuid():N}", TypeAttributes.Public | TypeAttributes.Class);
            _typeBuilder.AddInterfaceImplementation(typeof(ISerializer<>).MakeGenericType(_descriptor.Type));

            GenerateSerialize(_typeBuilder);
            GenerateDeserialize(_typeBuilder);
            GenerateCanHandle(_typeBuilder);

            GenerateConstructor(_typeBuilder);

            var serializerType = _typeBuilder.CreateTypeInfo();
            var parameters = Fields.Select(f => (object)f.Serializer).ToArray();
            return (ISerializer)Activator.CreateInstance(serializerType, parameters);
        }

        public GeneratedField CreateField(ISerializer serializer)
        {
            var field = Fields.FirstOrDefault(f => f.Serializer == serializer);
            if (field == null)
            {
                var fieldBuilder = _typeBuilder.DefineField($"_{Guid.NewGuid().ToString("N")}", serializer.GetType(),
                    FieldAttributes.Private | FieldAttributes.Static);
                field = new GeneratedField(fieldBuilder, serializer);
                Fields.Add(field);
            }

            return field;
        }

        private void GenerateConstructor(TypeBuilder typeBuilder)
        {
            var parameters = Fields.Select(f => f.Serializer.GetType()).ToArray();
            var emiter = Emit.BuildConstructor(parameters, typeBuilder, MethodAttributes.Public);

            for (var i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];

                emiter.LoadArgument((ushort) (i+1));
                emiter.StoreField(field.FieldBuilder);
            }
            emiter.Return();
            emiter.CreateConstructor();
        }

        private void GenerateSerialize(TypeBuilder typeBuilder)
        {
            var emiter = Emit.BuildInstanceMethod(typeof(void),
                new[] { typeof(BinaryWriter), _descriptor.Type },
                typeBuilder,
                nameof(ISerializer<object>.Serialize),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
            emiter.SetProperty(Key, this);

            using (var obj = emiter.DeclareLocal(_descriptor.Type, "obj"))
            {
                emiter.LoadArgument(2);
                emiter.StoreLocal(obj);

                if (_descriptor.Compiler != null)
                {
                    _descriptor.Compiler.EmitSerialize(emiter, obj);
                }
                else
                {
                    foreach (var descriptor in _descriptor.GetTree())
                    {
                        foreach (var property in descriptor.Descriptors.Values)
                        {
                            Sigil.Label skipPropertyLabel = null;
                            var propertyType = property.PropertyInfo.PropertyType;

                            var shouldSerializeMethod = descriptor.Type
                                .GetMethod($"ShouldSerialize{property.PropertyInfo.Name}");
                            if (shouldSerializeMethod != null)
                            {
                                skipPropertyLabel = emiter.DefineLabel();

                                emiter.LoadLocal(obj);
                                emiter.Call(shouldSerializeMethod);
                                emiter.BranchIfFalse(skipPropertyLabel);
                            }

                            using (var value = emiter.DeclareLocal(propertyType, "value"))
                            {
                                if (_descriptor.Type.IsValueType)
                                    emiter.LoadLocalAddress(obj);
                                else
                                    emiter.LoadLocal(obj);
                                emiter.Call(property.PropertyInfo.GetMethod);
                                emiter.StoreLocal(value);

                                if (property.Serializer != null)
                                    emiter.CallSerializer(property.Serializer, value);
                                else if (property.Compiler != null)
                                    property.Compiler.EmitSerialize(emiter, value);
                                else
                                    Debug.Assert(false);
                            }

                            if (skipPropertyLabel != null)
                                emiter.MarkLabel(skipPropertyLabel);
                        }
                    }
                }
            }
            emiter.Return();

            var methodBuilder = emiter.CreateMethod();

            var genericType = typeof(ISerializer<>).MakeGenericType(_descriptor.Type);
            typeBuilder.DefineMethodOverride(methodBuilder, genericType.GetMethod(nameof(ISerializer<object>.Serialize)));
        }

        private void GenerateDeserialize(TypeBuilder typeBuilder)
        {
            var emiter = Emit.BuildInstanceMethod(_descriptor.Type,
                new[] { typeof(BinaryReader) },
                typeBuilder,
                nameof(ISerializer<object>.Deserialize),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);
            emiter.SetProperty(Key, this);

            using (var obj = emiter.DeclareLocal(_descriptor.Type, "obj"))
            {
                if (_descriptor.Type.IsValueType)
                {
                    emiter.LoadLocalAddress(obj);
                    emiter.InitializeObject(_descriptor.Type);
                }
                else
                {
                    emiter.NewObject(_descriptor.Type);
                    emiter.StoreLocal(obj);
                }

                if (_descriptor.Compiler != null)
                {
                    _descriptor.Compiler.EmitDeserialize(emiter, obj);
                }
                else
                {
                    foreach (var descriptor in _descriptor.GetTree())
                    {
                        foreach (var property in descriptor.Descriptors.Values)
                        {
                            Sigil.Label skipPropertyLabel = null;
                            var propertyType = property.PropertyInfo.PropertyType;

                            var shouldSerializeMethod = descriptor.Type
                                .GetMethod($"ShouldSerialize{property.PropertyInfo.Name}");
                            if (shouldSerializeMethod != null)
                            {
                                skipPropertyLabel = emiter.DefineLabel();

                                emiter.LoadLocal(obj);
                                emiter.Call(shouldSerializeMethod);
                                emiter.BranchIfFalse(skipPropertyLabel);
                            }

                            using (var value = emiter.DeclareLocal(propertyType, "value"))
                            {
                                if (property.Serializer != null)
                                    emiter.CallDeserializer(property.Serializer, value);
                                else if (property.Compiler != null)
                                    property.Compiler.EmitDeserialize(emiter, value);
                                else
                                    Debug.Assert(false);

                                if (_descriptor.Type.IsValueType)
                                    emiter.LoadLocalAddress(obj);
                                else
                                    emiter.LoadLocal(obj);
                                emiter.LoadLocal(value);
                                emiter.Call(property.PropertyInfo.SetMethod);
                            }

                            if (skipPropertyLabel != null)
                                emiter.MarkLabel(skipPropertyLabel);
                        }
                    }
                }
                emiter.LoadLocal(obj);
                emiter.Return();
            }

            var methodBuilder = emiter.CreateMethod();
            var genericType = typeof(ISerializer<>).MakeGenericType(_descriptor.Type);
            typeBuilder.DefineMethodOverride(methodBuilder, genericType.GetMethod(nameof(ISerializer<object>.Deserialize)));
        }

        private void GenerateCanHandle(TypeBuilder typeBuilder)
        {
            var emiter = Emit<Func<Type, bool>>.BuildInstanceMethod(typeBuilder, nameof(ISerializer.CanHandle), MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot);

            emiter.LoadArgument(1);
            emiter.LoadConstant(_descriptor.Type);
            emiter.Call(s_getTypeFromHandleMethod);
            emiter.CompareEqual();
            emiter.Return();

            var methodBuilder = emiter.CreateMethod();
            typeBuilder.DefineMethodOverride(methodBuilder, typeof(ISerializer).GetMethod(nameof(ISerializer.CanHandle)));
        }
    }

    internal class GeneratedField
    {
        public FieldBuilder FieldBuilder { get; }
        public ISerializer Serializer { get; }

        public GeneratedField(FieldBuilder fieldBuilder, ISerializer serializer)
        {
            FieldBuilder = fieldBuilder;
            Serializer = serializer;
        }
    }
}
