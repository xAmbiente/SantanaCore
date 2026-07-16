using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SantanaLib.Threading.Tasks
{
    internal class AwaiterQueue
    {
        private readonly LinkedList<TaskCompletionSource> _queue = new LinkedList<TaskCompletionSource>();

        public int Count => _queue.Count;
        public bool IsEmpty => _queue.Count < 1;

        public Task Enqueue()
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _queue.AddLast(tcs);
            return tcs.Task;
        }

        public Task Enqueue(object mutex, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            if (cancellationToken.CanBeCanceled)
            {
                var registration = cancellationToken.Register(state =>
                {
                    lock (mutex)
                    {
                        _queue.Remove(state.Item1);
                        state.Item1.TrySetCanceled(state.Item2);
                    }
                }, Tuple.Create(tcs, cancellationToken), false);
                tcs.Task.ContinueWith<IDisposable>((_, state) => state?.Dispose(), registration, CancellationToken.None);
            }

            _queue.AddLast(tcs);
            return tcs.Task;
        }

        public TaskCompletionSource Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty");

            var item = _queue.First.Value;
            _queue.RemoveFirst();
            return item;
        }

        public bool CompleteOne()
        {
            return Dequeue().TrySetResult();
        }

        public void CompleteAll()
        {
            while (!IsEmpty)
                CompleteOne();
        }
    }

    internal class AwaiterQueue<T>
    {
        private readonly LinkedList<TaskCompletionSource<T>> _queue = new LinkedList<TaskCompletionSource<T>>();

        public int Count => _queue.Count;
        public bool IsEmpty => _queue.Count < 1;

        public Task<T> Enqueue()
        {
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            _queue.AddLast(tcs);
            return tcs.Task;
        }

        public Task<T> Enqueue(object mutex, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<T>(cancellationToken);

            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (cancellationToken.CanBeCanceled)
            {
                var registration = cancellationToken.Register(state =>
                {
                    lock (mutex)
                    {
                        _queue.Remove(state.Item1);
                    }
                    state.Item1.TrySetCanceled(state.Item2);
                }, Tuple.Create(tcs, cancellationToken), false);
                tcs.Task.ContinueWith<IDisposable>((_, state) => state?.Dispose(), registration, CancellationToken.None);
            }

            _queue.AddLast(tcs);
            return tcs.Task;
        }

        public TaskCompletionSource<T> Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty");

            var item = _queue.First.Value;
            _queue.RemoveFirst();
            return item;
        }

        public bool CompleteOne(T result)
        {
            return Dequeue().TrySetResult(result);
        }
    }
}