namespace WhiteEagles.Test.WebApi.Filters
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using AutoFixture;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using WhiteEagles.WebApi.Filters;
    using WhiteEagles.WebApi.Messages;

    [TestClass]
    public class ModelValidationAttribute_spec
    {
        [TestMethod]
        public void sut_inherits_ActionFilterAttribute()
        {
            typeof(ModelValidationAttribute).BaseType.Should().Be(typeof(ActionFilterAttribute));
        }

        [TestMethod]
        public void modelstate_valid_actionResult_is_null()
        {
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor(),

            };
            var fixture = new Fixture();
            var filters = Mock.Of<IList<IFilterMetadata>>();
            var actionArguments = Mock.Of<IDictionary<string, object>>();
            var controller = fixture.Create<object>();


            var actionExecutingContext = new ActionExecutingContext(
                actionContext, filters, actionArguments, controller);

            var sut = new ModelValidationAttribute();
            sut.OnActionExecuting(actionExecutingContext);

            var result = actionExecutingContext.Result;

            result.Should().BeNull();
        }

        [TestMethod]
        public void modelstate_isValid_actionResult_status_is_badRequest()
        {
            var actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            actionContext.ModelState.AddModelError("Id", "ID is null");


            var fixture = new Fixture();
            var filters = Mock.Of<IList<IFilterMetadata>>();
            var actionArguments = Mock.Of<IDictionary<string, object>>();
            var controller = fixture.Create<object>();


            var actionExecutingContext = new ActionExecutingContext(
                actionContext, filters, actionArguments, controller);

            var sut = new ModelValidationAttribute();
            sut.OnActionExecuting(actionExecutingContext);

            var result = (JsonResult)actionExecutingContext.Result;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var value = ((IEnumerable<Error>)result.Value).First();
            value.Message.Should().Be("ID is null");
        }
    }
}
