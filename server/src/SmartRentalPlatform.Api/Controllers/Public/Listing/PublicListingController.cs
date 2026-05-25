using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRentalPlatform.Application.AdminApproval.DTOs;
using SmartRentalPlatform.Application.AdminApproval.Services;

namespace SmartRentalPlatform.Api.Controllers;

/// <summary>
/// Controller xử lý Public Listing
/// Không cần đăng nhập - công khai cho Guest/Tenant
/// </summary>
[ApiController]
[Route("api/public/rooming-houses")]
public class PublicListingController : ControllerBase
{
    private readonly IPublicListingService _publicListingService;

    public PublicListingController(IPublicListingService publicListingService)
    {
        _publicListingService = publicListingService;
    }

    /// <summary>
    /// Lấy danh sách khu trọ công khai
    /// Chỉ hiển thị: Approved + Visible + có phòng Available
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<PublicRoomingHouseDto>>> GetPublicRoomingHouses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchKeyword = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        var result = await _publicListingService.GetPublicRoomingHousesAsync(
            pageNumber, pageSize, searchKeyword, minPrice, maxPrice, cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết khu trọ từ public listing
    /// Chỉ hiển thị phòng có trạng thái Available
    /// </summary>
    [HttpGet("{roomingHouseId}")]
    [AllowAnonymous]
    public async Task<ActionResult<PublicRoomingHouseDetailDto>> GetPublicRoomingHouseDetail(
        [FromRoute] Guid roomingHouseId,
        CancellationToken cancellationToken = default)
    {
        var result = await _publicListingService.GetPublicRoomingHouseDetailAsync(roomingHouseId, cancellationToken);
        if (result == null)
            return NotFound("Rooming house not found or not available");

        return Ok(result);
    }
}
