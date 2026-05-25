namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Entity lưu audit log các hành động duyệt của Admin
/// </summary>
public class ApprovalAuditLog
{
    public Guid Id { get; set; }
    
    // Người thực hiện duyệt
    public Guid AdminId { get; set; }
    
    // Loại duyệt: "KYC" hoặc "RoomingHouse"
    public string ApprovalType { get; set; } = string.Empty;
    
    // ID của entity được duyệt (KYC ID hoặc RoomingHouse ID)
    public Guid EntityId { get; set; }
    
    // Hành động: "Approved" hoặc "Rejected"
    public string Action { get; set; } = string.Empty;
    
    // Lý do (nếu có)
    public string? Reason { get; set; }
    
    // Thời gian thực hiện
    public DateTime CreatedAt { get; set; }
    
    // Chi tiết bổ sung (JSON format)
    public string? AdditionalInfo { get; set; }
}
