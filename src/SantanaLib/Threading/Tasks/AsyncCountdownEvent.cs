using System;
using System.Threading;
using System.Threading.Tasks;

namespace SantanaLib.Threading.Tasks
{
    public class AsyncCountdownEvent
    {
        private readonly AsyncManualResetEvent _finishedEvent;
        private int _currentCount;

        public int CurrentCount => _currentCount;
        public bool IsSet => _finishedEvent.IsSet;

        public AsyncCountdownEvent()
            : this(0)
        { }

        public AsyncCountdownEvent(int initialCount)
        {
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException(nameof(initialCount));

            _finishedEvent = new AsyncManualResetEvent();
            _currentCount = initialCount;
            if (initialCount < 1)
                _finishedEvent.Set();
        }

        public void Wait()
        {
            Wait(CancellationToken.None);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            WaitAsync(cancellationToken).WaitEx(CancellationToken.None);
        }

        public void SignalAndWait()
        {
            SignalAndWait(CancellationToken.None);
        }

        public void SignalAndWait(CancellationToken cancellationToken)
        {
            SignalAndWaitAsync(CancellationToken.None).WaitEx(CancellationToken.None);
        }

        public Task WaitAsync()
        {
            return WaitAsync(CancellationToken.None);
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return _finishedEvent.WaitAsync(cancellationToken);
        }

        public Task SignalAndWaitAsync()
        {
            return SignalAndWaitAsync(CancellationToken.None);
        }

        public Task SignalAndWaitAsync(CancellationToken cancellationToken)
        {
            Signal();
            return WaitAsync(cancellationToken);
        }

        public void AddCount()
        {
            AddCount(1);
        }

        public void AddCount(int signalCount)
        {
            if (signalCount < 1)
                throw new ArgumentOutOfRangeException(nameof(signalCount));

            ModifyCount(signalCount);
        }

        public void Signal()
        {
            Signal(1);
        }

        public void Signal(int signalCount)
        {
            if (signalCount < 1)
                throw new ArgumentOutOfRangeException(nameof(signalCount));

            ModifyCount(-signalCount);
        }

        private void ModifyCount(int signalCount)
        {
            var spin = new SpinWait();
            while (true)
            {
                var oldCount = _currentCount;
                var newCount = oldCount + signalCount;
                if (newCount < 0)
                    newCount = 0;

                if (Interlocked.CompareExchange(ref _currentCount, newCount, oldCount) == oldCount)
                {
                    if (oldCount < 1 && newCount > 0)
                        _finishedEvent.Reset();

                    if (oldCount > 0 && newCount < 1)
                        _finishedEvent.Set();

                    break;
                }

                spin.SpinOnce();
            }
        }
    }
}
