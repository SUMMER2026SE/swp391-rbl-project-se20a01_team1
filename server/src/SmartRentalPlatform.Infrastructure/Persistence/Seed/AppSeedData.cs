using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Domain.Entities;
using SmartRentalPlatform.Domain.Entities.AdminApproval;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Infrastructure.Persistence.Seed;

public static class AppSeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        var at = SeedIds.SeededAt;

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = SeedIds.AdminRoleId, Name = "Admin", Description = "Quản trị hệ thống / System administrator", CreatedAt = at },
            new Role { Id = SeedIds.TenantRoleId, Name = "Tenant", Description = "Người thuê (mặc định) / Default tenant role", CreatedAt = at },
            new Role { Id = SeedIds.LandlordRoleId, Name = "Landlord", Description = "Chủ trọ / Property owner", CreatedAt = at });

        modelBuilder.Entity<User>().HasData(
            CreateUser(SeedIds.AdminUserId, "admin@gmail.com", "Admin", OnboardingStatus.Completed, at),
            CreateUser(SeedIds.TenantDoneUserId, "tenant.done@gmail.com", "Tenant Done", OnboardingStatus.Completed, at),
            CreateUser(SeedIds.TenantKycUserId, "tenant.kyc@gmail.com", "Tenant KYC", OnboardingStatus.NeedKyc, at),
            CreateUser(SeedIds.LandlordDoneUserId, "landlord.done@gmail.com", "Landlord Done", OnboardingStatus.Completed, at),
            CreateUser(SeedIds.LandlordKycUserId, "landlord.kyc@gmail.com", "Landlord KYC", OnboardingStatus.NeedKyc, at));

        // landlord.kyc chưa có role Landlord — sẽ được cấp khi duyệt khu trọ PendingRoomingHouseId
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole { UserId = SeedIds.AdminUserId, RoleId = SeedIds.AdminRoleId, CreatedAt = at },
            new UserRole { UserId = SeedIds.TenantDoneUserId, RoleId = SeedIds.TenantRoleId, CreatedAt = at },
            new UserRole { UserId = SeedIds.TenantKycUserId, RoleId = SeedIds.TenantRoleId, CreatedAt = at },
            new UserRole { UserId = SeedIds.LandlordDoneUserId, RoleId = SeedIds.LandlordRoleId, CreatedAt = at });

        modelBuilder.Entity<KYCVerification>().HasData(
            new KYCVerification
            {
                Id = SeedIds.TenantKycVerificationId,
                UserId = SeedIds.TenantKycUserId,
                FullName = "Nguyễn Văn Tenant KYC",
                DateOfBirth = "1998-05-15",
                IdNumber = "079098001234",
                Address = "456 Lê Lợi, Quận 1, TP.HCM",
                IdImageObjectKey = "kyc/tenant-kyc/id-front.jpg",
                FaceImageObjectKey = "kyc/tenant-kyc/face.jpg",
                Status = KYCStatus.PendingAdminReview,
                CreatedAt = at,
                UpdatedAt = at
            },
            new KYCVerification
            {
                Id = SeedIds.LandlordKycVerificationId,
                UserId = SeedIds.LandlordKycUserId,
                FullName = "Trần Thị Landlord KYC",
                DateOfBirth = "1985-11-20",
                IdNumber = "079085009876",
                Address = "789 Nguyễn Huệ, Quận 1, TP.HCM",
                IdImageObjectKey = "kyc/landlord-kyc/id-front.jpg",
                FaceImageObjectKey = "kyc/landlord-kyc/face.jpg",
                Status = KYCStatus.PendingAdminReview,
                CreatedAt = at,
                UpdatedAt = at
            });

        modelBuilder.Entity<RoomingHouse>().HasData(
            new RoomingHouse
            {
                Id = SeedIds.AnBinhRoomingHouseId,
                LandlordUserId = SeedIds.LandlordDoneUserId,
                Name = "Khu trọ An Bình",
                Address = "123 Đường An Bình, Quận 7, TP.HCM",
                Description = "Khu trọ yên tĩnh, tiện nghi cơ bản — Peaceful boarding area (Interval 1 seed)",
                Amenities = "Wifi,Giữ xe,Máy lạnh,Nước nóng",
                ApprovalStatus = RoomingHouseApprovalStatus.Approved,
                Visibility = RoomingHouseVisibility.Visible,
                CreatedAt = at,
                UpdatedAt = at
            },
            new RoomingHouse
            {
                Id = SeedIds.PendingRoomingHouseId,
                LandlordUserId = SeedIds.LandlordKycUserId,
                Name = "Khu trọ Chờ Duyệt (Landlord KYC)",
                Address = "88 Đường Test, Quận 10, TP.HCM",
                Description = "Seed pending — duyệt để test cấp role Landlord",
                Amenities = "Wifi,Thang máy",
                ApprovalStatus = RoomingHouseApprovalStatus.PendingAdminReview,
                Visibility = RoomingHouseVisibility.Hidden,
                CreatedAt = at,
                UpdatedAt = at
            },
            new RoomingHouse
            {
                Id = SeedIds.HiddenPendingPublicTestHouseId,
                LandlordUserId = SeedIds.LandlordDoneUserId,
                Name = "Nhà Pending (không hiện public)",
                Address = "99 Hidden St",
                ApprovalStatus = RoomingHouseApprovalStatus.PendingAdminReview,
                Visibility = RoomingHouseVisibility.Hidden,
                CreatedAt = at,
                UpdatedAt = at
            });

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = SeedIds.RoomA101Id,
                RoomingHouseId = SeedIds.AnBinhRoomingHouseId,
                RoomNumber = "A101",
                Price = 2_000_000m,
                Area = 25m,
                Capacity = 3,
                Status = RoomStatus.Available,
                CreatedAt = at,
                UpdatedAt = at
            },
            new Room
            {
                Id = SeedIds.RoomA102MaintenanceId,
                RoomingHouseId = SeedIds.AnBinhRoomingHouseId,
                RoomNumber = "A102",
                Price = 2_500_000m,
                Area = 28m,
                Capacity = 2,
                Status = RoomStatus.Maintenance,
                CreatedAt = at,
                UpdatedAt = at
            });

        modelBuilder.Entity<RoomPriceTier>().HasData(
            new RoomPriceTier { Id = SeedIds.RoomA101PriceTier1Id, RoomId = SeedIds.RoomA101Id, OccupantCount = 1, MonthlyPrice = 2_000_000m },
            new RoomPriceTier { Id = SeedIds.RoomA101PriceTier2Id, RoomId = SeedIds.RoomA101Id, OccupantCount = 2, MonthlyPrice = 2_900_000m },
            new RoomPriceTier { Id = SeedIds.RoomA101PriceTier3Id, RoomId = SeedIds.RoomA101Id, OccupantCount = 3, MonthlyPrice = 3_600_000m });

        modelBuilder.Entity<RoomingHouseImage>().HasData(
            new RoomingHouseImage
            {
                Id = SeedIds.AnBinhCoverImageId,
                RoomingHouseId = SeedIds.AnBinhRoomingHouseId,
                ObjectKey = "rooming-houses/an-binh/cover.jpg",
                IsCover = true,
                SortOrder = 0
            });

        modelBuilder.Entity<RoomImage>().HasData(
            new RoomImage
            {
                Id = SeedIds.RoomA101ImageId,
                RoomId = SeedIds.RoomA101Id,
                ObjectKey = "rooms/a101/gallery-1.jpg",
                SortOrder = 0
            });
    }

    private static User CreateUser(
        Guid id, string email, string displayName, OnboardingStatus onboardingStatus, DateTime seededAt) =>
        new()
        {
            Id = id,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            DisplayName = displayName,
            PasswordHash = SeedPasswordHashes.Password123,
            Status = UserStatus.Active,
            OnboardingStatus = onboardingStatus,
            EmailComfirmed = true,
            PhoneConfirmed = false,
            AccessFailedCount = 0,
            CreatedAt = seededAt,
            UpdatedAt = seededAt
        };
}
