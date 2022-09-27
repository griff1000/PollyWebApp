namespace PollyWebApp.Controllers
{
    using Polly;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Text.Json;

    /// <summary>
    /// This is the class we are using to demo Polly for HttpClient calls
    /// </summary>
    public class WeatherController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IHttpClientFactory clientFactory, ILogger<WeatherController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            // Note this gets a named client, which is needed for the AddPolicyHandler extension method to work
            var httpClient = _clientFactory.CreateClient(nameof(WeatherController));

            httpClient.Timeout = TimeSpan.FromSeconds(5); 

            var response = await httpClient.GetAsync("https://localhost:7223/WeatherForecast");


            #region examples of a pattern using _retryPolicy that does work

            //var response = await RetryPolicies.BasicRetryPolicy.ExecuteAsync(() => httpClient.GetAsync("https://localhost:7223/WeatherForecast"));

            //var response = await RetryPolicies.ExponentialBackoffRetryPolicy.ExecuteAsync(() => httpClient.GetAsync("https://localhost:7223/WeatherForecast"));

            //var response = await RetryPolicies.JitteredExponentialBackoffRetryPolicy.ExecuteAsync(() => httpClient.GetAsync("https://localhost:7223/WeatherForecast"));


            #endregion

            #region example of a pattern using _retryPolicy that does NOT work

            //var message = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7223/WeatherForecast");
            //// This one does work
            //var response = await httpClient.SendAsync(message);
            //// This one doesn't since you can't reuse a HttpRequestMessage.  StackOverGoogle is your friend here, there are solutions if you need to use HttpRequestMessage
            //var response = await RetryPolicies.JitteredExponentialBackoffRetryPolicy.ExecuteAsync(() => httpClient.SendAsync(message));

            #endregion


            var weather = Enumerable.Empty<WeatherForecastModel>();
            if (response.IsSuccessStatusCode)
            {
                await using var contentStream = await response.Content.ReadAsStreamAsync();
                try
                {
                    weather =
                        await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecastModel>>(contentStream);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Oops");
                }
            }
            return View(weather);
        }
    }
}
