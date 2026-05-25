namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

public class PublicRoomingHouseDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Amenities { get; set; } = new();
    public string? LandlordName { get; set; }
    public string? LandlordPhoneNumber { get; set; }
    public string? LandlordEmail { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<PublicRoomDto> AvailableRooms { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class PublicRoomDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Area { get; set; }
    public int Capacity { get; set; }
    public string PriceFromLabel { get; set; } = string.Empty;
    public List<PublicRoomPriceTierDto> PriceTiers { get; set; } = new();
    public string? Description { get; set; }
    public List<string> ImageUrls { get; set; } = new();
}

public class PublicRoomPriceTierDto
{
    public int OccupantCount { get; set; }
    public decimal MonthlyPrice { get; set; }
    public string Label { get; set; } = string.Empty;
}
