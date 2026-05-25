using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Entity phòng trong khu trọ - Người 5 chỉ xem trạng thái
/// (Định nghĩa này là reference, thực tế do Người 4 quản lý)
/// </summary>
public class Room
{
    public Guid Id { get; set; }
    public Guid RoomingHouseId { get; set; }
    
    public string RoomNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Area { get; set; }
    public int Capacity { get; set; }
    
    // Trạng thái phòng
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    
    public RoomingHouse? RoomingHouse { get; set; }
    public ICollection<RoomPriceTier> PriceTiers { get; set; } = new List<RoomPriceTier>();
    public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
