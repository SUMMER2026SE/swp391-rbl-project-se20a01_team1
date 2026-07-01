namespace SmartRentalPlatform.Application.Common.Interfaces;

public interface IPayOSWebhookSignatureVerifier
{
    bool VerifyPayment(string rawPayload, string? signatureHeader);
    bool VerifyPayout(string rawPayload, string? signatureHeader);
}
