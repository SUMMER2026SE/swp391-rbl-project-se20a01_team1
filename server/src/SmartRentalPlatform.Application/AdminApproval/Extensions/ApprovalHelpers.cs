namespace SmartRentalPlatform.Application.AdminApproval.Extensions;

/// <summary>
/// Helper methods cho Admin Approval
/// </summary>
public static class ApprovalHelpers
{
    /// <summary>
    /// Mask CCCD: 123456789 -> 123456****
    /// </summary>
    public static string? MaskCCCD(string? cccd, int visibleDigits = 6)
    {
        if (string.IsNullOrEmpty(cccd) || cccd.Length <= visibleDigits)
            return cccd;

        return cccd.Substring(0, visibleDigits) + new string('*', cccd.Length - visibleDigits);
    }

    /// <summary>
    /// Kiểm tra email là công khai (demo purposes only)
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra số điện thoại Việt Nam (demo purposes only)
    /// </summary>
    public static bool IsValidVietnamesePhoneNumber(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return false;

        // Kiểm tra: bắt đầu bằng 0 hoặc +84, dài 10-11 ký tự, chỉ có số
        var pattern = @"^(0|\+84)[0-9]{9,10}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phone, pattern);
    }
}
