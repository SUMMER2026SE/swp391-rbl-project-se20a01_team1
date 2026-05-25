using SmartRentalPlatform.Infrastructure.Persistence;
using SmartRentalPlatform.Domain.Entities.AdminApproval;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

/// <summary>
/// Implementation của Approval Audit Service
/// Lưu log tất cả các hành động duyệt/từ chối
/// </summary>
public class ApprovalAuditService : IApprovalAuditService
{
    private readonly AppDbContext _dbContext;

    public ApprovalAuditService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogApprovalAsync(
        Guid adminId,
        string approvalType, // "KYC" hoặc "RoomingHouse"
        Guid entityId,
        string action, // "Approved" hoặc "Rejected"
        string? reason,
        string? additionalInfo,
        CancellationToken cancellationToken)
    {
        var auditLog = new ApprovalAuditLog
        {
            Id = Guid.NewGuid(),
            AdminId = adminId,
            ApprovalType = approvalType,
            EntityId = entityId,
            Action = action,
            Reason = reason,
            AdditionalInfo = additionalInfo,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.ApprovalAuditLogs.AddAsync(auditLog, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
