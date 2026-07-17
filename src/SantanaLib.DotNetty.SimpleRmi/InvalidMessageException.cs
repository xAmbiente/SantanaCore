using System;

namespace SantanaLib.DotNetty.SimpleRmi
{
    public class InvalidMessageException : Exception
    {
        internal InvalidMessageException(ushort opCode)
            : base($"Received invalid message with opCode {opCode}")
        { }

        internal InvalidMessageException(string message)
            : base(message)
        { }
    }
}
