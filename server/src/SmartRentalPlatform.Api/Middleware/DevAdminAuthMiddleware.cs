using System.Security.Claims;
using SmartRentalPlatform.Infrastructure.Persistence.Seed;

namespace SmartRentalPlatform.Api.Middleware;

/// <summary>
/// Dev: Admin mặc định cho /api/admin/*; header X-Dev-Role: Tenant để test 403.
/// </summary>
public class DevAdminAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public DevAdminAuthMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _next = next;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var enabled = _configuration.GetValue<bool>("DevAuth:Enabled")
            || (_environment.IsDevelopment() && _configuration.GetValue("DevAuth:Enabled", true));

        if (enabled && !(context.User.Identity?.IsAuthenticated ?? false))
        {
            var devRole = context.Request.Headers["X-Dev-Role"].FirstOrDefault();

            if (string.Equals(devRole, "Tenant", StringComparison.OrdinalIgnoreCase))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Role, "Tenant"),
                    new("role", "Tenant"),
                    new("sub", SeedIds.TenantDoneUserId.ToString()),
                    new("userId", SeedIds.TenantDoneUserId.ToString())
                };
                context.User = new ClaimsPrincipal(new ClaimsIdentity(
                    claims, "DevAuth", ClaimTypes.Name, ClaimTypes.Role));
            }
            else if (context.Request.Path.StartsWithSegments("/api/admin"))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Role, "Admin"),
                    new("role", "Admin"),
                    new("sub", SeedIds.AdminUserId.ToString()),
                    new("userId", SeedIds.AdminUserId.ToString()),
                    new(ClaimTypes.Email, "admin@gmail.com")
                };
                context.User = new ClaimsPrincipal(new ClaimsIdentity(
                    claims, "DevAuth", ClaimTypes.Name, ClaimTypes.Role));
            }
        }

        await _next(context);
    }
}

public static class DevAdminAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseDevAdminAuth(this IApplicationBuilder app) =>
        app.UseMiddleware<DevAdminAuthMiddleware>();
}
