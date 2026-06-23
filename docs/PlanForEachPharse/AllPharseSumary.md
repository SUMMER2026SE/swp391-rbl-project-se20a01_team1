# Kế Hoạch Cập Nhật Interval 2 - Thêm AI Chatbot Ở Phase Cuối

## Summary

- Interval 2 vẫn ưu tiên nền tảng trước: Google Map input, mock data, public search/filter, detail map, rule parser + AI fallback.
- AI Chatbot sẽ là **phase cuối**, sau khi search/filter đã ổn.
- Chatbot không query DB riêng và không tự bịa phòng; nó chỉ hỏi thêm nhu cầu, tạo `finalFilter`, rồi gọi lại `PublicRoomSearchService`.

## Phase Order

- Phase 1: Google Map input khi landlord tạo/cập nhật khu trọ
  - Places autocomplete + map picker/kéo marker.
  - Lưu `latitude/longitude` vào `rooming_houses`.
  - Fallback nhập tay nếu Google Maps lỗi.

- Phase 2: Mock data cho public search
  - Seed nhiều khu trọ/phòng public.
  - Có dữ liệu đa dạng về giá, số người, diện tích, tiện ích, địa chỉ, trạng thái.
  - Có dữ liệu fail visibility để test không hiển thị.

- Phase 3: Public search/filter
  - `POST /api/public/rooms/search`.
  - `/rooms` với search box, filter panel, tags, clear all, sort, paging.
  - Search theo `finalFilter`, không cần login.

- Phase 4: Room detail + Google Map display
  - `GET /api/public/rooms/{roomId}`.
  - `/rooms/:id` hiển thị ảnh, giá, tiện ích, địa chỉ, map 1 marker.
  - Nút “Đặt lịch xem phòng” chỉ làm điểm nối sang Người 2.

- Phase 5: Rule parser + Gemini fallback
  - `POST /api/public/rooms/parse-search`.
  - Rule parser parse trước.
  - Gemini chỉ parse filter khi câu phức tạp.
  - Frontend fill filter UI rồi search bằng `finalFilter`.

- Phase 6: AI Chatbot tìm phòng trọ
  - Chatbot nằm trên `/rooms` hoặc panel riêng trong trang search.
  - Tenant/guest chat bằng ngôn ngữ tự nhiên.
  - Chatbot hỏi thêm khi thiếu thông tin quan trọng như ngân sách, số người, khu vực.
  - Khi đủ thông tin, chatbot tạo `finalFilter` và gọi search service chung.
  - Kết quả chatbot hiển thị bằng cùng room cards/list của `/rooms`.

## Chatbot API / Behavior

- Thêm `POST /api/public/rooms/chat-search`
  - Request:
    ```ts
    {
      conversationId?: string;
      message: string;
      currentFilter?: PublicRoomSearchRequest;
    }
    ```
  - Response:
    ```ts
    {
      conversationId: string;
      assistantMessage: string;
      parsedFilter: PublicRoomSearchRequest;
      results?: PagedResult<PublicRoomSearchItemResponse>;
      needsMoreInfo: boolean;
      suggestedQuestions: string[];
      warnings: string[];
    }
    ```

- Chatbot rules:
  - Không trả phòng nếu chưa gọi `PublicRoomSearchService`.
  - Không gửi dữ liệu landlord/tenant private vào Gemini.
  - Không gửi toàn bộ database phòng vào prompt.
  - Chỉ gửi schema filter, catalog amenities active, và conversation context tối thiểu.
  - Nếu AI lỗi, fallback sang rule parser/keyword search.
  - Nếu user hỏi ngoài scope thuê phòng, trả lời ngắn và kéo lại về nhu cầu tìm phòng.

## Test Cases

- User hỏi: “Tìm phòng dưới 3 triệu cho 2 người có máy lạnh”
  - Chatbot tạo filter và trả kết quả.
- User hỏi: “Phòng cho sinh viên giá mềm”
  - Chatbot hỏi thêm khu vực hoặc ngân sách.
- User chỉnh yêu cầu: “thêm wifi và chỗ để xe”
  - Chatbot merge vào `currentFilter`, search lại.
- AI lỗi/timeout
  - Chatbot không crash, fallback keyword hoặc báo cần thử lại.
- Chatbot không trả phòng hidden/unapproved/private.

## Assumptions

- Chatbot là phase cuối của Interval 2, không ảnh hưởng deadline phase search cơ bản.
- Chatbot dùng Gemini, nhưng dùng lại rule parser và `PublicRoomSearchService`.
- Chatbot chưa làm booking/appointment; khi user muốn đặt lịch, chỉ chuyển sang flow Người 2.
