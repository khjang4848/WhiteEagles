namespace WhiteEagles.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpMessageHandlerDouble : HttpMessageHandler
    {
        private readonly List<CannedAnswer> _cannedAnswers;

        public HttpMessageHandlerDouble()
            => _cannedAnswers = new List<CannedAnswer>();

        public void AddAnswer(
            Func<HttpRequestMessage, bool> predicate,
            HttpResponseMessage answer)
            => _cannedAnswers.Add(new CannedAnswer(predicate, answer));

        public void AddAnswer(
            Func<HttpRequestMessage, bool> predicate,
            HttpStatusCode statusCode)
            => AddAnswer(predicate, new HttpResponseMessage(statusCode));


        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(millisecondsDelay: 1, cancellationToken);

            foreach (var cannedAnswer in
                _cannedAnswers.Where(x => x.Predicate.Invoke(request)))
            {
                return cannedAnswer.Answer;
            }

            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }

    public readonly struct CannedAnswer
    {
        public CannedAnswer(
            Func<HttpRequestMessage, bool> predicate,
            HttpResponseMessage answer)
            => (Predicate, Answer) = (predicate, answer);

        public Func<HttpRequestMessage, bool> Predicate { get; }

        public HttpResponseMessage Answer { get; }
    }
}
