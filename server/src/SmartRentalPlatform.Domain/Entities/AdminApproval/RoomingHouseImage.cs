namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Ảnh khu trọ (reference — owner: Người 4).
/// </summary>
public class RoomingHouseImage
{
    public Guid Id { get; set; }
    public Guid RoomingHouseId { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
    public RoomingHouse? RoomingHouse { get; set; }
}
