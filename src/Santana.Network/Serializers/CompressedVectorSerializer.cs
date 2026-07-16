using System;
using System.IO;
using System.Numerics;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Santana.Network.Serializers
{
  public class CompressedVectorSerializer : ISerializerCompiler
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
          new[] { typeof(BinaryWriter), typeof(Vector3) }));
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.Call(typeof(SantanaExtensions).GetMethod(nameof(SantanaExtensions.ReadCompressedVector3)));
      emiter.StoreLocal(value);
    }
  }
}
