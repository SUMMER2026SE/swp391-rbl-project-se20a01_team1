# 🚀 Hướng Dẫn Bắt Đầu - Người 5 (Admin Approval & Public Listing)

## ✅ Những Gì Đã Hoàn Thành

### Backend Structure (100% ✅)
- ✅ 4 Enums (KYCStatus, RoomingHouseApprovalStatus, RoomingHouseVisibility, RoomStatus)
- ✅ 4 Entities (KYCVerification, RoomingHouse, Room, ApprovalAuditLog)
- ✅ 9 DTOs (các DTO cho admin & public)
- ✅ 4 Service Interfaces (KYC, RoomingHouse, PublicListing, Audit)
- ✅ 4 Service Implementations (đầy đủ business logic)
- ✅ 3 Controllers (AdminKYC, AdminRoomingHouse, PublicListing)
- ✅ 4 EF Core Configurations (mapping database)
- ✅ Authorization & Security (middleware, policies, extensions)
- ✅ Helper Classes (constants, exceptions, helpers)
- ✅ REST Client file (testing)

---

## 📋 Công Việc Cần Làm Tiếp

### **PHASE 1: Database Setup** (1-2 giờ)

```bash
# 1. Mở terminal
cd server/src/SmartRentalPlatform.Infrastructure

# 2. Tạo migration
dotnet ef migrations add AddAdminApprovalEntities --project ..

# 3. Update database
dotnet ef database update
```

**Kiểm tra:** Mở database tool (pgAdmin hoặc Azure Data Studio) xem 4 tables mới trong schema `admin_approval`:
- `admin_approval.kyc_verifications`
- `admin_approval.rooming_houses`
- `admin_approval.rooms`
- `admin_approval.approval_audit_logs`

---

### **PHASE 2: Program.cs Configuration** (30 phút)

**File:** `SmartRentalPlatform.Api/Program.cs`

Thêm vào:
```csharp
// Add services
builder.Services.AddApplication();
builder.Services.AddAdminApprovalServices(builder.Configuration);

// Add middleware (sau app.Build())
app.UseAdminApprovalMiddleware();
app.UseAuthentication();
app.UseAuthorization();
```

**appsettings.json:**
```json
{
  "AdminApprovalSecurity": {
    "SignedUrlExpirationMinutes": 15,
    "EnableCCIDMasking": true,
    "EnableAuditLogging": true,
    "S3BucketName": "your-bucket-name",
    "AzureBlobContainerName": "your-container-name"
  }
}
```

---

### **PHASE 3: Fix TODO's trong Code** (2-3 giờ)

#### TODO #1: Join User Table
**Files:** 
- `KYCApprovalService.cs` (line 30+)
- `RoomingHouseApprovalService.cs` (line 30+)
- `PublicListingService.cs` (line 100+)

**Task:** 
```csharp
// Thay thế:
UserEmail = "user@email.com", // TODO
UserDisplayName = "User Name", // TODO

// Bằng:
UserEmail = k.User.Email,
UserDisplayName = k.User.DisplayName,
```

**Cần:** Join `Users` table từ Người 1

---

#### TODO #2: Cấp Role Landlord
**File:** `RoomingHouseApprovalService.cs` (line 120+)

**Task:**
```csharp
// Uncomment và implement:
if (isFirstHouse)
{
    // Gọi service từ Người 2
    var roleService = serviceProvider.GetRequiredService<IUserRoleService>();
    await roleService.AssignLandlordRoleAsync(house.LandlordUserId, cancellationToken);
}
```

**Cần:** Interface `IUserRoleService` từ Người 2
**Endpoint:** `POST /api/users/{userId}/assign-landlord-role`

---

#### TODO #3: Signed URL Generator
**File:** Tạo mới `Application/AdminApproval/Services/ISignedUrlService.cs`

**Task:**
```csharp
public interface ISignedUrlService
{
    Task<string> GenerateSignedUrlAsync(string bucketKey, int expirationMinutes);
    Task<string> UploadImageAsync(Stream imageStream, string keyPrefix);
}
```

**Cần:** Integration S3 hoặc Azure Blob
**Dùng cho:** KYC approval (ảnh CCCD, ảnh mặt)

---

#### TODO #4: Lấy Images từ Rooming House/Room
**Files:**
- `PublicListingService.cs` (line 80+)
- DTOs

**Task:** Thêm field `ImageUrls` vào RoomingHouse & Room entity, sau đó:
```csharp
ImageUrls = h.Images.Select(i => i.Url).ToList(),
```

**Cần:** Tọa RoomingHouseImage, RoomImage entities (Người 4)

---

### **PHASE 4: Database Seeding (Demo Data)** (1 giờ)

**File:** Tạo mới `Infrastructure/Persistence/Seed/AdminApprovalSeed.cs`

**Task:** Tạo mock data:
```csharp
public static async Task SeedAdminApprovalDataAsync(AppDbContext context)
{
    // 1. Thêm KYCVerification test
    var kyc = new KYCVerification
    {
        Id = Guid.NewGuid(),
        UserId = /* user_id từ Người 1 */,
        FullName = "Nguyễn Văn A",
        IdNumber = "123456789012",
        Status = KYCStatus.PendingAdminReview,
        CreatedAt = DateTime.UtcNow
    };
    context.KYCVerifications.Add(kyc);
    
    // 2. Thêm RoomingHouse test
    var house = new RoomingHouse
    {
        Id = Guid.NewGuid(),
        LandlordUserId = /* landlord_id */,
        Name = "Khu trọ A",
        Address = "123 Đường ABC",
        ApprovalStatus = RoomingHouseApprovalStatus.PendingAdminReview,
        CreatedAt = DateTime.UtcNow
    };
    context.RoomingHouses.Add(house);
    
    // 3. Thêm Room
    var room = new Room
    {
        Id = Guid.NewGuid(),
        RoomingHouseId = house.Id,
        RoomNumber = "101",
        Price = 3000000,
        Capacity = 2,
        Status = RoomStatus.Available,
        CreatedAt = DateTime.UtcNow
    };
    context.Rooms.Add(room);
    
    await context.SaveChangesAsync();
}
```

---

### **PHASE 5: Testing** (1-2 giờ)

#### Test 5.1: API Testing
- Mở file `admin-approval.rest` (VS Code REST Client extension)
- Test từng endpoint:
  1. GET KYC List
  2. GET KYC Detail
  3. POST KYC Approve
  4. POST KYC Reject
  5. GET Rooming House List
  6. GET Rooming House Detail
  7. POST Rooming House Approve
  8. POST Rooming House Reject
  9. GET Public Listing (no auth)
  10. GET Public Rooming House Detail (no auth)

#### Test 5.2: Authorization
- Thử access `/api/admin/*` **mà không có admin token** → Expect 403
- Thử access `/api/public/*` **mà không login** → Expect 200 ✅

#### Test 5.3: Business Logic
- Approve KYC → Check trạng thái thay đổi ✅
- Reject KYC **mà không có lý do** → Expect 400 ✅
- Approve first RoomingHouse → Check role Landlord được cấp ✅

---

### **PHASE 6: Frontend Development** (4-6 giờ)

#### Trang 1: Admin KYC List
```
URL: /admin/kyc-approvals
Components:
- Table (Email, Tên, Trạng thái, Actions)
- Pagination
- Button "Chi tiết"
```

#### Trang 2: Admin KYC Detail
```
URL: /admin/kyc-approvals/:kycId
Components:
- Ảnh CCCD (Signed URL)
- Ảnh mặt (Signed URL)
- OCR Data (masked CCCD)
- Button "Duyệt" & "Từ chối"
- Input "Lý do từ chối"
```

#### Trang 3: Admin Rooming House List
```
URL: /admin/rooming-house-approvals
Components:
- Table (Tên, Địa chỉ, Chủ trọ, Số phòng, Actions)
- Pagination
- Button "Chi tiết"
```

#### Trang 4: Admin Rooming House Detail
```
URL: /admin/rooming-house-approvals/:roomingHouseId
Components:
- Thông tin khu trọ
- Danh sách phòng (table)
- Button "Duyệt" & "Từ chối"
- Input "Lý do từ chối"
```

#### Trang 5: Public Rooming House Listing
```
URL: /listings
Components:
- Search box (tên, địa chỉ)
- Price filter (min-max)
- Card grid (ảnh, tên, giá, số phòng trống)
- Click card → Chi tiết
- Danh sách phòng Available (chỉ Available!)
- Contact info chủ trọ
```

---

## 🔐 Security Checklist (Before Production)

- [ ] ✅ API `/api/admin/*` có `[Authorize(Roles = "Admin")]`
- [ ] ✅ API `/api/public/*` có `[AllowAnonymous]`
- [ ] ✅ CCCD Masking logic implemented
- [ ] ✅ Signed URL cho ảnh (15 min expiration)
- [ ] ✅ Reject reason bắt buộc
- [ ] ✅ Audit logging enable
- [ ] ✅ Transaction rollback khi cấp role fail
- [ ] ✅ Public listing chỉ show Approved+Visible+Available
- [ ] ✅ Không expose API keys frontend
- [ ] ✅ CORS configured correctly

---

## 📞 Cần Phối Hợp

### Với Người 1 (Authentication):
- User entity structure
- Join KYCVerification, RoomingHouse
- Sample admin user để test

### Với Người 2 (User Role):
- `IUserRoleService.AssignLandlordRoleAsync(userId)`
- Gọi khi approve RoomingHouse đầu tiên

### Với Người 3 (KYC):
- Xác nhận KYCVerification entity structure
- Lỗi OCR, Face Match logic

### Với Người 4 (Property):
- Xác nhận RoomingHouse, Room entity structure
- RoomingHouseImage, RoomImage entities

---

## 🎯 Timeline Ước Tính

| Phase | Công Việc | Thời Gian | Status |
|-------|-----------|----------|--------|
| 1 | Database Migration | 1-2h | Ready |
| 2 | Program.cs Setup | 30m | Ready |
| 3 | Fix TODOs | 2-3h | **Need Collaboration** |
| 4 | Seeding | 1h | Optional |
| 5 | Testing | 1-2h | Ready |
| 6 | Frontend | 4-6h | **You build** |
| **TOTAL** | | **9-15h** | |

---

## 📝 Important Notes

### ⚠️ Don't Forget:
1. **Database Backup** trước khi run migration
2. **Test CCID Masking** - không leak số CCCD
3. **Test Signed URL** - phải expire sau 15 phút
4. **Test Transaction** - rollback khi cấp role fail
5. **Test Public Filter** - chỉ Approved+Visible+Available

### 📚 Documentation Files:
- `server/PERSON_5_GUIDE.md` - Chi tiết nghiệp vụ
- `server/FILES_CREATED_SUMMARY.md` - Danh sách files
- `server/src/SmartRentalPlatform.Api/admin-approval.rest` - API test

### 🔗 Đường Dẫn Quan Trọng:
- Controllers: `src/SmartRentalPlatform.Api/Controllers/`
- Services: `src/SmartRentalPlatform.Application/AdminApproval/Services/`
- DTOs: `src/SmartRentalPlatform.Application/AdminApproval/DTOs/`
- Entities: `src/SmartRentalPlatform.Domain/Entities/AdminApproval/`

---

## ✨ Status: **READY FOR EXECUTION** 🚀

Tất cả backend structure đã sẵn sàng!

**Next Step:** Chạy migration → Fix TODOs → Test API → Build Frontend

**Questions?** Xem `PERSON_5_GUIDE.md` hoặc hỏi team!

---

**Created by: GitHub Copilot | May 21, 2026**
