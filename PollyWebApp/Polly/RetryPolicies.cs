namespace PollyWebApp.Polly
{
    using global::Polly;
    using global::Polly.Contrib.WaitAndRetry;
    using System.Net;

    public static class RetryPolicies
    {
        private static readonly PolicyBuilder<HttpResponseMessage> _corePolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            //.Or<TaskCanceledException>() // Uncomment this to handle timeouts
            .OrResult(x => x.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
            ;

        #region retry variants

        /// <summary>
        /// Basic retrying - no delay between retries
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> BasicRetryPolicy =
            _corePolicy
                .RetryAsync(5);

        /// <summary>
        /// Exponential backoff between retries - better, but no jitter
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> ExponentialBackoffRetryPolicy =
            _corePolicy
                .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)));

        /// <summary>
        /// Recommended way of doing things if you're using a policy directly
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> JitteredExponentialBackoffRetryPolicy =
                _corePolicy
                    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));

        #endregion
    }
}
