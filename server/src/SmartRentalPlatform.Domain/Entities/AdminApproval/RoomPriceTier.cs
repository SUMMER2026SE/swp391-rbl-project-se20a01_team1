namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Bảng giá theo số người ở trong phòng.
/// </summary>
public class RoomPriceTier
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public int OccupantCount { get; set; }
    public decimal MonthlyPrice { get; set; }

    public Room? Room { get; set; }
}
