using System;
using System.Collections.Generic;
using SantanaLib;

namespace Santana.Network
{
  public class SantanaBadFormatException : Exception
  {
    public SantanaBadFormatException(Type type)
        : base($"Bad format in {type.Name}")
    {
    }

    public SantanaBadFormatException(Type type, IEnumerable<byte> data)
        : base($"Bad format in {type.Name}: {data.ToHexString()}")
    {
    }
  }

  public class SantanaBadOpCodeException : Exception
  {
    public SantanaBadOpCodeException(ushort opCode)
        : base($"Bad opCode: {opCode}")
    {
    }
  }
}
