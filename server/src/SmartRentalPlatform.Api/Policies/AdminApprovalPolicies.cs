using Microsoft.AspNetCore.Authorization;

namespace SmartRentalPlatform.Api.Policies;

/// <summary>
/// Authorization Policies cho Admin Approval
/// </summary>
public static class AdminApprovalPolicies
{
    public const string REQUIRE_ADMIN_ROLE = "RequireAdminRole";

    /// <summary>
    /// Đăng ký authorization policies
    /// </summary>
    public static IServiceCollection AddAdminApprovalPolicies(
        this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(REQUIRE_ADMIN_ROLE, policy =>
            {
                policy.RequireRole("Admin");
            });

        return services;
    }
}

/// <summary>
/// Requirement cho Admin Approval
/// </summary>
public class AdminApprovalRequirement : IAuthorizationRequirement
{
    public AdminApprovalRequirement(string approvalType)
    {
        ApprovalType = approvalType;
    }

    public string ApprovalType { get; set; }
}

/// <summary>
/// Handler cho Admin Approval Requirement
/// </summary>
public class AdminApprovalHandler : AuthorizationHandler<AdminApprovalRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminApprovalRequirement requirement)
    {
        // Check if user has Admin role
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
