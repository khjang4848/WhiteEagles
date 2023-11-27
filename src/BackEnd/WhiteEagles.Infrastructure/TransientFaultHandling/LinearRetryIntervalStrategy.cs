namespace WhiteEagles.Infrastructure.TransientFaultHandling
{
    using System;

    public class LinearRetryIntervalStrategy : RetryIntervalStrategy
    {
        public TimeSpan InitialInterval { get; }

        public TimeSpan Increment { get; }

        public TimeSpan MaximumInterval { get; }


        public LinearRetryIntervalStrategy(
            TimeSpan initialInterval,
            TimeSpan increment,
            TimeSpan maximumInterval,
            bool immediateFirstRetry)
            : base(immediateFirstRetry)
        {
            InitialInterval = initialInterval;
            Increment = increment;
            MaximumInterval = maximumInterval;
        }

        protected override TimeSpan GetIntervalFromZeroBasedTick(int tick)
            => TimeSpan.FromTicks(
                Math.Min(
                    MaximumInterval.Ticks,
                    InitialInterval.Ticks + (Increment.Ticks * tick)));

    }
}
