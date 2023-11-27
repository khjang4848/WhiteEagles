namespace WhiteEagles.Test.TransientFaultHandling
{
    using System;
    using AutoFixture;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using WhiteEagles.Infrastructure.TransientFaultHandling;

    [TestClass]
    public class DelegatingTransientFaultDetectionStrategyT_specs
    {
        public interface IFunctionProvider
        {
            bool ExceptionFunc(Exception exception);
            bool ResultFunc<T>(T result);
        }


        [TestMethod]
        public void sut_inherits_TransientFaultDetectionStrategeT()
            => typeof(DelegatingTransientFaultDetectionStrategy<>)
                .BaseType.GetGenericTypeDefinition()
                .Should().Be(typeof(TransientFaultDetectionStrategy<>));


        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsTransientException_relays_to_func(bool excepted)
        {
            var fixture = new Fixture();
            var exception = fixture.Create<Exception>();
            var functionProvider = Mock.Of<IFunctionProvider>(
                x => x.ExceptionFunc(exception) == excepted);
            Func<Exception, bool> exceptionFunc = functionProvider.ExceptionFunc;
            Func<Result, bool> resultFunc = functionProvider.ResultFunc;

            var sut = new DelegatingTransientFaultDetectionStrategy<Result>(
                exceptionFunc, resultFunc);

            var actual = sut.IsTransientException(exception);

            actual.Should().Be(excepted);
        }


        [TestMethod]
        public void sut_has_guard_clause()
        {
            var tut = typeof(DelegatingTransientFaultDetectionStrategy<>);
            new GuardClauseAssertion(new Fixture()).Verify(tut);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsTransientResult_relays_to_func(bool expected)
        {
            var fixture = new Fixture();
            var result = fixture.Create<Result>();
            var functionProvider = Mock.Of<IFunctionProvider>(
                x => x.ResultFunc(result) == expected);
            Func<Exception, bool> exceptionFunc = functionProvider.ExceptionFunc;
            Func<Result, bool> resultFunc = functionProvider.ResultFunc;

            var sut = new DelegatingTransientFaultDetectionStrategy<Result>(
                exceptionFunc, resultFunc);

            var actual = sut.IsTransientResult(result);

            actual.Should().Be(expected);
        }


        public class Result
        {
        }
    }
}
