using System;
using System.IO;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Santana.Network.Serializers
{
  public class ArrayWithShortPrefixSerializer : ISerializerCompiler
  {
    public bool CanHandle(Type type)
    {
      throw new NotImplementedException();
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      var elementType = value.LocalType.GetElementType();
      var emptyArray = emiter.DefineLabel();
      var end = emiter.DefineLabel();

      using (var length = emiter.DeclareLocal<short>("length"))
      {
        emiter.CallDeserializerForType(length.LocalType, length);

        emiter.LoadLocal(length);
        emiter.Convert<int>();
        emiter.LoadConstant(nameof(ArrayWithShortPrefixSerializer));
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
      using (var length = emiter.DeclareLocal<short>("length"))
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
