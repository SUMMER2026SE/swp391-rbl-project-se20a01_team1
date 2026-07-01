using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var payoutClientId = "186008dc-3733-4f24-9b57-60e587ff02bc";
        var payoutApiKey = "3ea87707-cbfd-4401-b593-9c8c93a02931";
        var payoutChecksumKey = "616f73db5f86642d93e18a0b07e78dcb7bda1400e28fcfb5e7d441da02534f59";

        var referenceId = DateTimeOffset.UtcNow.ToString("yyMMddHHmmssfff");
        var amount = 10000;
        var description = $"WD {referenceId}";
        var bankBin = "970422";
        var accountNumber = "0827177005";
        var accountName = "HOANG PHUC NHAT QUANG";

        var fields = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["amount"] = amount.ToString(),
            ["category"] = "[\"withdrawal\"]",
            ["description"] = description,
            ["referenceId"] = referenceId,
            ["toAccountName"] = accountName,
            ["toAccountNumber"] = accountNumber,
            ["toBin"] = bankBin
        };

        // URL ENCODE USING Uri.EscapeDataString
        var query = string.Join("&", fields.Select(kvp => 
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(payoutChecksumKey));
        var signature = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(query))).ToLowerInvariant();

        var requestBody = new {
            referenceId = referenceId,
            amount = amount,
            description = description,
            toBin = bankBin,
            toAccountNumber = accountNumber,
            toAccountName = accountName,
            category = new[] { "withdrawal" }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api-merchant.payos.vn/v1/payouts")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.TryAddWithoutValidation("x-client-id", payoutClientId);
        request.Headers.TryAddWithoutValidation("x-api-key", payoutApiKey);
        request.Headers.TryAddWithoutValidation("x-idempotency-key", Guid.NewGuid().ToString());
        request.Headers.TryAddWithoutValidation("x-signature", signature);

        var response = await httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"With Uri.EscapeDataString: {body}");
    }
}
