# KFC Scenario - Mock Data And Full Test Plan

## 1. Mục tiêu

Tạo một bộ mock data mới tên `KFC` có cùng logic với kịch bản gốc `Khu trọ Hoa Sen`, dùng để test trọn vẹn các nghiệp vụ:

- Tenant đang có hợp đồng và còn nợ invoice thì bị chặn thuê phòng mới.
- Tenant còn invoice quá hạn thì bị chặn chấm dứt hợp đồng.
- Sau khi trả invoice quá hạn, tenant được chấm dứt hợp đồng nhưng vẫn bị chặn thuê nếu còn nghĩa vụ invoice kỳ cuối.
- Co-tenant trong hợp đồng cũ không bị dính nợ của main tenant.
- Tenant chỉ được thuê lại khi đã thanh toán đầy đủ mọi invoice.
- Luồng thuê mới phải kiểm tra KYC của toàn bộ người ở.
- Nếu hợp đồng bị chấm dứt trước ngày vào ở thì không tạo invoice kỳ cuối và phải hoàn cọc đúng.

## 2. Mapping dữ liệu từ kịch bản gốc sang KFC

| Kịch bản gốc | Mock data KFC |
| --- | --- |
| Khu trọ Hoa Sen | Khu trọ KFC Riverside |
| Phòng 101 | KFC-101 |
| Phòng 102 | KFC-102 |
| Phòng 201 | KFC-201 |
| Prefix HOA-SEN | Prefix KFC-SCENARIO |

Tên người, email account, trạng thái KYC và vai trò được giữ giống kịch bản seed test gốc để tester dễ đăng nhập và đối chiếu.

Mật khẩu mặc định cho toàn bộ account test: `Demo@123456`

## 3. Account test

| Vai trò | Tên | Email | Trạng thái |
| --- | --- | --- | --- |
| Admin | Quản trị viên hệ thống | `admin.hoasen@example.com` | Active |
| Landlord | Nguyễn Xuân Huấn | `nguyen.xuan.huan@example.com` | Active, KYC approved |
| Main tenant | Lê Quang Linh | `le.quang.linh@example.com` | Active, KYC approved |
| Co-tenant | Phan Văn Thành | `phan.van.thanh@example.com` | Active, KYC approved |
| Tenant chưa KYC | Hoàng Phúc Nhật Quang | `hoang.phuc.nhat.quang@example.com` | Active, chưa KYC approved |
| Người ở không account | Nguyễn Hoàng Minh | Không có account | Có thông tin giấy tờ occupant |

## 4. Mock data cần có trước khi test

### 4.1. Khu trọ

Khu trọ: `Khu trọ KFC Riverside`

Landlord sở hữu: `Nguyễn Xuân Huấn`

Trạng thái:

- `approval_status`: Approved
- `visibility_status`: Visible

Thông tin chính sách:

- Tiền cọc: 1 tháng
- Số tháng thuê tối thiểu: 6 tháng
- Số tháng thuê tối đa: 12 tháng
- Ngày thanh toán mặc định: ngày 5 hàng tháng
- Có nội quy, tiện nghi, giấy tờ pháp lý, ảnh khu trọ, ảnh phòng

### 4.2. Phòng

| Phòng | Trạng thái ban đầu | Max occupants | Giá 1 người | Giá 2 người | Giá 3 người | Ghi chú |
| --- | --- | ---: | ---: | ---: | ---: | --- |
| KFC-101 | Occupied | 3 | 2.500.000 | 3.000.000 | 3.500.000 | Đang có hợp đồng với Lê Quang Linh |
| KFC-102 | Available | 3 | 2.500.000 | 3.000.000 | 3.500.000 | Dùng để test thuê mới |
| KFC-201 | Hidden/Draft | 2 | 2.500.000 | 3.000.000 | Không có | Dùng để kiểm tra phòng ẩn không xuất hiện trong flow thuê |

### 4.3. Hợp đồng seed sẵn cho KFC-101

Main tenant: `Lê Quang Linh`

Landlord: `Nguyễn Xuân Huấn`

Phòng: `KFC-101`

Thông tin hợp đồng:

- Rental request tạo ngày `10/04/2026`
- Ngày thuê bắt đầu: `20/04/2026`
- Ngày thuê kết thúc: `20/04/2027`
- Số người ở: 3
- Giá thuê: `3.500.000 VND/tháng`
- Tiền cọc: `3.500.000 VND`
- Landlord accept ngày `15/04/2026`
- Tenant thanh toán cọc sau khi landlord accept 1 tiếng
- Hợp đồng được tạo ngày `15/04/2026`
- Landlord đã ký
- Tenant đã ký
- Contract đang active

Người ở trong hợp đồng:

| Người ở | Loại | KYC |
| --- | --- | --- |
| Lê Quang Linh | Main tenant | Approved |
| Phan Văn Thành | Co-tenant có account | Approved |
| Nguyễn Hoàng Minh | Occupant không có account | Có giấy tờ occupant |

### 4.4. Invoice seed sẵn

| Invoice | Kỳ hóa đơn | Tenant phải trả | Số tiền | Trạng thái |
| --- | --- | --- | ---: | --- |
| Invoice tháng 4 | 20/04/2026 - 30/04/2026 | Lê Quang Linh | 1.283.333 | Paid |
| Invoice tháng 5 | 01/05/2026 - 31/05/2026 | Lê Quang Linh | 3.500.000 | Overdue/Unpaid |

### 4.5. Wallet seed sẵn

Tenant `Lê Quang Linh`:

- Balance sau seed: `50.000.000 VND`
- Có payment transaction nạp ví đủ để sau khi trừ cọc và invoice tháng 4 vẫn còn `50.000.000 VND`
- Có wallet transaction thanh toán cọc
- Có wallet transaction thanh toán invoice tháng 4

Landlord `Nguyễn Xuân Huấn`:

- Balance sau seed: `50.000.000 VND`
- Reserved balance ban đầu: `3.500.000 VND`
- Có wallet transaction nhận cọc
- Có wallet transaction nhận tiền invoice tháng 4
- Khoản `3.500.000 VND` đang bị nền tảng giữ, chưa thể rút

## 5. Kịch bản test chi tiết

### TC-01 - Chặn tenant thuê phòng mới khi còn invoice quá hạn

User: `Lê Quang Linh`

Điều kiện đầu vào:

- Tenant đang có hợp đồng active ở `KFC-101`.
- Invoice tháng 5 đang quá hạn và chưa thanh toán.
- Phòng `KFC-102` đang trống.

Các bước:

1. Đăng nhập bằng `le.quang.linh@example.com`.
2. Mở trang chi tiết `Khu trọ KFC Riverside`.
3. Chọn phòng `KFC-102`.
4. Tạo yêu cầu thuê:
   - Ngày bắt đầu: `10/07/2026`
   - Ngày kết thúc: `10/07/2027`
   - Số người ở: 3
5. Bấm gửi yêu cầu thuê.

Kết quả mong đợi:

- Hệ thống không cho tạo rental request.
- UI hiển thị lỗi rõ nghĩa: tenant còn hóa đơn chưa thanh toán hoặc còn nghĩa vụ tài chính.
- Không phát sinh rental request mới đến `KFC-102`.
- Trạng thái phòng `KFC-102` vẫn là available.

DB cần kiểm:

```sql
select *
from rental_requests
where room_id = '<KFC-102-ID>'
  and tenant_user_id = '<TRAN-MINH-KHANG-ID>'
order by created_at desc;
```

Pass khi không có request mới được tạo.

### TC-02 - Chặn chấm dứt hợp đồng khi còn invoice quá hạn

User: `Lê Quang Linh`

Điều kiện đầu vào:

- Contract `KFC-101` đang active.
- Invoice tháng 5 chưa thanh toán.

Các bước:

1. Tenant vào trang hợp đồng đang thuê `KFC-101`.
2. Chọn chức năng chấm dứt hợp đồng.
3. Xác nhận thao tác.

Kết quả mong đợi:

- Hệ thống chặn chấm dứt hợp đồng.
- UI báo còn hóa đơn tháng 5 chưa thanh toán.
- Contract vẫn giữ trạng thái active.
- Tiền cọc vẫn đang được nền tảng giữ ở wallet landlord.

DB cần kiểm:

```sql
select status
from contracts
where id = '<KFC-101-CONTRACT-ID>';

select status
from invoices
where invoice_no like 'KFC-SCENARIO-%'
order by billing_period_start;
```

Pass khi contract vẫn active và invoice tháng 5 vẫn unpaid/overdue.

### TC-03 - Thanh toán invoice tháng 5 rồi chấm dứt hợp đồng

User: `Lê Quang Linh`

Điều kiện đầu vào:

- Invoice tháng 5 đang unpaid/overdue.
- Tenant đủ tiền trong ví.

Các bước:

1. Tenant mở danh sách hóa đơn.
2. Thanh toán invoice tháng 5.
3. Kiểm tra invoice chuyển sang paid.
4. Quay lại hợp đồng `KFC-101`.
5. Thực hiện chấm dứt hợp đồng.

Kết quả mong đợi:

- Invoice tháng 5 chuyển sang paid.
- Có wallet transaction trừ tiền tenant.
- Có wallet transaction cộng tiền landlord.
- Chấm dứt hợp đồng được phép thực hiện.
- Contract chuyển sang trạng thái chấm dứt theo logic hiện tại của hệ thống.
- Khoản cọc `3.500.000 VND` đang bị giữ được giải phóng cho landlord nếu rule hiện tại là landlord nhận cọc sau khi hợp đồng kết thúc.

DB cần kiểm:

```sql
select invoice_no, status, paid_at
from invoices
where invoice_no like 'KFC-SCENARIO-%';

select balance, reserved_balance
from wallet_accounts
where user_id = '<LANDLORD-ID>';
```

Pass khi invoice tháng 5 paid, contract đã chấm dứt, reserved balance landlord thay đổi đúng.

### TC-04 - Sau khi kết thúc hợp đồng, tenant vẫn bị chặn thuê nếu chưa có hoặc chưa thanh toán invoice kỳ cuối

User: `Lê Quang Linh`

Điều kiện đầu vào:

- Contract `KFC-101` đã kết thúc.
- Landlord chưa tạo invoice kỳ cuối hoặc invoice kỳ cuối chưa paid.

Các bước:

1. Tenant mở `Khu trọ KFC Riverside`.
2. Chọn phòng `KFC-102`.
3. Tạo yêu cầu thuê từ `10/07/2026` đến `10/07/2027`.
4. Bấm gửi yêu cầu.

Kết quả mong đợi:

- Tenant vẫn bị chặn.
- Lý do chặn phải liên quan tới nghĩa vụ cuối hợp đồng cũ.
- Không tạo rental request mới.

Pass khi hệ thống không cho thuê mới cho tới khi nghĩa vụ kỳ cuối được xử lý xong.

### TC-05 - Landlord tạo invoice kỳ cuối, tenant thấy nhưng chưa thanh toán nên vẫn bị chặn

User landlord: `Nguyễn Xuân Huấn`

User tenant: `Lê Quang Linh`

Điều kiện đầu vào:

- Contract `KFC-101` đã kết thúc.
- Tenant chưa được thuê mới vì còn thiếu invoice kỳ cuối.

Các bước:

1. Landlord đăng nhập.
2. Vào hợp đồng/phòng `KFC-101`.
3. Tạo invoice kỳ cuối cho tenant.
4. Tenant đăng nhập lại.
5. Tenant mở danh sách hóa đơn và xác nhận thấy invoice kỳ cuối.
6. Tenant chưa thanh toán invoice này.
7. Tenant thử thuê `KFC-102`.

Kết quả mong đợi:

- Invoice kỳ cuối được tạo thành công.
- Invoice kỳ cuối có trạng thái unpaid/pending.
- Tenant vẫn bị chặn thuê phòng mới.
- UI không được báo chung chung, cần chỉ ra còn invoice chưa thanh toán.

DB cần kiểm:

```sql
select invoice_no, status, billing_period_start, billing_period_end, total_amount
from invoices
where contract_id = '<KFC-101-CONTRACT-ID>'
order by created_at desc;
```

Pass khi invoice kỳ cuối tồn tại nhưng chưa paid và rental request mới vẫn không được tạo.

### TC-06 - Co-tenant có account vẫn thuê riêng bình thường sau khi hợp đồng cũ kết thúc

User: `Phan Văn Thành`

Điều kiện đầu vào:

- `Phan Văn Thành` từng là người ở cùng trong hợp đồng `KFC-101`.
- Invoice/nợ thuộc main tenant `Lê Quang Linh`, không thuộc `Phan Văn Thành`.
- `Phan Văn Thành` đã KYC approved.

Các bước:

1. Đăng nhập bằng `phan.van.thanh@example.com`.
2. Mở khu trọ có phòng trống.
3. Chọn phòng hợp lệ đang available.
4. Tạo yêu cầu thuê.

Kết quả mong đợi:

- Hệ thống cho tạo rental request.
- Không block do invoice của `Lê Quang Linh`.
- Rule nợ chỉ áp dụng cho người có nghĩa vụ thanh toán, không áp dụng nhầm sang co-tenant.

Pass khi rental request của `Phan Văn Thành` được tạo thành công.

### TC-07 - Tenant thanh toán invoice kỳ cuối rồi được thuê bình thường

User: `Lê Quang Linh`

Điều kiện đầu vào:

- Invoice tháng 5 đã paid.
- Invoice kỳ cuối đã được tạo nhưng chưa paid.
- Tenant đủ tiền trong ví.

Các bước:

1. Tenant mở danh sách hóa đơn.
2. Thanh toán invoice kỳ cuối.
3. Kiểm tra invoice kỳ cuối chuyển paid.
4. Mở phòng `KFC-102`.
5. Tạo rental request:
   - Ngày bắt đầu: `10/07/2026`
   - Ngày kết thúc: `10/07/2027`
   - Số người ở: 3
6. Gửi yêu cầu thuê.

Kết quả mong đợi:

- Invoice kỳ cuối paid.
- Rental request mới được tạo thành công.
- Request ở trạng thái chờ landlord xử lý.
- Tenant không còn bị chặn bởi hợp đồng cũ.

Pass khi request tới `KFC-102` được tạo.

### TC-08 - Landlord accept request, tenant thanh toán cọc, nhập occupant chưa KYC thì bị chặn

User tenant: `Lê Quang Linh`

User landlord: `Nguyễn Xuân Huấn`

Điều kiện đầu vào:

- Rental request thuê `KFC-102` đã được tạo ở TC-07.
- Phòng `KFC-102` còn khả dụng.
- User `Hoàng Phúc Nhật Quang` tồn tại nhưng chưa KYC approved.

Các bước:

1. Landlord đăng nhập.
2. Mở danh sách yêu cầu thuê.
3. Accept request của `Lê Quang Linh` cho phòng `KFC-102`.
4. Tenant đăng nhập.
5. Tenant thanh toán cọc `3.500.000 VND`.
6. Tenant nhập danh sách người ở có account `Hoàng Phúc Nhật Quang`.
7. Tenant gửi thông tin người ở hoặc tiếp tục bước ký hợp đồng.

Kết quả mong đợi:

- Landlord accept thành công.
- Tenant thanh toán cọc thành công.
- Khi nhập `Hoàng Phúc Nhật Quang`, hệ thống chặn vì account chưa KYC approved.
- Không cho hoàn tất occupant list/ký hợp đồng nếu còn occupant chưa KYC.
- UI báo rõ người ở nào chưa KYC.

Pass khi tenant bị chặn đúng tại bước occupant/KYC.

### TC-09 - Nhập lại occupant hợp lệ, landlord ký, tenant ký

User tenant: `Lê Quang Linh`

User landlord: `Nguyễn Xuân Huấn`

Điều kiện đầu vào:

- Request `KFC-102` đã được accept.
- Cọc đã thanh toán.
- Danh sách occupant trước đó bị chặn do có `Hoàng Phúc Nhật Quang`.

Các bước:

1. Tenant sửa danh sách người ở.
2. Bỏ `Hoàng Phúc Nhật Quang`.
3. Chỉ giữ occupant hợp lệ:
   - `Lê Quang Linh`
   - `Phan Văn Thành`, nếu cần người ở có account đã KYC
   - Hoặc `Nguyễn Hoàng Minh`, nếu test occupant không account có giấy tờ hợp lệ
4. Tenant gửi lại thông tin occupant.
5. Landlord ký hợp đồng.
6. Tenant ký hợp đồng.

Kết quả mong đợi:

- Occupant list hợp lệ được lưu.
- Contract được tạo hoặc cập nhật đúng.
- Có chữ ký landlord.
- Có chữ ký tenant.
- Contract chuyển sang trạng thái signed/active theo rule hiện tại.
- Phòng `KFC-102` chuyển sang trạng thái đang thuê hoặc được giữ theo contract.

DB cần kiểm:

```sql
select status, room_id, tenant_user_id
from contracts
where room_id = '<KFC-102-ID>'
order by created_at desc;

select signer_user_id, signer_role, signed_at
from contract_signatures
where contract_id = '<KFC-102-CONTRACT-ID>';
```

Pass khi đủ 2 chữ ký và contract đi đúng trạng thái.

### TC-10 - Landlord chấm dứt hợp đồng mới trước ngày vào ở, không tạo invoice kỳ cuối và hoàn cọc

User: `Nguyễn Xuân Huấn`

Điều kiện đầu vào:

- Contract mới của `KFC-102` đã ký.
- Ngày bắt đầu thuê là `10/07/2026`.
- Landlord chấm dứt ngay trước ngày vào ở.
- Tenant chưa phát sinh kỳ thuê thực tế.

Các bước:

1. Landlord mở contract mới của `KFC-102`.
2. Chọn chấm dứt hợp đồng.
3. Xác nhận lý do chấm dứt trước ngày vào ở.
4. Thực hiện hoàn cọc cho tenant.
5. Kiểm tra ví landlord.
6. Kiểm tra ví tenant.

Kết quả mong đợi:

- Contract mới được chấm dứt.
- Không tạo invoice kỳ cuối vì tenant chưa vào ở.
- Landlord hoàn lại cọc `3.500.000 VND`.
- Reserved balance của landlord liên quan tới contract mới về `0`.
- Tenant nhận lại tiền cọc.
- Có wallet transaction refund deposit.

DB cần kiểm:

```sql
select *
from invoices
where contract_id = '<KFC-102-CONTRACT-ID>'
order by created_at desc;

select balance, reserved_balance
from wallet_accounts
where user_id in ('<LANDLORD-ID>', '<TENANT-ID>');

select *
from wallet_transactions
where reference_type in ('Deposit', 'RoomDeposit', 'Contract')
order by created_at desc;
```

Pass khi không có final invoice cho contract chưa vào ở và dòng tiền hoàn cọc đúng.

## 6. Checklist kiểm tra dữ liệu sau khi chạy migration

### 6.1. Kiểm tra user

```sql
select email, display_name, status, onboarding_status
from users
where email in (
  'admin.hoasen@example.com',
  'nguyen.xuan.huan@example.com',
  'le.quang.linh@example.com',
  'phan.van.thanh@example.com',
  'hoang.phuc.nhat.quang@example.com'
);
```

### 6.2. Kiểm tra KYC

```sql
select u.email, k.status, k.ocr_full_name
from users u
left join kyc_verifications k on k.user_id = u.id
where u.email in (
  'nguyen.xuan.huan@example.com',
  'le.quang.linh@example.com',
  'phan.van.thanh@example.com',
  'hoang.phuc.nhat.quang@example.com'
);
```

Kỳ vọng:

- Landlord, main tenant, co-tenant có KYC approved.
- `Hoàng Phúc Nhật Quang` không có KYC approved.

### 6.3. Kiểm tra khu trọ và phòng

```sql
select name, approval_status, visibility_status
from rooming_houses
where name = 'Khu trọ KFC Riverside';

select r.room_number, r.status, r.max_occupants
from rooms r
join rooming_houses h on h.id = r.rooming_house_id
where h.name = 'Khu trọ KFC Riverside'
order by r.room_number;
```

Kỳ vọng:

- `KFC-101`: occupied.
- `KFC-102`: available.
- `KFC-201`: hidden/draft.

### 6.4. Kiểm tra contract seed sẵn

```sql
select c.id, c.status, c.lease_start_date, c.lease_end_date, c.monthly_rent, c.deposit_amount
from contracts c
join rooms r on r.id = c.room_id
where r.room_number = 'KFC-101';
```

### 6.5. Kiểm tra invoice

```sql
select invoice_no, billing_period_start, billing_period_end, total_amount, status
from invoices
where invoice_no like 'KFC-SCENARIO-%'
order by billing_period_start;
```

Kỳ vọng:

- Invoice tháng 4 paid.
- Invoice tháng 5 overdue/unpaid.

### 6.6. Kiểm tra wallet

```sql
select u.email, wa.balance, wa.reserved_balance
from wallet_accounts wa
join users u on u.id = wa.user_id
where u.email in (
  'nguyen.xuan.huan@example.com',
  'le.quang.linh@example.com',
  'phan.van.thanh@example.com',
  'hoang.phuc.nhat.quang@example.com'
);
```

Kỳ vọng:

- Tenant `Lê Quang Linh`: balance `50.000.000`.
- Landlord `Nguyễn Xuân Huấn`: balance `50.000.000`, reserved balance `3.500.000`.

## 7. Tiêu chí pass toàn bộ kịch bản

Kịch bản được xem là pass khi đủ các điều kiện:

1. Mock data KFC seed thành công, không đụng tên phòng/khu trọ cũ.
2. Tenant có nợ invoice bị chặn thuê mới.
3. Tenant có nợ invoice bị chặn chấm dứt hợp đồng.
4. Sau khi thanh toán invoice tháng 5, tenant chấm dứt được hợp đồng.
5. Tenant vẫn bị chặn thuê mới nếu invoice kỳ cuối chưa xử lý.
6. Co-tenant không bị block bởi nợ của main tenant.
7. Tenant thanh toán xong toàn bộ invoice thì thuê mới được.
8. Occupant có account nhưng chưa KYC bị chặn.
9. Occupant hợp lệ thì hai bên ký hợp đồng được.
10. Chấm dứt trước ngày vào ở không tạo invoice kỳ cuối và hoàn cọc đúng.

## 8. Ghi chú triển khai

- Migration mock data mới: `20260622113000_SeedKfcScenarioDataset`.
- Prefix dữ liệu: `KFC-SCENARIO`.
- Object key ảnh/tài liệu: `kfc-scenario/...`.
- Bộ dữ liệu này độc lập với seed `Hoa Sen`.
- Nếu phòng `KFC-102` đã bị dùng trong một lần test trước, cần rollback/reset lại migration KFC trước khi chạy lại full scenario từ đầu.
