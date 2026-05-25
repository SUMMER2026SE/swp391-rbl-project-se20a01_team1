# 📋 Hướng Dẫn Triển Khai Người 5 - Admin Approval & Public Listing

## 🎯 Tổng Quan

Người 5 phụ trách **2 luồng chính**:
1. **Admin Approval Flow**: Duyệt KYC & Khu trọ
2. **Public Listing Flow**: Hiển thị công khai cho khách hàng

---

## 📊 Cấu Trúc Project Đã Được Tạo

### Backend Layers:
```
SmartRentalPlatform.Domain/
├── Entities/AdminApproval/
│   ├── KYCVerification.cs         ✅ Entity xác thực danh tính
│   ├── RoomingHouse.cs            ✅ Entity khu trọ
│   ├── Room.cs                    ✅ Entity phòng
│   └── ApprovalAuditLog.cs        ✅ Entity lưu audit
├── Enums/
│   ├── KYCStatus.cs               ✅ Trạng thái KYC
│   ├── RoomingHouseApprovalStatus.cs ✅ Trạng thái phê duyệt
│   ├── RoomingHouseVisibility.cs  ✅ Trạng thái hiển thị
│   └── RoomStatus.cs              ✅ Trạng thái phòng

SmartRentalPlatform.Application/AdminApproval/
├── DTOs/
│   ├── KYCDetailDto.cs            ✅ Chi tiết KYC
│   ├── KYCListDto.cs              ✅ Danh sách KYC
│   ├── KYCApprovalRequestDto.cs   ✅ Request duyệt KYC
│   ├── RoomingHouseApprovalDetailDto.cs  ✅ Chi tiết khu trọ
│   ├── RoomingHouseApprovalListDto.cs    ✅ Danh sách khu trọ
│   ├── RoomingHouseApprovalRequestDto.cs ✅ Request duyệt khu trọ
│   ├── PublicRoomingHouseDto.cs   ✅ DTO public listing
│   └── PublicRoomingHouseDetailDto.cs    ✅ DTO chi tiết public
├── Services/
│   ├── IKYCApprovalService.cs     ✅ Interface service KYC
│   ├── IRoomingHouseApprovalService.cs  ✅ Interface service khu trọ
│   ├── IPublicListingService.cs   ✅ Interface service public
│   └── IApprovalAuditService.cs   ✅ Interface service audit

SmartRentalPlatform.Api/Controllers/
├── AdminKYCController.cs          ✅ API duyệt KYC
├── AdminRoomingHouseController.cs ✅ API duyệt khu trọ
└── PublicListingController.cs     ✅ API public listing
```

---

## 🛠️ Các Công Việc Cần Hoàn Thành

### **1️⃣ Implement Services (Backend Logic)**

#### ✅ Công việc 1.1: Implement `KYCApprovalService`
**File:** `SmartRentalPlatform.Application/AdminApproval/Services/KYCApprovalService.cs`

**Chức năng:**
- `GetPendingKYCsAsync()`: Query danh sách KYC có `Status = PendingAdminReview`
- `GetKYCDetailAsync()`: Lấy chi tiết KYC, mask CCCD và trả về Signed URL cho ảnh
- `ApproveKYCAsync()`: Cập nhật `Status = Approved`, set `ReviewedByAdminId`, `ReviewedAt`
- `RejectKYCAsync()`: Cập nhật `Status = Rejected`, set `RejectedReason`, `ReviewedByAdminId`

**Lưu ý bảo mật:**
- Không trả về CCCD dạng plaintext, phải mask (VD: `123456****`)
- Ảnh giấy tờ phải là Signed URL có thời hạn (15 phút)
- Chỉ Admin mới xem được dữ liệu nhạy cảm này

---

#### ✅ Công việc 1.2: Implement `RoomingHouseApprovalService`
**File:** `SmartRentalPlatform.Application/AdminApproval/Services/RoomingHouseApprovalService.cs`

**Chức năng:**
- `GetPendingRoomingHousesAsync()`: Query khu trọ có `ApprovalStatus = PendingAdminReview`
- `GetRoomingHouseDetailAsync()`: Lấy chi tiết khu trọ + danh sách phòng
- `ApproveRoomingHouseAsync()`: 
  - ✅ Cập nhật `ApprovalStatus = Approved`
  - ✅ **QUAN TRỌNG**: Kiểm tra nếu đây là khu trọ **đầu tiên** của chủ trọ
  - ✅ Nếu là đầu tiên → **Gọi Người 2** để cấp role `Landlord` (sử dụng Transaction)
  - ✅ Set `ReviewedByAdminId`, `ReviewedAt`, lưu `Visibility = Hidden` (chủ trọ sẽ bật sau)
- `RejectRoomingHouseAsync()`: Cập nhật `ApprovalStatus = Rejected`, set `RejectedReason`

**Lưu ý quan trọng:**
```csharp
// Khi approve khu trọ đầu tiên:
using var transaction = await _dbContext.Database.BeginTransactionAsync();
try
{
    // 1. Cập nhật RoomingHouse
    roomingHouse.ApprovalStatus = RoomingHouseApprovalStatus.Approved;
    
    // 2. Gọi service Người 2 để cấp role Landlord
    await _roleManagementService.AssignLandlordRoleAsync(roomingHouse.LandlordUserId);
    
    // 3. Save & commit
    await _dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---

#### ✅ Công việc 1.3: Implement `PublicListingService`
**File:** `SmartRentalPlatform.Application/AdminApproval/Services/PublicListingService.cs`

**Chức năng:**
- `GetPublicRoomingHousesAsync()`: 
  - ✅ Lọc **CHẶT**: `ApprovalStatus = Approved` **AND** `Visibility = Visible` **AND** có phòng `Status = Available`
  - ✅ Hỗ trợ search, filter giá, pagination
  - ✅ Không hiển thị ảnh nhạy cảm, chỉ public images
  
- `GetPublicRoomingHouseDetailAsync()`: 
  - ✅ Verify khu trọ thỏa 3 điều kiện (Approved, Visible, có phòng Available)
  - ✅ Chỉ trả về phòng `Status = Available` (bỏ Occupied, Maintenance, Hidden)
  - ✅ Không hiển thị giá phòng của chủ nhà riêng tư, chỉ public info

**Lưu ý:**
```csharp
var publicHouses = await _dbContext.RoomingHouses
    .Where(h => h.ApprovalStatus == RoomingHouseApprovalStatus.Approved
             && h.Visibility == RoomingHouseVisibility.Visible
             && h.Rooms.Any(r => r.Status == RoomStatus.Available && r.DeletedAt == null))
    .ToListAsync();
```

---

#### ✅ Công việc 1.4: Implement `ApprovalAuditService`
**File:** `SmartRentalPlatform.Application/AdminApproval/Services/ApprovalAuditService.cs`

**Chức năng:**
- `LogApprovalAsync()`: Ghi log mỗi khi Admin duyệt/từ chối
  - Lưu `AdminId`, `ApprovalType` (KYC/RoomingHouse), `EntityId`, `Action` (Approved/Rejected), `Reason`
  - Insert vào `ApprovalAuditLog` table

**Ví dụ:**
```csharp
var auditLog = new ApprovalAuditLog
{
    Id = Guid.NewGuid(),
    AdminId = adminId,
    ApprovalType = "KYC",
    EntityId = kycId,
    Action = "Approved",
    Reason = null,
    CreatedAt = DateTime.UtcNow
};
await _dbContext.ApprovalAuditLogs.AddAsync(auditLog);
```

---

### **2️⃣ Database Migrations**

#### ✅ Công việc 2.1: Tạo Migration cho Entities

```bash
cd server/src/SmartRentalPlatform.Infrastructure
dotnet ef migrations add AddAdminApprovalEntities
dotnet ef database update
```

**Cấu hình EF cho entities:**
- Tạo file `KYCVerificationConfiguration.cs` trong `Persistence/Configurations/`
- Tạo file `RoomingHouseConfiguration.cs`
- Tạo file `RoomConfiguration.cs`
- Tạo file `ApprovalAuditLogConfiguration.cs`

---

### **3️⃣ Update AppDbContext**

**File:** `SmartRentalPlatform.Infrastructure/Persistence/AppDbContext.cs`

Thêm DbSets:
```csharp
public DbSet<KYCVerification> KYCVerifications => Set<KYCVerification>();
public DbSet<RoomingHouse> RoomingHouses => Set<RoomingHouse>();
public DbSet<Room> Rooms => Set<Room>();
public DbSet<ApprovalAuditLog> ApprovalAuditLogs => Set<ApprovalAuditLog>();
```

---

### **4️⃣ Dependency Injection Setup**

**File:** `SmartRentalPlatform.Application/ApplicationServiceRegistration.cs`

Đăng ký các services:
```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    services.AddScoped<IKYCApprovalService, KYCApprovalService>();
    services.AddScoped<IRoomingHouseApprovalService, RoomingHouseApprovalService>();
    services.AddScoped<IPublicListingService, PublicListingService>();
    services.AddScoped<IApprovalAuditService, ApprovalAuditService>();
    
    return services;
}
```

---

### **5️⃣ Authorization Attribute (xác thực quyền Admin)**

**Tạo file:** `SmartRentalPlatform.Api/Attributes/AuthorizeAdminAttribute.cs`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAdminAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.IsInRole("Admin"))
        {
            context.Result = new ForbidResult(); // 403 Forbidden
        }
    }
}
```

---

### **6️⃣ Frontend - Tạo 5 Trang Chính**

#### Trang 1: Admin KYC List
**Endpoint:** `GET /api/admin/kyc/pending`
**Tính năng:**
- ✅ Hiển thị danh sách KYC (Email, Tên, Trạng thái)
- ✅ Pagination (10 items/trang)
- ✅ Button "Chi tiết" để xem ảnh & dữ liệu OCR

---

#### Trang 2: Admin KYC Detail
**Endpoint:** `GET /api/admin/kyc/{kycId}`
**Tính năng:**
- ✅ Hiển thị ảnh giấy tờ, ảnh mặt (Signed URL)
- ✅ Dữ liệu OCR (Họ tên, Ngày sinh, CCCD masked, Địa chỉ)
- ✅ Kết quả xác thực (Face Match Score, Liveness Score)
- ✅ Button "Duyệt" và "Từ chối"
- ✅ Input "Lý do từ chối" (bắt buộc khi từ chối)

---

#### Trang 3: Admin Rooming House List
**Endpoint:** `GET /api/admin/rooming-houses/pending`
**Tính năng:**
- ✅ Hiển thị danh sách khu trọ (Tên, Địa chỉ, Chủ trọ, Số phòng)
- ✅ Pagination
- ✅ Button "Chi tiết" để xem thông tin chi tiết

---

#### Trang 4: Admin Rooming House Detail
**Endpoint:** `GET /api/admin/rooming-houses/{roomingHouseId}`
**Tính năng:**
- ✅ Hiển thị thông tin khu trọ (Tên, Địa chỉ, Mô tả)
- ✅ Danh sách phòng (Phòng số, Giá, Sức chứa, Trạng thái)
- ✅ Button "Duyệt" và "Từ chối"
- ✅ Input "Lý do từ chối" (bắt buộc)

---

#### Trang 5: Public Rooming House Listing
**Endpoints:** 
- `GET /api/public/rooming-houses` (danh sách)
- `GET /api/public/rooming-houses/{roomingHouseId}` (chi tiết)

**Tính năng:**
- ✅ Hiển thị danh sách khu trọ công khai (không cần đăng nhập)
- ✅ Search theo tên, địa chỉ
- ✅ Filter theo giá (min-max)
- ✅ Hiển thị số phòng trống & giá min-max
- ✅ Click vào khu trọ → Chi tiết (chỉ phòng Available)
- ✅ Hiển thị thông tin chủ trọ (Tên, Số điện thoại)

---

## 🔐 Checklist Bảo Mật & Kiểm tra

### ✅ Security Checklist:
- [ ] Tất cả API `/api/admin/*` có kiểm tra role Admin (403 nếu không)
- [ ] Ảnh giấy tờ KYC dùng Signed URL (hết hạn sau 15 phút)
- [ ] CCCD display dạng masked: `123456****`
- [ ] API công khai không yêu cầu đăng nhập
- [ ] Mỗi hành động duyệt/từ chối được ghi log (Admin ID, thời gian)
- [ ] Lý do từ chối là **bắt buộc** khi reject
- [ ] Public listing **chỉ** hiển thị Approved + Visible + có phòng Available
- [ ] Không trả về dữ liệu "rác" hoặc hồ sơ chưa duyệt

### ✅ Business Logic Checklist:
- [ ] Khi approve khu trọ **đầu tiên** → cấp role Landlord (transaction)
- [ ] Khu trọ sau approval vẫn ở trạng thái Hidden (chủ trọ bật sau)
- [ ] Public listing filter đúng 3 điều kiện
- [ ] Chi tiết public chỉ show phòng Available

---

## 📞 Tương Tác Với Các Người Khác

### 🔗 Cần từ Người 2 (Role Management):
- Interface/Service để cấp role Landlord: `IUserRoleService.AssignLandlordRoleAsync(userId)`

### 🔗 Cần từ Người 3 (KYC):
- Entity `KYCVerification` với các trường OCR, Face Match, Liveness
- Hoặc Người 5 sẽ tạo interface để update trạng thái từ Người 3

### 🔗 Cần từ Người 4 (Property):
- Entity `RoomingHouse` và `Room` với các trường cần thiết
- Hoặc Người 5 sẽ tạo interface để update trạng thái

---

## 🚀 Quy Trình Hoàn Thành

1. ✅ **Implement 4 Services** (KYC, RoomingHouse, PublicListing, Audit)
2. ✅ **Tạo Database Migrations** (EF Core)
3. ✅ **Update AppDbContext** + Configurations
4. ✅ **Đăng ký DI** trong ApplicationServiceRegistration
5. ✅ **Test API endpoints** (Postman/REST Client)
6. ✅ **Tạo 5 trang Frontend** (React/Vue/Angular)
7. ✅ **Unit & Integration Tests**
8. ✅ **Audit & Security Review**

---

## 📝 Ghi Chú

- Các Entity KYC, RoomingHouse, Room được tạo đơn giản. Nếu Người 3 & 4 có định nghĩa khác, hãy sync lại
- Signed URL cho ảnh cần tích hợp S3/Blob Storage
- Gọi Người 2 để cấp role cần phải là async và đúng transaction
- Mỗi rejection phải có lý do rõ ràng để người dùng biết tại sao bị từ chối

---

**Good luck! 🎉**
