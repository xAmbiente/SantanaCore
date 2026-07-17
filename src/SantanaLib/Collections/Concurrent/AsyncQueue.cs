using System.Collections.Concurrent;

namespace SantanaLib.Collections.Concurrent
{
    public class AsyncQueue<T> : AsyncCollection<T>
    {
        public AsyncQueue()
            : base(new ConcurrentQueue<T>())
        { }
    }
}
