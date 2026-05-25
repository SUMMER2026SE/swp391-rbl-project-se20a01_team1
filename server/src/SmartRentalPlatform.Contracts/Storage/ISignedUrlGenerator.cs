namespace SmartRentalPlatform.Contracts.Storage;

/// <summary>
/// Sinh Signed URL đọc file KYC / media (S3/Blob) có thời hạn.
/// </summary>
public interface ISignedUrlGenerator
{
    string? GenerateReadUrl(string? objectKey, CancellationToken cancellationToken = default);
}
