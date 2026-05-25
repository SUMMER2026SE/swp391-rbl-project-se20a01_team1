using System.Globalization;

namespace SmartRentalPlatform.Application.AdminApproval.Extensions;

public static class ListingFormatExtensions
{
    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string FormatVnd(this decimal amount) =>
        string.Format(ViCulture, "{0:N0}đ", amount);

    public static string ToPriceFromLabel(this decimal minPrice) =>
        $"Từ {minPrice.FormatVnd()}/tháng";

    public static string ToTierLabel(this int occupantCount, decimal monthlyPrice) =>
        $"{occupantCount} người: {monthlyPrice.FormatVnd()}/tháng";
}
