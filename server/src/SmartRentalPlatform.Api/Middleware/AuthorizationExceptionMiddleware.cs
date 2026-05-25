namespace SmartRentalPlatform.Api.Middleware;

/// <summary>
/// Middleware xử lý lỗi Authorization (403 Forbidden)
/// </summary>
public class AuthorizationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden
                && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                var isAuth = context.User.Identity?.IsAuthenticated ?? false;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = isAuth ? "Forbidden" : "Unauthorized",
                    message = isAuth
                        ? "User does not have permission to access this resource."
                        : "User is not authenticated. Please login first.",
                    statusCode = isAuth ? 403 : 401
                });
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = ex.Message,
                    statusCode = 401
                });
            }
        }
    }
}

/// <summary>
/// Extension methods for middleware
/// </summary>
public static class AuthorizationExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthorizationExceptionMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthorizationExceptionMiddleware>();
    }
}
