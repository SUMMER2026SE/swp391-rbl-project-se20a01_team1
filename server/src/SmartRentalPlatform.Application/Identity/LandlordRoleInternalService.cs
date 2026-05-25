using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Contracts.Identity;
using SmartRentalPlatform.Domain.Entities;
using SmartRentalPlatform.Domain.Enums;
using SmartRentalPlatform.Infrastructure.Persistence;

namespace SmartRentalPlatform.Application.Identity;

/// <summary>
/// Triển khai tạm Interval 1 (thay thế bởi Người 2 khi sẵn sàng).
/// </summary>
public class LandlordRoleInternalService : ILandlordRoleInternalService
{
    private readonly AppDbContext _dbContext;

    public LandlordRoleInternalService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task GrantLandlordRoleAfterRoomingHouseApprovedAsync(
        Guid roomingHouseId,
        CancellationToken cancellationToken = default)
    {
        var house = await _dbContext.RoomingHouses
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == roomingHouseId, cancellationToken)
            ?? throw new InvalidOperationException($"Rooming house {roomingHouseId} not found.");

        if (house.ApprovalStatus != RoomingHouseApprovalStatus.Approved)
            throw new InvalidOperationException("Rooming house must be Approved before granting Landlord role.");

        var landlordUserId = house.LandlordUserId;

        var landlordRole = await _dbContext.Role
            .FirstOrDefaultAsync(r => r.Name == "Landlord", cancellationToken)
            ?? throw new InvalidOperationException("Landlord role is not configured.");

        var alreadyLandlord = await _dbContext.UserRoles
            .AnyAsync(ur => ur.UserId == landlordUserId && ur.RoleId == landlordRole.Id, cancellationToken);

        if (alreadyLandlord)
            return;

        var isFirstApprovedHouse = !await _dbContext.RoomingHouses
            .AnyAsync(h => h.LandlordUserId == landlordUserId
                       && h.Id != roomingHouseId
                       && h.ApprovalStatus == RoomingHouseApprovalStatus.Approved
                       && h.DeletedAt == null,
                cancellationToken);

        if (!isFirstApprovedHouse)
            return;

        _dbContext.UserRoles.Add(new UserRole
        {
            UserId = landlordUserId,
            RoleId = landlordRole.Id,
            CreatedAt = DateTime.UtcNow
        });
        // Caller (trong transaction) gọi SaveChangesAsync.
    }
}
