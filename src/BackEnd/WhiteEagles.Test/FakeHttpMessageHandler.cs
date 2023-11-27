namespace WhiteEagles.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeHttpMessageHandler : HttpClientHandler
    {
        private readonly HttpRequestException _exception;

        private readonly IDictionary<string, HttpMessageOptions> _lotsOfOptions
            = new Dictionary<string, HttpMessageOptions>();

        public FakeHttpMessageHandler(HttpMessageOptions options)
            : this(new List<HttpMessageOptions> { options })

        {
        }

        public FakeHttpMessageHandler(IEnumerable<HttpMessageOptions> lotsOfOptions)
            => Initialize(lotsOfOptions.ToArray());

        public FakeHttpMessageHandler(HttpRequestException exception)
            => _exception = exception ??
                            throw new ArgumentNullException(nameof(exception));

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (_exception != null)
            {
                throw _exception;
            }

            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            var requestUri = new Uri(request.RequestUri.AbsoluteUri);

            var option = new HttpMessageOptions
            {

                RequestUri = requestUri,
                HttpMethod = request.Method,
                HttpContent = request.Content,
                Headers = request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value)
            };

            var expectedOption = GetExpectedOption(option);

            if (expectedOption == null)
            {
                var setupOptionsText = new StringBuilder();
                var setupOptions = _lotsOfOptions.Values.ToList();

                for (var i = 0; i < setupOptions.Count; i++)
                {
                    if (i > 0)
                    {
                        setupOptionsText.Append(" ");
                    }

                    setupOptionsText.Append($"{i + 1}) {setupOptions[i]}.");
                }

                var errorMessage = $"No HttpResponseMessage found for the Request => What was called: [{option}]. At least one of these option(s) should have been matched: [{setupOptionsText}]";
                throw new InvalidOperationException(errorMessage);
            }

            expectedOption.IncrementNumberOfTimesCalled();
            expectedOption.HttpResponseMessage.RequestMessage = request;

            tcs.SetResult(expectedOption.HttpResponseMessage);

            return tcs.Task;
        }

        private HttpMessageOptions GetExpectedOption(HttpMessageOptions option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            return _lotsOfOptions.Values.SingleOrDefault(x => 
                (x.RequestUri == null || 
                 x.RequestUri.AbsoluteUri.Equals(option.RequestUri.AbsoluteUri,
                 StringComparison.OrdinalIgnoreCase)) &&
                (x.HttpMethod == null || x.HttpMethod == option.HttpMethod) &&
                (x.HttpContent == null || ContentAreEqual(x.HttpContent, option.HttpContent)) &&
                (x.Headers == null || x.Headers.Count == 0 || 
                 HeadersAreEqual(x.Headers, option.Headers)));
        }



        private void Initialize(HttpMessageOptions[] lotsOfOptions)
        {
            if (lotsOfOptions == null)
            {
                throw new ArgumentNullException(nameof(lotsOfOptions));
            }



            if (!lotsOfOptions.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(lotsOfOptions),
                    "Need at least _one_ expected request/response setup.");
            }

            foreach (var option in lotsOfOptions)
            {

                if (_lotsOfOptions.ContainsKey(option.ToString()))
                {

                    throw new InvalidOperationException(
                        $"Trying to add a request/response which has already been setup. Can only have one unique request/response, setup. Unique info: {option}");
                }

                _lotsOfOptions.Add(option.ToString(), option);
            }
        }

        private static bool ContentAreEqual(HttpContent source, HttpContent destination)

        {

            if (source == null && destination == null)
            {
                return true;
            }



            if (source == null || destination == null)
            {
                return false;
            }


            var sourceContentTask = source.ReadAsStringAsync();

            var destinationContentTask = destination.ReadAsStringAsync();

            var tasks = new List<Task>
            {

                sourceContentTask,
                destinationContentTask

            };

            Task.WaitAll(tasks.ToArray());

            return sourceContentTask.Result == destinationContentTask.Result;

        }



        private static bool HeadersAreEqual(IDictionary<string, IEnumerable<string>> source,
                                IDictionary<string, IEnumerable<string>> destination)

        {

            if (source == null && destination == null)
            {
                return true;
            }


            if (source == null || destination == null)
            {
                return false;
            }

            if (source.Count != destination.Count)
            {
                return false;
            }

            foreach (var key in source.Keys)
            {
                if (!destination.ContainsKey(key))
                {
                    return false;
                }

                if (source[key].Count() != destination[key].Count())
                {
                    return false;
                }

                if (source[key].Any(value => !destination[key].Contains(value)))
                {
                    return false;
                }

            }

            return true;

        }
    }
}
