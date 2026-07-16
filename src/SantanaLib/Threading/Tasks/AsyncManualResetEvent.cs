using System;
using System.Threading;
using System.Threading.Tasks;

namespace SantanaLib.Threading.Tasks
{
    public class AsyncManualResetEvent
    {
        private volatile TaskCompletionSource _tcs;
        private readonly object _mutex = new object();

        public bool IsSet => _tcs.Task.IsCompleted;

        public AsyncManualResetEvent()
            : this(false)
        { }

        public AsyncManualResetEvent(bool set)
        {
            _tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            if (set)
                _tcs.TrySetResult();
        }

        public void Wait()
        {
            Wait(CancellationToken.None);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            WaitAsync(cancellationToken).WaitEx(CancellationToken.None);
        }

        public Task WaitAsync()
        {
            lock (_mutex)
                return _tcs.Task;
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            lock (_mutex)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Task.FromCanceled(cancellationToken);

                return _tcs.Task.WaitAsync(cancellationToken);
            }
        }

        public void Set()
        {
            lock (_mutex)
                _tcs.TrySetResult();
        }

        public void Reset()
        {
            if (!_tcs.Task.IsCompleted)
                return;

            lock (_mutex)
            {
                if (_tcs.Task.IsCompleted)
                    _tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }
    }
}
