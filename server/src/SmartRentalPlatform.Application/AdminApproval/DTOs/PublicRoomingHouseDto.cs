namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

public class PublicRoomingHouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LandlordName { get; set; }
    public string? LandlordPhoneNumber { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public int AvailableRoomCount { get; set; }
    public decimal MinRoomPrice { get; set; }
    public decimal MaxRoomPrice { get; set; }
    /// <summary>Ví dụ: "Từ 2.000.000đ/tháng"</summary>
    public string PriceFromLabel { get; set; } = string.Empty;
}
