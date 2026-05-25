# Cấu trúc thư mục Frontend (theo role / nghiệp vụ)

Mục tiêu: mỗi người / mỗi role merge vào **một nhánh thư mục riêng**, tránh đụng file của nhau.

## Cây thư mục hiện tại

```
client/src/
├── app/                    # Shell app, router, layout
│   ├── App.tsx
│   └── routes.tsx          # Hằng số path (dùng khi tách route file)
├── features/
│   ├── admin/              # Người 5 — Admin duyệt KYC & khu trọ
│   │   ├── kyc/pages/
│   │   ├── rooming-house/pages/
│   │   └── types/
│   ├── public/             # Người 5 — Guest/Tenant xem listing
│   │   ├── listing/pages/
│   │   └── types/
│   ├── auth/               # (trống) Person 1 — đăng nhập, JWT
│   ├── tenant/             # (trống) Person 3 — thuê phòng, booking...
│   └── landlord/           # (trống) Person 2 — quản lý khu trọ
└── shared/                 # Dùng chung mọi role
    ├── api/apiClient.ts
    └── constants/seed-ids.ts
```

## Quy ước khi thêm code mới

| Role / API | Thư mục page | URL prefix |
|------------|--------------|------------|
| Public | `features/public/<module>/pages/` | `/`, `/houses/:id` |
| Admin | `features/admin/<module>/pages/` | `/admin/...` |
| Tenant | `features/tenant/<module>/pages/` | `/tenant/...` (sau này) |
| Landlord | `features/landlord/<module>/pages/` | `/landlord/...` (sau này) |
| Auth | `features/auth/<module>/pages/` | `/login`, `/register` |

- **Types**: đặt cạnh feature (`features/admin/types/`, `features/public/types/`).
- **Components** dùng trong một module: `features/<role>/<module>/components/`.
- **Không** đặt page phẳng trong `src/pages/` — dùng `features/<role>/`.

## Route (Interval 1 — Người 5)

- Public: `/`, `/houses/:id`
- Admin: `/admin/kyc`, `/admin/kyc/:kycId`, `/admin/rooming-houses`, `/admin/rooming-houses/:id`

## Seed IDs

`shared/constants/seed-ids.ts` — đồng bộ với `server/.../Persistence/Seed/SeedIds.cs`.
