using SmartRentalPlatform.Application.AdminApproval.DTOs;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

/// <summary>
/// Service xử lý Public Listing
/// Chỉ hiển thị khu trọ: Approved + Visible + có phòng Available
/// </summary>
public interface IPublicListingService
{
    /// <summary>
    /// Lấy danh sách khu trọ công khai (chỉ Approved + Visible + có phòng trống)
    /// </summary>
    Task<List<PublicRoomingHouseDto>> GetPublicRoomingHousesAsync(
        int pageNumber, 
        int pageSize, 
        string? searchKeyword,
        decimal? minPrice, 
        decimal? maxPrice,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lấy chi tiết khu trọ trên public listing (chỉ hiển thị phòng Available)
    /// </summary>
    Task<PublicRoomingHouseDetailDto?> GetPublicRoomingHouseDetailAsync(Guid roomingHouseId, CancellationToken cancellationToken);
}
