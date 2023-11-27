namespace WhiteEagles.Test.TransientFaultHandling
{
    using AutoFixture;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WhiteEagles.Infrastructure.TransientFaultHandling;

    [TestClass]
    public class TransientFaultDetectionStrategyT_specs
    {
        [TestMethod]
        public void sut_inherits_TransientFaultDetectionStrategy()
            => typeof(TransientFaultDetectionStrategy<>).BaseType
                .Should().Be(typeof(TransientFaultDetectionStrategy));

        [TestMethod]
        public void sut_hast_InTransientResult_method()
        {
            var sut = typeof(TransientFaultDetectionStrategy<Result>);
            const string methodName = "IsTransientResult";
            sut.Should().HaveMethod(methodName, new[] { typeof(Result) });
            var mut = sut.GetMethod(methodName);
            mut.ReturnType.Should().Be(typeof(bool));
            mut.Should().BeVirtual();
        }

        [TestMethod]
        public void IsTransientResult_returns_false()
        {
            var fixture = new Fixture();
            var result = fixture.Create<Result>();
            var sut = new TransientFaultDetectionStrategy<Result>();

            var actual = sut.IsTransientResult(result);

            actual.Should().BeFalse();
        }

        public class Result
        {
        }

    }
}
