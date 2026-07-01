using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRentalPlatform.Application.Common.Exceptions;
using SmartRentalPlatform.Application.Common.Interfaces;
using SmartRentalPlatform.Application.Wallets;
using SmartRentalPlatform.Contracts.Common;
using SmartRentalPlatform.Contracts.Wallets.Requests;
using SmartRentalPlatform.Contracts.Wallets.Responses;
using System.Text;
using System.Text.Json;

namespace SmartRentalPlatform.Api.Controllers;

[ApiController]
[Route("api/v1/withdrawals")]
[Authorize]
public class WithdrawalsController : ControllerBase
{
    private readonly IWithdrawalService withdrawalService;
    private readonly IWithdrawalWebhookService webhookService;
    private readonly ICurrentUserService currentUserService;

    public WithdrawalsController(
        IWithdrawalService withdrawalService,
        IWithdrawalWebhookService webhookService,
        ICurrentUserService currentUserService)
    {
        this.withdrawalService = withdrawalService;
        this.webhookService = webhookService;
        this.currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<WithdrawalRequestResponse>>> RequestWithdrawal(
        [FromBody] CreateWithdrawalRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest(new { Error = "X-Idempotency-Key header is required." });
        }

        var userId = GetCurrentUserId();
        var withdrawal = await withdrawalService.RequestWithdrawalAsync(
            userId,
            request.Amount,
            request.BankBin,
            request.AccountNumber,
            request.AccountName,
            idempotencyKey,
            cancellationToken);

        if (withdrawal.Status == SmartRentalPlatform.Domain.Enums.Payments.WithdrawalStatus.Failed)
        {
            return BadRequest(new ApiResponse<WithdrawalRequestResponse>
            {
                Success = false,
                Message = withdrawal.Description ?? "Lỗi khi tạo yêu cầu rút tiền qua PayOS.",
                Data = null
            });
        }

        return Ok(new ApiResponse<WithdrawalRequestResponse>
        {
            Success = true,
            Message = "Yêu cầu rút tiền của bạn đang được hệ thống xử lý. Vui lòng kiểm tra lại trạng thái trong lịch sử giao dịch sau ít phút.",
            Data = new WithdrawalRequestResponse
            {
                Id = withdrawal.Id,
                WalletAccountId = withdrawal.WalletAccountId,
                Amount = withdrawal.Amount,
                Fee = withdrawal.Fee,
                Status = withdrawal.Status.ToString(),
                ProviderOrderCode = withdrawal.ProviderOrderCode,
                BankBin = withdrawal.BankBin,
                AccountName = withdrawal.AccountName,
                AccountNumber = withdrawal.AccountNumber,
                Description = withdrawal.Description,
                CreatedAt = withdrawal.CreatedAt,
                UpdatedAt = withdrawal.UpdatedAt
            }
        });
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<PagedResult<WithdrawalRequestResponse>>>> GetMyWithdrawals(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var result = await withdrawalService.GetMyWithdrawalRequestsAsync(
            userId,
            page,
            pageSize,
            cancellationToken);

        var response = new PagedResult<WithdrawalRequestResponse>
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            Items = result.Items.Select(withdrawal => new WithdrawalRequestResponse
            {
                Id = withdrawal.Id,
                WalletAccountId = withdrawal.WalletAccountId,
                Amount = withdrawal.Amount,
                Fee = withdrawal.Fee,
                Status = withdrawal.Status.ToString(),
                ProviderOrderCode = withdrawal.ProviderOrderCode,
                BankBin = withdrawal.BankBin,
                AccountName = withdrawal.AccountName,
                AccountNumber = withdrawal.AccountNumber,
                Description = withdrawal.Description,
                CreatedAt = withdrawal.CreatedAt,
                UpdatedAt = withdrawal.UpdatedAt
            }).ToList()
        };

        return Ok(new ApiResponse<PagedResult<WithdrawalRequestResponse>>
        {
            Success = true,
            Message = "Lấy danh sách yêu cầu rút tiền thành công.",
            Data = response
        });
    }

    [HttpPost("payos-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PayOSWebhook(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var rawPayload = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["x-payos-signature"].FirstOrDefault();

        PayOSPayoutWebhookRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<PayOSPayoutWebhookRequest>(rawPayload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return BadRequest();
        }

        var payout = request?.Data?.Payouts?.FirstOrDefault();
        if (payout == null)
        {
            return BadRequest();
        }

        var status = payout.Transactions?.FirstOrDefault()?.State ?? payout.ApprovalState;

        await webhookService.ProcessWebhookAsync(
            payout.ReferenceId,
            status,
            rawPayload,
            signature,
            false,
            cancellationToken);

        return Ok(new { success = true });
    }

    private Guid GetCurrentUserId()
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new UnauthorizedException(
                ErrorCodes.Unauthorized,
                "Bạn cần đăng nhập để thực hiện thao tác này.");
        }

        return currentUserService.UserId.Value;
    }
}
