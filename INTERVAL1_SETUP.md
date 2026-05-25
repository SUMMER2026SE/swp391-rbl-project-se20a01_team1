# Interval 1 — Monorepo Setup (Người 5)

## Cấu trúc

```
├── client/          # React + Vite + TypeScript (port 5173)
└── server/          # ASP.NET Core API (port 5000)
```

## Database (Docker — khuyến nghị)

```powershell
# Từ thư mục gốc repo
docker compose up -d
```

PostgreSQL: `localhost:5432`, user/pass `postgres`/`postgres`, DB `smart-rental_platform`.

Sau đó apply migration:

```powershell
cd server
dotnet ef database update `
  --project src\SmartRentalPlatform.Infrastructure\SmartRentalPlatform.Infrastructure.csproj `
  --startup-project src\SmartRentalPlatform.Api\SmartRentalPlatform.Api.csproj
```

Migration: `AddAdminApprovalEntities` (schema + seed HasData).

## Seed đăng nhập

| Email | Password | Ghi chú |
|-------|----------|---------|
| admin@gmail.com | Password123! | DevAuth middleware gán Role Admin cho `/api/admin/*` |
| tenant.kyc@gmail.com | Password123! | Có bản ghi KYC `PendingAdminReview` |
| landlord.kyc@gmail.com | Password123! | Có bản ghi KYC `PendingAdminReview` |

Guid đồng bộ: `SeedIds.cs` ↔ `client/src/lib/constants.ts`.

## Chạy API

```powershell
cd server/src/SmartRentalPlatform.Api
dotnet run
# http://localhost:5000
```

## Chạy React

```powershell
cd client
npm install
npm run dev
# http://localhost:5173 — proxy /api → :5000
```

## 5 trang FE

1. `/admin/kyc` — Danh sách KYC chờ duyệt
2. `/admin/kyc/:kycId` — Chi tiết + duyệt/từ chối
3. `/admin/rooming-houses` — Khu trọ chờ duyệt
4. `/admin/rooming-houses/:id` — Chi tiết khu trọ
5. `/` — Public listing (giá theo tier)
