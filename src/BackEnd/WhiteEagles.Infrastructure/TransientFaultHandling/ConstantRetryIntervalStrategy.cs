﻿namespace WhiteEagles.Infrastructure.TransientFaultHandling
{
    using System;

    public class ConstantRetryIntervalStrategy : RetryIntervalStrategy
    {
        public ConstantRetryIntervalStrategy(TimeSpan interval, bool immediateFirstRetry)
            : base(immediateFirstRetry)
            => Interval = interval;

        public ConstantRetryIntervalStrategy(TimeSpan interval)
            : this(interval, immediateFirstRetry: false)
        {
        }

        public TimeSpan Interval { get; }

        protected override TimeSpan GetIntervalFromZeroBasedTick(int tick)
            => Interval;
    }
}
