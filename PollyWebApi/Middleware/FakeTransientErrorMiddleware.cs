namespace PollyWebApi.Middleware;

using System.Net;

public class FakeTransientErrorMiddleware
{
    private readonly RequestDelegate _next;
    private static int _requestCount;

    public FakeTransientErrorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _requestCount++;
        if (_requestCount > 3 && _requestCount < 6)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return;
        }

        await _next(context);
    }
}

public static class FakeTransientErrorMiddlewareExtensions
{
    public static IApplicationBuilder UseFakeTransientErrorMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FakeTransientErrorMiddleware>();
    }
}