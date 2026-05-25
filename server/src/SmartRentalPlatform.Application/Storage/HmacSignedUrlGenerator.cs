using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SmartRentalPlatform.Contracts.Storage;

namespace SmartRentalPlatform.Application.Storage;

public class SignedUrlOptions
{
    public const string SectionName = "AdminApprovalSecurity";
    public int SignedUrlExpirationMinutes { get; set; } = 15;
    public string MediaSigningSecret { get; set; } = "CHANGE_ME_MEDIA_SIGNING_SECRET";
    public string? PublicMediaBaseUrl { get; set; } = "http://localhost:5000/api/media";
}

/// <summary>
/// Signed URL dạng HMAC (dev / Azure Blob / S3 gateway thay thế sau).
/// </summary>
public class HmacSignedUrlGenerator : ISignedUrlGenerator
{
    private readonly SignedUrlOptions _options;

    public HmacSignedUrlGenerator(IOptions<SignedUrlOptions> options)
    {
        _options = options.Value;
    }

    public string? GenerateReadUrl(string? objectKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
            return null;

        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.SignedUrlExpirationMinutes);
        var payload = $"{objectKey}|{expires.ToUnixTimeSeconds()}";
        var signature = ComputeSignature(payload);

        var baseUrl = (_options.PublicMediaBaseUrl ?? "http://localhost:5000/api/media").TrimEnd('/');
        return $"{baseUrl}/read?key={Uri.EscapeDataString(objectKey)}&expires={expires.ToUnixTimeSeconds()}&sig={signature}";
    }

    private string ComputeSignature(string payload)
    {
        var key = Encoding.UTF8.GetBytes(_options.MediaSigningSecret);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
