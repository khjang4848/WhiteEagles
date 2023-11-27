﻿namespace WhiteEagles.Test.TransientFaultHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoFixture.AutoMoq;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using WhiteEagles.Infrastructure.TransientFaultHandling;


    [TestClass]
    public class RetryPolicy_specs
    {
        public interface IFunctionProvider
        {
            TResult Func<T, TResult>(T arg);
            TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
        }


        [TestMethod]
        public void sut_has_guard_clauses()
        {
            var sut = typeof(RetryPolicy);
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            new GuardClauseAssertion(fixture).Verify(sut);

        }

        [TestMethod]
        public void sut_implements_IRetryPolicy()
            => typeof(RetryPolicy).Should().Implement<IRetryPolicy>();

        [TestMethod]
        public void constructor_sets_properties_correctly()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var maximumRetryCount = fixture.Create<int>();
            var detectionStrategy = fixture.Create<TransientFaultDetectionStrategy>();
            var intervalStrategy = fixture.Create<RetryIntervalStrategy>();

            var sut = new RetryPolicy(maximumRetryCount, detectionStrategy,
                intervalStrategy);

            sut.MaximumRetryCount.Should().Be(maximumRetryCount);
            sut.TransientFaultDetectionStrategy.Should().BeSameAs(detectionStrategy);
            sut.RetryIntervalStrategy.Should().BeSameAs(intervalStrategy);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-10)]
        public void constructor_has_guard_clause_against_negative_maximumRetryCount(
            int maximumRetryCount)
        {
            TransientFaultDetectionStrategy transientFaultDetectionStrategy
                = new DelegatingTransientFaultDetectionStrategy(exception => true);
            RetryIntervalStrategy retryIntervalStrategy
                = new DelegatingRetryIntervalStrategy(
                    retried => TimeSpan.FromMilliseconds(1),
                    false);


            Action action = () => new RetryPolicy(maximumRetryCount,
                transientFaultDetectionStrategy, retryIntervalStrategy);

            action.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x
                    => x.ParamName == "maximumRetryCount");
        }


        [TestMethod]
        [DataRow(0, true)]
        [DataRow(1, true)]
        [DataRow(10, true)]
        [DataRow(0, false)]
        [DataRow(1, false)]
        [DataRow(10, false)]
        public async Task Run_invoke_operation_at_least_once(int maximumRetryCount,
            bool canceled)
        {
            TransientFaultDetectionStrategy transientFaultDetectionStrategy =
                new DelegatingTransientFaultDetectionStrategy(exception => true);

            RetryIntervalStrategy retryIntervalStrategy =
                new DelegatingRetryIntervalStrategy(
                    retried => TimeSpan.FromMilliseconds(1),
                    immediateFirstRetry: false);

            var sut = new RetryPolicy(
                maximumRetryCount,
                transientFaultDetectionStrategy,
                retryIntervalStrategy);

            var functionProvider = Mock.Of<IFunctionProvider>();
            var cancellationToken = new CancellationToken(canceled);

            await sut.Run(functionProvider.Func<CancellationToken, Task>, cancellationToken);

            Mock.Get(functionProvider).Verify(x =>
                x.Func<CancellationToken, Task>(cancellationToken), Times.AtLeastOnce());

        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Run_invokes_operation_repeatedly_until_succeeds(bool canceled)
        {
            var generator = new Generator<int>(new Fixture());
            var transientFaultCount = generator.First(x => x > 0);
            var maximumRetryCount = transientFaultCount + generator.First(x => x > 0);
            var oper = new EventualSuccessOperator(transientFaultCount);
            var sut = new RetryPolicy(
                maximumRetryCount,
                new TransientFaultDetectionStrategy(),
                new ConstantRetryIntervalStrategy(TimeSpan.Zero));
            var cancellationToken = new CancellationToken(canceled);


            await sut.Run(oper.Operation, cancellationToken);


            Mock.Get(oper.FunctionProvider).Verify(x => x.Func<CancellationToken, Task>(cancellationToken), Times.Exactly(transientFaultCount + 1));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task RunT_throws_exception_if_retry_count_reaches_maximumRetryCount(bool canceled)
        {
            var generator = new Generator<int>(new Fixture());
            var maximumRetryCount = generator.First(x => x > 0);
            var transientFaultCount = maximumRetryCount + generator.First(x => x > 0);
            var exceptions = Enumerable.Range(0, transientFaultCount)
                .Select(_ => new Exception()).ToArray();

            var oper = new EventualSuccessOperator(transientFaultCount,
                triedTimes => exceptions[triedTimes]);

            var sut = new RetryPolicy(maximumRetryCount, new TransientFaultDetectionStrategy(),
                new ConstantRetryIntervalStrategy(TimeSpan.Zero));

            var arg = new Arg();
            var cancellationToken = new CancellationToken(canceled);

            Func<Task> action = () => sut.Run(oper.Operation, arg, cancellationToken);

            var actionResult = await action.Should().ThrowAsync<Exception>();

            actionResult.Which.Should().BeSameAs(exceptions[maximumRetryCount]);
            
            Mock.Get(oper.FunctionProvider).Verify(x =>
                x.Func<Arg, CancellationToken, Task>(arg, cancellationToken),
                Times.Exactly(maximumRetryCount + 1));
        }


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task RunT_throws_exception_immediately_if_not_transient(bool canceled)
        {
            var exception = new Exception();
            var maximumRetryCount = 2;
            var oper = new EventualSuccessOperator(
                transientFaultCount: 1,
                transientExceptionFactory: () => exception);
            var sut = new RetryPolicy(
                maximumRetryCount,
                new DelegatingTransientFaultDetectionStrategy(x => x != exception),
                new ConstantRetryIntervalStrategy(TimeSpan.Zero));
            var arg = new Arg();
            var cancellationToken = new CancellationToken(canceled);

            Func<Task> action = () => sut.Run(oper.Operation, arg, cancellationToken);

            var actionResult = await action.Should().ThrowAsync<Exception>();

            actionResult.Which.Should().BeSameAs(exception);
            
            Mock.Get(oper.FunctionProvider).Verify(x
                => x.Func<Arg, CancellationToken, Task>(arg, cancellationToken),
                Times.Once());
        }

        [TestMethod]
        public async Task RunT_delays_before_retry()
        {
            var generator = new Generator<int>(new Fixture());
            var maximumRetryCount = generator.First(x => x < 10);
            var delays = Enumerable
                .Range(0, maximumRetryCount)
                .Select(_ => TimeSpan.FromMilliseconds(new Fixture().Create<int>()))
                .ToArray();
            var spy = new EventualSuccessOperator(transientFaultCount: maximumRetryCount);
            var sut = new RetryPolicy(
                maximumRetryCount,
                new TransientFaultDetectionStrategy(),
                new DelegatingRetryIntervalStrategy(t => delays[t], false));


            await sut.Run(spy.Operation, new Arg(), CancellationToken.None);


            for (var i = 0; i < spy.Invocations.Count - 1; i++)
            {
                var actual = spy.Invocations[i + 1] - spy.Invocations[i];
                var expected = delays[i];
                actual.Should().BeGreaterOrEqualTo(expected);
                var precision =
#if DEBUG
                    100;
#else
                    20;
#endif
                actual.Should().BeCloseTo(expected, TimeSpan.FromMilliseconds(precision));
            }
        }


        [TestMethod]
        public void Linear_builds_RetryPolicy_correctly()
        {
            var fixture = new Fixture();
            var maximumRetryCount = fixture.Create<int>();
            var increment = fixture.Create<TimeSpan>();

            var actual = RetryPolicy.Linear(maximumRetryCount, increment);

            actual.Should().NotBeNull();
            actual.MaximumRetryCount.Should().Be(maximumRetryCount);
            actual.TransientFaultDetectionStrategy.Should().BeOfType<TransientFaultDetectionStrategy>();
            actual.RetryIntervalStrategy.Should().Match<LinearRetryIntervalStrategy>(
                x =>
                    x.MaximumInterval == TimeSpan.MaxValue &&
                    x.InitialInterval == TimeSpan.Zero &&
                    x.Increment == increment &&
                    x.ImmediateFirstRetry == false);
        }


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Constant_builds_RetryPolicy_correctly(bool immediateFirstRetry)
        {
            var fixture = new Fixture();
            var maximumRetryCount = fixture.Create<int>();
            var interval = fixture.Create<TimeSpan>();

            var actual = RetryPolicy.Constant(maximumRetryCount, interval, immediateFirstRetry);

            actual.Should().NotBeNull();
            actual.MaximumRetryCount.Should().Be(maximumRetryCount);
            actual.TransientFaultDetectionStrategy.Should().BeOfType<TransientFaultDetectionStrategy>();
            actual.RetryIntervalStrategy.Should().Match<ConstantRetryIntervalStrategy>(
                x =>
                    x.Interval == interval &&
                    x.ImmediateFirstRetry == immediateFirstRetry);
        }


        public class Arg
        {
        }

        public class EventualSuccessOperator
        {
            private readonly int _transientFaultCount;
            private readonly Func<int, Exception> _transientExceptionFactory;
            private readonly List<DateTime> _invocations;
            private readonly IFunctionProvider _functionProvider;

            public EventualSuccessOperator(int transientFaultCount,
                Func<int, Exception> transientExceptionFactory)
            {
                _transientFaultCount = transientFaultCount;
                _transientExceptionFactory = transientExceptionFactory;
                _invocations = new List<DateTime>();
                _functionProvider = Mock.Of<IFunctionProvider>();
            }

            public EventualSuccessOperator(int transientFaultCount,
                Func<Exception> transientExceptionFactory)
                : this(transientFaultCount,
                    triedTimes =>
                        transientExceptionFactory.Invoke())
            { }

            public EventualSuccessOperator(int transientFaultCount)
                : this(transientFaultCount,
                    triedTimes => new Exception())
            {
            }

            public IReadOnlyList<DateTime> Invocations => _invocations;

            public IFunctionProvider FunctionProvider => _functionProvider;

            public async Task Operation(CancellationToken token)
            {
                var now = DateTime.Now;

                try
                {
                    await _functionProvider.Func<CancellationToken, Task>(token);

                    var triedTimes = _invocations.Count;
                    if (triedTimes < _transientFaultCount)
                    {
                        throw _transientExceptionFactory.Invoke(triedTimes);
                    }

                }
                finally
                {
                    _invocations.Add(now);
                }
            }

            public async Task Operation<T>(T arg, CancellationToken token)
            {
                var now = DateTime.Now;

                try
                {
                    await _functionProvider.Func<T, CancellationToken, Task>(arg, token);

                    var triedTimes = _invocations.Count;
                    if (triedTimes < _transientFaultCount)
                    {
                        throw _transientExceptionFactory.Invoke(triedTimes);
                    }
                }
                finally
                {
                    _invocations.Add(now);
                }
            }

        }

    }
}
