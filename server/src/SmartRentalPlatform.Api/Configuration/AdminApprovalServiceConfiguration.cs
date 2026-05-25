using SmartRentalPlatform.Api.Middleware;
using SmartRentalPlatform.Api.Options;
using SmartRentalPlatform.Api.Policies;
using SmartRentalPlatform.Application.Storage;

namespace SmartRentalPlatform.Api.Configuration;

/// <summary>
/// Extension để cấu hình Admin Approval Services
/// </summary>
public static class AdminApprovalServiceConfiguration
{
    /// <summary>
    /// Đăng ký Admin Approval services vào DI container
    /// </summary>
    public static IServiceCollection AddAdminApprovalServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Đăng ký authorization policies (AddApplication gọi một lần tại Program.cs)
        services.AddAdminApprovalPolicies();

        // Đọc config từ appsettings.json
        services.Configure<AdminApprovalSecurityOptions>(
            configuration.GetSection(AdminApprovalSecurityOptions.SectionName));

        services.Configure<SignedUrlOptions>(options =>
        {
            var section = configuration.GetSection(AdminApprovalSecurityOptions.SectionName);
            options.SignedUrlExpirationMinutes = section.GetValue(nameof(AdminApprovalSecurityOptions.SignedUrlExpirationMinutes), 15);
            options.MediaSigningSecret = configuration["Jwt:SecretKey"]
                ?? "CHANGE_ME_MEDIA_SIGNING_SECRET";
            options.PublicMediaBaseUrl = configuration["AdminApprovalSecurity:PublicMediaBaseUrl"]
                ?? "http://localhost:5000/api/media";
        });

        return services;
    }

    /// <summary>
    /// Cấu hình middleware cho Admin Approval
    /// </summary>
    public static IApplicationBuilder UseAdminApprovalMiddleware(
        this IApplicationBuilder app)
    {
        // Đăng ký authorization exception middleware
        app.UseAuthorizationExceptionMiddleware();

        return app;
    }
}

// TODO: Thêm vào Program.cs như sau:
/*
 * // Startup configuration
 * builder.Services.AddAdminApprovalServices(builder.Configuration);
 * 
 * // Build app
 * var app = builder.Build();
 * 
 * // Middleware
 * app.UseAdminApprovalMiddleware();
 * app.UseAuthentication();
 * app.UseAuthorization();
 * 
 * app.MapControllers();
 * app.Run();
 */
