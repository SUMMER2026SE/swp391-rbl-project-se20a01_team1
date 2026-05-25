namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

/// <summary>
/// DTO hiển thị chi tiết KYC cần duyệt
/// </summary>
public class KYCDetailDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string? UserEmail { get; set; }
    public string? UserDisplayName { get; set; }
    
    // Dữ liệu OCR
    public string? FullName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? IdNumber { get; set; } // Masked
    public string? Address { get; set; }
    
    // Kết quả xác thực
    public string? FaceMatchScore { get; set; }
    public string? LivenessScore { get; set; }
    
    // Ảnh giấy tờ (Signed URL)
    public string? IdImageUrl { get; set; }
    public string? FaceImageUrl { get; set; }
    
    // Trạng thái và lý do từ chối
    public string Status { get; set; } = string.Empty;
    public string? RejectedReason { get; set; }
    
    // Người duyệt và thời gian
    public Guid? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
