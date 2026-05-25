using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Entity lưu thông tin xác thực danh tính (KYC) - Người 5 chỉ cập nhật trạng thái phê duyệt
/// (Định nghĩa này là reference, thực tế do Người 3 quản lý)
/// </summary>
public class KYCVerification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    // Dữ liệu OCR từ VNPT
    public string? FullName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? IdNumber { get; set; }
    public string? Address { get; set; }
    
    // Object keys ảnh KYC (Người 3 upload — Admin xem qua Signed URL)
    public string? IdImageObjectKey { get; set; }
    public string? FaceImageObjectKey { get; set; }

    // Kết quả xác thực
    public string? FaceMatchScore { get; set; }
    public string? LivenessScore { get; set; }
    
    // Trạng thái xác thực (Người 3 set, Người 5 duyệt)
    public KYCStatus Status { get; set; }
    
    // Lý do từ chối (Người 5 set khi reject)
    public string? RejectedReason { get; set; }
    
    // Người duyệt (Người 5 set)
    public Guid? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
