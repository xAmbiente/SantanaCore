using System;
using System.Threading.Tasks;

namespace SantanaLib.Threading.Tasks
{
    public class AsyncLock
    {
        private readonly AsyncSemaphore _semaphore = new AsyncSemaphore(1, 1);
        private readonly IDisposable _releaser;

        public AsyncLock()
        {
            _releaser = new Releaser(this);
        }

        public IDisposable Lock()
        {
            _semaphore.Wait();
            return _releaser;
        }

        public Task<IDisposable> LockAsync()
        {
            return _semaphore.WaitAsync()
                .ContinueWith<IDisposable, IDisposable>((_, state) => state, _releaser);
        }

        internal void Release()
        {
            _semaphore.Release();
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            public Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            public void Dispose()
            {
                _toRelease.Release();
            }
        }
    }
}
