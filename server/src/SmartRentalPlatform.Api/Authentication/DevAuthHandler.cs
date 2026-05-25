using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace SmartRentalPlatform.Api.Authentication;

/// <summary>
/// Scheme dev: đọc principal từ DevAdminAuthMiddleware; Forbid trả 403 đúng chuẩn.
/// </summary>
public class DevAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.User?.Identity?.IsAuthenticated == true)
        {
            var ticket = new AuthenticationTicket(Context.User, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties? properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties? properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
}
