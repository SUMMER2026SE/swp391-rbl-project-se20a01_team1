using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartRentalPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdminApprovalEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005") });

            migrationBuilder.AddColumn<string>(
                name: "Amenities",
                schema: "admin_approval",
                table: "rooming_houses",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "admin_approval",
                table: "kyc_verifications",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "FaceImageObjectKey",
                schema: "admin_approval",
                table: "kyc_verifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdImageObjectKey",
                schema: "admin_approval",
                table: "kyc_verifications",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "room_images",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_room_images_rooms_RoomId",
                        column: x => x.RoomId,
                        principalSchema: "admin_approval",
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooming_house_images",
                schema: "admin_approval",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomingHouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsCover = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooming_house_images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rooming_house_images_rooming_houses_RoomingHouseId",
                        column: x => x.RoomingHouseId,
                        principalSchema: "admin_approval",
                        principalTable: "rooming_houses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "admin_approval",
                table: "kyc_verifications",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001"),
                columns: new[] { "FaceImageObjectKey", "IdImageObjectKey", "Status" },
                values: new object[] { "kyc/tenant-kyc/face.jpg", "kyc/tenant-kyc/id-front.jpg", 0 });

            migrationBuilder.UpdateData(
                schema: "admin_approval",
                table: "kyc_verifications",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002"),
                columns: new[] { "FaceImageObjectKey", "IdImageObjectKey", "Status" },
                values: new object[] { "kyc/landlord-kyc/face.jpg", "kyc/landlord-kyc/id-front.jpg", 0 });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "room_images",
                columns: new[] { "Id", "ObjectKey", "RoomId", "SortOrder" },
                values: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffff0002"), "rooms/a101/gallery-1.jpg", new Guid("cccccccc-cccc-cccc-cccc-cccccccc0001"), 0 });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "rooming_house_images",
                columns: new[] { "Id", "IsCover", "ObjectKey", "RoomingHouseId", "SortOrder" },
                values: new object[] { new Guid("ffffffff-ffff-ffff-ffff-ffffffff0001"), true, "rooming-houses/an-binh/cover.jpg", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001"), 0 });

            migrationBuilder.UpdateData(
                schema: "admin_approval",
                table: "rooming_houses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001"),
                column: "Amenities",
                value: "Wifi,Giữ xe,Máy lạnh,Nước nóng");

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "rooming_houses",
                columns: new[] { "Id", "Address", "Amenities", "ApprovalStatus", "Visibility", "CreatedAt", "DeletedAt", "Description", "LandlordUserId", "Name", "RejectedReason", "ReviewedAt", "ReviewedByAdminId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002"), "88 Đường Test, Quận 10, TP.HCM", "Wifi,Thang máy", 0, 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Seed pending — duyệt để test cấp role Landlord", new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005"), "Khu trọ Chờ Duyệt (Landlord KYC)", null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0003"), "99 Hidden St", null, 0, 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0004"), "Nhà Pending (không hiện public)", null, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                schema: "admin_approval",
                table: "rooms",
                columns: new[] { "Id", "Area", "Capacity", "CreatedAt", "DeletedAt", "Price", "RoomNumber", "RoomingHouseId", "Status", "UpdatedAt" },
                values: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccc0002"), 28m, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2500000m, "A102", new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0001"), 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_room_images_RoomId",
                schema: "admin_approval",
                table: "room_images",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_rooming_house_images_RoomingHouseId",
                schema: "admin_approval",
                table: "rooming_house_images",
                column: "RoomingHouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_images",
                schema: "admin_approval");

            migrationBuilder.DropTable(
                name: "rooming_house_images",
                schema: "admin_approval");

            migrationBuilder.DeleteData(
                schema: "admin_approval",
                table: "rooming_houses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0002"));

            migrationBuilder.DeleteData(
                schema: "admin_approval",
                table: "rooming_houses",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbb0003"));

            migrationBuilder.DeleteData(
                schema: "admin_approval",
                table: "rooms",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccc0002"));

            migrationBuilder.DropColumn(
                name: "Amenities",
                schema: "admin_approval",
                table: "rooming_houses");

            migrationBuilder.DropColumn(
                name: "FaceImageObjectKey",
                schema: "admin_approval",
                table: "kyc_verifications");

            migrationBuilder.DropColumn(
                name: "IdImageObjectKey",
                schema: "admin_approval",
                table: "kyc_verifications");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "admin_approval",
                table: "kyc_verifications",
                type: "integer",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "admin_approval",
                table: "kyc_verifications",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeee0001"),
                column: "Status",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "admin_approval",
                table: "kyc_verifications",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeee0002"),
                column: "Status",
                value: 3);

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "RoleId", "UserId", "CreatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
