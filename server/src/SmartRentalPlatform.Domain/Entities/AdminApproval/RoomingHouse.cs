using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Entity khu trọ - Người 5 chỉ cập nhật trạng thái phê duyệt
/// (Định nghĩa này là reference, thực tế do Người 4 quản lý)
/// </summary>
public class RoomingHouse
{
    public Guid Id { get; set; }
    public Guid LandlordUserId { get; set; }
    
    // Thông tin cơ bản
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>Tiện nghi (chuỗi, phân tách bằng dấu phẩy) — hiển thị public.</summary>
    public string? Amenities { get; set; }

    public ICollection<RoomingHouseImage> Images { get; set; } = new List<RoomingHouseImage>();
    
    // Trạng thái phê duyệt (Người 5 quản lý)
    public RoomingHouseApprovalStatus ApprovalStatus { get; set; } = RoomingHouseApprovalStatus.Draft;
    
    // Lý do từ chối (Người 5 set khi reject)
    public string? RejectedReason { get; set; }
    
    // Người duyệt (Người 5 set)
    public Guid? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    
    // Trạng thái hiển thị công khai (Chủ trọ bật khi chuẩn bị xong)
    public RoomingHouseVisibility Visibility { get; set; } = RoomingHouseVisibility.Hidden;
    
    // Danh sách phòng trong khu trọ
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
