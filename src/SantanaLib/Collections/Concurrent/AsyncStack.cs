using System.Collections.Concurrent;

namespace SantanaLib.Collections.Concurrent
{
    public class AsyncStack<T> : AsyncCollection<T>
    {
        public AsyncStack()
            : base(new ConcurrentStack<T>())
        { }
    }
}
