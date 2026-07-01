using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRentalPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixWithdrawalColumnNamesToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_withdrawal_requests_wallet_accounts_WalletAccountId",
                table: "withdrawal_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_withdrawal_webhook_logs_withdrawal_requests_WithdrawalReque~",
                table: "withdrawal_webhook_logs");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "withdrawal_webhook_logs",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "withdrawal_webhook_logs",
                newName: "payload");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "withdrawal_webhook_logs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WithdrawalRequestId",
                table: "withdrawal_webhook_logs",
                newName: "withdrawal_request_id");

            migrationBuilder.RenameColumn(
                name: "ReceivedAt",
                table: "withdrawal_webhook_logs",
                newName: "received_at");

            migrationBuilder.RenameColumn(
                name: "ProviderOrderCode",
                table: "withdrawal_webhook_logs",
                newName: "provider_order_code");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_webhook_logs_WithdrawalRequestId_Status",
                table: "withdrawal_webhook_logs",
                newName: "IX_withdrawal_webhook_logs_withdrawal_request_id_status");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "withdrawal_requests",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Fee",
                table: "withdrawal_requests",
                newName: "fee");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "withdrawal_requests",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "withdrawal_requests",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "withdrawal_requests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WalletAccountId",
                table: "withdrawal_requests",
                newName: "wallet_account_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "withdrawal_requests",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProviderTransactionCode",
                table: "withdrawal_requests",
                newName: "provider_transaction_code");

            migrationBuilder.RenameColumn(
                name: "ProviderOrderCode",
                table: "withdrawal_requests",
                newName: "provider_order_code");

            migrationBuilder.RenameColumn(
                name: "IdempotencyKey",
                table: "withdrawal_requests",
                newName: "idempotency_key");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "withdrawal_requests",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BankBin",
                table: "withdrawal_requests",
                newName: "bank_bin");

            migrationBuilder.RenameColumn(
                name: "AccountNumber",
                table: "withdrawal_requests",
                newName: "account_number");

            migrationBuilder.RenameColumn(
                name: "AccountName",
                table: "withdrawal_requests",
                newName: "account_name");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_WalletAccountId",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_wallet_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_ProviderOrderCode",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_provider_order_code");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_IdempotencyKey",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_idempotency_key");

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawal_requests_wallet_accounts_wallet_account_id",
                table: "withdrawal_requests",
                column: "wallet_account_id",
                principalTable: "wallet_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawal_webhook_logs_withdrawal_requests_withdrawal_requ~",
                table: "withdrawal_webhook_logs",
                column: "withdrawal_request_id",
                principalTable: "withdrawal_requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_withdrawal_requests_wallet_accounts_wallet_account_id",
                table: "withdrawal_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_withdrawal_webhook_logs_withdrawal_requests_withdrawal_requ~",
                table: "withdrawal_webhook_logs");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "withdrawal_webhook_logs",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "payload",
                table: "withdrawal_webhook_logs",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "withdrawal_webhook_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "withdrawal_request_id",
                table: "withdrawal_webhook_logs",
                newName: "WithdrawalRequestId");

            migrationBuilder.RenameColumn(
                name: "received_at",
                table: "withdrawal_webhook_logs",
                newName: "ReceivedAt");

            migrationBuilder.RenameColumn(
                name: "provider_order_code",
                table: "withdrawal_webhook_logs",
                newName: "ProviderOrderCode");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_webhook_logs_withdrawal_request_id_status",
                table: "withdrawal_webhook_logs",
                newName: "IX_withdrawal_webhook_logs_WithdrawalRequestId_Status");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "withdrawal_requests",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "fee",
                table: "withdrawal_requests",
                newName: "Fee");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "withdrawal_requests",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "withdrawal_requests",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "withdrawal_requests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "wallet_account_id",
                table: "withdrawal_requests",
                newName: "WalletAccountId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "withdrawal_requests",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "provider_transaction_code",
                table: "withdrawal_requests",
                newName: "ProviderTransactionCode");

            migrationBuilder.RenameColumn(
                name: "provider_order_code",
                table: "withdrawal_requests",
                newName: "ProviderOrderCode");

            migrationBuilder.RenameColumn(
                name: "idempotency_key",
                table: "withdrawal_requests",
                newName: "IdempotencyKey");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "withdrawal_requests",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "bank_bin",
                table: "withdrawal_requests",
                newName: "BankBin");

            migrationBuilder.RenameColumn(
                name: "account_number",
                table: "withdrawal_requests",
                newName: "AccountNumber");

            migrationBuilder.RenameColumn(
                name: "account_name",
                table: "withdrawal_requests",
                newName: "AccountName");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_wallet_account_id",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_WalletAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_provider_order_code",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_ProviderOrderCode");

            migrationBuilder.RenameIndex(
                name: "IX_withdrawal_requests_idempotency_key",
                table: "withdrawal_requests",
                newName: "IX_withdrawal_requests_IdempotencyKey");

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawal_requests_wallet_accounts_WalletAccountId",
                table: "withdrawal_requests",
                column: "WalletAccountId",
                principalTable: "wallet_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_withdrawal_webhook_logs_withdrawal_requests_WithdrawalReque~",
                table: "withdrawal_webhook_logs",
                column: "WithdrawalRequestId",
                principalTable: "withdrawal_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
