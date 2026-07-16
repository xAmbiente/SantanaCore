using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProudNetSrc
{
    public interface ISchedulerService
    {
        void Execute(Action action);

        void Execute(Action<object, object> action, object context, object state);

        Task ScheduleAsync(Action action, TimeSpan delay);

        Task ScheduleAsync(Action<object, object> action, object context, object state, TimeSpan delay);

        Task ScheduleAsync(Action<object, object> action, object context, object state,
            TimeSpan delay, CancellationToken cancellationToken);

        Task<T> SubmitAsync<T>(Func<T> func);

        Task<T> SubmitAsync<T>(Func<T> func, CancellationToken cancellationToken);

        Task<T> SubmitAsync<T>(Func<object, T> func, object state);

        Task<T> SubmitAsync<T>(Func<object, T> func, object state, CancellationToken cancellationToken);

        Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state);

        Task<T> SubmitAsync<T>(Func<object, object, T> func, object context, object state, CancellationToken cancellationToken);
    }
}
