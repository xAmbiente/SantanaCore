
namespace ProudNetSrc.Codecs.Extensions
{
  using System;
  using System.Collections.Generic;

  using DotNetty.Buffers;
  using DotNetty.Codecs;
  using DotNetty.Transport.Channels;

  public class LengthFieldBasedFrameDecoder : ByteToMessageDecoder
  {
    readonly ByteOrder byteOrder;
    readonly int maxFrameLength;
    readonly int lengthFieldOffset;
    readonly int lengthFieldLength;
    readonly int lengthFieldEndOffset;
    readonly int lengthAdjustment;
    readonly int initialBytesToStrip;
    readonly bool failFast;
    bool discardingTooLongFrame;
    long tooLongFrameLength;
    long bytesToDiscard;

    public LengthFieldBasedFrameDecoder(int maxFrameLength, int lengthFieldOffset, int lengthFieldLength)
        : this(maxFrameLength, lengthFieldOffset, lengthFieldLength, 0, 0)
    {
    }

    public LengthFieldBasedFrameDecoder(int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip)
        : this(maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, true)
    {
    }

    public LengthFieldBasedFrameDecoder(int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool failFast)
        : this(ByteOrder.BigEndian, maxFrameLength, lengthFieldOffset, lengthFieldLength, lengthAdjustment, initialBytesToStrip, failFast)
    {
    }

    public LengthFieldBasedFrameDecoder(ByteOrder byteOrder, int maxFrameLength, int lengthFieldOffset, int lengthFieldLength, int lengthAdjustment, int initialBytesToStrip, bool failFast)
    {
      if (maxFrameLength <= 0)
      {
        throw new ArgumentOutOfRangeException(nameof(maxFrameLength), "maxFrameLength must be a positive integer: " + maxFrameLength);
      }
      if (lengthFieldOffset < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(lengthFieldOffset), "lengthFieldOffset must be a non-negative integer: " + lengthFieldOffset);
      }
      if (initialBytesToStrip < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(initialBytesToStrip), "initialBytesToStrip must be a non-negative integer: " + initialBytesToStrip);
      }
      if (lengthFieldOffset > maxFrameLength - lengthFieldLength)
      {
        throw new ArgumentOutOfRangeException(nameof(maxFrameLength), "maxFrameLength (" + maxFrameLength + ") " +
            "must be equal to or greater than " +
            "lengthFieldOffset (" + lengthFieldOffset + ") + " +
            "lengthFieldLength (" + lengthFieldLength + ").");
      }

      this.byteOrder = byteOrder;
      this.maxFrameLength = maxFrameLength;
      this.lengthFieldOffset = lengthFieldOffset;
      this.lengthFieldLength = lengthFieldLength;
      this.lengthAdjustment = lengthAdjustment;
      this.lengthFieldEndOffset = lengthFieldOffset + lengthFieldLength;
      this.initialBytesToStrip = initialBytesToStrip;
      this.failFast = failFast;
    }

    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
      object decoded = this.Decode(context, input);
      if (decoded != null)
      {
        output.Add(decoded);
      }
    }

    protected virtual object Decode(IChannelHandlerContext context, IByteBuffer input)
    {
      if (this.discardingTooLongFrame)
      {
        long bytesToDiscard = this.bytesToDiscard;
        int localBytesToDiscard = (int)Math.Min(bytesToDiscard, input.ReadableBytes);
        input.SkipBytes(localBytesToDiscard);
        bytesToDiscard -= localBytesToDiscard;
        this.bytesToDiscard = bytesToDiscard;

        this.FailIfNecessary(false);
      }

      if (input.ReadableBytes < this.lengthFieldEndOffset)
      {
        return null;
      }

      int actualLengthFieldOffset = input.ReaderIndex + this.lengthFieldOffset;
      long frameLength = this.GetUnadjustedFrameLength(input, actualLengthFieldOffset, this.lengthFieldLength, this.byteOrder);

      if (frameLength < 0)
      {
        input.SkipBytes(this.lengthFieldEndOffset);
        throw new CorruptedFrameException("negative pre-adjustment length field: " + frameLength);
      }

      frameLength += this.lengthAdjustment + this.lengthFieldEndOffset;

      if (frameLength < this.lengthFieldEndOffset)
      {
        input.SkipBytes(this.lengthFieldEndOffset);
        throw new CorruptedFrameException("Adjusted frame length (" + frameLength + ") is less " +
            "than lengthFieldEndOffset: " + this.lengthFieldEndOffset);
      }

      if (frameLength > this.maxFrameLength)
      {
        long discard = frameLength - input.ReadableBytes;
        this.tooLongFrameLength = frameLength;

        if (discard < 0)
        {
          input.SkipBytes((int)frameLength);
        }
        else
        {
          this.discardingTooLongFrame = true;
          this.bytesToDiscard = discard;
          input.SkipBytes(input.ReadableBytes);
        }
        this.FailIfNecessary(true);
        return null;
      }

      int frameLengthInt = (int)frameLength;
      if (input.ReadableBytes < frameLengthInt)
      {
        return null;
      }

      if (this.initialBytesToStrip > frameLengthInt)
      {
        input.SkipBytes(frameLengthInt);
        throw new CorruptedFrameException("Adjusted frame length (" + frameLength + ") is less " +
            "than initialBytesToStrip: " + this.initialBytesToStrip);
      }
      input.SkipBytes(this.initialBytesToStrip);

      int readerIndex = input.ReaderIndex;
      int actualFrameLength = frameLengthInt - this.initialBytesToStrip;
      IByteBuffer frame = this.ExtractFrame(context, input, readerIndex, actualFrameLength);
      input.SetReaderIndex(readerIndex + actualFrameLength);
      return frame;
    }

    protected virtual long GetUnadjustedFrameLength(IByteBuffer buffer, int offset, int length, ByteOrder order)
    {
      long frameLength;
      switch (length)
      {
        case 1:
          frameLength = buffer.GetByte(offset);
          break;
        case 2:
          frameLength = order == ByteOrder.BigEndian ? buffer.GetUnsignedShort(offset) : buffer.GetUnsignedShortLE(offset);
          break;
        case 3:
          frameLength = order == ByteOrder.BigEndian ? buffer.GetUnsignedMedium(offset) : buffer.GetUnsignedMediumLE(offset);
          break;
        case 4:
          frameLength = order == ByteOrder.BigEndian ? buffer.GetInt(offset) : buffer.GetIntLE(offset);
          break;
        case 8:
          frameLength = order == ByteOrder.BigEndian ? buffer.GetLong(offset) : buffer.GetLongLE(offset);
          break;
        default:
          throw new DecoderException("unsupported lengthFieldLength: " + this.lengthFieldLength + " (expected: 1, 2, 3, 4, or 8)");
      }
      return frameLength;
    }

    protected virtual IByteBuffer ExtractFrame(IChannelHandlerContext context, IByteBuffer buffer, int index, int length)
    {
      IByteBuffer buff = buffer.Slice(index, length);
      buff.Retain();
      return buff;
    }

    void FailIfNecessary(bool firstDetectionOfTooLongFrame)
    {
      if (this.bytesToDiscard == 0)
      {
        long tooLongFrameLength = this.tooLongFrameLength;
        this.tooLongFrameLength = 0;
        this.discardingTooLongFrame = false;
        if (!this.failFast ||
            this.failFast && firstDetectionOfTooLongFrame)
        {
          this.Fail(tooLongFrameLength);
        }
      }
      else
      {
        if (this.failFast && firstDetectionOfTooLongFrame)
        {
          this.Fail(this.tooLongFrameLength);
        }
      }
    }

    void Fail(long frameLength)
    {
      if (frameLength > 0)
      {
        throw new TooLongFrameException("Adjusted frame length exceeds " + this.maxFrameLength +
            ": " + frameLength + " - discarded");
      }
      else
      {
        throw new TooLongFrameException(
            "Adjusted frame length exceeds " + this.maxFrameLength +
                " - discarding");
      }
    }
  }
}
