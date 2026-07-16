using System;
using System.IO;
using SantanaLib.Reflection;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNetSrc.Serialization.Serializers
{
  public class ArrayWithScalarSerializer : ISerializerCompiler
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

      using (var length = emiter.DeclareLocal<int>("length"))
      {
        emiter.LoadArgument(1);
        emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadScalar()));
        emiter.StoreLocal(length);

        emiter.LoadLocal(length);
        emiter.LoadConstant(nameof(ArrayWithScalarSerializer));
        emiter.Call(typeof(SecurityGuard).GetMethod(nameof(SecurityGuard.EnsureArrayLength)));

        emiter.LoadLocal(length);
        emiter.LoadConstant(1);
        emiter.BranchIfLess(emptyArray);

        emiter.LoadLocal(length);
        emiter.NewArray(elementType);
        emiter.StoreLocal(value);

        if (elementType == typeof(byte))
        {
          emiter.LoadArgument(1);
          emiter.LoadLocal(length);
          emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadBytes(default(int))));
          emiter.StoreLocal(value);
        }
        else
        {
          var loop = emiter.DefineLabel();
          var loopCheck = emiter.DefineLabel();

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
      using (var length = emiter.DeclareLocal<int>("length"))
      {
        emiter.LoadLocal(value);
        emiter.Call(value.LocalType.GetProperty(nameof(Array.Length)).GetMethod);
        emiter.StoreLocal(length);

        emiter.LoadArgument(1);
        emiter.LoadLocal(length);
        emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.WriteScalar(default(int))));

        if (elementType == typeof(byte))
        {
          emiter.LoadArgument(1);
          emiter.LoadLocal(value);
          emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) => x.Write(default(byte[]))));
          return;
        }

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
