# Demo Full Flow Test Plan

## 1. Migration demo

Migration file:

```text
server/src/SmartRentalPlatform.Infrastructure/Persistence/Migrations/20260623120000_DemoFullFlowDataset.cs
```

Migration này reset dữ liệu nghiệp vụ/user hiện tại và seed lại bộ demo sạch:

- Chỉ giữ dữ liệu catalog nền: role, amenity, tỉnh/phường, service type.
- Xóa dữ liệu user, ví, thanh toán, khu trọ, phòng, yêu cầu thuê, cọc, hợp đồng, hóa đơn, thông báo, lịch xem phòng.
- Seed lại `Khu trọ KFC Riverside` với `KFC-101`, `KFC-102`, `KFC-201`.
- Invoice tháng 5 dùng kỳ `01/05/2026-31/05/2026`.

Chạy migration:

```powershell
cd B:\FPT\2026\Summer2026\SWP\smart-rental-platform\server
dotnet ef database update 20260623120000_DemoFullFlowDataset --project .\src\SmartRentalPlatform.Infrastructure\SmartRentalPlatform.Infrastructure.csproj --startup-project .\src\SmartRentalPlatform.Api\SmartRentalPlatform.Api.csproj
```

Rollback riêng migration demo:

```powershell
cd B:\FPT\2026\Summer2026\SWP\smart-rental-platform\server
dotnet ef database update 20260622113000_SeedKfcScenarioDataset --project .\src\SmartRentalPlatform.Infrastructure\SmartRentalPlatform.Infrastructure.csproj --startup-project .\src\SmartRentalPlatform.Api\SmartRentalPlatform.Api.csproj
```

## 2. Account demo

Mật khẩu chung:

```text
Demo@123456
```

| Vai trò | Email | Trạng thái |
|---|---|---|
| Admin | admin.hoasen@example.com | Active |
| Landlord | nguyenxuanhuan.dev@gmail.com | KYC approved, ví 50 triệu, reserved 3.5 triệu |
| Main tenant cũ | hoctienganh4english@gmail.com | KYC approved, ví 50 triệu, đang thuê KFC-101 |
| Co-tenant | phan.van.thanh@example.com | KYC approved |
| Tenant chưa KYC | hoang.phuc.nhat.quang@example.com | Chưa KYC, dùng để test block người ở |
| Tenant thuê mới | demoThueTro@gmail.com | KYC approved, ví 50 triệu |

## 3. Chạy local

API:

```powershell
cd B:\FPT\2026\Summer2026\SWP\smart-rental-platform\server
dotnet run --project .\src\SmartRentalPlatform.Api\SmartRentalPlatform.Api.csproj --urls http://localhost:5294
```

Client:

```powershell
cd B:\FPT\2026\Summer2026\SWP\smart-rental-platform\client
npm run dev
```

URL:

```text
API:    http://localhost:5294
Client: http://localhost:5173
```

## 4. Chạy ngrok

Expose API:

```powershell
ngrok http 5294
```

Expose client nếu cần demo từ máy khác:

```powershell
ngrok http 5173
```

Khi dùng PayOS thật, cập nhật URL webhook trên PayOS dashboard:

```text
https://<api-ngrok-domain>/api/payment-webhooks/payos
```

Nếu chỉ test local/mock payment thì không cần ngrok PayOS.

## 5. Kịch bản test chính

### A. Search

1. Mở home/search.
2. Tìm `KFC`, `Khu trọ KFC Riverside`, hoặc khu vực quanh địa chỉ.
3. Kiểm tra danh sách có `Khu trọ KFC Riverside`.
4. Vào chi tiết khu trọ:
   - `KFC-101`: đang thuê.
   - `KFC-102`: còn trống.
   - `KFC-201`: ẩn/draft.

### B. Nạp ví

1. Đăng nhập `demoThueTro@gmail.com`.
2. Vào ví cá nhân.
3. Tạo giao dịch nạp ví PayOS/mock.
4. Nếu môi trường đang mock, mở checkout mock và xác nhận thành công.
5. Kiểm tra balance tăng và có wallet transaction.

### C. Tạo khu trọ mới

1. Đăng nhập landlord `nguyenxuanhuan.dev@gmail.com`.
2. Tạo khu trọ mới đầy đủ:
   - Thông tin khu trọ.
   - Ảnh.
   - Tiện nghi.
   - Giấy tờ pháp lý.
   - Nội quy.
   - Chính sách thuê.
   - Bảng giá phòng.
3. Kiểm tra khu trọ mới ở trạng thái đúng theo flow duyệt/hiển thị hiện tại.

### D. Block thuê khi còn invoice cũ

1. Đăng nhập `hoctienganh4english@gmail.com`.
2. Gửi yêu cầu thuê `KFC-102` từ `10/07/2026` đến `10/07/2027`.
3. Kỳ vọng: bị block vì còn invoice tháng 5 quá hạn của hợp đồng `KFC-101`.

### E. Block chấm dứt hợp đồng khi còn invoice

1. Đăng nhập tenant hoặc landlord liên quan hợp đồng `KFC-101`.
2. Thử chấm dứt hợp đồng.
3. Kỳ vọng: bị block vì invoice `KFC-SCENARIO-INV-202605-01` chưa thanh toán.

### F. Thanh toán hóa đơn tháng 5

1. Đăng nhập `hoctienganh4english@gmail.com`.
2. Vào danh sách hóa đơn.
3. Thanh toán invoice tháng 5 `01/05/2026-31/05/2026`.
4. Kiểm tra:
   - Invoice chuyển `Paid`.
   - Ví tenant trừ 3.500.000.
   - Ví landlord cộng 3.500.000.

### G. Chấm dứt hợp đồng và hóa đơn kỳ cuối

1. Sau khi invoice tháng 5 đã paid, chấm dứt hợp đồng `KFC-101`.
2. Kiểm tra tiền cọc/reserved của landlord được xử lý theo logic hiện tại.
3. Tenant thử thuê lại phòng.
4. Kỳ vọng: vẫn bị block nếu landlord chưa tạo hóa đơn kỳ cuối.
5. Landlord tạo hóa đơn kỳ cuối.
6. Tenant thấy hóa đơn kỳ cuối nhưng chưa thanh toán thì vẫn bị block.
7. Tenant thanh toán hóa đơn kỳ cuối.
8. Tenant được thuê bình thường.

### H. Co-tenant thuê riêng

1. Đăng nhập `phan.van.thanh@example.com`.
2. Gửi yêu cầu thuê phòng trống.
3. Kỳ vọng: không bị block bởi lịch sử hợp đồng của main tenant cũ sau khi hợp đồng đã kết thúc.

### I. Full flow thuê mới, ký OTP, hoàn cọc

1. Đăng nhập `demoThueTro@gmail.com`.
2. Gửi yêu cầu thuê `KFC-102` từ `10/07/2026` đến `10/07/2027`.
3. Landlord `nguyenxuanhuan.dev@gmail.com` accept yêu cầu.
4. Tenant thanh toán cọc.
5. Tenant nhập người ở:
   - Thử nhập `hoang.phuc.nhat.quang@example.com`.
   - Kỳ vọng: bị block vì chưa KYC.
   - Nhập lại người đã KYC hoặc nhập thủ công đủ giấy tờ.
6. Landlord ký hợp đồng bằng OTP.
7. Tenant ký hợp đồng bằng OTP.
8. Landlord chấm dứt hợp đồng ngay khi chưa vào ở.
9. Kỳ vọng:
   - Không tạo invoice kỳ cuối.
   - Landlord hoàn cọc.
   - Reserved balance landlord về 0.
   - Tenant nhận lại tiền cọc.

## 6. SQL kiểm tra nhanh

```sql
select email, display_name from users order by email;

select u.email, w.balance, w.reserved_balance
from wallet_accounts w
join users u on u.id = w.user_id
order by u.email;

select rh.name, r.room_number, r.status
from rooming_houses rh
join rooms r on r.rooming_house_id = rh.id
order by rh.name, r.room_number;

select invoice_no, billing_period_start, billing_period_end, total_amount, status
from invoices
order by billing_period_start;
```
