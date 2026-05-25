using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRentalPlatform.Application.AdminApproval.DTOs;
using SmartRentalPlatform.Application.AdminApproval.Services;

namespace SmartRentalPlatform.Api.Controllers;

/// <summary>
/// Controller xử lý duyệt khu trọ cho Admin
/// </summary>
[ApiController]
[Route("api/admin/rooming-houses")]
[Authorize(Roles = "Admin")]
public class AdminRoomingHouseController : ControllerBase
{
    private readonly IRoomingHouseApprovalService _roomingHouseApprovalService;
    private readonly IApprovalAuditService _auditService;

    public AdminRoomingHouseController(
        IRoomingHouseApprovalService roomingHouseApprovalService,
        IApprovalAuditService auditService)
    {
        _roomingHouseApprovalService = roomingHouseApprovalService;
        _auditService = auditService;
    }

    /// <summary>
    /// Lấy danh sách khu trọ cần duyệt (PendingAdminReview)
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<RoomingHouseApprovalListResponseDto>> GetPendingRoomingHouses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        var result = await _roomingHouseApprovalService.GetPendingRoomingHousesAsync(pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết khu trọ cần duyệt
    /// </summary>
    [HttpGet("{roomingHouseId}")]
    public async Task<ActionResult<RoomingHouseApprovalDetailDto>> GetRoomingHouseDetail(
        [FromRoute] Guid roomingHouseId,
        CancellationToken cancellationToken = default)
    {
        var result = await _roomingHouseApprovalService.GetRoomingHouseDetailAsync(roomingHouseId, cancellationToken);
        if (result == null)
            return NotFound("Rooming house not found");

        return Ok(result);
    }

    /// <summary>
    /// Duyệt khu trọ (Approve)
    /// Nếu là khu trọ đầu tiên, hệ thống sẽ cấp role Landlord cho chủ trọ
    /// </summary>
    [HttpPost("{roomingHouseId}/approve")]
    public async Task<ActionResult> ApproveRoomingHouse(
        [FromRoute] Guid roomingHouseId,
        CancellationToken cancellationToken = default)
    {
        var adminId = GetCurrentUserId();
        
        var success = await _roomingHouseApprovalService.ApproveRoomingHouseAsync(roomingHouseId, adminId, cancellationToken);
        if (!success)
            return BadRequest("Failed to approve rooming house");

        // Ghi log audit
        await _auditService.LogApprovalAsync(
            adminId,
            "RoomingHouse",
            roomingHouseId,
            "Approved",
            null,
            null,
            cancellationToken);

        return Ok(new { message = "Rooming house approved successfully" });
    }

    /// <summary>
    /// Từ chối khu trọ (Reject) - bắt buộc nhập lý do
    /// </summary>
    [HttpPost("{roomingHouseId}/reject")]
    public async Task<ActionResult> RejectRoomingHouse(
        [FromRoute] Guid roomingHouseId,
        [FromBody] RejectRoomingHouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RejectedReason))
            return BadRequest("RejectedReason is required");

        var adminId = GetCurrentUserId();
        
        var success = await _roomingHouseApprovalService.RejectRoomingHouseAsync(roomingHouseId, request.RejectedReason, adminId, cancellationToken);
        if (!success)
            return BadRequest("Failed to reject rooming house");

        // Ghi log audit
        await _auditService.LogApprovalAsync(
            adminId,
            "RoomingHouse",
            roomingHouseId,
            "Rejected",
            request.RejectedReason,
            null,
            cancellationToken);

        return Ok(new { message = "Rooming house rejected successfully" });
    }

    /// <summary>
    /// Lấy ID người dùng hiện tại từ claims
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;

        throw new UnauthorizedAccessException("User ID not found in claims");
    }
}
