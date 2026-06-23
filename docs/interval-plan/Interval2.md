**SMART RENTAL PLATFORM**

**INTERVAL 2 \- GIẢI THÍCH ERD CHÍNH VÀ BUSINESS LOGIC**

*Tài liệu giải thích chi tiết từng bảng, từng trường và luồng nghiệp vụ giữa các bảng*

| Dự án | Smart Rental Platform |
| :---- | :---- |
| **Phạm vi** | Interval 2 \- tích hợp nghiệp vụ của 5 người |
| **Nguồn thiết kế** | Project structure, ERD tổng quát, business rules Interval 1 và business rules Interval 2 |
| **Ngày tạo** | 01/06/2026 |
| **Mục tiêu** | Dùng làm database contract chung trước khi backend/frontend implement |

 

# **1\. Tổng quan phạm vi Interval 2**

ERD chính Interval 2 nối tiếp nền tảng Interval 1\. Các bảng users, KYC, rooming\_houses, rooms, room\_price\_tiers, amenities và property\_images vẫn là lõi để các module mới sử dụng. Interval 2 bổ sung luồng search public nâng cao, viewing appointment, rental request, deposit, contract signing, billing, invoice, wallet và PayOS top-up.

| Người | Module | Bảng chính | Kết quả nghiệp vụ |
| :---- | :---- | :---- | :---- |
| Người 1 | Public Search, Filter, AI Search, Google Map Detail | rooming\_houses, rooms, room\_price\_tiers, amenities, property\_images | Tenant/Guest tìm phòng public và xem detail map. |
| Người 2 | Viewing Appointment | viewing\_appointments | Tenant đặt lịch; landlord confirm/reject/cancel/complete. |
| Người 3 | Rental Request, Deposit, Contract, OTP Signing | rental\_requests, room\_deposits, contracts, contract\_occupants, contract\_signatures | Yêu cầu thuê \-\> cọc 2 tiếng \-\> hợp đồng Draft \-\> ký OTP \-\> Active. |
| Người 4 | Billing, Meter Reading, Invoice | billing\_service\_types, rooming\_house\_service\_prices, meter\_readings, invoices, invoice\_items, invoice\_payments | Tạo hóa đơn tiền phòng, điện, nước, wifi, rác và thanh toán bằng ví. |
| Người 5 | Wallet, PayOS, Mock Payment | wallet\_accounts, wallet\_transactions, payment\_transactions, payment\_webhook\_logs | Nạp ví PayOS/Mock; thanh toán cọc/hóa đơn bằng ví; audit tiền. |

# **2\. Nguyên tắc nghiệp vụ và thiết kế dữ liệu**

·         Public search chỉ hiển thị khu trọ Approved, Visible, chưa xóa mềm và phòng Available.  
·         AI Search Parser chỉ parse câu tự nhiên thành filter JSON; không query database trực tiếp và không tự quyết định phòng trả về.  
·         Lịch xem phòng là module độc lập; trùng lịch chỉ cảnh báo landlord trước khi confirm, không chặn nếu landlord xác nhận tiếp.  
·         Yêu cầu thuê chỉ tạo được cho phòng Available. Khi landlord approve, hệ thống tạo room\_deposit WaitingPayment với deadline 2 tiếng.  
·         Cọc thanh toán bằng ví: tenant debit, landlord credit, landlord reserved\_balance tăng. Đây là tiền landlord đang giữ, chưa được rút tự do.  
·         Hợp đồng chỉ sửa được trước khi MainTenant ký. Sau khi MainTenant ký, hợp đồng, người ở và giấy tờ không sửa trực tiếp trong MVP.  
·         OTP ký hợp đồng lưu trong user\_tokens dạng hash, gắn đúng user và contract, hết hạn sau 5 phút và chỉ dùng một lần.  
·         Giá thuê phòng được chốt vào contracts.monthly\_rent. Giá điện/nước/wifi/rác không lưu trong hợp đồng, mà lấy từ rooming\_house\_service\_prices khi tạo hóa đơn.  
·         Invoice đã tạo snapshot đơn giá vào invoice\_items.unit\_price; bảng giá khu trọ đổi sau đó không làm đổi hóa đơn cũ.  
·         PayOS/Mock chỉ dùng để nạp tiền vào ví. Cọc và hóa đơn không gọi PayOS trực tiếp mà dùng số dư ví.  
·         Mọi thao tác tiền phải chạy trong database transaction, lock dữ liệu cần thiết và chống double payment.

# **3\. Giải thích chi tiết từng bảng và từng trường**

Mỗi bảng bên dưới gồm mục đích, cách liên kết nghiệp vụ và danh sách trường quan trọng. Required/PK/FK/unique có thể được tinh chỉnh khi tạo migration, nhưng không nên đổi ý nghĩa nghiệp vụ nếu chưa review chéo.

## **A. Nền tảng người dùng, xác thực và dữ liệu chung**

### **users**

**Mục đích:** Lưu tài khoản đăng nhập của toàn bộ actor: Tenant, Landlord, Admin.

**Logic liên quan:** Là bảng gốc cho hầu hết module. User phải đăng nhập để đặt lịch, gửi yêu cầu thuê, ký hợp đồng, nạp ví và thanh toán.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính của user. |
| email | varchar | Required, unique | Email đăng nhập gốc. |
| normalized\_email | varchar | Required, unique | Email chuẩn hóa để check trùng không phân biệt hoa thường. |
| phone\_number | varchar | Nullable | Số điện thoại liên hệ. |
| password\_hash | text | Nullable | Hash mật khẩu local; nullable nếu user đăng nhập Google. |
| display\_name | varchar | Required | Tên hiển thị trong hệ thống. |
| avatar\_url | text | Nullable | URL avatar public. |
| status | enum/string | Required | Active, Banned, Deleted... dùng để chặn login/hành động. |
| onboarding\_status | enum/string | Required | NeedProfileUpdate, KycPending, Completed... dùng điều hướng user. |
| email\_confirmed | boolean | Required | Đã xác thực email OTP hay chưa. |
| phone\_confirmed | boolean | Required | Đã xác thực số điện thoại hay chưa. |
| access\_failed\_count | int | Required | Số lần đăng nhập sai để lock account. |
| lockout\_end\_at | datetime | Nullable | Thời điểm hết khóa tài khoản. |
| last\_login\_at | datetime | Nullable | Lần đăng nhập gần nhất. |
| created\_at / updated\_at / deleted\_at | datetime | Audit | Ngày tạo, cập nhật và xóa mềm. |

 

### **user\_profiles**

**Mục đích:** Lưu hồ sơ cá nhân bổ sung của user.

**Logic liên quan:** Thông tin định danh có thể được sync từ KYC Approved. MainTenant cần hồ sơ/KYC hợp lệ trước khi ký hợp đồng.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| user\_id | uuid | PK, FK users.id | Mỗi user có tối đa một profile. |
| full\_name | varchar | Nullable | Họ tên thật, ưu tiên sync từ OCR KYC. |
| date\_of\_birth | date | Nullable | Ngày sinh. |
| gender | varchar | Nullable | Giới tính. |
| address\_line | text | Nullable | Địa chỉ thường trú hoặc liên hệ. |
| verified\_citizen\_id\_masked | varchar | Nullable | Số CCCD đã mask sau khi KYC approved. |
| emergency\_contact\_name | varchar | Nullable | Người liên hệ khẩn cấp. |
| emergency\_contact\_phone | varchar | Nullable | SĐT liên hệ khẩn cấp. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo và cập nhật profile. |

 

### **roles**

**Mục đích:** Danh mục vai trò hệ thống.

**Logic liên quan:** Dùng phân quyền API và UI: Tenant, Landlord, Admin.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | int | PK | Khóa chính role. |
| name | enum/string | Required, unique | Tên role: Admin, Tenant, Landlord. |
| description | text | Nullable | Mô tả vai trò. |
| created\_at | datetime | Audit | Ngày tạo role seed. |

 

### **user\_roles**

**Mục đích:** Bảng nối user \- role.

**Logic liên quan:** User đăng ký mặc định Tenant. Landlord được cấp sau khi đủ điều kiện/KYC và khu trọ được duyệt theo rule Interval 1\.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| user\_id | uuid | PK, FK users.id | User được gán role. |
| role\_id | int | PK, FK roles.id | Role được gán. |
| created\_at | datetime | Audit | Thời điểm gán role. |

 

### **external\_logins**

**Mục đích:** Lưu thông tin đăng nhập ngoài như Google.

**Logic liên quan:** Cho phép user đăng nhập Google nhưng vẫn link về users để dùng chung KYC, ví, hợp đồng.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính external login. |
| user\_id | uuid | FK users.id | User nội bộ được link. |
| provider | string | Required | Google hoặc provider khác. |
| provider\_user\_id | string | Required | ID user ở provider. |
| provider\_email | string | Nullable | Email từ provider. |
| provider\_display\_name | string | Nullable | Tên hiển thị từ provider. |
| provider\_avatar\_url | text | Nullable | Avatar từ provider. |
| created\_at / last\_login\_at | datetime | Audit | Thời điểm tạo và lần login gần nhất. |

 

### **user\_tokens**

**Mục đích:** Lưu các token stateful: refresh token, OTP email, reset password, OTP ký hợp đồng.

**Logic liên quan:** Interval 2 dùng thêm token\_type ContractSignatureOtp, gắn related\_entity\_type \= Contract và related\_entity\_id \= contracts.id.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính token. |
| user\_id | uuid | FK users.id | Chủ sở hữu token. |
| token\_type | enum/string | Required | RefreshToken, EmailVerification, PasswordReset, ContractSignatureOtp... |
| token\_hash | text | Required, unique | Hash token/OTP, không lưu raw token. |
| token\_family\_id | uuid | Nullable | Nhóm refresh token để revoke cả family khi phát hiện reuse. |
| replaced\_by\_token\_id | uuid | FK user\_tokens.id | Token mới thay thế token cũ khi rotation. |
| related\_entity\_type | string | Nullable | Loại entity liên quan, ví dụ Contract. |
| related\_entity\_id | uuid | Nullable | ID entity liên quan, ví dụ contracts.id. |
| expires\_at | datetime | Required | Thời điểm hết hạn. |
| used\_at | datetime | Nullable | Thời điểm token được dùng. |
| revoked\_at / revoked\_reason | datetime/string | Nullable | Thông tin thu hồi token. |
| created\_by\_ip / user\_agent | string/text | Nullable | Thông tin môi trường tạo token. |
| created\_at | datetime | Audit | Ngày tạo token. |

 

### **login\_logs**

**Mục đích:** Lưu lịch sử đăng nhập thành công/thất bại.

**Logic liên quan:** Dùng audit bảo mật, lock account, điều tra lỗi login.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính log. |
| user\_id | uuid | FK users.id, nullable | Có thể null nếu login sai email chưa map user. |
| login\_provider | string | Required | Local, Google... |
| email\_attempted | string | Nullable | Email user đã nhập khi login. |
| ip\_address | string | Nullable | IP đăng nhập. |
| user\_agent | text | Nullable | Thiết bị/trình duyệt. |
| is\_success | boolean | Required | Kết quả login. |
| failure\_reason | string | Nullable | Lý do thất bại. |
| created\_at | datetime | Audit | Thời điểm login. |

 

### **kyc\_verifications**

**Mục đích:** Lưu hồ sơ eKYC/KYC của user.

**Logic liên quan:** Điều kiện quan trọng cho top-up ví PayOS và ký hợp đồng. Kết quả KYC approved cho phép user nạp ví.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính hồ sơ KYC. |
| user\_id | uuid | FK users.id | User nộp KYC. |
| document\_type | string | Required | CCCD/CMND/passport... |
| ekyc\_provider / ekyc\_session\_id | string | Nullable | Thông tin provider VNPT/mock. |
| front\_image\_object\_key | text | Required | Ảnh mặt trước giấy tờ trong private storage. |
| back\_image\_object\_key | text | Required | Ảnh mặt sau giấy tờ trong private storage. |
| selfie\_image\_object\_key | text | Required | Ảnh selfie/liveness trong private storage. |
| ocr\_full\_name | string | Nullable | Họ tên OCR trả về. |
| ocr\_citizen\_id\_masked | string | Nullable | Số giấy tờ đã mask. |
| citizen\_id\_hash | text | Nullable, indexed | Hash CCCD để chống trùng giữa account. |
| ocr\_date\_of\_birth / ocr\_gender / ocr\_address | date/string/text | Nullable | Dữ liệu OCR snapshot. |
| ocr\_confidence / face\_match\_score | decimal | Nullable | Điểm tin cậy OCR và face matching. |
| liveness\_result | string | Nullable | Kết quả kiểm tra người thật. |
| risk\_level | string | Nullable | Low, Medium, High để admin review. |
| status | string | Required | PendingAdminReview, Approved, Rejected, EkycFailed... |
| reviewed\_by\_admin\_id | uuid | FK users.id, nullable | Admin duyệt/từ chối KYC. |
| rejected\_reason | text | Nullable | Lý do từ chối. |
| submitted\_at / reviewed\_at | datetime | Audit | Thời điểm nộp và duyệt. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **provinces**

**Mục đích:** Danh mục tỉnh/thành.

**Logic liên quan:** Dùng cho địa chỉ khu trọ, filter public search theo khu vực.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| code | string | PK | Mã tỉnh/thành. |
| name | string | Required | Tên tỉnh/thành. |
| type | string | Required | Province/City... |
| is\_active | boolean | Required | Chỉ dùng địa giới active. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **wards**

**Mục đích:** Danh mục phường/xã.

**Logic liên quan:** Dùng cho địa chỉ khu trọ và filter. ward\_code phải thuộc province\_code nếu truyền cả hai.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| code | string | PK | Mã phường/xã. |
| province\_code | string | FK provinces.code | Tỉnh/thành chứa phường/xã. |
| name | string | Required | Tên phường/xã. |
| type | string | Required | Ward/Commune... |
| is\_active | boolean | Required | Chỉ dùng ward active. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **notifications**

**Mục đích:** Lưu thông báo trong hệ thống.

**Logic liên quan:** Dùng optional cho lịch xem phòng, yêu cầu thuê, cọc, hợp đồng, hóa đơn đổi trạng thái.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính thông báo. |
| recipient\_user\_id | uuid | FK users.id | Người nhận thông báo. |
| actor\_user\_id | uuid | FK users.id, nullable | Người tạo hành động nếu có. |
| type | string | Required | Loại thông báo: Viewing, Deposit, Contract, Invoice... |
| title | string | Required | Tiêu đề thông báo. |
| message | text | Required | Nội dung thông báo. |
| entity\_type / entity\_id | string/uuid | Nullable | Entity liên quan để điều hướng. |
| action\_url | text | Nullable | Link frontend cần mở. |
| priority | string | Required | Normal, High... |
| status | string | Required | Unread, Read, Archived... |
| read\_at | datetime | Nullable | Thời điểm đọc. |
| created\_at | datetime | Audit | Thời điểm tạo. |

 

### **approval\_audit\_logs**

**Mục đích:** Audit các hành động duyệt KYC/khu trọ/tài liệu.

**Logic liên quan:** Giữ dấu vết admin approve/reject từ Interval 1; không phải bảng nghiệp vụ tiền.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính audit. |
| admin\_id | uuid | FK users.id | Admin thực hiện hành động. |
| approval\_type | string | Required | KycVerification, RoomingHouse, LegalDocument... |
| entity\_id | uuid | Required | ID entity được duyệt. |
| action | string | Required | Approve, Reject, Resubmit... |
| reason | text | Nullable | Lý do hoặc ghi chú. |
| additional\_info | text/json | Nullable | Thông tin bổ sung nếu cần. |
| created\_at | datetime | Audit | Thời điểm ghi audit. |

 

## **B. Nhà trọ, phòng, public search và Google Map**

### **rooming\_houses**

**Mục đích:** Lưu thông tin khu trọ của landlord.

**Logic liên quan:** Public search chỉ lấy khu trọ Approved, Visible và chưa xóa mềm. Detail dùng latitude/longitude để hiển thị Google Map 1 marker.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính khu trọ. |
| landlord\_user\_id | uuid | FK users.id | Chủ trọ sở hữu khu trọ. |
| name | string | Required | Tên khu trọ. |
| description | text | Nullable | Mô tả public. |
| address\_line | text | Required | Địa chỉ chi tiết landlord nhập. |
| ward\_code | string | FK wards.code | Phường/xã. |
| province\_code | string | FK provinces.code | Tỉnh/thành. |
| address\_display | text | Required | Địa chỉ hiển thị public đã build từ address \+ ward \+ province. |
| latitude / longitude | decimal | Nullable | Tọa độ Google Map; không có tọa độ vẫn hiển thị detail bình thường. |
| approval\_status | string | Required | Draft, Pending, Approved, Rejected. |
| visibility\_status | string | Required | Hidden, Visible. |
| rejected\_reason | text | Nullable | Lý do admin reject. |
| reviewed\_by\_admin\_id / reviewed\_at | uuid/datetime | Nullable | Admin và thời điểm duyệt. |
| created\_at / updated\_at / deleted\_at | datetime | Audit | Ngày tạo, cập nhật, xóa mềm. |

 

### **rooming\_house\_legal\_documents**

**Mục đích:** Lưu giấy tờ pháp lý của khu trọ.

**Logic liên quan:** Chỉ landlord/admin có quyền xem. Không trả ra public search/detail.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính giấy tờ. |
| rooming\_house\_id | uuid | FK rooming\_houses.id | Khu trọ liên quan. |
| document\_type | string | Required | Loại giấy tờ pháp lý. |
| front\_image\_object\_key | text | Required | Ảnh mặt trước private storage. |
| back\_image\_object\_key | text | Required | Ảnh mặt sau private storage. |
| extra\_image\_object\_key | text | Nullable | Ảnh bổ sung nếu có. |
| document\_number\_masked | string | Nullable | Số giấy tờ đã mask. |
| document\_number\_hash | text | Nullable | Hash số giấy tờ để check trùng/audit. |
| status | string | Required | Pending, Approved, Rejected... |
| reviewed\_by\_admin\_id / reviewed\_at | uuid/datetime | Nullable | Admin review. |
| rejected\_reason | text | Nullable | Lý do từ chối. |
| uploaded\_at / created\_at / updated\_at | datetime | Audit | Thời điểm upload/tạo/cập nhật. |

 

### **rooms**

**Mục đích:** Lưu từng phòng trong khu trọ.

**Logic liên quan:** Public search chỉ hiển thị phòng Available, chưa xóa mềm. Người 3 chuyển phòng Reserved khi cọc paid và Occupied khi contract Active.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính phòng. |
| rooming\_house\_id | uuid | FK rooming\_houses.id | Khu trọ chứa phòng. |
| room\_number | string | Required | Số/tên phòng. |
| floor | int | Required | Tầng. |
| area\_m2 | decimal | Nullable | Diện tích phòng. |
| max\_occupants | int | Required | Số người tối đa. |
| is\_tiered\_pricing | boolean | Required | Có dùng giá theo số người hay không. |
| status | string | Required | Hidden, Available, Reserved, Occupied, Maintenance. |
| description | text | Nullable | Mô tả phòng. |
| created\_at / updated\_at / deleted\_at | datetime | Audit | Ngày tạo, cập nhật, xóa mềm. |

 

### **room\_price\_tiers**

**Mục đích:** Lưu giá thuê phòng theo số người.

**Logic liên quan:** Search/filter giá phải lấy từ bảng này. Khi tạo rental request/contract, giá được snapshot sang rental\_requests và contracts.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính tier giá. |
| room\_id | uuid | FK rooms.id | Phòng áp dụng giá. |
| occupant\_count | int | Required | Số người tương ứng với mức giá. |
| monthly\_rent | decimal | Required | Tiền thuê tháng. |
| is\_active | boolean | Required | Chỉ tier active được dùng cho search/tạo yêu cầu thuê. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **amenities**

**Mục đích:** Danh mục tiện ích.

**Logic liên quan:** Search tiện ích kiểm tra cả cấp phòng và khu trọ. AI parser map amenity name về amenity id.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính tiện ích. |
| name | string | Required | Tên tiện ích: Wifi, Máy lạnh... |
| scope | string | Required | House, Room, Both. |
| icon\_code | string | Nullable | Mã icon frontend. |
| is\_active | boolean | Required | Chỉ tiện ích active được public/filter. |
| created\_at | datetime | Audit | Ngày tạo seed. |

 

### **room\_amenities**

**Mục đích:** Bảng nối phòng \- tiện ích.

**Logic liên quan:** Phòng match tiện ích nếu có amenity trong bảng này.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| room\_id | uuid | PK, FK rooms.id | Phòng có tiện ích. |
| amenity\_id | uuid | PK, FK amenities.id | Tiện ích được gán. |

 

### **rooming\_house\_amenities**

**Mục đích:** Bảng nối khu trọ \- tiện ích.

**Logic liên quan:** Khu trọ match tiện ích nếu có amenity trong bảng này.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| rooming\_house\_id | uuid | PK, FK rooming\_houses.id | Khu trọ có tiện ích. |
| amenity\_id | uuid | PK, FK amenities.id | Tiện ích được gán. |

 

### **property\_images**

**Mục đích:** Lưu ảnh public của khu trọ hoặc phòng.

**Logic liên quan:** Search result ưu tiên cover phòng, nếu không có thì cover khu trọ; không lưu ảnh giấy tờ private.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính ảnh. |
| rooming\_house\_id | uuid | FK nullable | Ảnh thuộc khu trọ nếu có. |
| room\_id | uuid | FK nullable | Ảnh thuộc phòng nếu có. |
| object\_key | text | Required | Key file trong storage. |
| image\_url | text | Required | URL public để frontend hiển thị. |
| caption | string | Nullable | Chú thích ảnh. |
| is\_cover | boolean | Required | Có phải ảnh cover không. |
| sort\_order | int | Required | Thứ tự hiển thị. |
| created\_at | datetime | Audit | Ngày tạo. |

 

### **rental\_policies**

**Mục đích:** Lưu chính sách thuê/cọc/gia hạn ở cấp khu trọ.

**Logic liên quan:** Người 3 dùng deposit\_months để tính deposit\_amount\_snapshot. Detail public có thể hiển thị chính sách thuê.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính chính sách. |
| rooming\_house\_id | uuid | FK rooming\_houses.id | Khu trọ áp dụng chính sách. |
| min\_rental\_months | int | Nullable | Số tháng thuê tối thiểu. |
| max\_rental\_months | int | Nullable | Số tháng thuê tối đa nếu có. |
| allow\_short\_term\_renewal | boolean | Required | Cho phép gia hạn ngắn hạn hay không. |
| renewal\_notice\_days | int | Required | Số ngày báo trước khi gia hạn. |
| deposit\_months | decimal | Required | Số tháng tiền cọc. |
| is\_active | boolean | Required | Chính sách đang áp dụng. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

## **C. Viewing Appointment \- Người 2**

### **viewing\_appointments**

**Mục đích:** Lưu lịch hẹn xem phòng.

**Logic liên quan:** Tenant tạo lịch Pending. Landlord confirm/reject/cancel/complete. Check trùng lịch chỉ warning, không chặn confirm nếu landlord chọn confirmDespiteConflict.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính lịch xem phòng. |
| room\_id | uuid | FK rooms.id | Phòng được hẹn xem. |
| tenant\_user\_id | uuid | FK users.id | Tenant đặt lịch. |
| created\_by\_user\_id | uuid | FK users.id | User tạo lịch; thường bằng tenant\_user\_id. |
| scheduled\_at | datetime | Required | Thời điểm xem phòng; không được ở quá khứ khi tạo. |
| duration\_minutes | int | Required | Thời lượng, mặc định 30 phút. |
| status | string | Required | Pending, Confirmed, Rejected, CancelledByTenant, CancelledByLandlord, Completed, Expired. |
| tenant\_note | text | Nullable | Ghi chú tenant gửi landlord. |
| landlord\_note | text | Nullable | Ghi chú landlord khi confirm. |
| cancel\_reason | text | Nullable | Lý do hủy/từ chối. |
| responded\_at | datetime | Nullable | Thời điểm landlord phản hồi. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

## **D. Rental Request, Deposit, Contract và OTP Signing \- Người 3**

### **rental\_requests**

**Mục đích:** Lưu yêu cầu thuê phòng của tenant.

**Logic liên quan:** Sau lịch xem phòng hoặc detail, tenant gửi yêu cầu thuê. Landlord approve tạo room\_deposit WaitingPayment với deadline 2h.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính yêu cầu thuê. |
| room\_id | uuid | FK rooms.id | Phòng tenant muốn thuê. |
| tenant\_user\_id | uuid | FK users.id | Tenant gửi yêu cầu. |
| approved\_by\_landlord\_id | uuid | FK users.id, nullable | Landlord approve yêu cầu. |
| desired\_start\_date | date | Required | Ngày tenant muốn bắt đầu thuê. |
| expected\_end\_date | date | Nullable | Ngày dự kiến kết thúc nếu có. |
| expected\_occupant\_count | int | Required | Số người dự kiến ở, không vượt max\_occupants. |
| monthly\_rent\_snapshot | decimal | Required | Giá thuê chốt tại thời điểm gửi/approve yêu cầu. |
| deposit\_amount\_snapshot | decimal | Required | Tiền cọc chốt từ chính sách đang active. |
| tenant\_note | text | Nullable | Ghi chú tenant. |
| status | string | Required | PendingLandlordApproval, RejectedByLandlord, WaitingDepositPayment, DepositExpired, DepositPaid, ContractDrafting, ContractSigning, Completed, Cancelled. |
| responded\_at | datetime | Nullable | Thời điểm landlord phản hồi. |
| rejected\_reason | text | Nullable | Lý do landlord từ chối. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **room\_deposits**

**Mục đích:** Lưu khoản cọc của yêu cầu thuê.

**Logic liên quan:** Cọc có hạn thanh toán 2 tiếng. Thanh toán cọc bằng ví sẽ debit tenant, credit landlord và tăng reserved\_balance landlord.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính khoản cọc. |
| rental\_request\_id | uuid | FK rental\_requests.id | Yêu cầu thuê tạo ra khoản cọc. |
| room\_id | uuid | FK rooms.id | Phòng liên quan. |
| tenant\_user\_id | uuid | FK users.id | Người trả cọc. |
| landlord\_user\_id | uuid | FK users.id | Người nhận cọc. |
| deposit\_amount | decimal | Required | Số tiền cọc phải thanh toán. |
| currency | string | Required | VND. |
| status | string | Required | WaitingPayment, Paid, Expired, WaitingLandlordDecision, Refunded, Forfeited, Cancelled. |
| payment\_deadline\_at | datetime | Required | Deadline thanh toán cọc \= now \+ 2 hours. |
| paid\_at | datetime | Nullable | Thời điểm thanh toán thành công. |
| refunded\_at / forfeited\_at | datetime | Nullable | Thời điểm hoàn/giữ cọc. |
| refund\_amount / forfeited\_amount | decimal | Nullable | Số tiền hoàn hoặc giữ. |
| deposit\_decision\_by\_landlord\_id | uuid | FK users.id, nullable | Landlord quyết định hoàn/giữ cọc khi MainTenant quá hạn ký. |
| deposit\_decision\_at | datetime | Nullable | Thời điểm ra quyết định. |
| deposit\_decision\_reason | text | Nullable | Lý do giữ/hoàn cọc. |
| note | text | Nullable | Ghi chú nội bộ. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **contracts**

**Mục đích:** Lưu hợp đồng thuê.

**Logic liên quan:** Tạo Draft sau khi cọc Paid. MainTenant ký trước bằng OTP, Landlord ký sau bằng OTP. Hai bên ký xong thì Active và room Occupied.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính hợp đồng. |
| rental\_request\_id | uuid | FK rental\_requests.id | Yêu cầu thuê nguồn. |
| room\_deposit\_id | uuid | FK room\_deposits.id | Khoản cọc bảo đảm hợp đồng. |
| room\_id | uuid | FK rooms.id | Phòng được thuê. |
| main\_tenant\_user\_id | uuid | FK users.id | Tenant chính đứng tên hợp đồng. |
| contract\_number | string | Required, unique | Mã hợp đồng. |
| start\_date / end\_date | date | Required | Thời hạn hợp đồng. |
| monthly\_rent | decimal | Required | Tiền thuê tháng chốt vào hợp đồng. |
| deposit\_amount | decimal | Required | Tiền cọc chốt vào hợp đồng. |
| payment\_day | int | Required | Ngày thanh toán hàng tháng, nên từ 1 đến 28\. |
| status | string | Required | Draft, PendingMainTenantSignature, PendingLandlordSignature, Active, Rejected, Cancelled, SignatureExpired. |
| room\_snapshot | json | Required | Snapshot thông tin phòng tại thời điểm tạo hợp đồng. |
| draft\_completed\_at | datetime | Nullable | Thời điểm MainTenant hoàn thành draft. |
| main\_tenant\_signature\_deadline\_at | datetime | Nullable | Deadline ký của MainTenant, bắt đầu sau complete draft. |
| main\_tenant\_signed\_at | datetime | Nullable | Thời điểm MainTenant ký. |
| landlord\_signature\_deadline\_at | datetime | Nullable | Deadline ký của Landlord, bắt đầu sau MainTenant ký. |
| landlord\_signed\_at | datetime | Nullable | Thời điểm Landlord ký. |
| activated\_at | datetime | Nullable | Thời điểm contract Active. |
| signature\_expired\_at / signature\_expired\_reason | datetime/string | Nullable | Thông tin quá hạn ký. |
| rejected\_reason | text | Nullable | Lý do từ chối nếu có. |
| created\_at / updated\_at / deleted\_at | datetime | Audit | Ngày tạo/cập nhật/xóa mềm. |

 

### **contract\_occupants**

**Mục đích:** Lưu danh sách người ở trong hợp đồng.

**Logic liên quan:** Mỗi contract có đúng một MainTenant. CoTenant/Dependent không ký hợp đồng trong MVP. Không sửa sau khi MainTenant đã ký.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính người ở. |
| contract\_id | uuid | FK contracts.id | Hợp đồng chứa người ở. |
| user\_id | uuid | FK users.id, nullable | Tài khoản hệ thống nếu người ở có account. |
| guardian\_occupant\_id | uuid | Self FK nullable | Người giám hộ cho dependent nếu cần. |
| kyc\_verification\_id | uuid | FK kyc\_verifications.id, nullable | KYC dùng để auto-fill/xác minh. |
| email | string | Nullable | Email người ở. |
| full\_name | string | Required | Họ tên người ở. |
| phone\_number | string | Nullable | Số điện thoại. |
| date\_of\_birth | date | Nullable | Ngày sinh. |
| gender | string | Nullable | Giới tính. |
| residence\_role | string | Required | MainTenant, CoTenant, Dependent. |
| relationship\_to\_main\_tenant | string | Nullable | Quan hệ với tenant chính. |
| is\_ekyc\_verified | boolean | Required | Đã eKYC hợp lệ hay chưa. |
| document\_type | string | Nullable | Loại giấy tờ. |
| document\_number\_masked / document\_number\_hash | string/text | Nullable | Số giấy tờ đã mask/hash. |
| document\_verification\_status | string | Required | Pending, Approved, Rejected... |
| move\_in\_date / move\_out\_date | date | Nullable | Ngày vào/ra nếu có. |
| status | string | Required | Active, PendingReview, Rejected, MovedOut... |
| entered\_by\_user\_id / entered\_at | uuid/datetime | Audit | Người nhập và thời điểm nhập. |
| reviewed\_by\_landlord\_id / reviewed\_at | uuid/datetime | Nullable | Landlord review thông tin người ở. |
| rejected\_reason | text | Nullable | Lý do reject thông tin/giấy tờ. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **contract\_occupant\_documents**

**Mục đích:** Lưu file giấy tờ của người ở.

**Logic liên quan:** Lưu private object key, không public URL cố định. Tenant khác không được xem.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính tài liệu. |
| contract\_occupant\_id | uuid | FK contract\_occupants.id | Người ở sở hữu giấy tờ. |
| document\_type | string | Required | CCCD, BirthCertificate, Other... |
| document\_file\_type | string | Required | Front, Back, Extra, Supplement... |
| file\_object\_key | text | Required | Key file trong private storage. |
| uploaded\_by\_user\_id | uuid | FK users.id | Người upload, thường là MainTenant. |
| status | string | Required | Pending, Approved, Rejected. |
| reviewed\_by\_landlord\_id / reviewed\_at | uuid/datetime | Nullable | Landlord review tài liệu. |
| rejected\_reason | text | Nullable | Lý do reject. |
| uploaded\_at | datetime | Audit | Thời điểm upload. |

 

### **contract\_files**

**Mục đích:** Lưu file PDF hợp đồng theo version.

**Logic liên quan:** DraftPdf tạo khi hoàn thành draft; FinalSignedPdf tạo sau khi Landlord ký.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính file hợp đồng. |
| contract\_id | uuid | FK contracts.id | Hợp đồng chứa file. |
| file\_type | string | Required | DraftPdf, MainTenantSignedPdf, FinalSignedPdf. |
| file\_version | int | Required | Version file. |
| storage\_object\_key | text | Required | Key file trong storage. |
| file\_url | text | Nullable | URL xem/tải nếu được phép. |
| file\_hash | text | Required | Hash nội dung để kiểm tra không bị thay đổi. |
| generated\_at / created\_at | datetime | Audit | Thời điểm sinh file/tạo record. |

 

### **contract\_signatures**

**Mục đích:** Lưu chữ ký OTP của MainTenant và Landlord.

**Logic liên quan:** Mỗi bên chỉ có một chữ ký hợp lệ. contract\_snapshot\_hash gắn nội dung hợp đồng tại thời điểm ký.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính chữ ký. |
| contract\_id | uuid | FK contracts.id | Hợp đồng được ký. |
| signer\_user\_id | uuid | FK users.id | Người ký. |
| signer\_role | string | Required | MainTenant hoặc Landlord. |
| signature\_method | string | Required | Otp trong MVP. |
| otp\_token\_id | uuid | FK user\_tokens.id | OTP token đã verify. |
| contract\_snapshot\_hash | text | Required | Hash snapshot hợp đồng tại thời điểm ký. |
| signed\_at | datetime | Required | Thời điểm ký. |
| created\_at | datetime | Audit | Ngày tạo record. |

 

## **E. Billing, Meter Reading và Invoice \- Người 4**

### **billing\_service\_types**

**Mục đích:** Danh mục loại dịch vụ tính hóa đơn.

**Logic liên quan:** Scope Người 4 chỉ gồm Electric, Water, Wifi, Trash; không có gửi xe.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính service type. |
| code | string | Required, unique | Electric, Water, Wifi, Trash. |
| name | string | Required | Tên hiển thị dịch vụ. |
| is\_metered | boolean | Required | Electric/Water \= true; Wifi/Trash \= false. |
| is\_active | boolean | Required | Chỉ service active được dùng. |
| created\_at | datetime | Audit | Ngày tạo seed. |

 

### **rooming\_house\_service\_prices**

**Mục đích:** Lưu bảng giá dịch vụ ở cấp khu trọ.

**Logic liên quan:** Khi đổi giá tạo dòng mới, không sửa đè dòng cũ. Hóa đơn lấy giá hiện hành theo billing\_period\_end.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính bảng giá. |
| rooming\_house\_id | uuid | FK rooming\_houses.id | Khu trọ áp dụng giá. |
| service\_type\_id | uuid | FK billing\_service\_types.id | Loại dịch vụ. |
| billing\_method | string | Required | Metered, PerRoom... |
| unit\_name | string | Required | kWh, m3, room... |
| unit\_price | decimal | Required | Đơn giá, \>= 0\. |
| effective\_from | date | Required | Ngày bắt đầu áp dụng. |
| effective\_to | date | Nullable | Ngày kết thúc; null là đang mở. |
| is\_active | boolean | Required | Còn active hay không. |
| note | text | Nullable | Ghi chú giá. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **meter\_readings**

**Mục đích:** Lưu chỉ số điện/nước theo kỳ.

**Logic liên quan:** Chỉ Electric/Water cần meter\_readings. consumption \= current\_reading \- previous\_reading. UsedInInvoice thì không sửa trực tiếp.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính chỉ số. |
| room\_id | uuid | FK rooms.id | Phòng được ghi chỉ số. |
| contract\_id | uuid | FK contracts.id | Hợp đồng Active liên quan. |
| service\_type\_id | uuid | FK billing\_service\_types.id | Dịch vụ metered: Electric/Water. |
| billing\_period\_start / billing\_period\_end | date | Required | Kỳ hóa đơn. |
| previous\_reading | decimal | Required | Chỉ số đầu kỳ. |
| current\_reading | decimal | Required | Chỉ số cuối kỳ, \>= previous\_reading. |
| consumption | decimal | Required | Sản lượng tiêu thụ. |
| proof\_image\_object\_key | text | Nullable | Ảnh công tơ private/public tùy thiết kế. |
| status | string | Required | Draft, Confirmed, UsedInInvoice, Cancelled. |
| recorded\_by\_landlord\_user\_id | uuid | FK users.id | Landlord ghi chỉ số. |
| reading\_at | datetime | Required | Thời điểm đọc chỉ số. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **invoices**

**Mục đích:** Lưu hóa đơn tổng theo hợp đồng và kỳ thanh toán.

**Logic liên quan:** Tạo Draft từ contract Active, phát hành Issued, tenant thanh toán bằng ví để chuyển Paid.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính hóa đơn. |
| contract\_id | uuid | FK contracts.id | Hợp đồng được lập hóa đơn. |
| room\_id | uuid | FK rooms.id | Phòng được tính tiền. |
| tenant\_user\_id | uuid | FK users.id | Người nhận hóa đơn. |
| landlord\_user\_id | uuid | FK users.id | Người phát hành/nhận tiền. |
| invoice\_no | string | Required, unique | Số hóa đơn. |
| billing\_period\_start / billing\_period\_end | date | Required | Kỳ hóa đơn. |
| issue\_date | date | Nullable | Ngày phát hành. |
| due\_date | date | Required | Ngày đến hạn. |
| rent\_amount | decimal | Required | Tổng tiền phòng. |
| utility\_amount | decimal | Required | Tổng điện/nước. |
| service\_amount | decimal | Required | Tổng wifi/rác. |
| discount\_amount | decimal | Required | Giảm giá nếu có. |
| total\_amount | decimal | Required | Tổng phải trả. |
| paid\_amount | decimal | Required | Số tiền đã thanh toán. |
| remaining\_amount | decimal | Required | Số tiền còn lại. |
| status | string | Required | Draft, Issued, Overdue, Paid, Cancelled. |
| note | text | Nullable | Ghi chú hóa đơn. |
| sent\_at / paid\_at / cancelled\_at | datetime | Nullable | Các mốc phát hành/thanh toán/hủy. |
| cancel\_reason | text | Nullable | Lý do hủy hóa đơn. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **invoice\_items**

**Mục đích:** Lưu chi tiết từng dòng hóa đơn.

**Logic liên quan:** Đơn giá dịch vụ đã dùng phải snapshot vào unit\_price để bảng giá thay đổi sau này không làm đổi hóa đơn cũ.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính dòng hóa đơn. |
| invoice\_id | uuid | FK invoices.id | Hóa đơn chứa item. |
| service\_type\_id | uuid | FK billing\_service\_types.id, nullable | Loại dịch vụ nếu item là điện/nước/wifi/rác. |
| meter\_reading\_id | uuid | FK meter\_readings.id, nullable | Chỉ số dùng cho item điện/nước. |
| item\_type | string | Required | Rent, MeteredUtility, FixedService, Discount, Other. |
| description | text | Required | Mô tả dòng phí. |
| quantity | decimal | Required | Số lượng: tháng, kWh, m3, room... |
| unit\_price | decimal | Required | Đơn giá snapshot. |
| amount | decimal | Required | Thành tiền \= quantity x unit\_price hoặc giá cố định. |
| created\_at | datetime | Audit | Ngày tạo item. |

 

### **invoice\_payments**

**Mục đích:** Lưu lần thanh toán hóa đơn bằng ví.

**Logic liên quan:** Link invoice với transfer\_group\_id của cặp wallet\_transactions tenant debit và landlord credit.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính payment. |
| invoice\_id | uuid | FK invoices.id | Hóa đơn được thanh toán. |
| tenant\_user\_id | uuid | FK users.id | Tenant trả tiền. |
| landlord\_user\_id | uuid | FK users.id | Landlord nhận tiền. |
| amount | decimal | Required | Số tiền thanh toán, MVP thường bằng remaining\_amount. |
| wallet\_transfer\_group\_id | uuid | Required | Nhóm giao dịch ví debit/credit. |
| status | string | Required | Succeeded, Failed. |
| paid\_at | datetime | Nullable | Thời điểm thành công. |
| created\_at | datetime | Audit | Ngày tạo. |

 

## **F. Wallet, PayOS và Mock Payment \- Người 5**

### **wallet\_accounts**

**Mục đích:** Lưu ví nội bộ của user.

**Logic liên quan:** User có thể xem ví; chỉ KYC Approved mới được nạp ví PayOS. Landlord reserved\_balance dùng giữ tiền cọc.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính ví. |
| user\_id | uuid | FK users.id, unique | Chủ ví; mỗi user một ví. |
| balance | decimal | Required | Số dư tổng. |
| reserved\_balance | decimal | Required | Số dư bị giữ, chủ yếu tiền cọc landlord chưa được rút tự do. |
| currency | string | Required | VND. |
| status | string | Required | Active, Locked, Closed. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **wallet\_transactions**

**Mục đích:** Sổ cái ví ghi mọi biến động tiền.

**Logic liên quan:** Mọi top-up, thanh toán cọc, hoàn/giữ cọc, thanh toán hóa đơn đều phải có wallet\_transactions để audit.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính giao dịch ví. |
| wallet\_account\_id | uuid | FK wallet\_accounts.id | Ví bị tác động. |
| user\_id | uuid | FK users.id | Chủ ví. |
| transfer\_group\_id | uuid | Required for transfer | Nhóm các dòng debit/credit của cùng nghiệp vụ. |
| transaction\_type | string | Required | WalletTopUp, DepositPayment, DepositReceive, DepositRefund, InvoicePayment, InvoiceReceive... |
| direction | string | Required | Debit hoặc Credit. |
| amount | decimal | Required | Số tiền thay đổi. |
| balance\_before / balance\_after | decimal | Required | Số dư trước/sau giao dịch. |
| reserved\_balance\_before / reserved\_balance\_after | decimal | Required | Số dư giữ trước/sau giao dịch. |
| related\_entity\_type | string | Nullable | RoomDeposit, Invoice, PaymentTransaction... |
| related\_entity\_id | uuid | Nullable | ID entity nghiệp vụ liên quan. |
| description | text | Nullable | Mô tả giao dịch. |
| status | string | Required | Succeeded, Failed, Reversed... |
| created\_at | datetime | Audit | Thời điểm ghi sổ. |

 

### **payment\_transactions**

**Mục đích:** Lưu giao dịch nạp ví với PayOS hoặc Mock.

**Logic liên quan:** PayOS/Mock chỉ dùng để nạp ví, không thanh toán trực tiếp cọc/hóa đơn. Backend không cộng balance khi tạo transaction, chỉ cộng sau webhook success hợp lệ.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính giao dịch provider. |
| wallet\_account\_id | uuid | FK wallet\_accounts.id | Ví sẽ được cộng tiền nếu success. |
| payer\_user\_id | uuid | FK users.id | User tạo yêu cầu nạp ví. |
| idempotency\_key | string | Required, unique | Chống tạo trùng giao dịch top-up. |
| amount | decimal | Required | Số tiền nạp. |
| currency | string | Required | VND. |
| payment\_purpose | string | Required | WalletTopUp trong Interval 2\. |
| payment\_method | string | Required | PayOS hoặc Mock. |
| provider\_order\_code | string | Unique | Mã order dùng map webhook về đúng transaction. |
| provider\_transaction\_code | string | Nullable | Mã giao dịch provider trả về. |
| provider\_checkout\_url | text | Nullable | URL checkout PayOS trả cho frontend. |
| provider\_qr\_code | text | Nullable | QR/code thanh toán PayOS nếu có. |
| gateway\_response\_code | string | Nullable | Mã phản hồi provider. |
| gateway\_response\_message | text | Nullable | Thông điệp phản hồi provider. |
| status | string | Required | Pending, Succeeded, Failed, Cancelled, Expired. |
| expires\_at | datetime | Nullable | Hạn thanh toán QR/checkout. |
| paid\_at / failed\_at / confirmed\_at | datetime | Nullable | Thời điểm provider paid/failed và backend đã cộng ví. |
| created\_at / updated\_at | datetime | Audit | Ngày tạo/cập nhật. |

 

### **payment\_webhook\_logs**

**Mục đích:** Lưu webhook/callback từ PayOS hoặc mock.

**Logic liên quan:** Mọi webhook phải được ghi log, validate signature/amount, chống xử lý trùng bằng raw\_payload\_hash.

| Trường | Kiểu dữ liệu | Ràng buộc | Ý nghĩa / logic nghiệp vụ |
| :---- | :---- | :---- | :---- |
| id | uuid | PK | Khóa chính webhook log. |
| payment\_transaction\_id | uuid | FK payment\_transactions.id, nullable | Giao dịch provider map được. |
| payment\_method | string | Required | PayOS hoặc Mock. |
| provider\_event\_id | string | Nullable | ID sự kiện provider nếu có. |
| provider\_order\_code | string | Nullable | Order code trong webhook. |
| provider\_transaction\_code | string | Nullable | Transaction code trong webhook. |
| idempotency\_key | string | Nullable | Key chống trùng nếu provider gửi. |
| raw\_payload | text | Required | Payload webhook gốc. |
| raw\_payload\_hash | text | Required, unique | Hash payload để chống xử lý trùng. |
| signature\_status | string | Required | Valid, Invalid, SkippedForMock... |
| processing\_status | string | Required | Received, Processed, Duplicate, Failed, Unmatched. |
| error\_message | text | Nullable | Lỗi xử lý nếu có. |
| retry\_count | int | Required | Số lần retry xử lý. |
| received\_at / processed\_at | datetime | Audit | Thời điểm nhận/xử lý. |
| created\_at | datetime | Audit | Ngày tạo log. |

 

# **4\. Luồng nghiệp vụ liên quan giữa các bảng**

## **4.1 Public Search, Filter, AI Search và Map**

·         Nguồn dữ liệu chính là rooming\_houses, rooms, room\_price\_tiers, amenities, room\_amenities, rooming\_house\_amenities và property\_images.  
·         Backend chỉ trả room thuộc rooming\_house có approval\_status \= Approved, visibility\_status \= Visible, deleted\_at \= null và rooms.status \= Available.  
·         Filter giá dùng room\_price\_tiers active. Nếu user chọn occupantCount thì lấy đúng tier có occupant\_count tương ứng; nếu không chọn thì hiển thị giá thấp nhất active.  
·         AI parser hoặc rule parser chỉ tạo parsedFilter. Kết quả cuối luôn dựa trên finalFilter, không dựa trực tiếp vào câu search.  
·         Trang detail lấy address\_display và latitude/longitude từ rooming\_houses để hiển thị Google Map một marker. Thiếu tọa độ thì vẫn hiển thị thông tin phòng bình thường.

## **4.2 Viewing Appointment**

·         Tenant tạo viewing\_appointments với status Pending, room\_id là phòng muốn xem, tenant\_user\_id là user hiện tại.  
·         Khi landlord confirm, backend kiểm tra quyền sở hữu thông qua rooms \-\> rooming\_houses \-\> landlord\_user\_id.  
·         Trước khi confirm, hệ thống tìm các viewing\_appointments Confirmed của cùng landlord có khoảng thời gian giao nhau. Nếu trùng, trả warning; landlord vẫn có thể confirmDespiteConflict.  
·         Tenant chỉ xem/hủy lịch của chính mình; landlord chỉ xem/xử lý lịch thuộc phòng/khu trọ của mình.  
·         Khi lịch Completed hoặc Confirmed và tenant muốn thuê, luồng chuyển sang rental\_requests của Người 3\.

## **4.3 Rental Request và Deposit**

·         Tenant gửi rental\_requests cho rooms.status \= Available. Backend snapshot giá thuê từ room\_price\_tiers và tiền cọc từ rental\_policies.  
·         Landlord approve rental\_requests thì tạo room\_deposits status WaitingPayment và payment\_deadline\_at \= now \+ 2 hours.  
·         Tenant thanh toán cọc bằng ví qua wallet\_accounts và wallet\_transactions. payment\_transactions không dùng trực tiếp cho cọc.  
·         Khi cọc paid: tenant balance giảm, landlord balance tăng, landlord reserved\_balance tăng, room\_deposits chuyển Paid, rooms chuyển Reserved và contracts Draft được tạo.  
·         Nếu quá hạn 2 tiếng, room\_deposits chuyển Expired, rental\_requests chuyển DepositExpired và tenant không được thanh toán khoản cọc đó nữa.

## **4.4 Contract Form và OTP Signing**

·         contracts được tạo từ rental\_requests và room\_deposits đã Paid. Contract lưu monthly\_rent, deposit\_amount và room\_snapshot để không lệ thuộc thay đổi phòng sau này.  
·         MainTenant điền contract\_occupants và contract\_occupant\_documents khi contracts.status \= Draft.  
·         Khi MainTenant hoàn thành draft, contracts.status chuyển PendingMainTenantSignature và main\_tenant\_signature\_deadline\_at \= now \+ 24 hours.  
·         OTP ký hợp đồng nằm trong user\_tokens với token\_type \= ContractSignatureOtp, related\_entity\_type \= Contract, related\_entity\_id \= contracts.id.  
·         Khi MainTenant ký đúng OTP, contract\_signatures tạo dòng signer\_role \= MainTenant, contracts chuyển PendingLandlordSignature và bắt đầu deadline landlord 24 giờ.  
·         Khi Landlord ký đúng OTP, contract\_signatures tạo dòng signer\_role \= Landlord, contracts Active, rooms Occupied và tạo contract\_files FinalSignedPdf.  
·         Nếu MainTenant hoàn thành draft nhưng không ký đúng hạn, contract SignatureExpired, room Available và room\_deposits WaitingLandlordDecision để landlord chọn hoàn/giữ cọc. Nếu landlord không ký sau khi MainTenant đã ký, tenant nên được hoàn cọc.

## **4.5 Billing, Meter Reading và Invoice**

·         Landlord cấu hình billing\_service\_types và rooming\_house\_service\_prices cho khu trọ. Scope chỉ có điện, nước, wifi, rác.  
·         Cuối kỳ, landlord nhập meter\_readings cho Electric/Water của contract Active.  
·         Khi generate invoice Draft, hệ thống lấy monthly\_rent từ contracts.monthly\_rent, lấy consumption từ meter\_readings, lấy giá hiện hành từ rooming\_house\_service\_prices theo billing\_period\_end.  
·         Hệ thống tạo invoice\_items cho Rent, Electricity, Water, Wifi, Trash và snapshot unit\_price.  
·         Landlord issue invoice để tenant xem. Tenant thanh toán bằng ví khi invoice Issued hoặc Overdue.  
·         Thanh toán invoice tạo invoice\_payments Succeeded, 2 wallet\_transactions cùng transfer\_group\_id, invoices paid\_amount \= total\_amount, remaining\_amount \= 0, status \= Paid.

## **4.6 Wallet, PayOS và Mock Payment**

·         User muốn nạp ví phải đăng nhập và có KYC/eKYC Approved trong kyc\_verifications.  
·         Backend tạo payment\_transactions Pending với payment\_method \= PayOS hoặc Mock và idempotency\_key unique.  
·         Khi PayOS/Mock gửi webhook, backend ghi payment\_webhook\_logs, validate signature, validate amount và map bằng provider\_order\_code/idempotency\_key.  
·         Nếu webhook success hợp lệ, backend cộng wallet\_accounts.balance, tạo wallet\_transactions WalletTopUp Credit và chuyển payment\_transactions Succeeded.  
·         Webhook trùng, sai chữ ký hoặc sai amount không được cộng ví lần hai.  
·         Cọc và hóa đơn dùng ví nội bộ: deposit payment tăng reserved\_balance landlord, invoice payment không tăng reserved\_balance.

# **5\. Trạng thái chính cần thống nhất khi code**

| Entity.status | Giá trị đề xuất | Ý nghĩa |
| :---- | :---- | :---- |
| viewing\_appointments.status | Pending, Confirmed, Rejected, CancelledByTenant, CancelledByLandlord, Completed, Expired | Quản lý vòng đời lịch xem phòng. |
| rental\_requests.status | PendingLandlordApproval, RejectedByLandlord, WaitingDepositPayment, DepositExpired, DepositPaid, ContractDrafting, ContractSigning, Completed, Cancelled | Quản lý từ yêu cầu thuê đến hợp đồng active. |
| room\_deposits.status | WaitingPayment, Paid, Expired, WaitingLandlordDecision, Refunded, Forfeited, Cancelled | Quản lý cọc 2 tiếng, hoàn/giữ cọc. |
| contracts.status | Draft, PendingMainTenantSignature, PendingLandlordSignature, Active, Rejected, Cancelled, SignatureExpired | Quản lý soạn, ký và hiệu lực hợp đồng. |
| rooms.status | Hidden, Available, Reserved, Occupied, Maintenance | Available cho search/yêu cầu thuê; Reserved sau cọc paid; Occupied sau contract Active. |
| meter\_readings.status | Draft, Confirmed, UsedInInvoice, Cancelled | Draft sửa được; UsedInInvoice không sửa trực tiếp. |
| invoices.status | Draft, Issued, Overdue, Paid, Cancelled | Tenant chỉ thấy từ Issued; Paid không sửa trực tiếp. |
| wallet\_accounts.status | Active, Locked, Closed | Active mới dùng thanh toán/nạp ví. |
| payment\_transactions.status | Pending, Succeeded, Failed, Cancelled, Expired | Quản lý top-up PayOS/Mock. |
| payment\_webhook\_logs.processing\_status | Received, Processed, Duplicate, Failed, Unmatched | Theo dõi xử lý webhook. |

# **6\. Ghi chú implementation để tránh lệch logic**

·         Không để frontend truyền amount quyết định khi thanh toán cọc/hóa đơn. Backend phải lấy amount từ room\_deposits.deposit\_amount hoặc invoices.remaining\_amount trong database.  
·         Các thao tác tiền phải dùng database transaction: khóa ví tenant, ví landlord, entity liên quan và ghi wallet\_transactions trước khi commit.  
·         payment\_transactions chỉ dùng cho top-up PayOS/Mock. Không dùng payment\_transactions để đại diện trực tiếp cho cọc hoặc hóa đơn đã thanh toán.  
·         rooming\_house\_service\_prices không sửa đè dòng giá cũ khi đổi giá; tạo dòng mới và đóng effective\_to dòng cũ.  
·         Invoice đã Paid không cancel/sửa trực tiếp trong MVP. Nếu invoice Issued/Overdue sai thì cancel trước khi paid và tạo lại.  
·         Sau khi MainTenant ký, không cho sửa contract, contract\_occupants hoặc contract\_occupant\_documents. Muốn đổi nội dung thì phase sau dùng phụ lục/hợp đồng mới.  
·         Private object key của KYC, giấy tờ pháp lý và giấy tờ người ở không trả cho frontend không có quyền. Khi cần xem nên tạo URL tạm thời.  
·         property\_images chỉ nên thuộc một trong hai: rooming\_house\_id hoặc room\_id. Không gắn cả hai cùng lúc.  
·         Nên có unique/index ở các điểm chống trùng: normalized\_email, token\_hash, contract\_number, invoice\_no, payment\_transactions.idempotency\_key, payment\_transactions.provider\_order\_code, payment\_webhook\_logs.raw\_payload\_hash.

