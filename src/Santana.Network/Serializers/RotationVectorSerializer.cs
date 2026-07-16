using System;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Santana.Network.Serializers
{
  public class RotationVectorSerializer : ISerializerCompiler
  {
    public bool CanHandle(Type type)
    {
      throw new NotImplementedException();
    }

    public void EmitSerialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.LoadLocal(value);
      emiter.Call(typeof(SantanaExtensions).GetMethod(nameof(SantanaExtensions.WriteRotation)));
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.Call(typeof(SantanaExtensions).GetMethod(nameof(SantanaExtensions.ReadRotation)));
      emiter.StoreLocal(value);
    }
  }
}
