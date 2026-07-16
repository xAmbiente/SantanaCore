using System.Collections.Concurrent;

namespace SantanaLib.Collections.Concurrent
{
    public class AsyncBag<T> : AsyncCollection<T>
    {
        public AsyncBag()
            : base(new ConcurrentBag<T>())
        { }
    }
}
