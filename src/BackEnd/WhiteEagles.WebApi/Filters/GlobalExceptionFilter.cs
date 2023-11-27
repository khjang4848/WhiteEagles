namespace WhiteEagles.WebApi.Filters
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    using Data.ViewModels;
    using Exceptions;

    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;
        private bool _disposed;

        public GlobalExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger.CreateLogger("Exception Filter");
        }


        public void OnException(ExceptionContext context)
        {
            var response = new ResponseBaseViewModel
            {
                ResultCode = "500",
                ResultMessage = context.Exception.Message
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = GetHttpStatusCode(context.Exception),
            };

            _logger.LogError("GlobalExceptionFilter", context.Exception);

        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }

        private static int GetHttpStatusCode(Exception ex)
        {
            if (ex is HttpResponseException exception)
            {
                return (int)exception.HttpStatusCode;
            }

            return (int)HttpStatusCode.InternalServerError;
        }
    }
}
