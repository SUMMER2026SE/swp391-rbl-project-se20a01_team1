namespace SmartRentalPlatform.Api.Options;

/// <summary>
/// Cấu hình cho JWT Token
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}

/// <summary>
/// Cấu hình cho Admin Approval Security
/// </summary>
public class AdminApprovalSecurityOptions
{
    public const string SectionName = "AdminApprovalSecurity";

    /// <summary>
    /// Thời gian hết hạn của Signed URL (phút)
    /// </summary>
    public int SignedUrlExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Bật/tắt mask CCCD
    /// </summary>
    public bool EnableCCIDMasking { get; set; } = true;

    /// <summary>
    /// Bật/tắt audit logging
    /// </summary>
    public bool EnableAuditLogging { get; set; } = true;

    /// <summary>
    /// S3 bucket name (nếu dùng AWS)
    /// </summary>
    public string? S3BucketName { get; set; }

    /// <summary>
    /// Azure Blob container name (nếu dùng Azure)
    /// </summary>
    public string? AzureBlobContainerName { get; set; }
}
