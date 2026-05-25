using System.Security.Claims;

namespace SmartRentalPlatform.Api.Extensions;

/// <summary>
/// Extension methods cho User claims
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Lấy User ID từ claims
    /// </summary>
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("sub") ?? user.FindFirst("userId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Lấy Email từ claims
    /// </summary>
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Lấy DisplayName từ claims
    /// </summary>
    public static string? GetDisplayName(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Kiểm tra user có role Admin
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.IsInRole("Admin");
    }
}
