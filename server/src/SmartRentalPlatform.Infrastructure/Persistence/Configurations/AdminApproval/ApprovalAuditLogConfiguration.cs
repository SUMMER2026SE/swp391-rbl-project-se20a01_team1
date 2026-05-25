using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRentalPlatform.Domain.Entities.AdminApproval;

namespace SmartRentalPlatform.Infrastructure.Persistence.Configurations.AdminApproval;

public class ApprovalAuditLogConfiguration : IEntityTypeConfiguration<ApprovalAuditLog>
{
    public void Configure(EntityTypeBuilder<ApprovalAuditLog> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.AdminId)
            .IsRequired();

        builder.Property(a => a.ApprovalType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.EntityId)
            .IsRequired();

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Reason)
            .HasMaxLength(1000);

        builder.Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.AdditionalInfo)
            .HasColumnType("jsonb"); // PostgreSQL JSONB

        // Index
        builder.HasIndex(a => new { a.AdminId, a.CreatedAt });
        builder.HasIndex(a => new { a.ApprovalType, a.EntityId });

        builder.ToTable("approval_audit_logs", "admin_approval");
    }
}
