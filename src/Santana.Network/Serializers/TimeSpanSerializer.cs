using System;
using System.IO;
using SantanaLib.Serialization;
using Sigil;
using Sigil.NonGeneric;

namespace Santana.Network.Serializers
{
  public class TimeSpanSerializer : ISerializerCompiler
  {
    public bool CanHandle(Type type)
    {
      throw new NotImplementedException();
    }

    public void EmitSerialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.LoadLocalAddress(value);
      emiter.Call(typeof(TimeSpan).GetProperty(nameof(TimeSpan.TotalMinutes)).GetMethod);
      emiter.Convert<byte>();
      emiter.CallVirtual(typeof(BinaryWriter).GetMethod(nameof(BinaryWriter.Write), new[] { typeof(byte) }));
    }

    public void EmitDeserialize(Emit emiter, Local value)
    {
      emiter.LoadArgument(1);
      emiter.Call(typeof(BinaryReader).GetMethod(nameof(BinaryReader.ReadByte)));
      emiter.Convert<double>();
      emiter.Call(typeof(TimeSpan).GetMethod(nameof(TimeSpan.FromMinutes), new[] { typeof(double) }));
      emiter.StoreLocal(value);
    }
  }
}
