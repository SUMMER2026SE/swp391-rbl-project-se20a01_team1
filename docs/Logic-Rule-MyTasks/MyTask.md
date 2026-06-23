Business Rules — Người 1: Public Search, Filter, AI Search và Google Map Detail  
1\. Scope nghiệp vụ của Người 1  
Người 1 phụ trách phần giúp Tenant tìm thấy phòng phù hợp trước khi chuyển sang các luồng đặt lịch, đặt cọc và hợp đồng.  
Phạm vi bao gồm:  
•	Search phòng bằng filter.  
•	Search bằng box search theo ngữ cảnh.  
•	Sau khi search bằng box, hệ thống tự fill filter.  
•	Người dùng có thể chỉnh filter nhiều lần sau khi search.  
•	AI chỉ dùng để parse câu search phức tạp thành filter.  
•	Xem chi tiết phòng/khu trọ.  
•	Khi bấm vào chi tiết khu trọ/phòng, hiển thị vị trí khu trọ trên Google Map.  
•	Chưa làm chatbot ở giai đoạn đầu. Chatbot chỉ làm sau khi search/filter đã ổn.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
2\. Business Rules — Public Room Visibility  
Rule ID	Business Rule  
BR-PUBLIC-01	Chỉ hiển thị khu trọ có approval\_status \= Approved.  
BR-PUBLIC-02	Chỉ hiển thị khu trọ có visibility\_status \= Visible.  
BR-PUBLIC-03	Không hiển thị khu trọ đã bị xóa mềm, tức deleted\_at \!= null.  
BR-PUBLIC-04	Chỉ hiển thị phòng có status \= Available.  
BR-PUBLIC-05	Không hiển thị phòng đã bị xóa mềm, tức deleted\_at \!= null.  
BR-PUBLIC-06	Nếu khu trọ hợp lệ nhưng phòng không còn Available, phòng đó không được xuất hiện trong kết quả search.  
BR-PUBLIC-07	Phòng Reserved, Occupied, Maintenance, Hidden không được hiển thị trong public search.  
BR-PUBLIC-08	Tenant chưa đăng nhập vẫn được xem danh sách phòng public và chi tiết phòng public.  
BR-PUBLIC-09	Các chức năng sau khi bấm “Đặt lịch xem phòng” mới yêu cầu đăng nhập và thuộc phạm vi Người 2\.  
BR-PUBLIC-10	Public search không được trả thông tin nhạy cảm của landlord như email riêng, số giấy tờ, legal document, KYC hoặc thông tin ngân hàng.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
3\. Business Rules — Filter Search  
3.1 Filter theo keyword  
Rule ID	Business Rule  
BR-KEYWORD-01	Keyword được dùng để tìm trong tên khu trọ.  
BR-KEYWORD-02	Keyword được dùng để tìm trong địa chỉ hiển thị address\_display.  
BR-KEYWORD-03	Keyword được dùng để tìm trong mô tả khu trọ.  
BR-KEYWORD-04	Keyword được dùng để tìm trong số/tên phòng room\_number.  
BR-KEYWORD-05	Keyword được dùng để tìm trong mô tả phòng.  
BR-KEYWORD-06	Keyword rỗng hoặc chỉ có khoảng trắng thì bỏ qua filter keyword.  
BR-KEYWORD-07	Hệ thống nên normalize keyword để user gõ không dấu vẫn tìm được dữ liệu có dấu, ví dụ may lanh vẫn match máy lạnh.  
BR-KEYWORD-08	Keyword không được làm thay đổi các filter khác nếu user đã chọn filter thủ công.  
BR-KEYWORD-09	Keyword chỉ là một điều kiện trong finalFilter, không phải điều kiện duy nhất quyết định kết quả.  
3.2 Filter theo khu vực  
Rule ID	Business Rule  
BR-ADDR-01	Nếu user chọn provinceCode, chỉ lấy khu trọ thuộc tỉnh/thành đó.  
BR-ADDR-02	Nếu user chọn wardCode, chỉ lấy khu trọ thuộc phường/xã đó.  
BR-ADDR-03	Nếu truyền cả provinceCode và wardCode, wardCode phải thuộc provinceCode.  
BR-ADDR-04	Nếu wardCode không thuộc provinceCode, hệ thống trả lỗi validation hoặc bỏ wardCode không hợp lệ.  
BR-ADDR-05	Chỉ dùng province/ward đang active nếu có kiểm tra catalog.  
BR-ADDR-06	Địa chỉ hiển thị cho tenant lấy từ address\_display.  
BR-ADDR-07	Search theo khu vực không phụ thuộc vào Google Map. Google Map chỉ hiển thị ở trang chi tiết.  
3.3 Filter theo giá  
Rule ID	Business Rule  
BR-PRICE-01	Giá thuê phải lấy từ room\_price\_tiers, không lấy cứng từ bảng rooms.  
BR-PRICE-02	Chỉ dùng price tier có is\_active \= true.  
BR-PRICE-03	Nếu user không chọn số người, giá hiển thị trong danh sách là giá thấp nhất active của phòng.  
BR-PRICE-04	Nếu user chọn occupantCount, giá dùng để filter là price tier có occupant\_count \= occupantCount.  
BR-PRICE-05	Nếu user chọn occupantCount nhưng phòng không có price tier tương ứng, phòng đó không match kết quả.  
BR-PRICE-06	Nếu user nhập maxPrice, chỉ lấy phòng có giá phù hợp nhỏ hơn hoặc bằng maxPrice.  
BR-PRICE-07	Nếu user nhập minPrice, chỉ lấy phòng có giá phù hợp lớn hơn hoặc bằng minPrice.  
BR-PRICE-08	minPrice không được lớn hơn maxPrice.  
BR-PRICE-09	Nếu minPrice \> maxPrice, frontend nên cảnh báo hoặc backend trả lỗi validation.  
BR-PRICE-10	Nếu user đổi giá sau khi search bằng box, giá filter mới do user chỉnh phải được ưu tiên hơn giá parse từ box search.  
3.4 Filter theo số người  
Rule ID	Business Rule  
BR-OCC-01	occupantCount phải lớn hơn 0\.  
BR-OCC-02	occupantCount không được vượt quá rooms.max\_occupants.  
BR-OCC-03	Nếu user chọn số người, phòng phải có sức chứa tối đa lớn hơn hoặc bằng số người đó.  
BR-OCC-04	Nếu user chọn số người, giá hiển thị nên là giá theo đúng số người đó.  
BR-OCC-05	Nếu user không chọn số người, hệ thống không lọc theo max\_occupants.  
BR-OCC-06	Nếu box search parse ra “2 người”, hệ thống set occupantCount \= 2\.  
BR-OCC-07	Nếu user chỉnh lại từ 2 người sang 1 người sau khi search box, kết quả phải search lại theo occupantCount \= 1\.  
3.5 Filter theo diện tích  
Rule ID	Business Rule  
BR-AREA-01	minAreaM2 phải lớn hơn 0 nếu được truyền.  
BR-AREA-02	Nếu user nhập diện tích tối thiểu, chỉ lấy phòng có area\_m2 \>= minAreaM2.  
BR-AREA-03	Nếu phòng không có dữ liệu area\_m2, phòng đó không match khi user lọc theo diện tích.  
BR-AREA-04	Nếu box search có cụm “trên 20m2”, hệ thống parse thành minAreaM2 \= 20\.  
3.6 Filter theo tiện ích  
Rule ID	Business Rule  
BR-AMENITY-01	Chỉ dùng tiện ích có is\_active \= true.  
BR-AMENITY-02	Search tiện ích phải kiểm tra cả tiện ích cấp phòng và tiện ích cấp khu trọ.  
BR-AMENITY-03	Tiện ích cấp phòng match nếu phòng có tiện ích đó trong room\_amenities.  
BR-AMENITY-04	Tiện ích cấp khu trọ match nếu khu trọ chứa phòng có tiện ích đó trong rooming\_house\_amenities.  
BR-AMENITY-05	Nếu user chọn nhiều tiện ích, mặc định hiểu là kết quả phải có đủ các tiện ích đã chọn.  
BR-AMENITY-06	Nếu user search “máy lạnh”, “điều hòa”, “air conditioner”, hệ thống nên map về cùng một tiện ích nếu seed data có.  
BR-AMENITY-07	Nếu box search parse ra tiện ích bằng tên, backend phải map amenityNames sang amenityIds.  
BR-AMENITY-08	Nếu tiện ích được parse nhưng không tồn tại trong hệ thống, không áp dụng tiện ích đó và trả warning.  
BR-AMENITY-09	Nếu user bỏ chọn tiện ích sau khi box search đã parse, filter tiện ích đó phải bị xóa khỏi finalFilter.  
BR-AMENITY-10	Không trả tiện ích inactive trong response.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
4\. Business Rules — Kết hợp Search Box và Filter  
Rule ID	Business Rule  
BR-COMBO-01	Search box không trực tiếp quyết định kết quả cuối cùng, mà chỉ tạo filter ban đầu.  
BR-COMBO-02	Filter UI là nơi thể hiện điều kiện search cuối cùng.  
BR-COMBO-03	Kết quả search luôn dựa trên finalFilter.  
BR-COMBO-04	Sau khi search box parse xong, frontend phải tự fill các filter tương ứng.  
BR-COMBO-05	User được quyền chỉnh, thêm hoặc xóa filter sau khi search bằng box.  
BR-COMBO-06	Mỗi lần user chỉnh filter, hệ thống search lại theo finalFilter mới.  
BR-COMBO-07	Không cần parse lại search box khi user chỉ chỉnh filter thủ công.  
BR-COMBO-08	Nếu user xóa search box nhưng vẫn giữ filter, kết quả vẫn search theo filter đang chọn.  
BR-COMBO-09	Nếu user bấm “Clear all”, hệ thống xóa search box, parsed filter, filter thủ công và search lại danh sách mặc định.  
BR-COMBO-10	Nếu box search parse ra filter nhưng user chỉnh filter sau đó, filter do user chỉnh được ưu tiên hơn.  
BR-COMBO-11	Nếu user nhập một search box mới, hệ thống parse lại và cập nhật filter mới.  
BR-COMBO-12	Để đơn giản trong Interval 2, khi user search box lần mới, có thể reset filter cũ rồi fill filter mới.  
BR-COMBO-13	Các filter checkbox/dropdown như tiện ích, tỉnh/phường, số người, sort có thể search lại ngay khi thay đổi.  
BR-COMBO-14	Các filter nhập tay như giá hoặc diện tích nên debounce hoặc yêu cầu bấm “Áp dụng”.  
BR-COMBO-15	Frontend nên hiển thị tag filter hiện tại để user biết hệ thống đang lọc theo điều kiện nào.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
5\. Business Rules — Box Search theo ngữ cảnh  
Rule ID	Business Rule  
BR-CTX-01	Box search nhận câu tự nhiên từ user và parse thành filter.  
BR-CTX-02	Box search không query database riêng.  
BR-CTX-03	Sau khi parse, box search phải gọi lại search service chung bằng finalFilter.  
BR-CTX-04	Nếu câu có “dưới 3 triệu”, parse thành maxPrice \= 3000000\.  
BR-CTX-05	Nếu câu có “trên 20m2”, parse thành minAreaM2 \= 20\.  
BR-CTX-06	Nếu câu có “2 người”, parse thành occupantCount \= 2\.  
BR-CTX-07	Nếu câu có “máy lạnh”, “wifi”, “chỗ để xe”, parse thành amenityNames.  
BR-CTX-08	Nếu câu có tên khu vực hoặc từ khóa không parse được rõ, đưa vào keyword.  
BR-CTX-09	Response của parse search phải trả parsedFilter.  
BR-CTX-10	Response của parse search nên trả warnings nếu có phần không hiểu.  
BR-CTX-11	Nếu user nhập câu quá ngắn như “phòng trọ”, hệ thống chỉ dùng keyword hoặc trả danh sách mặc định.  
BR-CTX-12	Nếu không parse được gì, hệ thống không báo lỗi nặng mà vẫn search theo keyword thô.  
BR-CTX-13	Parser rule-based phải chạy trước AI để giảm chi phí và dễ kiểm soát.  
BR-CTX-14	AI chỉ được gọi khi rule parser không đủ hiểu câu hoặc câu có ngữ cảnh phức tạp.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
6\. Business Rules — AI Search Parser  
Rule ID	Business Rule  
BR-AI-01	AI chỉ có nhiệm vụ parse câu user thành filter JSON.  
BR-AI-02	AI không được trực tiếp query database.  
BR-AI-03	AI không được tự quyết định danh sách phòng trả về.  
BR-AI-04	Kết quả từ AI phải được backend validate trước khi áp dụng.  
BR-AI-05	AI chỉ được trả về các field nằm trong schema filter cho phép.  
BR-AI-06	Nếu AI trả về field ngoài schema, backend phải bỏ qua field đó.  
BR-AI-07	Nếu AI parse ra giá, giá phải là số hợp lệ và lớn hơn 0\.  
BR-AI-08	Nếu AI parse ra số người, occupantCount phải lớn hơn 0\.  
BR-AI-09	Nếu AI parse ra tiện ích bằng tên, backend phải map sang tiện ích có trong DB.  
BR-AI-10	Nếu tiện ích AI parse không tồn tại, hệ thống trả warning và không áp dụng filter đó.  
BR-AI-11	Nếu AI parse ra khu vực mơ hồ, ví dụ “gần trường”, hệ thống có thể đưa vào keyword thay vì tự suy đoán.  
BR-AI-12	AI không được bịa ra khu trọ, phòng, giá, tiện ích hoặc địa chỉ không có trong database.  
BR-AI-13	Nếu AI confidence thấp, hệ thống nên trả warnings để user chỉnh filter thủ công.  
BR-AI-14	AI search vẫn phải gọi lại RoomSearchService với finalFilter sau khi validate.  
BR-AI-15	Nếu AI service lỗi, hệ thống fallback về rule parser hoặc keyword search, không làm crash search page.  
BR-AI-16	Không lưu nội dung search nhạy cảm của user vào log nếu không cần thiết.  
BR-AI-17	Không gửi dữ liệu cá nhân của landlord/tenant vào prompt AI.  
BR-AI-18	AI parser chỉ dùng dữ liệu catalog cần thiết như tên tiện ích, không gửi toàn bộ database phòng lên AI.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
7\. Business Rules — Search Result Response  
Rule ID	Business Rule  
BR-RESULT-01	Mỗi item trong kết quả search phải có roomId.  
BR-RESULT-02	Mỗi item nên có roomingHouseId để điều hướng sang chi tiết khu trọ/phòng.  
BR-RESULT-03	Kết quả search phải có tên khu trọ.  
BR-RESULT-04	Kết quả search phải có địa chỉ hiển thị.  
BR-RESULT-05	Kết quả search phải có giá hiển thị.  
BR-RESULT-06	Nếu user đã lọc theo số người, giá hiển thị là giá theo số người đó.  
BR-RESULT-07	Nếu user không lọc theo số người, giá hiển thị là giá thấp nhất active.  
BR-RESULT-08	Kết quả search nên có ảnh cover.  
BR-RESULT-09	Nếu phòng có ảnh cover, ưu tiên ảnh cover của phòng.  
BR-RESULT-10	Nếu phòng không có ảnh cover, dùng ảnh cover của khu trọ.  
BR-RESULT-11	Nếu không có ảnh nào, frontend hiển thị placeholder.  
BR-RESULT-12	Kết quả search phải có totalCount để phân trang.  
BR-RESULT-13	Không trả thông tin private như legal document, KYC, token, số giấy tờ hoặc thông tin ngân hàng.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
8\. Business Rules — Sort và Paging  
Rule ID	Business Rule  
BR-PAGE-01	Search API phải có paging.  
BR-PAGE-02	pageNumber \< 1 thì set về 1 hoặc trả lỗi validation.  
BR-PAGE-03	pageSize phải có giới hạn tối đa, ví dụ 50\.  
BR-PAGE-04	Khi user đổi filter, pageNumber phải reset về 1\.  
BR-PAGE-05	Khi user chỉ chuyển trang, giữ nguyên finalFilter.  
BR-SORT-01	Sort mặc định là mới nhất hoặc theo cấu hình mặc định của hệ thống.  
BR-SORT-02	PriceAsc sort theo giá đang match với filter.  
BR-SORT-03	PriceDesc sort theo giá đang match với filter.  
BR-SORT-04	AreaAsc sort theo rooms.area\_m2 tăng dần.  
BR-SORT-05	AreaDesc sort theo rooms.area\_m2 giảm dần.  
BR-SORT-06	Sort không được làm mất điều kiện filter hiện tại.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
9\. Business Rules — Public Room Detail  
Rule ID	Business Rule  
BR-DETAIL-01	User bấm vào một phòng/khu trọ từ kết quả search thì mở trang chi tiết.  
BR-DETAIL-02	Trang chi tiết chỉ hiển thị phòng thuộc khu trọ Approved và Visible.  
BR-DETAIL-03	Trang chi tiết không hiển thị phòng đã bị xóa mềm.  
BR-DETAIL-04	Trang chi tiết không hiển thị khu trọ đã bị xóa mềm.  
BR-DETAIL-05	Nếu phòng không còn Available, frontend phải hiển thị trạng thái hiện tại và có thể ẩn nút đặt lịch.  
BR-DETAIL-06	Trang chi tiết phải hiển thị ảnh phòng/khu trọ.  
BR-DETAIL-07	Trang chi tiết phải hiển thị giá theo các price tiers đang active.  
BR-DETAIL-08	Trang chi tiết phải hiển thị tiện ích cấp phòng và tiện ích cấp khu trọ.  
BR-DETAIL-09	Trang chi tiết phải hiển thị địa chỉ address\_display.  
BR-DETAIL-10	Trang chi tiết có thể hiển thị chính sách thuê như số tháng cọc, gia hạn nếu có lease\_policy active.  
BR-DETAIL-11	Trang chi tiết không hiển thị giấy tờ pháp lý private của landlord.  
BR-DETAIL-12	Nút “Đặt lịch xem phòng” chỉ là điểm chuyển sang nghiệp vụ Người 2\.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
10\. Business Rules — Google Map khi xem khu trọ/phòng  
Rule ID	Business Rule  
BR-MAP-01	Google Map chỉ hiển thị khi user bấm vào xem chi tiết khu trọ/phòng.  
BR-MAP-02	Không làm trang map tổng cho toàn bộ phòng ở scope Người 1\.  
BR-MAP-03	Không làm search gần vị trí ở scope Người 1\.  
BR-MAP-04	Không hiển thị nhiều marker trong scope Người 1\.  
BR-MAP-05	Map chỉ hiển thị 1 marker tại vị trí khu trọ hiện tại.  
BR-MAP-06	Tọa độ map lấy từ rooming\_houses.latitude và rooming\_houses.longitude.  
BR-MAP-07	Địa chỉ text hiển thị chính vẫn lấy từ rooming\_houses.address\_display.  
BR-MAP-08	Nếu có latitude và longitude, frontend render Google Map.  
BR-MAP-09	Nếu thiếu latitude hoặc longitude, frontend không render map và hiển thị thông báo: “Khu trọ này chưa có vị trí trên bản đồ”.  
BR-MAP-10	Nếu latitude không nằm trong khoảng \-90 đến 90, không render map.  
BR-MAP-11	Nếu longitude không nằm trong khoảng \-180 đến 180, không render map.  
BR-MAP-12	Map không ảnh hưởng đến kết quả search/filter.  
BR-MAP-13	Việc không có tọa độ không làm phòng biến mất khỏi kết quả search.  
BR-MAP-14	Nếu Google Map load lỗi, trang chi tiết vẫn phải hiển thị thông tin phòng/khu trọ bình thường.  
BR-MAP-15	Nếu Google Map load lỗi, frontend hiển thị fallback text thay vì crash UI.  
BR-MAP-16	Marker trên map nên hiển thị tên khu trọ và địa chỉ ngắn.  
BR-MAP-17	Map không được dùng để cập nhật tọa độ trong scope Người 1\.  
BR-MAP-18	Việc tạo/cập nhật tọa độ khu trọ thuộc luồng landlord ở Interval 1 hoặc phần quản lý khu trọ, không thuộc public search.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
11\. Business Rules — Image Display  
Rule ID	Business Rule  
BR-IMG-01	Danh sách phòng phải ưu tiên ảnh cover của phòng.  
BR-IMG-02	Nếu phòng không có ảnh cover, dùng ảnh cover của khu trọ.  
BR-IMG-03	Nếu không có ảnh cover nhưng có ảnh thường, dùng ảnh có sort\_order nhỏ nhất.  
BR-IMG-04	Nếu không có ảnh nào, frontend dùng ảnh placeholder.  
BR-IMG-05	Trang chi tiết nên hiển thị cả ảnh phòng và ảnh khu trọ nếu có.  
BR-IMG-06	Ảnh phải lấy từ public image URL hoặc endpoint public hợp lệ.  
BR-IMG-07	Không hiển thị ảnh giấy tờ pháp lý private ở public page.  
BR-IMG-08	property\_images chỉ được thuộc phòng hoặc khu trọ, không thuộc cả hai cùng lúc.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
12\. Business Rules — API Design  
Rule ID	Business Rule  
BR-API-01	API search bằng filter nên dùng POST /api/public/rooms/search hoặc GET /api/public/rooms.  
BR-API-02	API parse search box nên tách riêng: POST /api/public/rooms/parse-search.  
BR-API-03	API parse search chỉ trả filter đã hiểu, không bắt buộc trả danh sách phòng.  
BR-API-04	API search nhận finalFilter và trả danh sách phòng.  
BR-API-05	API detail dùng GET /api/public/rooms/{roomId}.  
BR-API-06	Search API và detail API đều chỉ trả dữ liệu public.  
BR-API-07	API parse search phải trả warnings nếu có phần không hiểu.  
BR-API-08	API search phải trả totalCount, pageNumber, pageSize, items.  
BR-API-09	API detail phải trả thông tin map gồm latitude, longitude, addressDisplay.  
BR-API-10	API detail phải trả đủ dữ liệu để frontend hiển thị mà không cần gọi quá nhiều API nhỏ.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
13\. Business Rules — Frontend UX  
Rule ID	Business Rule  
BR-UX-01	Trang /rooms phải có search box chính.  
BR-UX-02	Trang /rooms phải có filter panel.  
BR-UX-03	Search box và filter panel phải dùng chung một finalFilter.  
BR-UX-04	Sau khi search box parse xong, filter panel phải được fill theo kết quả parse.  
BR-UX-05	User có thể chỉnh filter nhiều lần sau khi search box.  
BR-UX-06	Khi user chỉnh filter, danh sách phòng cập nhật theo filter mới.  
BR-UX-07	Filter hiện tại nên hiển thị dưới dạng tag để user biết hệ thống đang lọc gì.  
BR-UX-08	User có thể xóa từng filter tag.  
BR-UX-09	User có thể bấm “Clear all” để reset toàn bộ filter.  
BR-UX-10	Trang /rooms/:id phải hiển thị thông tin phòng, ảnh, giá, tiện ích, địa chỉ và map nếu có tọa độ.  
BR-UX-11	Nếu phòng không còn available, nút đặt lịch phải bị disable hoặc hiển thị trạng thái phù hợp.  
BR-UX-12	Nếu không có kết quả search, hiển thị empty state và gợi ý user chỉnh filter.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
14\. Business Rules — Chatbot để sau  
Rule ID	Business Rule  
BR-CHAT-01	Chatbot không làm trước khi filter search và box search hoàn thành.  
BR-CHAT-02	Chatbot sau này phải dùng lại RoomSearchService.  
BR-CHAT-03	Chatbot không được query DB riêng.  
BR-CHAT-04	Chatbot chỉ hỏi thêm thông tin, tạo filter, rồi gọi search service.  
BR-CHAT-05	Chatbot không được bịa phòng hoặc giá.  
BR-CHAT-06	Chatbot không nằm trong scope bắt buộc của phase đầu Người 1\.  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
15\. Final Flow của Người 1  
15.1 Flow search/filter chính  
Tenant vào /rooms  
→ Nhập search box hoặc chọn filter  
→ Nếu nhập search box: parse thành filter  
→ Fill filter UI  
→ Gọi search bằng finalFilter  
→ Hiển thị danh sách phòng  
→ User chỉnh filter nhiều lần nếu muốn  
→ Mỗi lần chỉnh filter, search lại theo finalFilter mới  
15.2 Flow xem chi tiết và map  
Tenant bấm vào một phòng/khu trọ  
→ Gọi GET /api/public/rooms/{roomId}  
→ Backend kiểm tra phòng/khu trọ có được public không  
→ Trả thông tin chi tiết  
→ Frontend hiển thị ảnh, giá, tiện ích, địa chỉ  
→ Nếu có latitude/longitude hợp lệ, hiển thị Google Map 1 marker  
→ Nếu không có tọa độ, hiển thị thông báo chưa có vị trí bản đồ  
15.3 Flow AI search parser  
User nhập câu search phức tạp  
→ Rule parser xử lý trước  
→ Nếu rule parser không đủ, gọi AI parser  
→ AI trả JSON filter  
→ Backend validate JSON filter  
→ Map amenity names sang amenityIds  
→ Trả parsedFilter \+ warnings  
→ Frontend fill filter UI  
→ Search bằng finalFilter  
\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_\_  
16\. Acceptance Criteria  
AC ID	Acceptance Criteria  
AC-01	User vào /rooms xem được danh sách phòng public available.  
AC-02	Không hiển thị phòng thuộc khu trọ chưa approved hoặc hidden.  
AC-03	User lọc được theo keyword, giá, số người, diện tích, khu vực, tiện ích.  
AC-04	User search box “phòng dưới 3 triệu cho 2 người có máy lạnh” thì hệ thống parse đúng giá, số người, tiện ích.  
AC-05	Sau khi search box, filter UI tự fill các điều kiện đã parse.  
AC-06	User chỉnh filter sau khi search box và kết quả cập nhật theo filter mới.  
AC-07	User xóa từng filter tag và kết quả cập nhật lại.  
AC-08	User bấm “Clear all” thì reset toàn bộ điều kiện search.  
AC-09	User bấm vào phòng mở được trang chi tiết.  
AC-10	Trang chi tiết hiển thị ảnh, giá, tiện ích, địa chỉ.  
AC-11	Nếu phòng có tọa độ hợp lệ, trang chi tiết hiển thị Google Map với 1 marker.  
AC-12	Nếu phòng thiếu tọa độ, trang chi tiết không lỗi và hiển thị fallback message.  
AC-13	AI chỉ parse filter, không trả danh sách phòng trực tiếp.  
AC-14	Nếu AI lỗi, hệ thống fallback về rule parser hoặc keyword search.  
AC-15	Chatbot chưa cần hoàn thành trong phase đầu Người 1\.

