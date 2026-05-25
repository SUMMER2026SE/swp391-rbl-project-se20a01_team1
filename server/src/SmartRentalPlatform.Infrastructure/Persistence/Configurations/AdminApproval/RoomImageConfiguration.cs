using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class RoomImageConfiguration : IEntityTypeConfiguration<RoomImage>
{
    public void Configure(EntityTypeBuilder<RoomImage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ObjectKey).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => x.RoomId);
        builder.ToTable("room_images", "admin_approval");
    }
}
