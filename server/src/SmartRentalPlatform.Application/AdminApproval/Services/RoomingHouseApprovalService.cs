using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Application.AdminApproval.DTOs;
using SmartRentalPlatform.Contracts.Identity;
using SmartRentalPlatform.Infrastructure.Persistence;
using SmartRentalPlatform.Domain.Enums;

namespace SmartRentalPlatform.Application.AdminApproval.Services;

public class RoomingHouseApprovalService : IRoomingHouseApprovalService
{
    private readonly AppDbContext _dbContext;
    private readonly ILandlordRoleInternalService _landlordRoleInternalService;

    public RoomingHouseApprovalService(
        AppDbContext dbContext,
        ILandlordRoleInternalService landlordRoleInternalService)
    {
        _dbContext = dbContext;
        _landlordRoleInternalService = landlordRoleInternalService;
    }

    public async Task<RoomingHouseApprovalListResponseDto> GetPendingRoomingHousesAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.RoomingHouses
            .Where(h => h.ApprovalStatus == RoomingHouseApprovalStatus.PendingAdminReview
                     && h.DeletedAt == null);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(h => h.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Join(
                _dbContext.Users,
                h => h.LandlordUserId,
                u => u.Id,
                (h, u) => new RoomingHouseApprovalListDto
                {
                    Id = h.Id,
                    LandlordUserId = h.LandlordUserId,
                    LandlordEmail = u.Email,
                    LandlordName = u.DisplayName,
                    Name = h.Name,
                    Address = h.Address,
                    ApprovalStatus = h.ApprovalStatus.ToString(),
                    AvailableRoomCount = h.Rooms.Count(r => r.Status == RoomStatus.Available),
                    CreatedAt = h.CreatedAt
                })
            .ToListAsync(cancellationToken);

        return new RoomingHouseApprovalListResponseDto
        {
            Items = items,
            TotalCount = totalCount,
            PageSize = pageSize,
            PageNumber = pageNumber
        };
    }

    public async Task<RoomingHouseApprovalDetailDto?> GetRoomingHouseDetailAsync(
        Guid roomingHouseId, CancellationToken cancellationToken)
    {
        var house = await _dbContext.RoomingHouses
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == roomingHouseId, cancellationToken);

        if (house == null)
            return null;

        var landlord = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == house.LandlordUserId, cancellationToken);

        return new RoomingHouseApprovalDetailDto
        {
            Id = house.Id,
            LandlordUserId = house.LandlordUserId,
            LandlordEmail = landlord?.Email,
            LandlordName = landlord?.DisplayName,
            Name = house.Name,
            Address = house.Address,
            Description = house.Description,
            ApprovalStatus = house.ApprovalStatus.ToString(),
            RejectedReason = house.RejectedReason,
            Rooms = house.Rooms
                .Where(r => r.DeletedAt == null)
                .Select(r => new RoomInfoDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    Price = r.Price,
                    Capacity = r.Capacity,
                    Status = r.Status.ToString()
                })
                .ToList(),
            ReviewedByAdminId = house.ReviewedByAdminId,
            ReviewedAt = house.ReviewedAt,
            CreatedAt = house.CreatedAt
        };
    }

    public async Task<bool> ApproveRoomingHouseAsync(
        Guid roomingHouseId, Guid adminId, CancellationToken cancellationToken)
    {
        var house = await _dbContext.RoomingHouses
            .FirstOrDefaultAsync(h => h.Id == roomingHouseId, cancellationToken);

        if (house == null || house.ApprovalStatus != RoomingHouseApprovalStatus.PendingAdminReview)
            return false;

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            house.ApprovalStatus = RoomingHouseApprovalStatus.Approved;
            house.ReviewedByAdminId = adminId;
            house.ReviewedAt = DateTime.UtcNow;
            house.Visibility = RoomingHouseVisibility.Hidden;

            _dbContext.RoomingHouses.Update(house);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _landlordRoleInternalService.GrantLandlordRoleAfterRoomingHouseApprovedAsync(
                roomingHouseId, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> RejectRoomingHouseAsync(
        Guid roomingHouseId, string rejectedReason, Guid adminId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rejectedReason))
            return false;

        var house = await _dbContext.RoomingHouses
            .FirstOrDefaultAsync(h => h.Id == roomingHouseId, cancellationToken);

        if (house == null || house.ApprovalStatus != RoomingHouseApprovalStatus.PendingAdminReview)
            return false;

        house.ApprovalStatus = RoomingHouseApprovalStatus.Rejected;
        house.RejectedReason = rejectedReason;
        house.ReviewedByAdminId = adminId;
        house.ReviewedAt = DateTime.UtcNow;

        _dbContext.RoomingHouses.Update(house);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
