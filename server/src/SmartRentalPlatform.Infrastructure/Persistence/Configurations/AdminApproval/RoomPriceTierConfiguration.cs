using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class RoomPriceTierConfiguration : IEntityTypeConfiguration<RoomPriceTier>
{
    public void Configure(EntityTypeBuilder<RoomPriceTier> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.RoomId)
            .IsRequired();

        builder.Property(t => t.OccupantCount)
            .IsRequired();

        builder.Property(t => t.MonthlyPrice)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.HasIndex(t => new { t.RoomId, t.OccupantCount })
            .IsUnique();

        builder.ToTable("room_price_tiers", "admin_approval");
    }
}
