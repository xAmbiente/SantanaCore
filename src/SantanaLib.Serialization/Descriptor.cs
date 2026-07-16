using SantanaLib.Collections.Concurrent;
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SantanaLib.Serialization
{
    internal class Descriptor
    {
        private Queue<Descriptor> _stack;

        public Type Type { get; set; }
        public Descriptor Parent { get; set; }
        public SortedList<int, PropertyDescriptor> Descriptors { get; set; }

        public ISerializer Serializer { get; set; }
        public ISerializerCompiler Compiler { get; set; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Descriptor> GetTree()
        {
            if (_stack == null)
            {
                _stack = new Queue<Descriptor>();
                AddRecursive(this);
            }

            return _stack;
        }

        private void AddRecursive(Descriptor descriptor)
        {
            if (descriptor.Parent != null)
                AddRecursive(descriptor.Parent);
            _stack.Enqueue(descriptor);
        }
    }

    internal class PropertyDescriptor : Descriptor
    {
        public PropertyInfo PropertyInfo { get; set; }
    }

    internal static class TypeModel
    {
        private static readonly ConcurrentDictionary<Type, Descriptor> s_descriptors = new ConcurrentDictionary<Type, Descriptor>();

        public static Descriptor GetDescriptor(Type type)
        {
            return s_descriptors.GetValueOrDefault(type);
        }

        public static Descriptor GetOrCreateDescriptor(Type type)
        {
            var descriptor = GetDescriptor(type) ?? CreateDescriptor(type);
            s_descriptors.TryAdd(type, descriptor);
            return descriptor;
        }

        private static Descriptor CreateDescriptor(Type type)
        {
            Descriptor parent;
            if (type.BaseType != typeof(object) && type.BaseType != typeof(ValueType) && type.BaseType != typeof(Enum))
                parent = GetOrCreateDescriptor(type.BaseType);
            else
                parent = null;

            var attribute = type.GetCustomAttribute<SantanaContractAttribute>();
            if (attribute == null)
                return null;

            var descriptor = new Descriptor { Type = type, Parent = parent };
            if (attribute.SerializerType != null)
            {
                if (typeof(ISerializer).IsAssignableFrom(attribute.SerializerType))
                {
                    descriptor.Serializer = (ISerializer)Activator.CreateInstance(attribute.SerializerType,
                        attribute.SerializerParameters);
                    return descriptor;
                }
                if (typeof(ISerializerCompiler).IsAssignableFrom(attribute.SerializerType))
                {
                    descriptor.Compiler = (ISerializerCompiler)Activator.CreateInstance(attribute.SerializerType,
                        attribute.SerializerParameters);
                    return descriptor;
                }
                throw new Exception($"Invalid serializer assigned to {type.FullName}");
            }

            descriptor.Serializer = Serializer.GetSerializerForType(type);
            if (descriptor.Serializer == null)
            {
                descriptor.Compiler = Serializer.GetCompilerForType(type);
            }

            var properties = type.GetTypeInfo().DeclaredProperties.ToArray();
            descriptor.Descriptors = new SortedList<int, PropertyDescriptor>(properties.Length);
            foreach (var property in properties)
            {
                var propAttrib = property.GetCustomAttribute<SantanaMemberAttribute>();
                if (propAttrib == null)
                    continue;

                if (!property.CanWrite || !property.CanRead)
                    throw new Exception($"Property needs a getter and setter. {property.DeclaringType.FullName}.{property.Name}");

                if (descriptor.Descriptors.ContainsKey(propAttrib.Order))
                    throw new Exception($"Member order is not unique in {descriptor.Type.FullName}");

                if (type == property.PropertyType)
                    throw new NotSupportedException("The current type cant be used as member");

                descriptor.Descriptors.Add(propAttrib.Order, CreateDescriptorFromProperty(property));
            }

            return descriptor;
        }

        private static PropertyDescriptor CreateDescriptorFromProperty(PropertyInfo property)
        {
            var propertyDescriptor = new PropertyDescriptor
            {
                Type = property.PropertyType,
                PropertyInfo = property
            };

            var attribute = property.GetCustomAttribute<SantanaMemberAttribute>();
            if (attribute.SerializerType != null)
            {
                if (typeof(ISerializer).IsAssignableFrom(attribute.SerializerType))
                {
                    propertyDescriptor.Serializer = (ISerializer)Activator.CreateInstance(attribute.SerializerType,
                        attribute.SerializerParameters);
                }
                else if (typeof(ISerializerCompiler).IsAssignableFrom(attribute.SerializerType))
                {
                    propertyDescriptor.Compiler = (ISerializerCompiler)Activator.CreateInstance(attribute.SerializerType,
                        attribute.SerializerParameters);
                }
                else
                {
                    throw new Exception($"Invalid serializer assigned to {property.DeclaringType.FullName}.{property.Name}");
                }
            }
            else
            {
                propertyDescriptor.Compiler = Serializer.GetCompilerForType(property.PropertyType);
                if (propertyDescriptor.Compiler == null)
                {
                    propertyDescriptor.Serializer = Serializer.GetOrCreateSerializer(property.PropertyType);
                    if (propertyDescriptor.Serializer == null)
                    {
                        throw new Exception($"No serializer available for {property.DeclaringType.FullName}.{property.Name}");
                    }
                }
            }
            return propertyDescriptor;
        }
    }
}
