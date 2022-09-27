namespace PollyWebApi.Middleware;

/// <summary>
/// Returns slow (> 35s) responses for the 4th and 5th requests; regular responses all other times
/// </summary>
public class FakeTransientSlowResponseMiddleware
{
    private readonly RequestDelegate _next;
    private static int _requestCount;

    public FakeTransientSlowResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _requestCount++;
        if (_requestCount is > 3 and < 6)
        {
            await Task.Delay(TimeSpan.FromSeconds(35));
        }

        await _next(context);
    }
}

public static class FakeTransientSlowResponseMiddlewareExtensions
{
    /// <summary>
    /// Adds middleware that returns slow (> 35s) responses for the 4th and 5th requests; regular responses all other times
    /// </summary>
    public static IApplicationBuilder UseFakeTransientSlowResponseMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FakeTransientSlowResponseMiddleware>();
    }
}