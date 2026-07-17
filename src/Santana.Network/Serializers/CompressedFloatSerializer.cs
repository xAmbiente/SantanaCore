using System;
using System.IO;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Santana.Network.Serializers
{
  public class CompressedFloatSerializer : ISerializerCompiler
  {
    public bool CanHandle(Type type)
    {
      throw new NotImplementedException();
    }

    public void EmitSerialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.LoadLocal(value);
      emiter.Call(typeof(SantanaExtensions).GetMethod(nameof(SantanaExtensions.WriteCompressed),
          new[] { typeof(BinaryWriter), typeof(float) }));
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.Call(typeof(SantanaExtensions).GetMethod(nameof(SantanaExtensions.ReadCompressedFloat)));
      emiter.StoreLocal(value);
    }
  }
}
