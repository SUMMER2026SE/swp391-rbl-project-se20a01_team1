using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Domain.Entities;
using SmartRentalPlatform.Domain.Entities.AdminApproval;
using SmartRentalPlatform.Infrastructure.Persistence.Seed;


namespace SmartRentalPlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    // Người 1 - Authentication
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    
    // Người 5 - Admin Approval & Public Listing
    public DbSet<KYCVerification> KYCVerifications => Set<KYCVerification>();
    public DbSet<RoomingHouse> RoomingHouses => Set<RoomingHouse>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomPriceTier> RoomPriceTiers => Set<RoomPriceTier>();
    public DbSet<RoomingHouseImage> RoomingHouseImages => Set<RoomingHouseImage>();
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();
    public DbSet<ApprovalAuditLog> ApprovalAuditLogs => Set<ApprovalAuditLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seed dữ liệu mẫu Interval 1 (HasData) — ID cố định trong SeedIds.
        AppSeedData.Apply(modelBuilder);
    }
}