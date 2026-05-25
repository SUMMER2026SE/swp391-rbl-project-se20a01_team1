using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Application.AdminApproval.DTOs;
using SmartRentalPlatform.Application.AdminApproval.Extensions;
using SmartRentalPlatform.Contracts.Listing;
using SmartRentalPlatform.Infrastructure.Persistence;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

public class PublicListingService : IPublicListingService
{
    private readonly AppDbContext _dbContext;
    private readonly IPublicListingImageService _imageService;

    public PublicListingService(AppDbContext dbContext, IPublicListingImageService imageService)
    {
        _dbContext = dbContext;
        _imageService = imageService;
    }

    public async Task<List<PublicRoomingHouseDto>> GetPublicRoomingHousesAsync(
        int pageNumber,
        int pageSize,
        string? searchKeyword,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.RoomingHouses
            .Where(h => h.ApprovalStatus == RoomingHouseApprovalStatus.Approved
                     && h.Visibility == RoomingHouseVisibility.Visible
                     && h.Rooms.Any(r => r.Status == RoomStatus.Available && r.DeletedAt == null)
                     && h.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchKeyword))
        {
            var keyword = searchKeyword.ToLower();
            query = query.Where(h => h.Name.ToLower().Contains(keyword)
                                  || h.Address.ToLower().Contains(keyword));
        }

        if (minPrice.HasValue || maxPrice.HasValue)
        {
            query = query.Where(h =>
                (minPrice == null || h.Rooms.Any(r =>
                    r.Status == RoomStatus.Available
                    && (r.Price >= minPrice
                        || r.PriceTiers.Any(t => t.MonthlyPrice >= minPrice))))
                &&
                (maxPrice == null || h.Rooms.Any(r =>
                    r.Status == RoomStatus.Available
                    && (r.Price <= maxPrice
                        || r.PriceTiers.Any(t => t.MonthlyPrice <= maxPrice)))));
        }

        var houses = await query
            .Include(h => h.Rooms.Where(r => r.Status == RoomStatus.Available && r.DeletedAt == null))
            .ThenInclude(r => r.PriceTiers)
            .OrderByDescending(h => h.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var result = new List<PublicRoomingHouseDto>();
        foreach (var h in houses)
        {
            var dto = MapListItem(h);
            dto.ImageUrls = (await _imageService.GetRoomingHouseImageUrlsAsync(h.Id, cancellationToken)).ToList();
            if (dto.ImageUrls.Count == 0 && h.Images.Count > 0)
            {
                dto.ImageUrls = h.Images.OrderByDescending(i => i.IsCover).Select(i => i.ObjectKey).ToList();
            }
            result.Add(dto);
        }

        return result;
    }

    public async Task<PublicRoomingHouseDetailDto?> GetPublicRoomingHouseDetailAsync(
        Guid roomingHouseId,
        CancellationToken cancellationToken)
    {
        var house = await _dbContext.RoomingHouses
            .Include(h => h.Rooms.Where(r => r.Status == RoomStatus.Available && r.DeletedAt == null))
            .ThenInclude(r => r.PriceTiers)
            .Include(h => h.Images)
            .FirstOrDefaultAsync(h => h.Id == roomingHouseId
                                    && h.ApprovalStatus == RoomingHouseApprovalStatus.Approved
                                    && h.Visibility == RoomingHouseVisibility.Visible
                                    && h.DeletedAt == null,
                cancellationToken);

        if (house == null || !house.Rooms.Any())
            return null;

        var landlord = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == house.LandlordUserId, cancellationToken);

        var imageUrls = (await _imageService.GetRoomingHouseImageUrlsAsync(house.Id, cancellationToken)).ToList();

        var availableRooms = new List<PublicRoomDto>();
        foreach (var r in house.Rooms)
        {
            var roomDto = MapRoom(r);
            roomDto.ImageUrls = (await _imageService.GetRoomImageUrlsAsync(r.Id, cancellationToken)).ToList();
            availableRooms.Add(roomDto);
        }

        return new PublicRoomingHouseDetailDto
        {
            Id = house.Id,
            Name = house.Name,
            Address = house.Address,
            Description = house.Description,
            Amenities = ParseAmenities(house.Amenities),
            LandlordName = landlord?.DisplayName,
            LandlordPhoneNumber = landlord?.PhoneNumber,
            LandlordEmail = landlord?.Email,
            ImageUrls = imageUrls,
            AvailableRooms = availableRooms,
            CreatedAt = house.CreatedAt
        };
    }

    private static PublicRoomingHouseDto MapListItem(Domain.Entities.AdminApproval.RoomingHouse h)
    {
        var tierPrices = h.Rooms
            .SelectMany(r => r.PriceTiers.Select(t => t.MonthlyPrice))
            .DefaultIfEmpty()
            .ToList();
        var roomPrices = h.Rooms.Select(r => r.Price).ToList();
        var allPrices = tierPrices.Concat(roomPrices).Where(p => p > 0).ToList();
        var min = allPrices.DefaultIfEmpty(0).Min();

        return new PublicRoomingHouseDto
        {
            Id = h.Id,
            Name = h.Name,
            Address = h.Address,
            Description = h.Description,
            AvailableRoomCount = h.Rooms.Count,
            MinRoomPrice = min,
            MaxRoomPrice = allPrices.DefaultIfEmpty(0).Max(),
            PriceFromLabel = min > 0 ? min.ToPriceFromLabel() : string.Empty
        };
    }

    private static PublicRoomDto MapRoom(Domain.Entities.AdminApproval.Room r)
    {
        var tiers = r.PriceTiers
            .OrderBy(t => t.OccupantCount)
            .Select(t => new PublicRoomPriceTierDto
            {
                OccupantCount = t.OccupantCount,
                MonthlyPrice = t.MonthlyPrice,
                Label = t.OccupantCount.ToTierLabel(t.MonthlyPrice)
            })
            .ToList();

        var minTier = tiers.FirstOrDefault()?.MonthlyPrice ?? r.Price;

        return new PublicRoomDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Price = r.Price,
            Area = r.Area,
            Capacity = r.Capacity,
            PriceFromLabel = minTier.ToPriceFromLabel(),
            PriceTiers = tiers
        };
    }

    private static List<string> ParseAmenities(string? amenities) =>
        string.IsNullOrWhiteSpace(amenities)
            ? new List<string>()
            : amenities.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}
