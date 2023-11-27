﻿namespace WhiteEagles.Infrastructure.TransientFaultHandling
{
    using System;

    public class DelegatingRetryIntervalStrategy : RetryIntervalStrategy
    {
        private readonly Func<int, TimeSpan> _func;

        public DelegatingRetryIntervalStrategy(Func<int, TimeSpan> func,
            bool immediateFirstRetry) : base(immediateFirstRetry)
            => _func = func ?? throw new ArgumentNullException(nameof(func));


        protected override TimeSpan GetIntervalFromZeroBasedTick(int tick)
            => _func.Invoke(tick);
    }
}
