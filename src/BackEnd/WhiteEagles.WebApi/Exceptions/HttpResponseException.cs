namespace WhiteEagles.WebApi.Exceptions
{
    using System;
    using System.Net;

    public class HttpResponseException : ApplicationException
    {
        public HttpResponseException(HttpStatusCode httpStatusCode)
            => HttpStatusCode = httpStatusCode;

        public HttpResponseException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            HttpStatusCode = httpStatusCode;
        }


        public HttpResponseException(HttpStatusCode httpStatusCode, string message,
            Exception innerException)
            : base(message, innerException)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (innerException == null)
            {
                throw new ArgumentNullException(nameof(innerException));
            }

            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; }
    }
}
