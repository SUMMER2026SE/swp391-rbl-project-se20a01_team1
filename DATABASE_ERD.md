# Database ERD - Smart Rental Platform

Ngay tao: 2026-05-30  
Nguon ra soat: `AppDbContext`, Domain Entities va EF Core Configurations hien tai.

> Ghi chu: File nay mo ta schema dang duoc dang ky trong `AppDbContext`. Entity `AdministrativeDistrict` van ton tai trong Domain, nhung hien tai khong co `DbSet` va khong thay configuration tuong ung dang apply, nen khong dua vao ERD chinh.

## 1. Mermaid ERD code

```mermaid
erDiagram
    users {
        uuid id PK
        string email
        string normalized_email UK
        string phone_number
        text password_hash
        string display_name
        text avatar_url
        string status
        string onboarding_status
        boolean email_confirmed
        boolean phone_confirmed
        int access_failed_count
        datetime lockout_end_at
        datetime last_login_at
        datetime created_at
        datetime updated_at
        datetime deleted_at
    }

    user_profiles {
        uuid user_id PK, FK
        string full_name
        date date_of_birth
        string gender
        text address_line
        string verified_citizen_id_masked
        string emergency_contact_name
        string emergency_contact_phone
        datetime created_at
        datetime updated_at
    }

    roles {
        int id PK
        string name UK
        text description
        datetime created_at
    }

    user_roles {
        uuid user_id PK, FK
        int role_id PK, FK
        datetime created_at
    }

    external_logins {
        uuid id PK
        uuid user_id FK
        string provider
        string provider_user_id
        string provider_email
        string provider_display_name
        text provider_avatar_url
        datetime created_at
        datetime last_login_at
    }

    user_tokens {
        uuid id PK
        uuid user_id FK
        string token_type
        text token_hash UK
        uuid token_family_id
        uuid replaced_by_token_id FK
        datetime expires_at
        datetime used_at
        datetime revoked_at
        string revoked_reason
        string created_by_ip
        text user_agent
        datetime created_at
    }

    login_logs {
        uuid id PK
        uuid user_id FK
        string email_attempted
        string login_provider
        string ip_address
        text user_agent
        boolean is_success
        string failure_reason
        datetime created_at
    }

    kyc_verifications {
        uuid id PK
        uuid user_id FK
        string document_type
        string ekyc_provider
        string ekyc_session_id
        text front_image_object_key
        text back_image_object_key
        text selfie_image_object_key
        string selfie_capture_method
        string ocr_full_name
        string ocr_citizen_id_masked
        text citizen_id_hash
        date ocr_date_of_birth
        string ocr_gender
        text ocr_address
        decimal ocr_confidence
        string document_check_result
        decimal face_match_score
        string face_match_result
        string liveness_result
        string ekyc_result
        string ekyc_error_code
        text ekyc_error_message
        string risk_level
        string status
        uuid reviewed_by_admin_id FK
        text rejected_reason
        datetime submitted_at
        datetime reviewed_at
        datetime created_at
        datetime updated_at
    }

    administrative_provinces {
        string code PK
        string name
        string type
        boolean is_active
        datetime created_at
        datetime updated_at
    }

    administrative_wards {
        string code PK
        string province_code FK
        string name
        string type
        boolean is_active
        datetime created_at
        datetime updated_at
    }

    rooming_houses {
        uuid id PK
        uuid landlord_user_id FK
        string name
        text description
        text address_line
        string ward_code FK
        string province_code FK
        text address_display
        decimal latitude
        decimal longitude
        string approval_status
        string visibility_status
        text rejected_reason
        uuid reviewed_by_admin_id FK
        datetime reviewed_at
        datetime created_at
        datetime updated_at
        datetime deleted_at
    }

    rooms {
        uuid id PK
        uuid rooming_house_id FK
        string room_number
        int floor
        decimal area_m2
        int max_occupants
        boolean is_tiered_pricing
        string status
        text description
        datetime created_at
        datetime updated_at
        datetime deleted_at
    }

    room_price_tiers {
        uuid id PK
        uuid room_id FK
        int occupant_count
        decimal monthly_rent
        boolean is_active
        datetime created_at
        datetime updated_at
    }

    amenities {
        int id PK
        string name
        string scope
        string icon_code
        boolean is_active
        datetime created_at
    }

    rooming_house_amenities {
        uuid rooming_house_id PK, FK
        int amenity_id PK, FK
    }

    room_amenities {
        uuid room_id PK, FK
        int amenity_id PK, FK
    }

    property_images {
        uuid id PK
        uuid rooming_house_id FK
        uuid room_id FK
        text object_key
        text image_url
        string caption
        boolean is_cover
        int sort_order
        datetime created_at
    }

    rooming_house_legal_documents {
        uuid rooming_house_id PK, FK
        string document_type
        text front_image_object_key
        text back_image_object_key
        text extra_image_object_key
        string document_number_masked
        text document_number_hash
        datetime uploaded_at
        datetime created_at
        datetime updated_at
    }

    lease_policies {
        uuid id PK
        uuid rooming_house_id FK, UK
        boolean allow_short_term_renewal
        int renewal_notice_days
        decimal deposit_months
        decimal discount_6_months_percent
        decimal discount_9_months_percent
        decimal discount_12_months_percent
        decimal discount_24_months_percent
        boolean is_active
        datetime created_at
        datetime updated_at
    }

    approval_audit_logs {
        uuid id PK
        uuid admin_id
        string approval_type
        uuid entity_id
        string action
        text reason
        text additional_info
        datetime created_at
    }

    users ||--o| user_profiles : has
    users ||--o{ user_roles : has
    roles ||--o{ user_roles : assigned
    users ||--o{ external_logins : has
    users ||--o{ user_tokens : owns
    user_tokens ||--o{ user_tokens : replaces
    users ||--o{ login_logs : writes
    users ||--o{ kyc_verifications : submits
    users ||--o{ kyc_verifications : reviews

    administrative_provinces ||--o{ administrative_wards : contains
    administrative_provinces ||--o{ rooming_houses : located_in
    administrative_wards ||--o{ rooming_houses : located_in

    users ||--o{ rooming_houses : owns_as_landlord
    users ||--o{ rooming_houses : reviews
    rooming_houses ||--o{ rooms : has
    rooming_houses ||--o{ property_images : has
    rooms ||--o{ property_images : has
    rooming_houses ||--o| rooming_house_legal_documents : has
    rooming_houses ||--o| lease_policies : has

    rooms ||--o{ room_price_tiers : has
    rooming_houses ||--o{ rooming_house_amenities : has
    amenities ||--o{ rooming_house_amenities : tagged_by
    rooms ||--o{ room_amenities : has
    amenities ||--o{ room_amenities : tagged_by
```

## 2. Nhom bang theo module

### Users/Auth

- `users`: tai khoan chinh, email, password hash, trang thai, onboarding status.
- `user_profiles`: profile mo rong, quan he 1-1 voi `users`.
- `roles`: danh muc role.
- `user_roles`: bang trung gian many-to-many giua `users` va `roles`.
- `external_logins`: login provider ben ngoai, hien dung cho Google.
- `user_tokens`: refresh/email/reset token da hash, co token family va replaced token.
- `login_logs`: lich su dang nhap thanh cong/that bai.

### KYC

- `kyc_verifications`: luu lan submit KYC, anh giay to/selfie, OCR, face match, liveness, risk, status approve/reject.
- `reviewed_by_admin_id` tro ve `users.id`, nhung day la admin user.

### Administrative

- `administrative_provinces`: tinh/thanh.
- `administrative_wards`: xa/phuong, lien ket voi province.

### Property/Rooming House

- `rooming_houses`: nha tro cua landlord, dia chi, trang thai duyet, trang thai hien thi.
- `rooms`: phong thuoc nha tro.
- `room_price_tiers`: gia theo so nguoi cua phong.
- `amenities`: tien ich dung chung cho nha tro/phong.
- `rooming_house_amenities`: many-to-many nha tro - tien ich.
- `room_amenities`: many-to-many phong - tien ich.
- `property_images`: anh cua nha tro hoac anh cua phong. Co check constraint: chi duoc co `rooming_house_id` hoac `room_id`, khong duoc ca hai.
- `rooming_house_legal_documents`: giay to phap ly 1-1 voi nha tro.
- `lease_policies`: chinh sach hop dong/gia han 1-1 voi nha tro.

### Admin Approval

- `approval_audit_logs`: audit log hanh dong approve/reject. Bang nay dang luu `admin_id`, `approval_type`, `entity_id` theo dang generic, khong cau hinh foreign key truc tiep den entity cu the.

## 3. Cac quan he quan trong

| Quan he | Kieu | Ghi chu |
|---|---|---|
| `users` - `user_profiles` | 1-1 | Xoa user se xoa profile |
| `users` - `roles` | N-N | Qua `user_roles` |
| `users` - `external_logins` | 1-N | Google/external provider |
| `users` - `user_tokens` | 1-N | Token dang nhap/OTP/reset |
| `user_tokens` - `user_tokens` | self reference | `replaced_by_token_id` |
| `users` - `kyc_verifications` | 1-N | User co nhieu lan KYC |
| `users` - `rooming_houses` | 1-N | Landlord so huu nha tro |
| `administrative_provinces` - `administrative_wards` | 1-N | Province gom nhieu ward |
| `administrative_provinces` - `rooming_houses` | 1-N | Dia chi nha tro |
| `administrative_wards` - `rooming_houses` | 1-N | Dia chi nha tro |
| `rooming_houses` - `rooms` | 1-N | Nha tro co nhieu phong |
| `rooms` - `room_price_tiers` | 1-N | Phong co nhieu bac gia |
| `rooming_houses` - `amenities` | N-N | Qua `rooming_house_amenities` |
| `rooms` - `amenities` | N-N | Qua `room_amenities` |
| `rooming_houses` - `rooming_house_legal_documents` | 1-1 | PK cua legal document la `rooming_house_id` |
| `rooming_houses` - `lease_policies` | 1-1 | `lease_policies.rooming_house_id` unique |
| `rooming_houses` - `property_images` | 1-N | Anh nha tro |
| `rooms` - `property_images` | 1-N | Anh phong |

## 4. Diem can luu y khi ve ERD/bao cao

- `approval_audit_logs.admin_id` hien la uuid thuong, co index nhung khong thay cau hinh foreign key truc tiep den `users`.
- `approval_audit_logs.entity_id` la generic id, phu thuoc `approval_type`, nen ERD khong the hien FK cu the.
- `property_images` la bang da hinh cho 2 owner: `rooming_house_id` hoac `room_id`.
- `AdministrativeDistrict` co entity trong Domain nhung khong nam trong `AppDbContext` hien tai. Neu muon dung cap quan/huyen, can them `DbSet`, configuration va quan he lai.
- Mot so quan he dung `DeleteBehavior.Restrict` de tranh xoa day chuyen, vi du landlord-rooming house, rooming house-room.
