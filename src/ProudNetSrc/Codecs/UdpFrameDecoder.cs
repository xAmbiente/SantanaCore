namespace ProudNetSrc.Codecs
{
  using System.Collections.Generic;
  using System.Net;
  using DotNetty.Codecs;
  using DotNetty.Transport.Channels;
  using DotNetty.Transport.Channels.Sockets;

  internal class UdpFrameDecoder : MessageToMessageDecoder<DatagramPacket>
  {
    private readonly int _maxFrameLength;

    public UdpFrameDecoder(int maxFrameLength)
    {
      _maxFrameLength = maxFrameLength;
    }

    protected override void Decode(IChannelHandlerContext context, DatagramPacket message, List<object> output)
    {
      var content = message.Content;
      if (content == null || content.ReadableBytes < 19)
        throw new ProudException($"{nameof(UdpFrameDecoder)}: packet too small ({content?.ReadableBytes ?? 0})");

      var flag = content.ReadUnsignedShortLE();
      var sessionId = content.ReadUnsignedShortLE();
      var length = content.ReadIntLE();
      var id = content.ReadUnsignedIntLE();
      var fragId = content.ReadUnsignedIntLE();

      if (length < 0 || length > _maxFrameLength)
        throw new TooLongFrameException("Received message is too long");

      if (content.ReadableBytes < 3)
        throw new ProudException($"{nameof(UdpFrameDecoder)}: missing struct payload");

      var buffer = content
          .SkipBytes(2)
          .ReadStruct();

      content.Retain();

      var endPoint = (IPEndPoint)message.Sender;
      output.Add(new UdpMessage
      {
        Flag = flag,
        SessionId = sessionId,
        Length = length,
        Id = id,
        FragId = fragId,
        Content = buffer,
        EndPoint = new IPEndPoint(endPoint.Address.MapToIPv4(), endPoint.Port)
      });
    }
  }
}
