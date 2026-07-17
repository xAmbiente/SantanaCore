using System;
using System.Collections.Concurrent;

namespace SantanaLib.Buffers
{
    public class BufferManager
    {
        public static BufferManager Default { get; set; } = new BufferManager(1024, 1024, true);

        private readonly ConcurrentStack<Buffer> _bufferPool = new ConcurrentStack<Buffer>();

        public bool CanGrow { get; }
        public int BufferSize { get; }
        public int BufferCount { get; }
        public int AvailableBuffers => _bufferPool.Count;

        public BufferManager(int bufferSize, int bufferCount, bool canGrow)
        {
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            if (bufferCount < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferCount));

            BufferSize = bufferSize;
            BufferCount = bufferCount;

            CanGrow = true;
            AllocateNewBuffers();
            CanGrow = canGrow;
        }

        public Buffer Rent()
        {
            Buffer buffer;
            if (_bufferPool.TryPop(out buffer))
            {
                buffer.IsUnused = false;
                return buffer;
            }

            if (CanGrow)
            {
                lock (_bufferPool)
                {
                    if (_bufferPool.TryPop(out buffer))
                    {
                        buffer.IsUnused = false;
                        return buffer;
                    }
                    AllocateNewBuffers();
                }

                while (!_bufferPool.TryPop(out buffer)) { }
                buffer.IsUnused = false;
                return buffer;
            }

            throw new OutOfMemoryException("No buffers available and BufferManager is not allowed to allocate new memory. Set canGrow to true or increase buffer count");
        }

        public void Return(Buffer buffer)
        {
            if (buffer.BufferManager != this)
                throw new ArgumentException("The buffer is not owned by this BufferManager", nameof(buffer));

            _bufferPool.Push(buffer);
            buffer.IsUnused = true;
        }

        private void AllocateNewBuffers()
        {
            if (!CanGrow)
                throw new OutOfMemoryException("BufferManager is not allowed to allocate new memory. Set canGrow to true or increase buffer count");

            var segment = new byte[BufferSize * BufferCount];
            for (var i = 0; i < BufferCount; ++i)
            {
                var offset = i * BufferSize;
                _bufferPool.Push(new Buffer(this, segment, offset, BufferSize));
            }
        }
    }
}
