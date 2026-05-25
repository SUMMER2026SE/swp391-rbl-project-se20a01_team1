namespace SmartRentalPlatform.Application.AdminApproval.DTOs;

/// <summary>
/// DTO danh sách KYC cần duyệt (hiển thị với pagination)
/// </summary>
public class KYCListDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string? UserEmail { get; set; }
    public string? UserDisplayName { get; set; }
    public string? FullName { get; set; }
    
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO phản hồi danh sách KYC (với pagination)
/// </summary>
public class KYCListResponseDto
{
    public List<KYCListDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
