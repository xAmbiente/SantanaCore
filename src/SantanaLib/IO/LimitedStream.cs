using System;
using System.IO;

namespace SantanaLib.IO
{
    public class LimitedStream : Stream
    {
        private readonly Stream _baseStream;
        private long _basePosition;
        private readonly long _startPosition;
        private readonly long _length;

        public override bool CanRead => _baseStream.CanRead;
        public override bool CanSeek => _baseStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _length;

        public override long Position
        {
            get { return _basePosition - _startPosition; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public LimitedStream(Stream stream)
            : this(stream, stream.Position)
        { }

        public LimitedStream(Stream stream, long startPosition)
            : this(stream, startPosition, stream.Length - startPosition)
        { }

        public LimitedStream(Stream stream, long startPosition, long length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (startPosition + length > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            _baseStream = stream;
            _basePosition = startPosition;
            _startPosition = startPosition;
            _length = length;
        }

        public override void Flush()
        { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Current:
                    _basePosition += offset;
                    break;

                case SeekOrigin.Begin:
                    _basePosition = _startPosition + offset;
                    break;

                case SeekOrigin.End:
                    _basePosition = _startPosition + _length + offset;
                    break;
            }
            _basePosition = Math.Max(_basePosition, _startPosition);
            _basePosition = Math.Min(_basePosition, _startPosition + _length);
            return _basePosition;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = (int)Math.Min(Length - Position, count);

            var oldPosition = _baseStream.Position;
            if (oldPosition != _basePosition)
                _baseStream.Position = _basePosition;

            var bytesRead = _baseStream.Read(buffer, offset, count);
            _baseStream.Position = oldPosition;

            if (bytesRead < 0)
                return bytesRead;

            _basePosition += bytesRead;
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _baseStream.Dispose();
            base.Dispose(disposing);
        }
    }
}
