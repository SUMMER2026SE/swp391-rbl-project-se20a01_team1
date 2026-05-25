using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartRentalPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminApprovalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "admin_approval");

            migrationBuilder.CreateTable(
                name: "approval_audit_logs",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    AdditionalInfo = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "kyc_verifications",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DateOfBirth = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IdNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FaceMatchScore = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LivenessScore = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    RejectedReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReviewedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kyc_verifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rooming_houses",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LandlordUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ApprovalStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    RejectedReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReviewedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Visibility = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooming_houses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    AvatarSource = table.Column<string>(type: "text", nullable: true),
                    AvatarObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OnboardingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmailComfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    LockoutEndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomingHouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Area = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rooms_rooming_houses_RoomingHouseId",
                        column: x => x.RoomingHouseId,
                        principalSchema: "admin_approval",
                        principalTable: "rooming_houses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    AddressLine = table.Column<string>(type: "text", nullable: true),
                    Ward = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_user_profiles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_price_tiers",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    OccupantCount = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_price_tiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_room_price_tiers_rooms_RoomId",
                        column: x => x.RoomId,
                        principalSchema: "admin_approval",
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "kyc_verifications",
                columns: new[] { "Id", "Address", "CreatedAt", "DateOfBirth", "FaceMatchScore", "FullName", "IdNumber", "LivenessScore", "RejectedReason", "ReviewedAt", "ReviewedByAdminId", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001"), "456 Lê Lợi, Quận 1, TP.HCM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1998-05-15", null, "Nguyễn Văn Tenant KYC", "079098001234", null, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0003") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002"), "789 Nguyễn Huệ, Quận 1, TP.HCM", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1985-11-20", null, "Trần Thị Landlord KYC", "079085009876", null, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005") }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Quản trị hệ thống / System administrator", "Admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Người thuê (mặc định) / Default tenant role", "Tenant" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chủ trọ / Property owner", "Landlord" }
                });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "rooming_houses",
                columns: new[] { "Id", "Address", "ApprovalStatus", "CreatedAt", "DeletedAt", "Description", "LandlordUserId", "Name", "RejectedReason", "ReviewedAt", "ReviewedByAdminId", "UpdatedAt", "Visibility" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001"), "123 Đường An Bình, Quận 7, TP.HCM", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Khu trọ yên tĩnh, tiện nghi cơ bản — Peaceful boarding area (Interval 1 seed)", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0004"), "Khu trọ An Bình", null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "AccessFailedCount", "AvatarObjectKey", "AvatarSource", "AvatarUrl", "CreatedAt", "DeletedAt", "DisplayName", "Email", "EmailComfirmed", "LastLoginAt", "LockoutEndAt", "NormalizedEmail", "OnboardingStatus", "PasswordHash", "PhoneConfirmed", "PhoneNumber", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0001"), 0, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", "admin@gmail.com", true, null, null, "ADMIN@GMAIL.COM", "Completed", "AQAAAAEAACcQAAAAEDgWBHiZ1iPUyxiEv7MBd9JJW2brX/EpWwEcRPTwOBNPrPLjzXBpYfgGGA+xSqHPvQ==", false, null, "Active", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0002"), 0, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tenant Done", "tenant.done@gmail.com", true, null, null, "TENANT.DONE@GMAIL.COM", "Completed", "AQAAAAEAACcQAAAAEDgWBHiZ1iPUyxiEv7MBd9JJW2brX/EpWwEcRPTwOBNPrPLjzXBpYfgGGA+xSqHPvQ==", false, null, "Active", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0003"), 0, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tenant KYC", "tenant.kyc@gmail.com", true, null, null, "TENANT.KYC@GMAIL.COM", "NeedKyc", "AQAAAAEAACcQAAAAEDgWBHiZ1iPUyxiEv7MBd9JJW2brX/EpWwEcRPTwOBNPrPLjzXBpYfgGGA+xSqHPvQ==", false, null, "Active", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0004"), 0, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Landlord Done", "landlord.done@gmail.com", true, null, null, "LANDLORD.DONE@GMAIL.COM", "Completed", "AQAAAAEAACcQAAAAEDgWBHiZ1iPUyxiEv7MBd9JJW2brX/EpWwEcRPTwOBNPrPLjzXBpYfgGGA+xSqHPvQ==", false, null, "Active", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005"), 0, null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Landlord KYC", "landlord.kyc@gmail.com", true, null, null, "LANDLORD.KYC@GMAIL.COM", "NeedKyc", "AQAAAAEAACcQAAAAEDgWBHiZ1iPUyxiEv7MBd9JJW2brX/EpWwEcRPTwOBNPrPLjzXBpYfgGGA+xSqHPvQ==", false, null, "Active", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "rooms",
                columns: new[] { "Id", "Area", "Capacity", "CreatedAt", "DeletedAt", "Price", "RoomNumber", "RoomingHouseId", "UpdatedAt" },
                values: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccc0001"), 25m, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2000000m, "A101", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "RoleId", "UserId", "CreatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "room_price_tiers",
                columns: new[] { "Id", "MonthlyPrice", "OccupantCount", "RoomId" },
                values: new object[,]
                {
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddd0001"), 2000000m, 1, new Guid("cccccccc-cccc-cccc-cccc-cccccccc0001") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddd0002"), 2900000m, 2, new Guid("cccccccc-cccc-cccc-cccc-cccccccc0001") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddd0003"), 3600000m, 3, new Guid("cccccccc-cccc-cccc-cccc-cccccccc0001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_approval_audit_logs_AdminId_CreatedAt",
                schema: "admin_approval",
                table: "approval_audit_logs",
                columns: new[] { "AdminId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_approval_audit_logs_ApprovalType_EntityId",
                schema: "admin_approval",
                table: "approval_audit_logs",
                columns: new[] { "ApprovalType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_roles_Name",
                table: "roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_price_tiers_RoomId_OccupantCount",
                schema: "admin_approval",
                table: "room_price_tiers",
                columns: new[] { "RoomId", "OccupantCount" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rooming_houses_ApprovalStatus",
                schema: "admin_approval",
                table: "rooming_houses",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_rooming_houses_LandlordUserId",
                schema: "admin_approval",
                table: "rooming_houses",
                column: "LandlordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_rooming_houses_Visibility",
                schema: "admin_approval",
                table: "rooming_houses",
                column: "Visibility");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_RoomingHouseId",
                schema: "admin_approval",
                table: "rooms",
                column: "RoomingHouseId");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_Status",
                schema: "admin_approval",
                table: "rooms",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_NormalizedEmail",
                table: "users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_PhoneNumber",
                table: "users",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_audit_logs",
                schema: "admin_approval");

            migrationBuilder.DropTable(
                name: "kyc_verifications",
                schema: "admin_approval");

            migrationBuilder.DropTable(
                name: "room_price_tiers",
                schema: "admin_approval");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "rooms",
                schema: "admin_approval");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "rooming_houses",
                schema: "admin_approval");
        }
    }
}
