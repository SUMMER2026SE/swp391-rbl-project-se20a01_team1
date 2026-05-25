using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.RoomingHouseId)
            .IsRequired();

        builder.Property(r => r.RoomNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Price)
            .HasPrecision(10, 2);

        builder.Property(r => r.Area)
            .HasPrecision(8, 2)
            .IsRequired();

        builder.Property(r => r.Capacity)
            .IsRequired();

        builder.HasMany(r => r.PriceTiers)
            .WithOne(t => t.Room)
            .HasForeignKey(t => t.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Images)
            .WithOne(i => i.Room)
            .HasForeignKey(i => i.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .HasDefaultValue(RoomStatus.Available);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Index
        builder.HasIndex(r => r.RoomingHouseId);
        builder.HasIndex(r => r.Status);

        builder.ToTable("rooms", "admin_approval");
    }
}
