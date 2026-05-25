namespace SmartRentalPlatform.Infrastructure.Persistence.Seed;

public static class SeedIds
{
    public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TenantRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid LandlordRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid AdminUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0001");
    public static readonly Guid TenantDoneUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0002");
    public static readonly Guid TenantKycUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0003");
    public static readonly Guid LandlordDoneUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0004");
    public static readonly Guid LandlordKycUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005");

    public static readonly Guid AnBinhRoomingHouseId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001");
    public static readonly Guid PendingRoomingHouseId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002");
    public static readonly Guid HiddenPendingPublicTestHouseId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0003");

    public static readonly Guid RoomA101Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccc0001");
    public static readonly Guid RoomA102MaintenanceId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccc0002");

    public static readonly Guid RoomA101PriceTier1Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0001");
    public static readonly Guid RoomA101PriceTier2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0002");
    public static readonly Guid RoomA101PriceTier3Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddd0003");

    public static readonly Guid TenantKycVerificationId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001");
    public static readonly Guid LandlordKycVerificationId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002");

    public static readonly Guid AnBinhCoverImageId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffff0001");
    public static readonly Guid RoomA101ImageId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffff0002");

    public static readonly DateTime SeededAt = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
