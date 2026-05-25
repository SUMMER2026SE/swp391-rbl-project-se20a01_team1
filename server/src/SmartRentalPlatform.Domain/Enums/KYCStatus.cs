namespace SmartRentalPlatform.Domain.Enums;

/// <summary>
/// Trạng thái xác thực danh tính KYC
/// </summary>
public enum KYCStatus
{
    /// <summary>Chờ duyệt từ Admin</summary>
    PendingAdminReview = 0,
    
    /// <summary>Đã được Admin duyệt</summary>
    Approved = 1,
    
    /// <summary>Bị Admin từ chối</summary>
    Rejected = 2,
    
    /// <summary>Chờ bước xác thực tiếp theo</summary>
    Pending = 3
}
