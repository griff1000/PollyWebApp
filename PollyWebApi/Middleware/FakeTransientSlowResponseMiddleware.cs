namespace PollyWebApi.Middleware;

using System.Net;

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
        if (_requestCount > 3 && _requestCount < 6)
        {
            await Task.Delay(TimeSpan.FromSeconds(35));
        }

        await _next(context);
    }
}

public static class FakeTransientSlowResponseMiddlewareExtensions
{
    public static IApplicationBuilder UseFakeTransientSlowResponseMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FakeTransientSlowResponseMiddleware>();
    }
}