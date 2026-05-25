using SmartRentalPlatform.Application.AdminApproval.DTOs;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

/// <summary>
/// Service xử lý duyệt KYC
/// </summary>
public interface IKYCApprovalService
{
    /// <summary>
    /// Lấy danh sách KYC cần duyệt
    /// </summary>
    Task<KYCListResponseDto> GetPendingKYCsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy chi tiết KYC cần duyệt
    /// </summary>
    Task<KYCDetailDto?> GetKYCDetailAsync(Guid kycId, CancellationToken cancellationToken);

    /// <summary>
    /// Duyệt KYC (Approve)
    /// </summary>
    Task<bool> ApproveKYCAsync(Guid kycId, Guid adminId, CancellationToken cancellationToken);

    /// <summary>
    /// Từ chối KYC (Reject)
    /// </summary>
    Task<bool> RejectKYCAsync(Guid kycId, string rejectedReason, Guid adminId, CancellationToken cancellationToken);
}
