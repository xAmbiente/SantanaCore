using SantanaLib.IO;
﻿using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi.Codecs
{
    internal class MessageDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer buffer, List<object> output)
        {
            using (var r = new ReadOnlyByteBufferStream(buffer, false).ToBinaryReader(false))
            {
                var opCode = r.ReadString();
                var message = MessageFactory.GetMessage(opCode, r);
                output.Add(message);
            }
        }
    }
}
