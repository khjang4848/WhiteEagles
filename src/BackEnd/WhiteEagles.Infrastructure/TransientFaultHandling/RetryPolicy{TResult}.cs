﻿namespace WhiteEagles.Infrastructure.TransientFaultHandling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class RetryPolicy<TResult> : RetryPolicy, IRetryPolicy, IRetryPolicy<TResult>
    {
        public new TransientFaultDetectionStrategy<TResult> TransientFaultDetectionStrategy { get; }

        public RetryPolicy(
            int maximumRetryCount,
            TransientFaultDetectionStrategy<TResult> transientFaultDetectionStrategy,
            RetryIntervalStrategy retryIntervalStrategy)
            : base(maximumRetryCount, transientFaultDetectionStrategy,
                retryIntervalStrategy)
        {
            TransientFaultDetectionStrategy = transientFaultDetectionStrategy;
        }

        public static RetryPolicy<TResult> LinearTransientDefault(int maximumRetryCount,
            TimeSpan increment)
        {
            return new(
                maximumRetryCount,
                new TransientDefaultDetectionStrategy<TResult>(),
                new LinearRetryIntervalStrategy(
                    TimeSpan.Zero,
                    increment,
                    maximumInterval: TimeSpan.MaxValue,
                    immediateFirstRetry: false));
        }


        public static RetryPolicy<TResult> ConstantTransientDefault(int maximumRetryCount,
            TimeSpan interval, bool immediateFirstRetry)
        {
            return new(
                maximumRetryCount,
                new TransientDefaultDetectionStrategy<TResult>(),
                new ConstantRetryIntervalStrategy(interval, immediateFirstRetry));
        }


        public Task<TResult> Run(Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            return PerformRun(operation, cancellationToken);
        }

        public Task<TResult> Run<T>(Func<T, CancellationToken, Task<TResult>> operation,
            T arg, CancellationToken cancellationToken)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            return Run(ct => operation.Invoke(arg, ct),
                cancellationToken);
        }

        private async Task<TResult> PerformRun(
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken)
        {
            var retryCount = 0;
            TResult result;
        Try:
            try
            {
                result = await operation.Invoke(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
                when (TransientFaultDetectionStrategy.IsTransientException(exception) 
                      && retryCount < MaximumRetryCount)
            {
                await Task.Delay(RetryIntervalStrategy.GetInterval(retryCount));
                retryCount++;
                goto Try;
            }

            if (!TransientFaultDetectionStrategy.IsTransientResult(result) 
                || retryCount >= MaximumRetryCount)
                return result;
            await Task.Delay(RetryIntervalStrategy.GetInterval(retryCount));
            retryCount++;
            goto Try;

        }

    }
}
