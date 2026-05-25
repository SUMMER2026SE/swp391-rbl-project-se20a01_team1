namespace SmartRentalPlatform.Domain.Enums;

/// <summary>
/// Trạng thái phê duyệt khu trọ
/// </summary>
public enum RoomingHouseApprovalStatus
{
    /// <summary>Chờ duyệt từ Admin</summary>
    PendingAdminReview = 0,
    
    /// <summary>Đã được Admin duyệt</summary>
    Approved = 1,
    
    /// <summary>Bị Admin từ chối</summary>
    Rejected = 2,
    
    /// <summary>Chưa được đăng ký duyệt</summary>
    Draft = 3
}
