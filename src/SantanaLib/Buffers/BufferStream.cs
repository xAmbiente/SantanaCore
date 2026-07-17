using System;
using System.IO;
using System.Collections.Generic;

namespace SantanaLib.Buffers
{
    public class BufferStream : Stream
    {
        private bool _disposed;
        private List<Buffer> _buffers;
        private int _position;
        private int _length;

        public BufferManager BufferManager { get; }
        public override bool CanRead => !_disposed;
        public override bool CanSeek => !_disposed;
        public override bool CanWrite => !_disposed;
        public override long Length => _length;
        public override long Position
        {
            get
            {
                ThrowIfDisposed();
                return _position;
            }
            set
            {
                ThrowIfDisposed();

                if (value < 0 || value > _length)
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (_position == value)
                    return;

                _position = (int)value;
            }
        }

        public BufferStream(BufferManager bufferManager)
            : this(bufferManager, 0)
        { }

        public BufferStream(BufferManager bufferManager, int initialCapacity)
        {
            if (bufferManager == null)
                throw new ArgumentNullException(nameof(bufferManager));

            BufferManager = bufferManager;
            _length = 0;
            _position = 0;
            _buffers = new List<Buffer>(initialCapacity / bufferManager.BufferSize + 1);
        }

        public override void Flush()
        { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfDisposed();

            if (offset > int.MaxValue || offset < int.MinValue)
                throw new ArgumentOutOfRangeException(nameof(offset));

            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = (int)offset;
                    break;

                case SeekOrigin.Current:
                    Position = _position + (int)offset;
                    break;

                case SeekOrigin.End:
                    Position = _length + (int)offset;
                    break;

                default:
                    throw new ArgumentException(nameof(origin));
            }
            return _position;
        }

        public override void SetLength(long value)
        {
            ThrowIfDisposed();

            if (value < 0 || value > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value));

            var newLength = (int)value;
            EnsureCapacity(newLength);
            _length = newLength;

            if (_position > Length)
                _position = newLength;
        }

        public override int Read(byte[] array, int offset, int count)
        {
            ThrowIfDisposed();

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (array.Length - offset < count)
                throw new ArgumentOutOfRangeException();

            count = Math.Min(_length - _position, count);
            if (count <= 0)
                return 0;

            var bytesCopied = 0;
            while (bytesCopied < count)
            {
                int bufferOffset;
                var buffer = GetCurrentBuffer(out bufferOffset);
                var bytesToCopy = Math.Min(count - bytesCopied, BufferManager.BufferSize - bufferOffset);

                Array.Copy(buffer.Array, buffer.Offset + bufferOffset, array, offset + bytesCopied, bytesToCopy);
                bytesCopied += bytesToCopy;
                _position += bytesToCopy;
            }

            return count;
        }

        public override void Write(byte[] array, int offset, int count)
        {
            ThrowIfDisposed();

            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (array.Length - offset < count)
                throw new ArgumentOutOfRangeException();

            var newLength = _position + count;
            if (newLength > _length)
            {
                EnsureCapacity(newLength);
                _length = newLength;
            }

            var bytesCopied = 0;
            while (bytesCopied < count)
            {
                int bufferOffset;
                var buffer = GetCurrentBuffer(out bufferOffset);
                var bytesToCopy = Math.Min(count - bytesCopied, BufferManager.BufferSize - bufferOffset);

                Array.Copy(array, offset + bytesCopied, buffer.Array, buffer.Offset + bufferOffset, bytesToCopy);
                bytesCopied += bytesToCopy;
                _position += bytesToCopy;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                for (var i = 0; i < _buffers.Count; ++i)
                    _buffers[i].Dispose();
                _buffers = null;
            }
            base.Dispose(disposing);
        }

        public virtual byte[] ToArray()
        {
            ThrowIfDisposed();

            var result = new byte[_length];
            var bytesCopied = 0;
            var currentBufferIndex = 0;
            while (bytesCopied < result.Length)
            {
                var count = Math.Min(BufferManager.BufferSize, result.Length - bytesCopied);
                var buffer = _buffers[currentBufferIndex++];
                Array.Copy(buffer.Array, buffer.Offset, result, bytesCopied, count);
                bytesCopied += count;
            }

            return result;
        }

        private Buffer GetCurrentBuffer(out int offset)
        {
            var index = _position / BufferManager.BufferSize;
            offset = (_position - index * BufferManager.BufferSize) % BufferManager.BufferSize;
            return _buffers[index];
        }

        private void EnsureCapacity(int newCapacity)
        {
            var currentCapacity = _buffers.Count * BufferManager.BufferSize;

            if (currentCapacity >= newCapacity)
                return;

            var neededBytes = newCapacity - currentCapacity;
            var neededBuffers = neededBytes / BufferManager.BufferSize + 1;

            for (var i = 0; i < neededBuffers; ++i)
                _buffers.Add(BufferManager.Rent());
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
