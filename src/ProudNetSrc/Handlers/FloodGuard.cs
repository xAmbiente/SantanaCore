namespace ProudNetSrc.Handlers
{
  using System;
  using DotNetty.Transport.Channels;

  internal class FloodGuard : ChannelHandlerAdapter
  {
    private const int WindowMs = 1000;
    private const int MaxMessagesPerWindow = 1000;

    private readonly ProudServer _server;
    private int _windowStart = Environment.TickCount;
    private int _count;

    public FloodGuard(ProudServer server) => _server = server;

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
      var now = Environment.TickCount;
      if (now - _windowStart >= WindowMs)
      {
        _windowStart = now;
        _count = 0;
      }

      if (++_count > MaxMessagesPerWindow)
      {
        _server.Configuration.Logger?.Warning(
            "Traffic from {EndPoint} broke the ceiling of {Cap} frames per second, dropping the peer",
            context.Channel.RemoteAddress, MaxMessagesPerWindow);
        context.CloseAsync();
        return;
      }

      context.FireChannelRead(message);
    }
  }
}
