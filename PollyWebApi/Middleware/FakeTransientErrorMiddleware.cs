namespace PollyWebApi.Middleware;

using System.Net;

/// <summary>
/// Returns internal server error responses for the 4th and 5th requests; regular responses all other times
/// </summary>
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
    /// <summary>
    /// Adds middleware that returns internal server error responses for the 4th and 5th requests; regular responses all other times
    /// </summary>
    public static IApplicationBuilder UseFakeTransientErrorMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FakeTransientErrorMiddleware>();
    }
}