namespace ProudNetSrc.Handlers
{
  using System;
  using System.Threading.Tasks;
  using SantanaLib;
  using SantanaLib.DotNetty.Handlers.MessageHandling;
  using DotNetty.Transport.Channels;

  public class ProudMessageHandler : MessageHandler
  {
    public override async Task<bool> OnMessageReceived(IChannelHandlerContext context, object message)
    {
      try
      {
        var _mt = ((message as RecvContext)?.Message ?? message)?.GetType().Name ?? "";
        if (_mt != "RmiMessage" && _mt != "UnreliablePingMessage" && _mt != "ReliableRelay2Message" &&
            _mt != "UnreliableRelay2Message" && _mt != "ReliablePingMessage" && !_mt.Contains("Move") && !_mt.Contains("P2P"))
          Serilog.Log.Information("[IN] {MessageType}", _mt);
        return await base.OnMessageReceived(context, message);
      }
      catch (Exception e)
      {
        context.FireExceptionCaught(e);
        return false;
      }
    }

    protected override Handler GetHandler(IChannelHandlerContext context, object message)
    {
      var recvContext = (RecvContext)message;
      return base.GetHandler(context, recvContext.Message);
    }

    protected override AsyncHandler GetAsyncHandler(IChannelHandlerContext context, object message)
    {
      var recvContext = (RecvContext)message;
      return base.GetAsyncHandler(context, recvContext.Message);
    }

    protected override object GetMessageObject(object message)
    {
      var context = (RecvContext)message;
      return context.Message;
    }

    protected override bool GetParameter<T>(IChannelHandlerContext context, object message, out T value)
    {
      if (typeof(ProudSession).IsAssignableFrom(typeof(T)))
      {
        var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
        value = DynamicCast<T>.From(session);
        return true;
      }

      if (typeof(ProudServer).IsAssignableFrom(typeof(T)))
      {
        var server = context.Channel.GetAttribute(ChannelAttributes.Server).Get();
        value = DynamicCast<T>.From(server);
        return true;
      }

      if (typeof(RecvContext).IsAssignableFrom(typeof(T)))
      {
        value = DynamicCast<T>.From(message);
        return true;
      }

      return base.GetParameter(context, message, out value);
    }
  }
}
