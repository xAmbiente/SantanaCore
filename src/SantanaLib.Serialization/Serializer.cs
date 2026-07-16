using SantanaLib.IO;
using SantanaLib.Collections.Concurrent;
using SantanaLib.Collections.Generic;
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SantanaLib.Serialization.Serializers;
using Sigil.NonGeneric;
namespace SantanaLib.Serialization
{
    public static class Serializer
    {
        internal static ModuleBuilder ModuleBuilder { get; }
        private static readonly IList<ISerializer> s_serializers = new List<ISerializer>();
        private static readonly IList<ISerializerCompiler> s_compiler = new List<ISerializerCompiler>();
        private static readonly IReadOnlyDictionary<Type, ISerializerCompiler> s_primitiveCompiler;
        private static readonly ConcurrentDictionary<Type, Action<BinaryWriter, object>> s_serializeWithWriterCache = new ConcurrentDictionary<Type, Action<BinaryWriter, object>>();
        private static readonly ConcurrentDictionary<Type, Action<Stream, object>> s_serializeWithStreamCache = new ConcurrentDictionary<Type, Action<Stream, object>>();
        private static readonly ConcurrentDictionary<Type, Func<BinaryReader, object>> s_deserializeWithReaderCache = new ConcurrentDictionary<Type, Func<BinaryReader, object>>();
        private static readonly ConcurrentDictionary<Type, Func<Stream, object>> s_deserializeWithStreamCache = new ConcurrentDictionary<Type, Func<Stream, object>>();
        private static readonly MethodInfo s_serializeWithWriterMethod;
        private static readonly MethodInfo s_serializeWithStreamMethod;
        private static readonly MethodInfo s_deserializeWithReaderMethod;
        private static readonly MethodInfo s_deserializeWithStreamMethod;
        static Serializer()
        {
            const string name = "SantanaLib.Serialization.SerializerAssembly";
            var ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name),
                AssemblyBuilderAccess.Run);
            ModuleBuilder = ab.DefineDynamicModule(name);
            s_serializeWithWriterMethod = typeof(Serializer).GetMethods()
                .First(m =>
                    m.Name == nameof(Serialize) &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(BinaryWriter));
            s_serializeWithStreamMethod = typeof(Serializer).GetMethods()
                .First(m =>
                    m.Name == nameof(Serialize) &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(Stream));
            s_deserializeWithReaderMethod = typeof(Serializer).GetMethods()
                .First(m =>
                    m.Name == nameof(Deserialize) &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(BinaryReader));
            s_deserializeWithStreamMethod = typeof(Serializer).GetMethods()
                .First(m =>
                    m.Name == nameof(Deserialize) &&
                    m.GetGenericArguments().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(Stream));
            s_primitiveCompiler = new Dictionary<Type, ISerializerCompiler>
            {
                {typeof(bool), new BoolSerializer()},
                {typeof(byte), new ByteSerializer()},
                {typeof(char), new CharSerializer()},
                {typeof(decimal), new DecimalSerializer()},
                {typeof(double), new DoubleSerializer()},
                {typeof(float), new FloatSerializer()},
                {typeof(short), new Int16Serializer()},
                {typeof(int), new Int32Serializer()},
                {typeof(long), new Int64Serializer()},
                {typeof(sbyte), new SByteSerializer()},
                {typeof(string), new StringSerializer()},
                {typeof(ushort), new UInt16Serializer()},
                {typeof(uint), new UInt32Serializer()},
                {typeof(ulong), new UInt64Serializer()},
                {typeof(Guid), new GuidSerializer()}
            };
            AddCompiler(new EnumSerializer());
        }
        public static void AddCompiler(ISerializerCompiler compiler)
        {
            if (s_compiler.Contains(compiler))
                throw new ArgumentException("Compiler was already added", nameof(compiler));
            s_compiler.Add(compiler);
        }
        public static void AddSerializer<T>(ISerializer<T> serializer)
        {
            if (s_serializers.Contains(serializer))
                throw new ArgumentException("Serializer was already added", nameof(serializer));
            s_serializers.Add(serializer);
        }
        public static ISerializer<T> GetSerializer<T>()
        {
            var type = typeof(T);
            var serializer = GetOrCreateSerializer(type);
            if (serializer == null)
                throw new ArgumentException($"{type.FullName} has no properties to serialize");
            return (ISerializer<T>)serializer;
        }
        public static void Serialize(BinaryWriter writer, object value)
        {
            var type = value.GetType();
            var func = s_serializeWithWriterCache.GetValueOrDefault(type);
            if (func != null)
            {
                func(writer, value);
                return;
            }
            lock (s_serializeWithWriterCache)
            {
                func = s_serializeWithWriterCache.GetValueOrDefault(type);
                if (func != null)
                {
                    func(writer, value);
                    return;
                }
                var emiter = Emit.NewDynamicMethod(typeof(void), new[] { typeof(BinaryWriter), typeof(object) });
                var method = s_serializeWithWriterMethod.MakeGenericMethod(type);
                EmitSerialize(emiter, type, method);
                func = (Action<BinaryWriter, object>)emiter.CreateDelegate(typeof(Action<BinaryWriter, object>));
                s_serializeWithWriterCache.TryAdd(type, func);
            }
            func(writer, value);
        }
        public static void Serialize(Stream stream, object value)
        {
            var type = value.GetType();
            var func = s_serializeWithStreamCache.GetValueOrDefault(type);
            if (func != null)
            {
                func(stream, value);
                return;
            }
            lock (s_serializeWithStreamCache)
            {
                func = s_serializeWithStreamCache.GetValueOrDefault(type);
                if (func != null)
                {
                    func(stream, value);
                    return;
                }
                var emiter = Emit.NewDynamicMethod(typeof(void), new[] { typeof(Stream), typeof(object) });
                var method = s_serializeWithStreamMethod.MakeGenericMethod(type);
                EmitSerialize(emiter, type, method);
                func = (Action<Stream, object>)emiter.CreateDelegate(typeof(Action<Stream, object>));
                s_serializeWithStreamCache.TryAdd(type, func);
            }
            func(stream, value);
        }
        public static void Serialize<T>(BinaryWriter writer, T value)
        {
            GetSerializer<T>().Serialize(writer, value);
        }
        public static void Serialize<T>(Stream stream, T value)
        {
            using (var writer = stream.ToBinaryWriter(true))
                GetSerializer<T>().Serialize(writer, value);
        }
        public static object Deserialize(BinaryReader reader, Type type)
        {
            var func = s_deserializeWithReaderCache.GetValueOrDefault(type);
            if (func != null)
                return func(reader);
            lock (s_deserializeWithReaderCache)
            {
                func = s_deserializeWithReaderCache.GetValueOrDefault(type);
                if (func != null)
                    return func(reader);
                var emiter = Emit.NewDynamicMethod(typeof(object), new[] { typeof(BinaryReader) });
                var method = s_deserializeWithReaderMethod.MakeGenericMethod(type);
                EmitDeserialize(emiter, type, method);
                func = (Func<BinaryReader, object>)emiter.CreateDelegate(typeof(Func<BinaryReader, object>));
                s_deserializeWithReaderCache.TryAdd(type, func);
            }
            return func(reader);
        }
        public static object Deserialize(Stream stream, Type type)
        {
            var func = s_deserializeWithStreamCache.GetValueOrDefault(type);
            if (func != null)
                return func(stream);
            lock (s_deserializeWithStreamCache)
            {
                func = s_deserializeWithStreamCache.GetValueOrDefault(type);
                if (func != null)
                    return func(stream);
                var emiter = Emit.NewDynamicMethod(typeof(object), new[] { typeof(Stream) });
                var method = s_deserializeWithStreamMethod.MakeGenericMethod(type);
                EmitDeserialize(emiter, type, method);
                func = (Func<Stream, object>)emiter.CreateDelegate(typeof(Func<Stream, object>));
                s_deserializeWithStreamCache.TryAdd(type, func);
            }
            return func(stream);
        }
        public static T Deserialize<T>(BinaryReader reader)
            where T : new()
        {
            return GetSerializer<T>().Deserialize(reader);
        }
        public static T Deserialize<T>(Stream stream)
            where T : new()
        {
            using (var reader = stream.ToBinaryReader(true))
                return GetSerializer<T>().Deserialize(reader);
        }
        internal static ISerializer GetOrCreateSerializer(Type type)
        {
            var descriptor = TypeModel.GetDescriptor(type);
            if (descriptor?.Serializer != null)
                return descriptor.Serializer;
            lock (s_serializers)
            {
                descriptor = TypeModel.GetOrCreateDescriptor(type);
                if (descriptor == null)
                    return null;
                if (descriptor.Serializer != null)
                    return descriptor.Serializer;
                var generator = new SerializerGenerator(descriptor);
                var serializer = generator.Generate();
                if (serializer == null)
                    return null;
                descriptor.Serializer = serializer;
                return serializer;
            }
        }
        internal static ISerializerCompiler GetCompilerForType(Type type)
        {
            return s_primitiveCompiler.GetValueOrDefault(type) ?? s_compiler.FirstOrDefault(compiler => compiler.CanHandle(type));
        }
        internal static ISerializer GetSerializerForType(Type type)
        {
            return s_serializers.FirstOrDefault(serializer => serializer.CanHandle(type));
        }
        private static void EmitSerialize(Emit emiter, Type type, MethodInfo method)
        {
            emiter.LoadArgument(0);
            emiter.LoadArgument(1);
            if (type.IsValueType)
                emiter.UnboxAny(type);
            else
                emiter.CastClass(type);
            emiter.Call(method);
            emiter.Return();
        }
        private static void EmitDeserialize(Emit emiter, Type type, MethodInfo method)
        {
            emiter.LoadArgument(0);
            emiter.Call(method);
            if (type.IsValueType)
                emiter.Box(type);
            else
                emiter.CastClass<object>();
            emiter.Return();
        }
    }
}
