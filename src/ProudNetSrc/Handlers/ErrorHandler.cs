namespace ProudNetSrc.Handlers
{
  using System;
  using System.Net.Sockets;
  using DotNetty.Codecs;
  using DotNetty.Transport.Channels;

  internal class ErrorHandler : ChannelHandlerAdapter
  {
    private readonly ProudServer _server;

    public ErrorHandler(ProudServer server)
    {
      _server = server;
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
      if (exception == null)
        return;

      var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
      var exc = exception.GetBaseException();

      if (IsProtocolOrConnectionException(exception) || IsProtocolOrConnectionException(exc))
      {
        Serilog.Log.Warning(exception, "Tearing down peer {HostId} after a transport fault of kind {Type} ({Msg})",
            session?.HostId, exc.GetType().Name, exc.Message);
        session?.CloseAsync();
        return;
      }

      _server.Configuration.Logger?.Error(exception, "Fault escaped the pipeline without a handler");
      _server.RaiseError(new ErrorEventArgs(session, exception));
    }

    private static bool IsProtocolOrConnectionException(Exception e)
    {
      return e is SocketException
          || e is ClosedChannelException
          || e is ProudFrameException
          || e is ProudException
          || e is DecoderException
          || e is CorruptedFrameException
          || e is TooLongFrameException
          || e is System.IO.EndOfStreamException;
    }
  }
}
