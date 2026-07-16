namespace ProudNetSrc.Handlers
{
  using System;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using SantanaLib;
  using SantanaLib.Collections.Concurrent;
  using DotNetty.Transport.Channels;
  using ProudNetSrc.Codecs;
  using ProudNetSrc.Serialization.Messages.Core;

  internal class UdpHandler : ChannelHandlerAdapter
  {
    private const int MaxUdpPacketsPerWindow = 2000;

    private readonly ProudServer _server;
    private readonly UdpSocket _socket;

    public UdpHandler(UdpSocket socket, ProudServer server)
    {
      _socket = socket;
      _server = server;
    }

    public override void ChannelRead(IChannelHandlerContext context, object obj)
    {
      var message = obj as UdpMessage;
      Debug.Assert(message != null);

      var log = _server.Configuration.Logger?.ForContext("EndPoint", message.EndPoint.ToString());

      try
      {
        var session = _server.SessionsByUdpId.GetValueOrDefault(message.SessionId);
        if (session == null)
        {
          if (message.Content == null || message.Content.ReadableBytes < 1)
            return;

          if (message.Content.GetByte(0) != (byte)ProudCoreOpCode.ServerHolepunch)
          {
            log?.Warning("Datagram from an unknown peer must open with a udp probe, received {MessageType} instead",
                (ProudCoreOpCode)message.Content.GetByte(0));
            return;
          }

          var holepunch = (ServerHolepunchMessage)CoreMessageDecoder.Decode(message.Content);

          session = _server.SessionsByMagic.GetValueOrDefault(holepunch.MagicNumber);

          if (session == null)
          {
            log?.Warning("Udp probe carries a token that matches no pending peer, dropping it");
            return;
          }

          if (session.UdpSocket != _socket)
          {
            log?.Warning("Datagram arrived on a port not assigned to this peer, ignoring it");
            return;
          }

          session.UdpSessionId = message.SessionId;
          session.UdpEndPoint = message.EndPoint;
          _server.SessionsByUdpId[session.UdpSessionId] = session;

          session.SendUdpAsync(new ServerHolepunchAckMessage(session.HolepunchMagicNumber, session.UdpEndPoint));
          return;
        }

        if (session.UdpSocket != _socket)
        {
          log?.Warning("Datagram arrived on a port not assigned to this peer, ignoring it");
          return;
        }

        var nowTick = Environment.TickCount;
        if (nowTick - session.UdpWindowStart >= 1000)
        {
          session.UdpWindowStart = nowTick;
          session.UdpPacketCount = 0;
        }
        if (++session.UdpPacketCount > MaxUdpPacketsPerWindow)
          return;

        var recvContext = new RecvContext
        {
          Message = message.Content.Retain(),
          UdpEndPoint = message.EndPoint
        };

        session.LastUdpPing = DateTimeOffset.Now;
        session.Channel.Pipeline.Context<RecvContextDecoder>().FireChannelRead(recvContext);
      }
      finally
      {
        message.Content.Release();
      }
    }

    public override Task WriteAsync(IChannelHandlerContext context, object message)
    {
      var sendContext = message as SendContext;
      Debug.Assert(sendContext != null);
      var coreMessage = sendContext.Message as ICoreMessage;
      Debug.Assert(coreMessage != null);

      var buffer = context.Allocator.Buffer();
      try
      {
        CoreMessageEncoder.Encode(coreMessage, buffer);

        var udpmessage = new UdpMessage
        {
          Flag = 43981,
          Content = buffer,
          EndPoint = sendContext.UdpEndPoint
        };

        return base.WriteAsync(context, udpmessage);
      }
      catch (Exception ex)
      {
        buffer.Release();
        ex.Rethrow();
        throw;
      }
    }
  }
}
