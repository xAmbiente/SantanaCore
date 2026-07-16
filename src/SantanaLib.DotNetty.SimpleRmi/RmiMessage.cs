using System;
using SantanaLib.Serialization;

namespace SantanaLib.DotNetty.SimpleRmi
{
    [SantanaContract]
    public class RmiMessage
    {
        [SantanaMember(0)]
        public Guid Guid { get; set; }
    }

    [SantanaContract]
    public class KeepAliveMessage : RmiMessage
    { }
}