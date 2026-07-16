namespace ProudNetSrc.Handlers
{
  using System.Net;
  using DotNetty.Transport.Channels;

  internal class ConnectionThrottle : ChannelHandlerAdapter
  {
    private readonly ProudServer _server;
    private IPAddress _ip;
    private bool _counted;

    public ConnectionThrottle(ProudServer server) => _server = server;

    public override void ChannelActive(IChannelHandlerContext context)
    {
      _ip = (context.Channel.RemoteAddress as IPEndPoint)?.Address;

      if (!_server.TryAddConnection(_ip))
      {
        _server.Configuration.Logger?.Warning(
            "Origin {Ip} is over its concurrent socket allowance of {Cap}, refusing the attempt", _ip, ProudServer.MaxConnectionsPerIp);
        context.CloseAsync();
        return;
      }

      _counted = _ip != null;
      base.ChannelActive(context);
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
      if (_counted)
        _server.RemoveConnection(_ip);
      base.ChannelInactive(context);
    }
  }
}
