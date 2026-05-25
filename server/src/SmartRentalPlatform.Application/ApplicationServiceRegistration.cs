using Microsoft.Extensions.DependencyInjection;
using SmartRentalPlatform.Application.AdminApproval.Services;
using SmartRentalPlatform.Application.Identity;
using SmartRentalPlatform.Application.Listing;
using SmartRentalPlatform.Application.Storage;
using SmartRentalPlatform.Contracts.Identity;
using SmartRentalPlatform.Contracts.Listing;
using SmartRentalPlatform.Contracts.Storage;

namespace SmartRentalPlatform.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILandlordRoleInternalService, LandlordRoleInternalService>();
        services.AddScoped<ISignedUrlGenerator, HmacSignedUrlGenerator>();
        services.AddScoped<IPublicListingImageService, PublicListingImageService>();

        services.AddScoped<IKYCApprovalService, KYCApprovalService>();
        services.AddScoped<IRoomingHouseApprovalService, RoomingHouseApprovalService>();
        services.AddScoped<IPublicListingService, PublicListingService>();
        services.AddScoped<IApprovalAuditService, ApprovalAuditService>();

        return services;
    }
}
