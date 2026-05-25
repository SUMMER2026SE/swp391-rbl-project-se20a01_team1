namespace SmartRentalPlatform.Application.AdminApproval.Services;

/// <summary>
/// Service xử lý Audit Logging
/// </summary>
public interface IApprovalAuditService
{
    /// <summary>
    /// Ghi log khi Admin duyệt/từ chối KYC hoặc Khu trọ
    /// </summary>
    Task LogApprovalAsync(
        Guid adminId,
        string approvalType, // "KYC" hoặc "RoomingHouse"
        Guid entityId,
        string action, // "Approved" hoặc "Rejected"
        string? reason,
        string? additionalInfo,
        CancellationToken cancellationToken);
}
