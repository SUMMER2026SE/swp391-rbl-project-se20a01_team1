using SmartRentalPlatform.Application.AdminApproval.DTOs;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

/// <summary>
/// Service xử lý duyệt khu trọ
/// </summary>
public interface IRoomingHouseApprovalService
{
    /// <summary>
    /// Lấy danh sách khu trọ cần duyệt
    /// </summary>
    Task<RoomingHouseApprovalListResponseDto> GetPendingRoomingHousesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy chi tiết khu trọ cần duyệt
    /// </summary>
    Task<RoomingHouseApprovalDetailDto?> GetRoomingHouseDetailAsync(Guid roomingHouseId, CancellationToken cancellationToken);

    /// <summary>
    /// Duyệt khu trọ (Approve) + cấp role Landlord nếu là khu trọ đầu tiên
    /// </summary>
    Task<bool> ApproveRoomingHouseAsync(Guid roomingHouseId, Guid adminId, CancellationToken cancellationToken);

    /// <summary>
    /// Từ chối khu trọ (Reject)
    /// </summary>
    Task<bool> RejectRoomingHouseAsync(Guid roomingHouseId, string rejectedReason, Guid adminId, CancellationToken cancellationToken);
}
