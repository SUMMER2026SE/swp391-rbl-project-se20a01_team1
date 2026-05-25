namespace SmartRentalPlatform.Domain.Entities.AdminApproval;

/// <summary>
/// Ảnh phòng (reference — owner: Người 4).
/// </summary>
public class RoomImage
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string ObjectKey { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public Room? Room { get; set; }
}
