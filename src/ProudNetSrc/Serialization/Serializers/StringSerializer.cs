using System;
using System.IO;
using SantanaLib.Reflection;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace ProudNetSrc.Serialization.Serializers
{
  public class StringSerializer : ISerializerCompiler
  {
    public bool CanHandle(Type type)
    {
      throw new NotImplementedException();
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.Call(ReflectionHelper.GetMethod((BinaryReader x) => x.ReadProudString()));
      emiter.LoadConstant(nameof(StringSerializer));
      emiter.Call(typeof(SecurityGuard).GetMethod(nameof(SecurityGuard.EnsureString)));
      emiter.StoreLocal(value);
    }

    public void EmitSerialize(Emit emiter, Local value)
    {
      var write = emiter.DefineLabel(nameof(StringSerializer) + "Write" + Guid.NewGuid());

      emiter.LoadLocal(value);
      emiter.LoadNull();
      emiter.CompareEqual();
      emiter.BranchIfFalse(write);

      emiter.LoadField(typeof(string).GetField(nameof(string.Empty)));
      emiter.StoreLocal(value);

      emiter.MarkLabel(write);
      emiter.LoadArgument(1);
      emiter.LoadLocal(value);
      emiter.LoadConstant(false);
      emiter.Call(ReflectionHelper.GetMethod((BinaryWriter x) =>
          x.WriteProudString(default(string), default(bool))));
    }
  }
}
