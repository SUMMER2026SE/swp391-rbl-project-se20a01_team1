namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

/// <summary>
/// Request duyệt KYC (Approve)
/// </summary>
public class ApproveKYCRequestDto
{
    public Guid KYCId { get; set; }
}

/// <summary>
/// Request từ chối KYC (Reject)
/// </summary>
public class RejectKYCRequestDto
{
    public Guid KYCId { get; set; }
    
    /// <summary>
    /// Bắt buộc - lý do từ chối để gửi cho người dùng
    /// </summary>
    public string RejectedReason { get; set; } = string.Empty;
}
