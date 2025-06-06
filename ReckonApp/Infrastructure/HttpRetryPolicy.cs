using Polly;
using Polly.Extensions.Http;

namespace ReckonApp.Infrastructure
{
    public static class HttpRetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handles 429, 500, 502, 503, 504, and HttpRequestException
                .Or<TimeoutException>() // Handle timeouts
                .WaitAndRetryAsync(
                    retryCount: 10, // Retry 3 times
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(2));
        }
    }
}
