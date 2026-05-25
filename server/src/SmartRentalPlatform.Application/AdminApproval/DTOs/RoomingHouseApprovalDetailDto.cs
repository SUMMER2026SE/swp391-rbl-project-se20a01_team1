namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

/// <summary>
/// DTO hiển thị chi tiết khu trọ cần duyệt
/// </summary>
public class RoomingHouseApprovalDetailDto
{
    public Guid Id { get; set; }
    public Guid LandlordUserId { get; set; }
    
    public string? LandlordEmail { get; set; }
    public string? LandlordName { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public string ApprovalStatus { get; set; } = string.Empty;
    public string? RejectedReason { get; set; }
    
    // Danh sách phòng
    public List<RoomInfoDto> Rooms { get; set; } = new();
    
    // Người duyệt và thời gian
    public Guid? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO thông tin phòng trong khu trọ
/// </summary>
public class RoomInfoDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
}
