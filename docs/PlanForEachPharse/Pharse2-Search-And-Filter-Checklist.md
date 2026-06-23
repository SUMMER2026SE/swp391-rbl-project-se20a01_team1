# Pharse 2 - Search And Filter Checklist

## Working Rule

- Mỗi task ban đầu là `[ ]`.
- Khi làm xong task, chạy test phù hợp.
- Test pass mới đổi thành `[x]`.
- Nếu test fail, giữ `[ ]`, ghi lỗi vào phần "Issue Log".
- Mỗi nhóm lớn xong phải build/test trước khi qua nhóm tiếp theo.
- Không làm frontend search page trước khi backend search API pass.
- Không làm radius UI trước khi backend radius API pass.
- Không chốt Phase 2 nếu frontend/backend build chưa pass.

## Step 0 - Create Checklist File

- [x] Tạo file `docs/PlanForEachPharse/Pharse2-Search-And-Filter-Checklist.md`.
- [x] Copy checklist Phase 2 vào file.
- [x] Thêm mục `Progress Log`.
- [x] Thêm mục `Issue Log`.
- [x] Test: mở file kiểm tra markdown render được.

## Step 1 - Backend Contracts

- [x] Tạo `PagedResult<T>`.
- [x] Tạo `RoomingHouseSearchItemResponse`.
- [x] Tạo query/request model cho search.
- [x] Thêm field `distanceKm`.
- [x] Thêm field `minMonthlyRent`.
- [x] Thêm field `maxMonthlyRent`.
- [x] Thêm field `minAreaM2`.
- [x] Thêm field `maxAreaM2`.
- [x] Thêm field `availableRooms`.
- [x] Thêm field `totalRooms`.
- [x] Test backend build.

## Step 2 - Rule-Based Search Parser

- [x] Tạo service parser.
- [x] Normalize tiếng Việt.
- [x] Parse city aliases.
- [x] Parse price.
- [x] Parse area.
- [x] Parse occupants.
- [x] Parse amenities.
- [x] Parse place text.
- [x] Parse radius.
- [x] Viết parser smoke test ở tầng application.
- [x] Test câu `Đà Nẵng dưới 4 triệu máy lạnh`.
- [x] Test câu `gần FPT Đà Nẵng bán kính 3km`.
- [x] Test câu `Sài Gòn có gác dưới 5tr`.
- [x] Test câu `Hà Nội 2 người dưới 5 triệu`.
- [x] Backend build pass.

## Step 3 - Radius Calculation

- [x] Tạo helper Haversine.
- [x] Tạo bounding box helper.
- [x] Validate `centerLat`, `centerLng`, `radiusKm`.
- [x] Default `radiusKm = 3` khi có place text nhưng không có radius.
- [x] Giới hạn `radiusKm <= 30`.
- [x] Test khoảng cách với vài cặp tọa độ mẫu.
- [x] Backend build pass.

## Step 4 - Backend Search Service

- [x] Tạo search method trong query service hoặc service riêng.
- [x] Base query chỉ lấy khu trọ `Approved`.
- [x] Base query chỉ lấy khu trọ `Visible`.
- [x] Base query chỉ lấy khu trọ chưa deleted.
- [x] Base query chỉ lấy khu trọ có phòng `Available`.
- [x] Apply filter province.
- [x] Apply filter ward.
- [x] Apply filter price.
- [x] Apply filter area.
- [x] Apply filter occupants.
- [x] Apply filter house amenities.
- [x] Apply filter room amenities.
- [x] Apply filter keyword.
- [x] Apply filter radius.
- [x] Tính aggregate min price.
- [x] Tính aggregate max price.
- [x] Tính aggregate min area.
- [x] Tính aggregate max area.
- [x] Tính aggregate available room count.
- [x] Tính aggregate distanceKm.
- [x] Apply sort.
- [x] Apply pagination.
- [x] Backend build pass.

## Step 5 - VietMap Geocode For Place Text

- [x] Nếu parser có `placeText`, gọi `IVietMapService.SearchAddressAsync`.
- [x] Không gọi VietMap nếu request có `centerLat` và `centerLng`.
- [x] Nếu VietMap lỗi, trả error rõ.
- [x] Test query có tọa độ không gọi VietMap.
- [x] Test query `gần FPT Đà Nẵng` có gọi VietMap.
- [x] Backend build pass.

## Step 6 - Backend Search Endpoint

- [x] Thêm endpoint `GET /api/public/rooming-houses/search`.
- [x] Bind query params.
- [x] Trả `ApiResponse<PagedResult<RoomingHouseSearchItemResponse>>`.
- [x] Test HTTP search rỗng.
- [x] Test HTTP filter price.
- [x] Test HTTP filter province.
- [x] Test HTTP radius search bằng tọa độ.
- [x] Test HTTP pagination.
- [x] Backend build pass.

## Step 7 - Mock Data Migration

- [x] Tạo migration seed mock Phase 2.
- [x] Seed 240 khu trọ.
- [x] Seed rooms.
- [x] Seed price tiers.
- [x] Seed amenities relation.
- [x] Seed images.
- [x] Đảm bảo rollback xóa sạch mock data.
- [x] Apply migration local.
- [x] Test DB tổng mock >= 200.
- [x] Test DB Đà Nẵng có dữ liệu.
- [x] Test DB HCM có dữ liệu.
- [x] Test DB Hà Nội có dữ liệu.
- [x] Test DB mỗi khu có phòng available.
- [x] Test endpoint search có kết quả.

## Step 8 - Frontend Types And API Client

- [x] Tạo `PagedResult<T>` type.
- [x] Tạo `RoomingHouseSearchItem` type.
- [x] Tạo `RoomingHouseSearchParams` type.
- [x] Tạo `searchPublicRoomingHouses(params)`.
- [x] Serialize array params đúng.
- [x] Frontend build pass.

## Step 9 - Home Search Bar

- [x] Thêm input search ở hero Home.
- [x] Submit navigate sang `/search?q=...`.
- [x] Empty query navigate `/search`.
- [x] Test UI bằng browser/dev server.
- [x] Frontend build pass.

## Step 10 - Search Route And Page Base

- [x] Tạo route `/search`.
- [x] Tạo Search Page.
- [x] Đọc query params từ URL.
- [x] Gọi search API.
- [x] Hiển thị loading.
- [x] Hiển thị error.
- [x] Hiển thị empty.
- [x] Hiển thị result cards.
- [x] Card click mở detail.
- [x] Frontend build pass.

## Step 11 - Search Filters UI

- [x] Filter city.
- [x] Filter ward.
- [x] Filter price.
- [x] Filter area.
- [x] Filter occupants.
- [x] Filter house amenities.
- [x] Filter room amenities.
- [x] Sort dropdown.
- [x] Clear filters.
- [x] Update URL khi đổi filter.
- [x] Refetch API khi đổi filter.
- [x] Frontend build pass.

## Step 12 - Radius Search UI

- [x] Filter radius.
- [x] Thêm "Dùng vị trí hiện tại".
- [x] Handle browser geolocation success.
- [x] Handle browser geolocation denied.
- [x] Thêm "Chọn điểm trên bản đồ".
- [x] Leaflet map click set center.
- [x] Hiển thị marker điểm trung tâm.
- [x] Gửi `centerLat`, `centerLng`, `radiusKm`.
- [x] Hiển thị `distanceKm` trên card.
- [x] Frontend build pass.

## Step 13 - Pagination UI

- [x] Hiển thị tổng kết quả.
- [x] Nút previous.
- [x] Nút next.
- [x] Page number hiện tại.
- [x] Đổi page cập nhật URL.
- [x] Đổi page gọi lại API.
- [x] Frontend build pass.

## Step 14 - End-To-End Test

- [x] Test `Đà Nẵng dưới 4 triệu máy lạnh`.
- [x] Test `gần FPT Đà Nẵng bán kính 3km`.
- [x] Test `Sài Gòn có gác dưới 5tr`.
- [x] Test filter city + price + amenity.
- [x] Test geolocation denied.
- [x] Test card detail.
- [x] Test pagination.
- [x] Backend build pass.
- [x] Frontend build pass.

## Step 15 - Final Cleanup

- [x] Rà `rg` không còn API/list cũ bị dùng sai cho search page.
- [x] Rà naming.
- [x] Rà warning/error console.
- [x] Cập nhật checklist toàn bộ task đã xong.
- [x] Ghi summary vào `Progress Log`.
- [x] Backend build pass.
- [x] Frontend build pass.

## Progress Log

- Step 0 hoàn thành: checklist file đã tạo và đọc được bằng `Get-Content`.
- Step 1 hoàn thành: backend contracts compile, `dotnet build` pass với output `artifacts/phase2-step1-build`.
- Step 2 hoàn thành: parser build pass và smoke test 4 câu bắt buộc pass trong `artifacts/phase2-parser-smoke`.
- Step 3 hoàn thành: Haversine/bounding/validation helper build pass và smoke test pass trong `artifacts/phase2-geo-smoke`.
- Step 4-6 hoàn thành: search service/endpoint build pass, HTTP smoke tests pass cho empty, price, province, radius, pagination và `gần FPT Đà Nẵng`.
- Step 7 hoàn thành: seed 240 khu trọ mock, 600 phòng, đủ 3 thành phố; migration apply pass; HTTP search smoke test pass với empty, semantic, province/price, radius, pagination.
- Step 8 hoàn thành: frontend search types/API client compile, `npm run build` pass.
- Step 9-15 hoàn thành: Tích hợp thanh tìm kiếm Home; cấu hình Router `/search`; hoàn thiện giao diện bộ lọc nâng cao (Filters UI), định vị bán kính tương tác Leaflet Map (Radius Search UI) hiển thị marker và đường tròn khoảng cách; Phân trang (Pagination UI) đồng bộ hóa URL params; Biên dịch thành công 100% không lỗi; Kiểm thử thực tế trên trình duyệt hiển thị kết quả và CORS hoạt động trơn tru.

## Issue Log

- Step 7: apply migration `AdjustPhase2MockAmenityCoverage` fail lần đầu vì dùng sai tên bảng `price_tiers`; DB thật dùng `room_price_tiers`. Đã sửa migration, Step 7 giữ `[ ]` cho tới khi apply/test pass.
- Step 7: apply migration `AdjustPhase2MockAmenityCoverage` fail lần hai vì dùng sai tên cột `monthly_price`; DB thật dùng `monthly_rent`. Đã sửa migration sau khi kiểm tra schema `room_price_tiers`.

## Assumptions

- Search semantic Phase 2 dùng rule-based parser.
- Chưa dùng LLM.
- Chưa dùng PostGIS.
- Radius search dùng Haversine.
- Mock data dùng migration.
- UI map result đầy đủ có thể để Phase sau; Phase 2 chỉ cần list + chọn điểm trung tâm bằng Leaflet.
