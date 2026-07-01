using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRentalPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletWithdrawals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "withdrawal_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderOrderCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BankBin = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AccountName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawal_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_withdrawal_requests_wallet_accounts_WalletAccountId",
                        column: x => x.WalletAccountId,
                        principalTable: "wallet_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "withdrawal_webhook_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WithdrawalRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderOrderCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_withdrawal_webhook_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_withdrawal_webhook_logs_withdrawal_requests_WithdrawalReque~",
                        column: x => x.WithdrawalRequestId,
                        principalTable: "withdrawal_requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_IdempotencyKey",
                table: "withdrawal_requests",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_ProviderOrderCode",
                table: "withdrawal_requests",
                column: "ProviderOrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_requests_WalletAccountId",
                table: "withdrawal_requests",
                column: "WalletAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_withdrawal_webhook_logs_WithdrawalRequestId_Status",
                table: "withdrawal_webhook_logs",
                columns: new[] { "WithdrawalRequestId", "Status" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "withdrawal_webhook_logs");

            migrationBuilder.DropTable(
                name: "withdrawal_requests");
        }
    }
}
