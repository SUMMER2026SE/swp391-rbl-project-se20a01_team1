namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

/// <summary>
/// Request duyệt khu trọ (Approve)
/// </summary>
public class ApproveRoomingHouseRequestDto
{
    public Guid RoomingHouseId { get; set; }
}

/// <summary>
/// Request từ chối khu trọ (Reject)
/// </summary>
public class RejectRoomingHouseRequestDto
{
    public Guid RoomingHouseId { get; set; }
    
    /// <summary>
    /// Bắt buộc - lý do từ chối để gửi cho chủ trọ
    /// </summary>
    public string RejectedReason { get; set; } = string.Empty;
}
