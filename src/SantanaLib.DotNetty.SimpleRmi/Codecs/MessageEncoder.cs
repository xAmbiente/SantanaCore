using SantanaLib.IO;
﻿using System.Collections.Generic;
using System.IO;
using SantanaLib.Serialization;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace SantanaLib.DotNetty.SimpleRmi.Codecs
{
    internal class MessageEncoder : MessageToMessageEncoder<RmiMessage>
    {
        protected override void Encode(IChannelHandlerContext context, RmiMessage message, List<object> output)
        {
            var buffer = context.Allocator.Buffer(16);
            using (var w = new WriteOnlyByteBufferStream(buffer, false).ToBinaryWriter(false))
            {
                var opCode = MessageFactory.GetOpCode(message.GetType());
                w.Write(opCode);
                Serializer.Serialize(w, (object)message);
            }
            output.Add(buffer);
        }
    }
}
