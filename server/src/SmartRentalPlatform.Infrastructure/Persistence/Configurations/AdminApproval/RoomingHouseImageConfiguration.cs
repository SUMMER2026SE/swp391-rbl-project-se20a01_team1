using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class RoomingHouseImageConfiguration : IEntityTypeConfiguration<RoomingHouseImage>
{
    public void Configure(EntityTypeBuilder<RoomingHouseImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ObjectKey).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => x.RoomingHouseId);
        builder.ToTable("rooming_house_images", "admin_approval");
    }
}
