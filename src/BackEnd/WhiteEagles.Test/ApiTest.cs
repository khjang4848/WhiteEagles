namespace WhiteEagles.Test
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WhiteEagles.Infrastructure.TransientFaultHandling;

    public abstract class ApiTest
    {
        public static HttpClient TestClient { get; } = new();

        public static RetryPolicy Retry { get; } =
            RetryPolicy.Linear(20, TimeSpan.FromMilliseconds(500));

        public TestContext TestContext { get; set; }

        protected Uri ComposeUri(string path)
        {
            dynamic endpoint = TestContext.Properties["ApiEndpoint"];
            return new Uri(new Uri(endpoint), path);
        }

        protected Uri ComposeUri(Uri path)
        {
            dynamic endpoint = TestContext.Properties["ApiEndpoint"];
            return new Uri(new Uri(endpoint), path);
        }

        protected Task<HttpResponseMessage> PostAsJson<T>(string path, T value)
            => TestClient.PostAsJsonAsync(ComposeUri(path), value);

        protected Task<HttpResponseMessage> Get(Uri uri)
            => TestClient.GetAsync(uri);
    }
}
