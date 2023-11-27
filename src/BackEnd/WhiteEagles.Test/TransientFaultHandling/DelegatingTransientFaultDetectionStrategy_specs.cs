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
    public class DelegatingTransientFaultDetectionStrategy_specs
    {
        public interface IFunctionProvider
        {
            bool Func(Exception exception);
        }

        [TestMethod]
        public void sut_inherits_TransientFaultDetectionStrategy()
            => typeof(DelegatingTransientFaultDetectionStrategy)
                .BaseType.Should().Be(typeof(TransientFaultDetectionStrategy));

        [TestMethod]
        public void sut_has_guard_clauses()
        {
            var sut = typeof(DelegatingTransientFaultDetectionStrategy);
            new GuardClauseAssertion(new Fixture()).Verify(sut);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsTransientException_relays_to_function(bool expected)
        {
            var fixture = new Fixture();
            var exception = fixture.Create<Exception>();
            var functionProvider = Mock.Of<IFunctionProvider>(x
                => x.Func(exception) == expected);

            Func<Exception, bool> func = functionProvider.Func;

            var sut = new DelegatingTransientFaultDetectionStrategy(func);

            var actual = sut.IsTransientException(exception);

            actual.Should().Be(expected);
        }
    }
}
