namespace WhiteEagles.Test.WebApi.Filters
{
    using System;
    using System.Reflection;
    using AutoFixture;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using WhiteEagles.WebApi.Filters;

    [TestClass]
    public class GlobalExceptionFilter_spec
    {
        [TestMethod]
        public void sut_implement_IExceptionFilters()
        {
            var sut = new GlobalExceptionFilter(Mock.Of<ILoggerFactory>());
            sut.Should().BeAssignableTo<IExceptionFilter>();
        }

        [TestMethod]
        public void sut_implement_IDisposable()
        {
            var sut = new GlobalExceptionFilter(Mock.Of<ILoggerFactory>());
            sut.Should().BeAssignableTo<IDisposable>();
        }

        [TestMethod]
        public void sut_has_guard_clauses()
        {
            var sut = typeof(GlobalExceptionFilter);
            new GuardClauseAssertion(new Fixture())
                .Verify(sut.GetConstructors(BindingFlags.Public));
        }
    }
}
