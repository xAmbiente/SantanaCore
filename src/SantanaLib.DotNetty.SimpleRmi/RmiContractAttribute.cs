using System;

namespace SantanaLib.DotNetty.SimpleRmi
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class RmiContractAttribute : Attribute
    { }
}
