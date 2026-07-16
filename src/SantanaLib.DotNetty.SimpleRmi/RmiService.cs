using System.Threading;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi
{
    public abstract class RmiService
    {
        private readonly AsyncLocal<IChannelHandlerContext> _currentContext = new AsyncLocal<IChannelHandlerContext>();

        public IChannelHandlerContext CurrentContext
        {
            get { return _currentContext.Value; }
            internal set { _currentContext.Value = value; }
        }
    }
}