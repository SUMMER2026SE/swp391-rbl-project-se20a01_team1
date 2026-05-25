namespace SmartRentalPlatform.Contracts.Identity;

/// <summary>
/// Internal service (Người 2) — cấp role Landlord sau khi Admin duyệt khu trọ đầu tiên.
/// </summary>
public interface ILandlordRoleInternalService
{
    /// <summary>
    /// Cấp role Landlord cho chủ trọ của khu trọ vừa được duyệt (nếu chưa có role).
    /// </summary>
    /// <exception cref="InvalidOperationException">Khi không thể cấp role (caller nên rollback transaction).</exception>
    Task GrantLandlordRoleAfterRoomingHouseApprovedAsync(
        Guid roomingHouseId,
        CancellationToken cancellationToken = default);
}
