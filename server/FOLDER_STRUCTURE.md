# Cấu trúc thư mục Backend (theo role / nghiệp vụ)

## API Controllers (`SmartRentalPlatform.Api/Controllers/`)

```
Controllers/
├── Admin/
│   ├── Kyc/AdminKYCController.cs              → /api/admin/kyc
│   └── RoomingHouse/AdminRoomingHouseController.cs → /api/admin/rooming-houses
├── Public/
│   └── Listing/PublicListingController.cs       → /api/public/rooming-houses
└── Common/
    ├── HealthController.cs
    └── MediaController.cs
```

**Khi thêm controller mới:** đặt đúng thư mục `Admin/`, `Public/`, `Landlord/`, `Tenant/`, `Common/` — không thêm file phẳng trong `Controllers/`.

## Application layer (đã có)

```
Application/
└── AdminApproval/          # Người 5 — services, DTOs duyệt KYC & khu trọ
```

Gợi ý mở rộng khi các person merge:

| Person | Thư mục gợi ý |
|--------|----------------|
| 1 Identity | `Application/Identity/`, `Api/Controllers/Auth/` |
| 2 Landlord | `Application/Landlord/`, `Api/Controllers/Landlord/` |
| 3 Tenant | `Application/Tenant/`, `Api/Controllers/Tenant/` |
| 4 Media | `Application/Media/` (hoặc mở rộng `Common/MediaController`) |

## Domain / Infrastructure

Giữ theo bounded context; seed: `Infrastructure/Persistence/Seed/`.
