namespace SmartRentalPlatform.Contracts.Listing;

/// <summary>
/// Lấy URL ảnh công khai cho khu trọ / phòng (thực thể ảnh do Người 4 quản lý).
/// </summary>
public interface IPublicListingImageService
{
    Task<IReadOnlyList<string>> GetRoomingHouseImageUrlsAsync(
        Guid roomingHouseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoomImageUrlsAsync(
        Guid roomId,
        CancellationToken cancellationToken = default);
}
