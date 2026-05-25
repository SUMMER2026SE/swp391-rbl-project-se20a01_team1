using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRentalPlatform.Application.AdminApproval.DTOs;
using SmartRentalPlatform.Application.AdminApproval.Services;

namespace SmartRentalPlatform.Api.Controllers;

/// <summary>
/// Controller xử lý duyệt KYC cho Admin
/// </summary>
[ApiController]
[Route("api/admin/kyc")]
[Authorize(Roles = "Admin")]
public class AdminKYCController : ControllerBase
{
    private readonly IKYCApprovalService _kycApprovalService;
    private readonly IApprovalAuditService _auditService;

    public AdminKYCController(
        IKYCApprovalService kycApprovalService,
        IApprovalAuditService auditService)
    {
        _kycApprovalService = kycApprovalService;
        _auditService = auditService;
    }

    /// <summary>
    /// Lấy danh sách KYC cần duyệt (PendingAdminReview)
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<KYCListResponseDto>> GetPendingKYCs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        var result = await _kycApprovalService.GetPendingKYCsAsync(pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết KYC cần duyệt
    /// </summary>
    [HttpGet("{kycId}")]
    public async Task<ActionResult<KYCDetailDto>> GetKYCDetail(
        [FromRoute] Guid kycId,
        CancellationToken cancellationToken = default)
    {
        var result = await _kycApprovalService.GetKYCDetailAsync(kycId, cancellationToken);
        if (result == null)
            return NotFound("KYC not found");

        return Ok(result);
    }

    /// <summary>
    /// Duyệt KYC (Approve)
    /// </summary>
    [HttpPost("{kycId}/approve")]
    public async Task<ActionResult> ApproveKYC(
        [FromRoute] Guid kycId,
        CancellationToken cancellationToken = default)
    {
        var adminId = GetCurrentUserId(); // Từ HttpContext claims
        
        var success = await _kycApprovalService.ApproveKYCAsync(kycId, adminId, cancellationToken);
        if (!success)
            return BadRequest("Failed to approve KYC");

        // Ghi log audit
        await _auditService.LogApprovalAsync(
            adminId,
            "KYC",
            kycId,
            "Approved",
            null,
            null,
            cancellationToken);

        return Ok(new { message = "KYC approved successfully" });
    }

    /// <summary>
    /// Từ chối KYC (Reject) - bắt buộc nhập lý do
    /// </summary>
    [HttpPost("{kycId}/reject")]
    public async Task<ActionResult> RejectKYC(
        [FromRoute] Guid kycId,
        [FromBody] RejectKYCRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RejectedReason))
            return BadRequest("RejectedReason is required");

        var adminId = GetCurrentUserId();
        
        var success = await _kycApprovalService.RejectKYCAsync(kycId, request.RejectedReason, adminId, cancellationToken);
        if (!success)
            return BadRequest("Failed to reject KYC");

        // Ghi log audit
        await _auditService.LogApprovalAsync(
            adminId,
            "KYC",
            kycId,
            "Rejected",
            request.RejectedReason,
            null,
            cancellationToken);

        return Ok(new { message = "KYC rejected successfully" });
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
