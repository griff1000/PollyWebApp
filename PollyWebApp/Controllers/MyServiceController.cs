namespace PollyWebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Polly;
    using Services;

    /// <summary>
    /// This is the class we are using to demo Polly for calls to basically any other resource
    /// </summary>
    public class MyServiceController : Controller
    {
        private readonly IMyService _myService;

        public MyServiceController(IMyService myService)
        {
            _myService = myService;
        }
        public async Task<IActionResult> Index()
        {
            var someDto = new SomeDtoModel();
            try
            {
                // Use the first call not to have any retry policy; use the second call to use the retry policy.
                someDto = await _myService.GetSome("Hello");
                //someDto = await RetryPolicies.JitteredExponentialBackoffRetryPolicyForMyService.ExecuteAsync(() => _myService.GetSome("Hello"));
                //someDto = await RetryPolicies.WrappedRetryAndFallbackPolicy.ExecuteAsync(() => _myService.GetSome("Hello"));
            }
            catch (ApplicationException)
            { }

            return View(someDto);
        }
    }
}
