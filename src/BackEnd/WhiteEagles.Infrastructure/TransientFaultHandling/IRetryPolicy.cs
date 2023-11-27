namespace WhiteEagles.Infrastructure.TransientFaultHandling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRetryPolicy
    {
        Task Run(Func<CancellationToken, Task> operation,
            CancellationToken cancellation);

        Task Run<T>(Func<T, CancellationToken, Task> operation, T arg,
            CancellationToken cancellation);
    }
}
