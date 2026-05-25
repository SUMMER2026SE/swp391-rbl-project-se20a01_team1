namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

/// <summary>
/// DTO danh sách khu trọ cần duyệt
/// </summary>
public class RoomingHouseApprovalListDto
{
    public Guid Id { get; set; }
    public Guid LandlordUserId { get; set; }
    
    public string? LandlordEmail { get; set; }
    public string? LandlordName { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    public string ApprovalStatus { get; set; } = string.Empty;
    public int AvailableRoomCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO phản hồi danh sách khu trọ cần duyệt (với pagination)
/// </summary>
public class RoomingHouseApprovalListResponseDto
{
    public List<RoomingHouseApprovalListDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
