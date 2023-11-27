namespace WhiteEagles.Test.TransientFaultHandling
{
    using System;
    using System.Reflection;
    using AutoFixture;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Protected;
    using WhiteEagles.Infrastructure.TransientFaultHandling;

    [TestClass]
    public class RetryIntervalStrategy_specs
    {
        [TestMethod]
        public void sut_is_abstract()
            => typeof(RetryIntervalStrategy).IsAbstract.Should().BeTrue("the class should be abstract");

        [TestMethod]
        public void sut_has_ImmediateFirstRetry_property()
            => typeof(RetryIntervalStrategy).Should()
                .HaveProperty<bool>("ImmediateFirstRetry")
                .Which.Should().NotBeWritable();

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void constructor_sets_ImmediateFirstRetry_correctly(bool immediateFirstRetry)
        {
            var sut = new Mock<RetryIntervalStrategy>(immediateFirstRetry).Object;
            sut.ImmediateFirstRetry.Should().Be(immediateFirstRetry);
        }

        [TestMethod]
        public void sut_has_GetInterval_method()
        {
            const string MethodName = "GetInterval";
            typeof(RetryIntervalStrategy).Should()
                .HaveMethod(MethodName, new[] { typeof(int) });

            MethodInfo mut = typeof(RetryIntervalStrategy).GetMethod(MethodName);
            mut.ReturnType.Should().Be(typeof(TimeSpan));

        }

        [TestMethod]
        public void given_true_ImmediateFirstRetry_and_zero_retried_GetInterval_returns_zero()
        {
            var sut = new Mock<RetryIntervalStrategy>(true).Object;
            var actual = sut.GetInterval(0);
            actual.Should().Be(TimeSpan.Zero);

        }

        [TestMethod]
        public void
            given_true_ImmediateFirstRetry_GetInterval_relays_to_GetIntervalFromZeroBasedTick_with_retried_minus_one()
        {
            const bool immediateFirstRetry = true;
            var mock = new Mock<RetryIntervalStrategy>(immediateFirstRetry);
            var sut = mock.Object;
            var fixture = new Fixture();
            var retried = fixture.Create<int>();
            var expected = fixture.Create<TimeSpan>();
            mock.Protected().Setup<TimeSpan>("GetIntervalFromZeroBasedTick", retried - 1)
                .Returns(expected);

            var actual = sut.GetInterval(retried);

            actual.Should().Be(expected);

            mock.Protected().Verify<TimeSpan>("GetIntervalFromZeroBasedTick",
                Times.Once(), ItExpr.IsAny<int>());
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void GetInterval_has_guard_clause_against_negative_retried(bool immediateFirstRetry)
        {
            var sut = new Mock<RetryIntervalStrategy>(immediateFirstRetry).Object;
            Action action = () => sut.GetInterval(-1);
            action.Should().Throw<ArgumentOutOfRangeException>(
                    because: "negative retried is not allowed")
                .Where(x => x.ParamName == "retried");
        }
    }
}
