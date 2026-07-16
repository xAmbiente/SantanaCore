using System;
using System.IO;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Santana.Network.Serializers
{
  public class ArrayWithIntPrefixSerializer : ISerializerCompiler
  {
    private readonly ISerializerCompiler _compiler;
    private readonly ISerializer _serializer;

    public ArrayWithIntPrefixSerializer()
    {
    }

    public ArrayWithIntPrefixSerializer(Type serializer, object[] parameters = null)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof(serializer));

      if (typeof(ISerializer).IsAssignableFrom(serializer))
        _serializer = (ISerializer)Activator.CreateInstance(serializer, parameters);
      else if (typeof(ISerializerCompiler).IsAssignableFrom(serializer))
        _compiler = (ISerializerCompiler)Activator.CreateInstance(serializer, parameters);
      else
        throw new ArgumentException($"{serializer.FullName} must be a ISerializer or ISerializerCompiler",
            nameof(serializer));
    }

    public bool CanHandle(Type type)
    {
      throw new NotImplementedException();
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      var elementType = value.LocalType.GetElementType();
      var emptyArray = emiter.DefineLabel();
      var end = emiter.DefineLabel();

      using (var length = emiter.DeclareLocal<int>("length"))
      {
        emiter.CallDeserializerForType(length.LocalType, length);

        emiter.LoadLocal(length);
        emiter.LoadConstant(nameof(ArrayWithIntPrefixSerializer));
        emiter.Call(typeof(PacketSecurity).GetMethod(nameof(PacketSecurity.EnsureArrayLength)));

        emiter.LoadLocal(length);
        emiter.LoadConstant(1);
        emiter.BranchIfLess(emptyArray);

        emiter.LoadLocal(length);
        emiter.NewArray(elementType);
        emiter.StoreLocal(value);

        var loop = emiter.DefineLabel();
        var loopCheck = emiter.DefineLabel();

        if (elementType == typeof(byte))
        {
          emiter.LoadArgument(1);
          emiter.LoadLocal(length);
          emiter.CallVirtual(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadBytes)));
          emiter.StoreLocal(value);
          emiter.Branch(end);
        }
        else
        {
          using (var element = emiter.DeclareLocal(elementType, "element"))
          using (var i = emiter.DeclareLocal<int>("i"))
          {
            emiter.MarkLabel(loop);

            if (_compiler != null)
              _compiler.EmitDeserialize(emiter, element);
            else if (_serializer != null)
              emiter.CallDeserializer(_serializer, element);
            else
              emiter.CallDeserializerForType(elementType, element);

            emiter.LoadLocal(value);
            emiter.LoadLocal(i);
            emiter.LoadLocal(element);
            emiter.StoreElement(elementType);

            emiter.LoadLocal(i);
            emiter.LoadConstant(1);
            emiter.Add();
            emiter.StoreLocal(i);

            emiter.MarkLabel(loopCheck);
            emiter.LoadLocal(i);
            emiter.LoadLocal(length);
            emiter.BranchIfLess(loop);
          }
        }

        emiter.Branch(end);
      }

      emiter.MarkLabel(emptyArray);
      emiter.Call(typeof(Array)
          .GetMethod(nameof(Array.Empty))
          .GetGenericMethodDefinition()
          .MakeGenericMethod(elementType));
      emiter.StoreLocal(value);
      emiter.MarkLabel(end);
    }

    public void EmitSerialize(Emit emiter, Local value)
    {
      var elementType = value.LocalType.GetElementType();
      using (var length = emiter.DeclareLocal<int>("length"))
      {
        emiter.LoadLocal(value);
        emiter.Call(value.LocalType.GetProperty(nameof(Array.Length)).GetMethod);
        emiter.StoreLocal(length);

        emiter.CallSerializerForType(length.LocalType, length);

        var loop = emiter.DefineLabel();
        var loopCheck = emiter.DefineLabel();

        using (var element = emiter.DeclareLocal(elementType, "element"))
        using (var i = emiter.DeclareLocal<int>("i"))
        {
          emiter.Branch(loopCheck);
          emiter.MarkLabel(loop);

          emiter.LoadLocal(value);
          emiter.LoadLocal(i);
          emiter.LoadElement(elementType);
          emiter.StoreLocal(element);

          if (_compiler != null)
            _compiler.EmitSerialize(emiter, element);
          else if (_serializer != null)
            emiter.CallSerializer(_serializer, element);
          else
            emiter.CallSerializerForType(elementType, element);

          emiter.LoadLocal(i);
          emiter.LoadConstant(1);
          emiter.Add();
          emiter.StoreLocal(i);

          emiter.MarkLabel(loopCheck);
          emiter.LoadLocal(i);
          emiter.LoadLocal(length);
          emiter.BranchIfLess(loop);
        }
      }
    }
  }
}
