using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmartRentalPlatform.Application.Common.Interfaces;
using SmartRentalPlatform.Infrastructure.Options;

namespace SmartRentalPlatform.Infrastructure.ExternalServices.PayOS;

public class PayOSWebhookSignatureVerifier : IPayOSWebhookSignatureVerifier
{
    private readonly PayOSOptions options;

    public PayOSWebhookSignatureVerifier(IOptions<PayOSOptions> options)
    {
        this.options = options.Value;
    }

    public bool VerifyPayment(string rawPayload, string? signatureHeader)
    {
        return VerifyCore(rawPayload, signatureHeader, options.ChecksumKey);
    }

    public bool VerifyPayout(string rawPayload, string? signatureHeader)
    {
        return VerifyCore(rawPayload, signatureHeader, options.PayoutChecksumKey);
    }

    private bool VerifyCore(string rawPayload, string? signatureHeader, string checksumKey)
    {
        if (string.IsNullOrWhiteSpace(checksumKey))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(rawPayload);
            var root = document.RootElement;
            var signature = ExtractSignature(root, signatureHeader);
            if (string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            var data = root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("data", out var dataElement)
                && dataElement.ValueKind == JsonValueKind.Object
                    ? dataElement
                    : root;

            var canonicalData = BuildCanonicalData(data);
            var expected = ComputeHmacSha256(canonicalData, checksumKey);
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(signature.Trim().ToLowerInvariant()));
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string? ExtractSignature(JsonElement root, string? signatureHeader)
    {
        if (!string.IsNullOrWhiteSpace(signatureHeader))
        {
            return signatureHeader;
        }

        return root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("signature", out var signature)
            && signature.ValueKind == JsonValueKind.String
                ? signature.GetString()
                : null;
    }

    private static string BuildCanonicalData(JsonElement data)
    {
        var values = data.EnumerateObject()
            .Where(x => !string.Equals(x.Name, "signature", StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name, StringComparer.Ordinal)
            .Select(x => $"{x.Name}={ToCanonicalValue(x.Value)}");

        return string.Join('&', values);
    }

    private static string ToCanonicalValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => string.Empty,
            _ => value.GetRawText()
        };
    }

    private string ComputeHmacSha256(string data, string checksumKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLowerInvariant();
    }
}
