namespace PollyWebApp.Polly
{
    using global::Polly;
    using global::Polly.Contrib.WaitAndRetry;
    using System.Net;
    using global::Polly.Wrap;
    using Models;

    public static class RetryPolicies

        #region retry variants for Http
    {
        /// <summary>
        /// Base PolicyBuilder that all the retry variants use.  This one is a policy for <see cref="HttpResponseMessage"/> operations and will handle <see cref="HttpRequestException"/> exceptions or non-exception results where the status code indicates a request timeout or any status code >= 500.  Since .Net 5, <see cref="TaskCanceledException"/> has been thrown by HttpClient if the call duration exceeds the configured timeout, so this can be enabled here too, demonstrating how a policy can handle multiple exception classes.
        /// </summary>
        private static readonly PolicyBuilder<HttpResponseMessage> CoreHttpPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            //.Or<TaskCanceledException>() // Uncomment this to handle timeout exceptions thrown by the HttpClient
            .OrResult(x => x.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout);

        
        /// <summary>
        /// Basic retrying - no delay between retries
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> BasicRetryPolicy =
            CoreHttpPolicy
                .RetryAsync(5);

        /// <summary>
        /// Exponential backoff between retries - better, but no jitter
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> ExponentialBackoffRetryPolicy =
            CoreHttpPolicy
                .WaitAndRetryAsync(5, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)));

        /// <summary>
        /// Recommended way of doing things if you're using a policy directly
        /// </summary>
        public static IAsyncPolicy<HttpResponseMessage> JitteredExponentialBackoffRetryPolicy =
                CoreHttpPolicy
                    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));

        #endregion

        #region retry and other variants for MyService

        public static readonly IAsyncPolicy<SomeDtoModel> JitteredExponentialBackoffRetryPolicyForMyService = Policy<SomeDtoModel>
            .Handle<ApplicationException>()
            .OrResult(x => x.Status is >= 10)
            //.OrResult(x => x.Content.Contains("red")) // will stop "Fred" being returned and retry instead
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));

        /// <summary>
        /// Wraps <see cref="JitteredExponentialBackoffRetryPolicyForMyService"/> with a fallback option if it doesn't work in 5 retries
        /// </summary>
        public static readonly AsyncPolicyWrap<SomeDtoModel> WrappedRetryAndFallbackPolicy = Policy<SomeDtoModel>
            .Handle<ApplicationException>()
            .FallbackAsync(GetBlankModel(), (result) => DoSomethingElseAsWell(result))
            .WrapAsync(JitteredExponentialBackoffRetryPolicyForMyService);

        #endregion

        #region helper methods

        /// <summary>
        /// Just a sample method for the Fallback to call
        /// </summary>
        /// <returns></returns>
        public static Task DoSomethingElseAsWell(DelegateResult<SomeDtoModel> result)
        {
            // As the great Quo once said - Whatever you want, Whatever you like...
            // could log result.Exception for example
            return Task.CompletedTask;
        }

        public static SomeDtoModel GetBlankModel()
        {
            return new SomeDtoModel{Content = "Bugger", Status = -1};
        }

        #endregion
    }
}
