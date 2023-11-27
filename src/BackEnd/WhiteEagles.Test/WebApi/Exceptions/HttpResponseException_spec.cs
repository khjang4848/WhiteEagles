namespace WhiteEagles.Test.WebApi.Exceptions
{
    using System;
    using FluentAssertions;
    using System.Reflection;
    using AutoFixture;
    using AutoFixture.Idioms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WhiteEagles.WebApi.Exceptions;

    [TestClass]
    public class HttpResponseException_spec
    {
        [TestMethod]
        public void sut_inherits_ApplicationException()
            => typeof(HttpResponseException).BaseType.Should().Be(typeof(ApplicationException));

        [TestMethod]
        public void sut_has_guard_clauses()
        {
            var sut = typeof(HttpResponseException);
            new GuardClauseAssertion(new Fixture())
                .Verify(sut.GetConstructors(BindingFlags.Public));
        }
    }
}
