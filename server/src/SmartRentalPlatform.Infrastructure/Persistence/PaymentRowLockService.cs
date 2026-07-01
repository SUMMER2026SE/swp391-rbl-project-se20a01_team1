using Microsoft.EntityFrameworkCore;
using SmartRentalPlatform.Application.Common.Interfaces;
using SmartRentalPlatform.Domain.Entities.Billing;
using SmartRentalPlatform.Domain.Entities.Payments;
using SmartRentalPlatform.Domain.Entities.Rental;
using SmartRentalPlatform.Domain.Entities.RentalContracts;
using SmartRentalPlatform.Domain.Entities.Users;

namespace SmartRentalPlatform.Infrastructure.Persistence;

public class PaymentRowLockService : IPaymentRowLockService
{
    private readonly AppDbContext context;

    public PaymentRowLockService(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<PaymentTransaction?> LockPaymentTransactionByProviderOrderCodeAsync(
        string providerOrderCode,
        CancellationToken cancellationToken = default)
    {
        var paymentTransaction_list = await context.PaymentTransactions
            .FromSqlInterpolated($"SELECT * FROM payment_transactions WHERE provider_order_code = {providerOrderCode} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var paymentTransaction = paymentTransaction_list.FirstOrDefault();

        return await ReloadAfterLockAsync(paymentTransaction, cancellationToken);
    }

    public async Task<PaymentTransaction?> LockPaymentTransactionByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var paymentTransaction_list = await context.PaymentTransactions
            .FromSqlInterpolated($"SELECT * FROM payment_transactions WHERE idempotency_key = {idempotencyKey} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var paymentTransaction = paymentTransaction_list.FirstOrDefault();

        return await ReloadAfterLockAsync(paymentTransaction, cancellationToken);
    }

    public async Task<WithdrawalRequest?> LockWithdrawalRequestByProviderOrderCodeAsync(
        string providerOrderCode,
        CancellationToken cancellationToken = default)
    {
        var withdrawalRequest_list = await context.WithdrawalRequests
            .FromSqlInterpolated($"SELECT * FROM withdrawal_requests WHERE provider_order_code = {providerOrderCode} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var withdrawalRequest = withdrawalRequest_list.FirstOrDefault();

        return await ReloadAfterLockAsync(withdrawalRequest, cancellationToken);
    }

    public async Task<WithdrawalRequest?> LockWithdrawalRequestByIdempotencyKeyAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        var withdrawalRequest_list = await context.WithdrawalRequests
            .FromSqlInterpolated($"SELECT * FROM withdrawal_requests WHERE idempotency_key = {idempotencyKey} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var withdrawalRequest = withdrawalRequest_list.FirstOrDefault();

        return await ReloadAfterLockAsync(withdrawalRequest, cancellationToken);
    }

    public async Task<WalletAccount?> LockWalletAccountAsync(
        Guid walletAccountId,
        CancellationToken cancellationToken = default)
    {
        var wallet_list = await context.WalletAccounts
            .FromSqlInterpolated($"SELECT * FROM wallet_accounts WHERE id = {walletAccountId} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var wallet = wallet_list.FirstOrDefault();

        return await ReloadAfterLockAsync(wallet, cancellationToken);
    }

    public async Task<Invoice?> LockInvoiceAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice_list = await context.Invoices
            .FromSqlInterpolated($"SELECT * FROM invoices WHERE id = {invoiceId} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var invoice = invoice_list.FirstOrDefault();

        return await ReloadAfterLockAsync(invoice, cancellationToken);
    }

    public async Task<User?> LockUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user_list = await context.Users
            .FromSqlInterpolated($"SELECT * FROM users WHERE id = {userId} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var user = user_list.FirstOrDefault();

        return await ReloadAfterLockAsync(user, cancellationToken);
    }

    public async Task<RoomDeposit?> LockRoomDepositAsync(
        Guid roomDepositId,
        CancellationToken cancellationToken = default)
    {
        var deposit_list = await context.RoomDeposits
            .FromSqlInterpolated($"SELECT * FROM room_deposits WHERE id = {roomDepositId} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var deposit = deposit_list.FirstOrDefault();

        return await ReloadAfterLockAsync(deposit, cancellationToken);
    }

    public async Task<RentalContract?> LockRentalContractAsync(
        Guid contractId,
        CancellationToken cancellationToken = default)
    {
        var contract_list = await context.RentalContracts
            .FromSqlInterpolated($"SELECT * FROM contracts WHERE id = {contractId} FOR UPDATE")
            .ToListAsync(cancellationToken);
        var contract = contract_list.FirstOrDefault();

        return await ReloadAfterLockAsync(contract, cancellationToken);
    }

    private async Task<TEntity?> ReloadAfterLockAsync<TEntity>(
        TEntity? entity,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (entity is not null)
        {
            await context.Entry(entity).ReloadAsync(cancellationToken);
        }

        return entity;
    }
}
