using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class KYCVerificationConfiguration : IEntityTypeConfiguration<KYCVerification>
{
    public void Configure(EntityTypeBuilder<KYCVerification> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.Id)
            .ValueGeneratedNever();

        builder.Property(k => k.UserId)
            .IsRequired();

        builder.Property(k => k.FullName)
            .HasMaxLength(255);

        builder.Property(k => k.DateOfBirth)
            .HasMaxLength(50);

        builder.Property(k => k.IdNumber)
            .HasMaxLength(50);

        builder.Property(k => k.Address)
            .HasMaxLength(500);

        builder.Property(k => k.IdImageObjectKey)
            .HasMaxLength(500);

        builder.Property(k => k.FaceImageObjectKey)
            .HasMaxLength(500);

        builder.Property(k => k.FaceMatchScore)
            .HasMaxLength(50);

        builder.Property(k => k.LivenessScore)
            .HasMaxLength(50);

        builder.Property(k => k.Status)
            .HasConversion<int>()
            .HasDefaultValue(KYCStatus.PendingAdminReview);

        builder.Property(k => k.RejectedReason)
            .HasMaxLength(1000);

        builder.Property(k => k.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(k => k.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.ToTable("kyc_verifications", "admin_approval");
    }
}
