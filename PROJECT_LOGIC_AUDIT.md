# Bao cao logic da dat duoc - Smart Rental Platform

Ngay ra soat: 2026-05-30  
Branch local: `interval-1-to-develop`

## 1. Tong quan ket qua

Du an da dat duoc nen tang logic chinh cho Interval 1/Integration 2 theo huong full-stack:

- Backend da tach cac layer chinh: `Api`, `Application`, `Contracts`, `Domain`, `Infrastructure`.
- Frontend da co routing, guard dang nhap, guard role, guard onboarding va cac man hinh chinh cho user/landlord/admin.
- Flow nghiep vu da co: dang ky, dang nhap, OTP email, quen/reset mat khau, Google login, refresh/logout token, profile, KYC, admin approve, tao nha tro, tao phong, upload file.
- Cac file local private nhu `.env`, `appsettings.Development.json`, `.agent`, `bin/obj`, uploads/private storage da duoc chan boi `.gitignore`.

Ket luan nhanh: code hien tai da co logic san pham kha day du cho vong Interval 1. Viec con lai nen tap trung vao test, QA flow thuc te, cat nho mot so file frontend lon, va chuan hoa secrets cho moi truong deploy.

## 2. Backend - logic da dat duoc

### 2.1 Auth va session

Trang thai: Da dat, can test tich hop end-to-end.

Logic hien co:

- Dang ky tai khoan: `POST /api/auth/register`.
- Xac thuc email bang OTP: `POST /api/auth/verify-email-otp`.
- Gui lai OTP email: `POST /api/auth/resend-email-otp`.
- Dang nhap bang email/password: `POST /api/auth/login`.
- Dang nhap bang Google: `POST /api/auth/google-login`.
- Refresh access token: `POST /api/auth/refresh-token`.
- Logout mot session: `POST /api/auth/logout`.
- Logout tat ca session: `POST /api/auth/logout-all`.
- Quen mat khau, verify reset OTP, reset mat khau.
- Doi mat khau cho user da dang nhap.
- Luu/revoke token, hash token, ghi login log.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Auth/AuthController.cs`
- `server/src/SmartRentalPlatform.Application/Auth/AuthService.cs`
- `server/src/SmartRentalPlatform.Application/Auth/AuthSessionService.cs`
- `server/src/SmartRentalPlatform.Application/Auth/AuthPasswordService.cs`
- `server/src/SmartRentalPlatform.Application/Auth/GoogleLoginService.cs`
- `server/src/SmartRentalPlatform.Infrastructure/Security/TokenService.cs`
- `server/src/SmartRentalPlatform.Infrastructure/Security/PasswordService.cs`

### 2.2 User profile va session management

Trang thai: Da dat, can test UI/profile upload neu co avatar.

Logic hien co:

- Lay thong tin user hien tai: `GET /api/users/me`.
- Lay/cap nhat profile: `GET|PUT /api/users/me/profile`.
- Kiem tra dieu kien len landlord: `GET /api/users/me/landlord-eligibility`.
- Lay danh sach session dang active: `GET /api/users/me/sessions`.
- Thu hoi mot session: `DELETE /api/users/me/sessions/{id}`.
- Gan role tenant mac dinh.
- Cap role landlord sau khi nha tro duoc admin approve.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Users/UsersController.cs`
- `server/src/SmartRentalPlatform.Application/Users/UserService.cs`
- `server/src/SmartRentalPlatform.Domain/Entities/Users`

### 2.3 KYC/eKYC

Trang thai: Da dat logic core, can test voi provider VNPT that va mock.

Logic hien co:

- Submit KYC: `POST /api/kyc/submissions`.
- Xem trang thai KYC ca nhan: `GET /api/kyc/my-status`.
- Xem lich su KYC ca nhan: `GET /api/kyc/my-history`.
- Test VNPT document-only: `POST /api/kyc/vnpt-document-test`.
- Co interface provider eKYC va 2 implementation: mock/real VNPT.
- Luu du lieu anh mat truoc, mat sau, selfie, provider status, provider response.
- Co enum ket qua check document, face match, liveness, risk level, verification status.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Kyc/KycController.cs`
- `server/src/SmartRentalPlatform.Application/Kyc/KycService.cs`
- `server/src/SmartRentalPlatform.Infrastructure/ExternalServices/Ekyc`
- `server/src/SmartRentalPlatform.Domain/Entities/Users/KycVerification.cs`
- `server/src/SmartRentalPlatform.Domain/Enums/Kyc`

### 2.4 Admin approval

Trang thai: Da dat logic duyet chinh, can test phan phan quyen Admin.

Logic hien co:

- Admin xem danh sach user: `GET /api/admin/users`.
- Admin xem chi tiet user: `GET /api/admin/users/{userId}`.
- Admin xem KYC pending/detail/history.
- Admin approve/reject KYC.
- Admin xem nha tro pending/public/detail.
- Admin approve/reject nha tro.
- Ghi audit log cho hanh dong approve/reject.
- Xem media private thong qua endpoint admin: `GET /api/admin/media/private`.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Admin`
- `server/src/SmartRentalPlatform.Application/AdminApproval`
- `server/src/SmartRentalPlatform.Domain/Entities/AdminApproval/ApprovalAuditLog.cs`

### 2.5 Rooming house/property onboarding

Trang thai: Da dat flow chinh cho landlord.

Logic hien co:

- Lay onboarding hien tai cua landlord: `GET /api/rooming-houses/my/onboarding`.
- Tao draft nha tro: `POST /api/rooming-houses/draft`.
- Lay danh sach nha tro cua landlord: `GET /api/rooming-houses/my`.
- Lay chi tiet nha tro: `GET /api/rooming-houses/{id}`.
- Cap nhat thong tin co ban nha tro.
- Cap nhat amenities.
- Cap nhat images.
- Cap nhat legal document.
- Submit nha tro cho admin duyet.
- Cap nhat lease policy.
- Public listing nha tro: `GET /api/public/rooming-houses`.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Properties/RoomingHousesController.cs`
- `server/src/SmartRentalPlatform.Api/Controllers/Public/PublicRoomingHousesController.cs`
- `server/src/SmartRentalPlatform.Application/RoomingHouses`
- `server/src/SmartRentalPlatform.Domain/Entities/Properties/RoomingHouse.cs`

### 2.6 Room management

Trang thai: Da dat CRUD/management core, can test cac rule gia va trang thai.

Logic hien co:

- Tao phong trong nha tro: `POST /api/rooming-houses/{roomingHouseId}/rooms`.
- Lay danh sach phong theo nha tro.
- Lay chi tiet phong.
- Cap nhat phong.
- Cap nhat anh phong.
- Cap nhat amenities cua phong.
- Cap nhat price tiers.
- Cap nhat status phong.
- Submit phong.
- Validate so phong khong trung.
- Validate required images.
- Validate tiered pricing.
- Check nha tro thuoc landlord va da approved truoc khi thao tac phong.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Properties/RoomsController.cs`
- `server/src/SmartRentalPlatform.Application/Rooms`
- `server/src/SmartRentalPlatform.Domain/Entities/Properties/Room.cs`
- `server/src/SmartRentalPlatform.Domain/Entities/Properties/RoomPriceTier.cs`

### 2.7 Catalog va administrative data

Trang thai: Da dat logic read/list.

Logic hien co:

- Lay danh sach amenities active: `GET /api/amenities`.
- Lay danh sach province: `GET /api/administrative/provinces`.
- Lay danh sach ward theo province: `GET /api/administrative/provinces/{provinceCode}/wards`.
- Co seed data cho role, amenity, administrative data va admin development.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Catalog`
- `server/src/SmartRentalPlatform.Application/Amenities`
- `server/src/SmartRentalPlatform.Application/Administrative`
- `server/src/SmartRentalPlatform.Infrastructure/Persistence/Seed`

### 2.8 File storage va media

Trang thai: Da dat local storage, can dinh huong storage production sau.

Logic hien co:

- Upload image qua `POST /api/files/images`.
- Ho tro scope upload cho avatar, KYC, rooming house, room, legal document.
- Co local public storage va local private storage.
- Runtime uploads/private storage da duoc `.gitignore` chan.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Controllers/Files/FilesController.cs`
- `server/src/SmartRentalPlatform.Infrastructure/Storage/LocalFileStorageService.cs`
- `server/src/SmartRentalPlatform.Infrastructure/Storage/LocalPrivateStorageService.cs`

### 2.9 Cross-cutting backend

Trang thai: Da dat nen tang.

Logic hien co:

- Exception middleware chuan hoa loi API.
- JWT authentication.
- CORS cho frontend React.
- Swagger cho Development.
- EF Core DbContext, migrations, entity configurations.
- Dependency Injection theo layer Application/Infrastructure.
- Health check endpoint: `GET /api/health`.

File lien quan:

- `server/src/SmartRentalPlatform.Api/Program.cs`
- `server/src/SmartRentalPlatform.Api/Middlewares/ExceptionHandlingMiddleware.cs`
- `server/src/SmartRentalPlatform.Api/Extensions`
- `server/src/SmartRentalPlatform.Infrastructure/Persistence`

## 3. Frontend - logic da dat duoc

### 3.1 Routing va guard

Trang thai: Da dat.

Logic hien co:

- Routing bang React Router.
- Protected route cho user da dang nhap.
- Onboarding guard.
- Role guard cho Admin.
- Redirect route cu/alias ve route chuan.

File lien quan:

- `client/src/app/router/routes.tsx`
- `client/src/app/router/ProtectedRoute.tsx`
- `client/src/app/router/OnboardingGuard.tsx`
- `client/src/app/router/RoleGuard.tsx`

### 3.2 Auth UI

Trang thai: Da dat flow UI chinh.

Logic hien co:

- Login page/form.
- Register page/form.
- Verify email OTP.
- Forgot password.
- Reset password.
- Google login button.
- Token storage va API client refresh token.

File lien quan:

- `client/src/features/auth`
- `client/src/shared/api/apiClient.ts`
- `client/src/shared/api/tokenStorage.ts`

### 3.3 Profile/KYC UI

Trang thai: Da dat, can test lai UX upload/camera tren nhieu trinh duyet.

Logic hien co:

- Me page.
- My profile page.
- KYC submit page.
- KYC status page.
- Webcam capture component.
- Profile API va KYC API typed services.

File lien quan:

- `client/src/features/home`
- `client/src/features/profile`
- `client/src/features/kyc`

### 3.4 Landlord/property UI

Trang thai: Da dat flow chinh.

Logic hien co:

- Create rooming house page.
- Landlord dashboard.
- Rooming house detail page.
- Rooming house editor.
- Property image editor.
- API layer cho rooming house, landlord, files, administrative data, rooms.

File lien quan:

- `client/src/features/rooming-houses`
- `client/src/features/landlord`
- `client/src/features/rooms`
- `client/src/features/files`
- `client/src/features/administrative`

### 3.5 Admin UI

Trang thai: Da dat trang tong hop admin, can tach component neu tiep tuc refactor.

Logic hien co:

- Admin home page.
- Danh sach KYC pending/detail/approve/reject.
- Danh sach rooming house pending/public/detail/approve/reject.
- Danh sach user/detail.
- Admin image component de load media.

File lien quan:

- `client/src/features/admin`

## 4. Cau truc va hygiene da dat

Trang thai: Tot cho local clean code Interval 1.

Da dat:

- Backend khong con gom tat ca logic vao controller; da co Application services.
- Contracts da tach request/response theo feature.
- Domain co entities/enums rieng.
- Infrastructure gom EF, storage, external services, security.
- Frontend da gom theo feature: `auth`, `profile`, `kyc`, `admin`, `landlord`, `rooming-houses`, `rooms`.
- `.gitignore` dang theo allow-list: chi uu tien `client/`, `server/`, `README.md`, `docker-compose.yml`, `.gitignore`.
- Chan secrets/local files: `.env`, `appsettings.Development.json`, `appsettings.Local.json`, `appsettings.Production.json`.
- Chan output: `.agent`, `bin`, `obj`, `node_modules`, `client/dist`, uploads/private storage.

## 5. Nhung diem can tiep tuc refactor sau Interval 1

Muc uu tien cao:

- Them test cho service quan trong: Auth, KYC, Admin approval, RoomingHouse, Room.
- Test build CI local truoc PR: `dotnet build server/SmartRentalPlatform.slnx` va `npm run build` trong `client`.
- Khong dua `appsettings.Development.json` len Git; khi deploy nen dung environment variables, user-secrets hoac secret manager.

Muc uu tien trung binh:

- Tach nho cac page frontend lon:
  - `client/src/features/admin/pages/AdminHomePage.tsx`
  - `client/src/features/landlord/pages/RoomingHouseDetailPage.tsx`
  - `client/src/features/profile/pages/MyProfilePage.tsx`
  - `client/src/features/rooming-houses/components/RoomingHouseEditor.tsx`
- Tach API endpoint constants theo feature neu `endpoints.ts` tiep tuc phinh to.
- Chuan hoa loading/error/toast UX tren cac flow submit.

Muc uu tien sau:

- Doi local storage sang cloud/object storage cho production.
- Them audit/logging ro hon cho flow nhay cam: KYC, approve/reject, revoke session.
- Them rate limit cho OTP/login/reset password neu chua cau hinh production.
- Them health check chi tiet hon cho database/external services.

## 6. Danh gia theo checklist

| Nhom logic | Trang thai | Ghi chu |
|---|---|---|
| Auth email/password | Da dat | Can test OTP/email SMTP local |
| Google login | Da dat | Can test voi Google Client ID that |
| Refresh/logout token | Da dat | Can test multi-session |
| Profile | Da dat | Can test upload/avatar neu dung |
| KYC | Da dat | Can test mock va VNPT real |
| Admin KYC approval | Da dat | Can test role Admin |
| Admin rooming house approval | Da dat | Can test approve/reject va audit |
| Rooming house draft/submit | Da dat | Can test du lieu legal/images |
| Room management | Da dat | Can test validation room number/price tier |
| Public rooming houses | Da dat | Can test listing theo status |
| File upload/storage | Da dat local | Can thay storage production sau |
| Frontend routing/guard | Da dat | Can test redirect/onboarding |
| Frontend admin | Da dat | Nen tach nho component sau |
| Tests tu dong | Chua thay | Can bo sung |
| Git hygiene/secrets | Da dat local | File private dang duoc ignore |

## 7. Ket luan

Du an hien tai da co day du khung logic chinh de xem la clean code hoan thanh Interval 1: authentication, user/profile, KYC, admin approval, landlord onboarding, room/property management va frontend tuong ung.

Chua nen coi la production-ready vi con thieu automated tests, can QA end-to-end, va can dua secrets sang co che quan ly moi truong an toan hon khi deploy. Tuy nhien, de day len branch `develop` cho muc tieu review Interval 1 thi logic hien tai da co nen tang tot.
