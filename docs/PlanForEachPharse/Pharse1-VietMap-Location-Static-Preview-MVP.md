# Pharse 1 - Location Picker And Leaflet Tenant Map MVP

## 1. Mục tiêu phase

Phase 1 dùng để hoàn thiện chức năng vị trí khu trọ sau khi bỏ Google Maps khỏi phần picker chính.

Hướng làm mới:

- Chủ trọ dùng form địa chỉ + Leaflet picker để chọn vị trí khu trọ.
- Backend gọi VietMap Search/Geocode để tìm tọa độ từ địa chỉ.
- Chủ trọ có thể kéo marker nếu tọa độ tìm được chưa chính xác.
- Khi kéo marker không gọi API.
- Khi lưu, backend lưu địa chỉ, lat/lng và link Google Maps nếu có.
- Tenant xem chi tiết khu trọ bằng Leaflet map tương tác.
- Tenant vẫn có nút mở Google Maps bằng `googleMapUrl` để chỉ đường ngoài app.

Mục tiêu chính:

- Không dùng Google Maps API trong app.
- Không phụ thuộc VietMap vector tile `.pbf` cho frontend vì dễ lỗi parse tile.
- Dùng Leaflet với raster tile để hiển thị ổn định ở Việt Nam.
- Chủ trọ chỉ tốn request VietMap khi bấm tìm địa chỉ.
- Tenant không gọi VietMap Search/Geocode, chỉ load tile map và dữ liệu lat/lng đã lưu.

## 2. Phạm vi phase 1

### Có làm

- Thêm input địa chỉ khu trọ.
- Thêm input link Google Maps cho người thuê.
- Thêm backend endpoint search địa chỉ.
- Backend gọi VietMap Search/Geocode API.
- Backend gọi VietMap Place API nếu Search chỉ trả `ref_id`.
- Frontend landlord dùng Leaflet để hiển thị map picker.
- Chủ trọ bấm "Tìm vị trí" để lấy lat/lng.
- Chủ trọ bấm "Chỉnh vị trí" để kéo marker.
- Kéo marker chỉ update state frontend, không gọi API.
- Lưu `addressLine`, `provinceCode`, `wardCode`, `latitude`, `longitude`, `googleMapUrl`.
- Tenant detail hiển thị Leaflet map với marker khu trọ.
- Tenant có nút zoom in, zoom out, về vị trí tin đăng.
- Tenant có nút mở Google Maps từ `googleMapUrl`.
- Giới hạn map trong khu vực Việt Nam bằng `maxBounds`.
- Có thể phủ mờ đất liền nước khác để map tập trung vào Việt Nam.

### Chưa làm

- Không làm routing/chỉ đường trong app.
- Không làm Matrix/TSP/VRP/Isochrone.
- Không reverse geocode khi kéo marker.
- Không tự động parse mọi loại Google Maps short URL.
- Không dùng Static Map API cho tenant trong hướng mới.
- Không dùng tile server không có quyền cho production.
- Không làm boundary hành chính cấp tỉnh/huyện chi tiết trong phase 1.

## 3. Công nghệ sử dụng

### Frontend

- React.
- Leaflet.
- Raster tile map.
- Custom marker SVG màu cam.
- Custom map controls:
  - Về vị trí tin đăng.
  - Phóng to.
  - Thu nhỏ.

### Backend

- .NET Web API.
- HttpClient gọi VietMap API.
- PostgreSQL lưu địa chỉ và tọa độ.

### Map provider

Phase 1 tách rõ 2 phần:

- VietMap API dùng cho search/geocode ở backend.
- Leaflet tile dùng cho hiển thị map ở frontend.

Tile provider cần là raster tile dạng:

```txt
https://tile-provider/{z}/{x}/{y}.png
```

Ví dụ test:

```txt
https://maps.chotot.com/tile/{z}/{x}/{y}.png
```

Lưu ý: tile của Chợ Tốt chỉ dùng để test giao diện/kỹ thuật nếu chưa có quyền sử dụng. Production cần tile provider hợp lệ.

## 4. API dùng cho chủ trọ

### 4.1. Frontend gọi backend search address

```http
GET /api/locations/search-address?text={fullAddress}
```

Ví dụ:

```http
GET /api/locations/search-address?text=144 Trần Đại Nghĩa, Ngũ Hành Sơn, Đà Nẵng, Việt Nam
```

Backend trả:

```json
{
  "displayAddress": "144 Trần Đại Nghĩa, Ngũ Hành Sơn, Đà Nẵng",
  "latitude": 15.9897626,
  "longitude": 108.2573143
}
```

### 4.2. Backend gọi VietMap Search/Geocode

Dùng khi chủ trọ bấm:

```txt
Tìm vị trí
```

Mục đích:

- Tìm địa chỉ từ full address.
- Lấy tọa độ nếu response có lat/lng.
- Lấy `ref_id` nếu cần gọi Place API.

### 4.3. Backend gọi VietMap Place API nếu cần

Dùng khi Search/Geocode chưa trả lat/lng đầy đủ mà chỉ trả `ref_id`.

Mục đích:

- Lấy `latitude`.
- Lấy `longitude`.
- Lấy địa chỉ chuẩn hóa nếu có.

### 4.4. API lưu thông tin cơ bản khu trọ

Phase 1 có thể tiếp tục dùng endpoint hiện tại:

```http
POST /api/rooming-houses
PUT /api/rooming-houses/{id}/basic-info
```

Request body cần có:

```json
{
  "name": "Khu trọ ABC",
  "description": "Gần trường, gần chợ",
  "addressLine": "144 Trần Đại Nghĩa",
  "provinceCode": "48",
  "wardCode": "20275",
  "latitude": 15.9897626,
  "longitude": 108.2573143,
  "googleMapUrl": "https://maps.google.com/..."
}
```

## 5. Flow chủ trọ

```txt
1. Chủ trọ nhập tên khu trọ.
2. Chủ trọ chọn tỉnh/thành phố.
3. Chủ trọ chọn phường/xã.
4. Chủ trọ nhập địa chỉ chi tiết.
5. Chủ trọ có thể dán link Google Maps.
6. Chủ trọ bấm "Tìm vị trí".
7. Frontend ghép full address.
8. Frontend gọi /api/locations/search-address.
9. Backend gọi VietMap Search/Geocode.
10. Backend gọi Place API nếu cần.
11. Backend trả lat/lng.
12. Frontend Leaflet flyTo tới lat/lng.
13. Hiển thị marker tại vị trí tìm được.
14. Nếu đúng, chủ trọ bấm Lưu.
15. Nếu sai, chủ trọ bấm "Chỉnh vị trí".
16. Chủ trọ kéo marker hoặc click map.
17. Frontend update lat/lng state.
18. Kéo marker không gọi API.
19. Chủ trọ bấm Lưu.
20. Backend lưu address + lat/lng + googleMapUrl.
```

## 6. UI chủ trọ

Form location gồm:

```txt
Tỉnh/Thành phố
Phường/Xã
Địa chỉ chi tiết
Link Google Maps cho người thuê
Nút Tìm vị trí
Leaflet map picker
Nút Chỉnh vị trí / Khóa vị trí
Hiển thị Vĩ độ - Kinh độ
Nút Lưu
```

Hành vi:

- Map mặc định center ở Việt Nam hoặc Đà Nẵng.
- Sau khi tìm thấy tọa độ, map zoom gần tới marker.
- Marker mặc định khóa.
- Bấm "Chỉnh vị trí" thì marker draggable.
- Khi marker drag end:

```tsx
setSelectedLocation({
  lat: position.lat,
  lng: position.lng
});
```

Không gọi:

```txt
Reverse geocode
Search API
Place API
Tile API backend
```

khi kéo marker.

## 7. API và UI tenant

### 7.1. Backend public detail

Tenant xem chi tiết khu trọ:

```http
GET /api/public/rooming-houses/{id}
```

Response cần có:

```json
{
  "id": "RH001",
  "name": "Khu trọ ABC",
  "addressLine": "144 Trần Đại Nghĩa, Ngũ Hành Sơn, Đà Nẵng",
  "latitude": 15.9897626,
  "longitude": 108.2573143,
  "googleMapUrl": "https://maps.google.com/..."
}
```

### 7.2. Tenant Leaflet map

Tenant page render:

```txt
Vị trí khu trọ
Leaflet map
Marker màu cam
Nút về vị trí tin đăng
Nút phóng to
Nút thu nhỏ
Nút mở Google Maps
Địa chỉ text
```

Hành vi:

- Khi mở detail, map zoom gần tới marker.
- Tenant có thể zoom/pan trong phạm vi Việt Nam.
- Bấm nút về vị trí thì map fly về marker.
- Bấm Google Maps thì mở `googleMapUrl` ở tab mới.

Nếu không có `googleMapUrl`, frontend có thể tạo link fallback:

```txt
https://www.google.com/maps/search/?api=1&query={latitude},{longitude}
```

## 8. Leaflet tenant config đề xuất

```ts
const vietnamBounds = L.latLngBounds(
  L.latLng(4.5, 100.0),
  L.latLng(24.5, 119.5)
);

const map = L.map(container, {
  center: [latitude, longitude],
  zoom: 16,
  minZoom: 5,
  maxZoom: 19,
  maxBounds: vietnamBounds,
  maxBoundsViscosity: 1.0,
  zoomControl: false,
  attributionControl: false
});

L.tileLayer(tileUrl, {
  maxZoom: 19,
  bounds: vietnamBounds,
  noWrap: true,
  keepBuffer: 1
}).addTo(map);
```

Marker:

```ts
const orangeMarkerIcon = L.divIcon({
  className: "rental-location-marker",
  iconSize: [48, 48],
  iconAnchor: [24, 44],
  popupAnchor: [0, -42],
  html: "SVG marker màu cam"
});

L.marker([latitude, longitude], {
  icon: orangeMarkerIcon
}).addTo(map);
```

## 9. Mask vùng ngoài Việt Nam

Phase 1 có thể làm tùy chọn.

Mục tiêu:

- Việt Nam hiển thị rõ.
- Đất liền nước khác bị mờ.
- Biển vẫn giữ màu xanh tile.

Dữ liệu cần:

```txt
world-countries.geojson
vietnam-boundary-detailed.geojson
```

Logic:

```txt
1. Load world countries.
2. Load Vietnam detailed boundary.
3. Với mỗi country khác Vietnam:
   country - vietnamBoundary
4. Render phần còn lại bằng fill xám mờ.
```

Dùng Turf:

```js
const difference = turf.difference(
  turf.featureCollection([countryFeature, vietnamFeature])
);
```

Lưu ý:

- Đây chỉ là lớp hiển thị UI, không phải dữ liệu pháp lý.
- Hoàng Sa/Trường Sa cần GeoJSON chính thức hoặc vùng xử lý riêng nếu muốn hiển thị rõ trong production.

## 10. Database

Thêm hoặc giữ các field trong `rooming_houses`:

```sql
latitude decimal(10,7)
longitude decimal(10,7)
address_line text
province_code varchar(...)
ward_code varchar(...)
google_map_url text
```

Nếu đã có các field static map từ plan cũ thì phase 1 mới có thể để lại nhưng chưa dùng:

```sql
map_preview_near_url text
map_preview_area_url text
map_preview_wide_url text
map_preview_status varchar(50)
map_preview_generated_at timestamp
```

## 11. Transaction theo flow

### Chủ trọ bấm tìm vị trí

Có thể tốn:

```txt
1 request backend
1 request VietMap Search/Geocode
0 hoặc 1 request VietMap Place API
```

### Chủ trọ kéo marker

Tốn:

```txt
0 backend API
0 VietMap API
```

### Chủ trọ bấm lưu

Tốn:

```txt
1 request backend save basic info
0 VietMap API
```

### Tenant xem chi tiết

Tốn:

```txt
1 request backend public detail
N tile requests từ tile provider
```

Không tốn:

```txt
VietMap Search
VietMap Geocode
VietMap Place
VietMap Static Map
```

## 12. Checklist backend

```txt
[ ] Cấu hình VietMapOptions
[ ] Cấu hình HttpClient cho VietMapService
[ ] Tạo IVietMapService
[ ] Implement SearchAddressAsync
[ ] Parse response Search/Geocode
[ ] Gọi Place API nếu cần
[ ] Tạo LocationsController
[ ] Tạo GET /api/locations/search-address
[ ] Validate text không rỗng
[ ] Thêm field googleMapUrl
[ ] Thêm field latitude/longitude nếu chưa có
[ ] Lưu location trong create/update basic info
[ ] Public detail trả latitude/longitude/googleMapUrl
[ ] Không expose VietMap API key ra frontend
```

## 13. Checklist frontend landlord

```txt
[ ] Cài leaflet
[ ] Cài @types/leaflet nếu dùng TypeScript
[ ] Tạo LeafletLocationPicker component
[ ] Thêm input Link Google Maps
[ ] Ghép full address từ addressLine + ward + province
[ ] Gọi /api/locations/search-address khi bấm Tìm vị trí
[ ] Map flyTo tới lat/lng tìm được
[ ] Hiển thị marker
[ ] Nút Chỉnh vị trí
[ ] Marker draggable khi chỉnh vị trí
[ ] Drag marker chỉ update state
[ ] Click map khi đang chỉnh vị trí có thể update marker
[ ] Lưu basic info kèm lat/lng/googleMapUrl
```

## 14. Checklist frontend tenant

```txt
[ ] Public detail nhận latitude/longitude/googleMapUrl
[ ] Tạo TenantLocationMap component
[ ] Render Leaflet map khi có lat/lng
[ ] Custom marker màu cam
[ ] Custom zoom in button
[ ] Custom zoom out button
[ ] Custom recenter button
[ ] Button mở Google Maps
[ ] maxBounds Việt Nam
[ ] Optional: mask đất liền nước khác
[ ] Fallback UI nếu không có lat/lng
```

## 15. Rủi ro và lưu ý

### Tile provider

Không nên dùng tile server không có quyền trong production.

Ví dụ tile Chợ Tốt:

```txt
https://maps.chotot.com/tile/{z}/{x}/{y}.png
```

Chỉ nên dùng để test giao diện. Production cần:

- Tile provider hợp lệ.
- Hoặc tile server riêng.
- Hoặc hợp đồng/quyền sử dụng rõ ràng.

### Attribution

Một số tile provider bắt buộc hiển thị attribution. Nếu production dùng provider có điều khoản attribution thì không được ẩn attribution.

### Performance

Tenant dùng map tương tác sẽ phát sinh tile requests.

Nên kiểm soát bằng:

- `maxBounds`.
- `minZoom`.
- `maxZoom`.
- `keepBuffer`.
- Lazy render map khi section vào viewport.
- Không render map ở danh sách card, chỉ render trong detail/modal.

## 16. Chốt nghiệp vụ phase 1

Hướng cuối cùng:

```txt
Landlord:
VietMap Search/Geocode + Leaflet picker

Tenant:
Leaflet raster map + marker + Google Maps link
```

Không dùng:

```txt
Google Maps API
VietMap vector tile .pbf
VietMap Static Map API cho tenant
```

Luồng ngắn:

```txt
1. Chủ trọ nhập địa chỉ
2. Bấm Tìm vị trí
3. Backend gọi VietMap lấy lat/lng
4. Leaflet flyTo marker
5. Chủ trọ kéo marker nếu cần
6. Bấm Lưu
7. Backend lưu lat/lng/googleMapUrl
8. Tenant xem detail
9. Tenant thấy Leaflet map tương tác
10. Tenant bấm Google Maps để chỉ đường ngoài app
```
