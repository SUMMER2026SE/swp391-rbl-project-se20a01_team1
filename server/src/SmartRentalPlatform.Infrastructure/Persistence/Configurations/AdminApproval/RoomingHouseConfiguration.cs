using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class RoomingHouseConfiguration : IEntityTypeConfiguration<RoomingHouse>
{
    public void Configure(EntityTypeBuilder<RoomingHouse> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .ValueGeneratedNever();

        builder.Property(h => h.LandlordUserId)
            .IsRequired();

        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(h => h.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(h => h.Description)
            .HasMaxLength(2000);

        builder.Property(h => h.Amenities)
            .HasMaxLength(1000);

        builder.Property(h => h.ApprovalStatus)
            .HasConversion<int>()
            .HasDefaultValue(RoomingHouseApprovalStatus.Draft);

        builder.Property(h => h.RejectedReason)
            .HasMaxLength(1000);

        builder.Property(h => h.Visibility)
            .HasConversion<int>()
            .HasDefaultValue(RoomingHouseVisibility.Hidden);

        builder.Property(h => h.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(h => h.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relationship: RoomingHouse - Rooms (1 to many)
        builder.HasMany(h => h.Rooms)
            .WithOne(r => r.RoomingHouse)
            .HasForeignKey(r => r.RoomingHouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(h => h.Images)
            .WithOne(i => i.RoomingHouse)
            .HasForeignKey(i => i.RoomingHouseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(h => h.ApprovalStatus);
        builder.HasIndex(h => h.Visibility);
        builder.HasIndex(h => h.LandlordUserId);

        builder.ToTable("rooming_houses", "admin_approval");
    }
}
