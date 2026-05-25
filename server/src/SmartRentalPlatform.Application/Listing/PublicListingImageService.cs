using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartRentalPlatform.Application.Storage;
using SmartRentalPlatform.Contracts.Listing;
using SmartRentalPlatform.Infrastructure.Persistence;

namespace SmartRentalPlatform.Application.Listing;

public class PublicListingImageService : IPublicListingImageService
{
    private readonly AppDbContext _dbContext;
    private readonly SignedUrlOptions _options;

    public PublicListingImageService(AppDbContext dbContext, IOptions<SignedUrlOptions> options)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<string>> GetRoomingHouseImageUrlsAsync(
        Guid roomingHouseId,
        CancellationToken cancellationToken = default)
    {
        var keys = await _dbContext.RoomingHouseImages
            .AsNoTracking()
            .Where(i => i.RoomingHouseId == roomingHouseId)
            .OrderByDescending(i => i.IsCover)
            .ThenBy(i => i.SortOrder)
            .Select(i => i.ObjectKey)
            .ToListAsync(cancellationToken);

        return keys.Select(ToPublicMediaUrl).ToList();
    }

    public async Task<IReadOnlyList<string>> GetRoomImageUrlsAsync(
        Guid roomId,
        CancellationToken cancellationToken = default)
    {
        var keys = await _dbContext.RoomImages
            .AsNoTracking()
            .Where(i => i.RoomId == roomId)
            .OrderBy(i => i.SortOrder)
            .Select(i => i.ObjectKey)
            .ToListAsync(cancellationToken);

        return keys.Select(ToPublicMediaUrl).ToList();
    }

    private string ToPublicMediaUrl(string objectKey)
    {
        var baseUrl = (_options.PublicMediaBaseUrl ?? "http://localhost:5000/api/media").TrimEnd('/');
        return $"{baseUrl}/public/{Uri.EscapeDataString(objectKey)}";
    }
}
