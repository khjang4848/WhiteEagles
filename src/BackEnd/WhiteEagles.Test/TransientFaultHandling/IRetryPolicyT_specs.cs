namespace WhiteEagles.Test.TransientFaultHandling
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WhiteEagles.Infrastructure.TransientFaultHandling;

    [TestClass]
    public class IRetryPolicyT_specs
    {
        [TestMethod]
        public void sut_inherits_IRetryPolicy()
            => typeof(IRetryPolicy<>).Should().Implement<IRetryPolicy>();
    }
}
