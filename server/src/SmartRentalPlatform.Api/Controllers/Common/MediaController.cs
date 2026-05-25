using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartRentalPlatform.Application.Storage;
using System.Security.Cryptography;
using System.Text;

namespace SmartRentalPlatform.Api.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly SignedUrlOptions _options;

    public MediaController(IOptions<SignedUrlOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>Đọc file KYC qua Signed URL (có thời hạn).</summary>
    [HttpGet("read")]
    [AllowAnonymous]
    public IActionResult ReadSigned([FromQuery] string key, [FromQuery] long expires, [FromQuery] string sig)
    {
        if (!ValidateSignature(key, expires, sig))
            return Forbid();

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expires)
            return Unauthorized("Signed URL expired");

        return Ok(new { key, message = "Placeholder media — connect S3/Blob in production" });
    }

    /// <summary>Ảnh công khai listing (Approved + Visible).</summary>
    [HttpGet("public/{*objectKey}")]
    [AllowAnonymous]
    public IActionResult ReadPublic(string objectKey)
    {
        return Ok(new { objectKey, message = "Placeholder public media — Người 4 CDN" });
    }

    private bool ValidateSignature(string key, long expires, string sig)
    {
        var payload = $"{key}|{expires}";
        var keyBytes = Encoding.UTF8.GetBytes(_options.MediaSigningSecret);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var expected = Convert.ToHexString(hash).ToLowerInvariant();
        return string.Equals(expected, sig, StringComparison.OrdinalIgnoreCase);
    }
}
