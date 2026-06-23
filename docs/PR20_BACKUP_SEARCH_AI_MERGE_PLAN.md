# Ke hoach merge PR20 voi search rule-based va AI tu backup

## 1. Muc tieu

Muc tieu cua merge nay la giu PR20 lam source chinh, vi PR20 dang chua phan lon logic chuan ve rental contract, deposit, wallet payment, invoice va appendix flow.

Tu branch `backup/my-current-code`, chi port cac phan sau:

- Public rooming house search.
- Rule-based Vietnamese search parser.
- Rule-based recommendation scorer.
- AI search fallback bang Gemini.
- AI guest recommendation reranker bang Gemini.
- Frontend guest behavior tracking.
- Frontend search/recommendation UI integration.

Khong merge nguyen branch `backup/my-current-code` vao PR20.

## 2. Branch va nguon code

Branch PR20 hien tai:

```bash
review/pr-20-rental-contract
```

Backup code hien tai cua minh:

```bash
backup/my-current-code
```

Nhanh merge nen tao moi tu PR20:

```bash
git switch review/pr-20-rental-contract
git status --short --branch
git switch -c merge/pr20-search-ai
```

Ky vong:

```text
## merge/pr20-search-ai
```

## 3. Nguyen tac merge

- PR20 la base chinh.
- Backup chi la nguon tham khao de port search/recommendation.
- Khong replace nguyen file lon neu file do da bi PR20 thay doi.
- Uu tien patch thu cong tung khoi logic nho.
- Moi step phai build/check truoc khi sang step tiep theo.
- Khong merge migration tu backup.
- Khong merge API key that trong `appsettings.json`.
- Khong keo nguoc ten cu `LeasePolicy` neu PR20 da doi sang `RentalPolicy`.

## 4. Nhung thu tuyet doi khong duoc merge thang

### 4.1. Appsettings co secret

Backup co cau hinh Gemini kem API key that. Khong duoc merge key nay.

Chi nen giu dang placeholder:

```json
"Gemini": {
  "ApiKey": "",
  "Model": "gemini-2.5-flash",
  "TimeoutSeconds": 8,
  "Enabled": false,
  "UseAiSearchFallback": true,
  "UseAiGuestRecommendations": true
}
```

Neu key da tung commit len branch backup/GitHub, nen rotate hoac revoke key do.

### 4.2. LeasePolicy cu

Backup van dung nhieu logic cu:

- `LeasePolicy`
- `IRoomingHouseLeasePolicyService`
- `/lease-policy`
- frontend type `LeasePolicy`

PR20 da chuan hoa theo:

- `RentalPolicy`
- `IRoomingHouseRentalPolicyService`
- `/rental-policy`
- frontend type `RentalPolicy`

Khi port code tu backup, phai doi theo PR20.

### 4.3. Dependency Injection cu

Khong replace nguyen cac file DI tu backup, vi PR20 co nhieu registration quan trong:

- Rental request.
- Room deposit.
- Rental contract.
- Contract appendix.
- Wallet.
- Billing.
- PayOS.
- Background workers.
- Data protection.

Chi them registration moi cho search/AI.

### 4.4. Migration

Khong port migration tu backup.

PR20 dang co migration/schema rieng cho contract, wallet, billing, appendix. Neu backup khong lien quan DB schema search/recommendation thi khong can migration.

## 5. Backend merge plan

### Step 1: Port shared contract DTO

Port cac file tu backup:

- `GuestRoomingHouseRecommendationRequest`
- `RoomingHouseRecommendationResponse`
- `RoomingHouseSearchMetadataResponse`

Update:

- `PagedResult<T>` them property:

```csharp
public object? Metadata { get; set; }
```

Luu y:

- Day la thay doi additive.
- Khong sua cac field cu cua `PagedResult<T>`.
- Khong sua DTO cua rental contract, wallet, invoice, appendix.

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Pass condition:

- Build pass.
- Khong co loi compile lien quan `PagedResult<T>`.

### Step 2: Port search parser

Port cac file:

- `RoomingHouseSearchParser`
- `ParsedRoomingHouseSearchCriteria`
- Cac helper lien quan neu co, vi du query normalizer/alias parser.

Logic can giu:

- Parse query tieng Viet.
- Parse gia nhu `2tr`, `2tr5`, `duoi 3 trieu`.
- Parse dien tich.
- Parse so nguoi.
- Parse dia diem.
- Parse tien ich.
- Tach keyword con lai sau khi da lay structured criteria.

Luu y khi port:

- Kiem tra namespace enum trong PR20.
- Neu PR20 da tach enum theo namespace moi, update using cho dung.
- Khong them lai dependency cu khong con ton tai.

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Test nen co:

- Query: `phong gan fpt da nang duoi 3 trieu`
- Query: `2 nguoi co may lanh`
- Query: `ngu hanh son 20m2`
- Query rong/null.

Pass condition:

- Parser khong throw.
- Criteria parse ra dung price/location/area/occupants neu co.
- Query rong van an toan.

### Step 3: Port search/recommendation interfaces va model noi bo

Port cac file:

- `IRoomingHouseSearchIntentEnricher`
- `IRoomingHouseRecommendationScorer`
- `IRoomingHouseRecommendationReranker`
- `RoomingHouseSearchIntentContext`
- `RoomingHouseSearchCandidate`
- `RoomingHouseRecommendationCandidate`
- `RoomingHouseRecommendationRerankResult`
- `NoopRoomingHouseSearchIntentEnricher`
- `RuleBasedRoomingHouseRecommendationScorer`

Luu y:

- Day la cac class kha doc lap.
- Neu co namespace conflict, doi theo structure cua PR20.
- Chua can wire DI neu build chua sach.

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Pass condition:

- Build pass.
- Khong co loi namespace/type missing.

### Step 4: Port Gemini infrastructure

Port cac file:

- `IAiStructuredOutputService`
- `GeminiStructuredOutputService`
- `GeminiOptions`
- `GeminiRoomingHouseSearchIntentEnricher`
- `GeminiRoomingHouseRecommendationReranker`

Luu y:

- Gemini phai fail gracefully.
- Khi `Gemini.Enabled=false`, he thong chi chay rule-based.
- Khi thieu API key, khong duoc throw.
- Khong hard-code API key.

Infrastructure DI chi them:

```csharp
services.Configure<GeminiOptions>(configuration.GetSection(GeminiOptions.SectionName));

services.AddHttpClient<IAiStructuredOutputService, GeminiStructuredOutputService>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
});
```

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Pass condition:

- Build pass.
- App co the chay khi Gemini disabled.

### Step 5: Patch `RoomingHouseQueryService`

Day la step rui ro cao nhat. Khong copy nguyen file tu backup.

Can lam:

- Mo file `RoomingHouseQueryService` hien tai cua PR20.
- Giu toan bo logic san co cua PR20.
- Them method public search.
- Them method guest recommendation.
- Them cac helper scoring/filter/sort can thiet tu backup.
- Update moi `LeasePolicy` thanh `RentalPolicy`.
- Update include/entity theo domain hien tai cua PR20.

Can them method:

```csharp
Task<PagedResult<RoomingHouseSearchItemResponse>> SearchPublicAsync(
    RoomingHouseSearchRequest request,
    CancellationToken cancellationToken = default);

Task<RoomingHouseRecommendationResponse> GetGuestRecommendationsAsync(
    GuestRoomingHouseRecommendationRequest request,
    CancellationToken cancellationToken = default);
```

Neu backup co:

```csharp
.Include(x => x.LeasePolicy)
```

Doi theo PR20:

```csharp
.Include(x => x.RentalPolicy)
```

Can canh giac:

- `HouseRule` trong backup co the da doi ten trong PR20.
- Namespace enum co the khac.
- Query projection co the dung property cu.
- Endpoint public cu cua PR20 khong duoc bi doi behavior.

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Manual API test:

```bash
curl "http://localhost:<port>/api/public/rooming-houses/search?q=phong%20gan%20fpt%20duoi%203tr&page=1&pageSize=8"
```

Pass condition:

- HTTP 200.
- Co data hoac empty list hop le.
- Response co `metadata`.
- API public list cu van hoat dong.

### Step 6: Patch `IRoomingHouseQueryService`

Them 2 method moi vao interface:

```csharp
Task<PagedResult<RoomingHouseSearchItemResponse>> SearchPublicAsync(
    RoomingHouseSearchRequest request,
    CancellationToken cancellationToken = default);

Task<RoomingHouseRecommendationResponse> GetGuestRecommendationsAsync(
    GuestRoomingHouseRecommendationRequest request,
    CancellationToken cancellationToken = default);
```

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Pass condition:

- Khong co class nao implement interface bi loi.

### Step 7: Patch `PublicRoomingHousesController`

Them endpoint:

```http
GET /api/public/rooming-houses/search
POST /api/public/rooming-houses/recommendations/guest
```

Khong replace controller tu backup.

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Manual test:

```bash
curl "http://localhost:<port>/api/public/rooming-houses/search?q=da%20nang"
```

```bash
curl -X POST "http://localhost:<port>/api/public/rooming-houses/recommendations/guest" ^
  -H "Content-Type: application/json" ^
  -d "{\"recentQueries\":[\"gan fpt\"],\"pageSize\":8}"
```

Pass condition:

- Search endpoint tra 200.
- Recommendation endpoint tra 200.
- Khi khong co behavior, recommendation fallback hop le.

### Step 8: Patch Application DI

Chi them registration moi, khong xoa service PR20:

```csharp
services.AddScoped<IRoomingHouseSearchIntentEnricher, NoopRoomingHouseSearchIntentEnricher>();
services.AddScoped<IRoomingHouseSearchIntentEnricher, GeminiRoomingHouseSearchIntentEnricher>();
services.AddScoped<IRoomingHouseRecommendationScorer, RuleBasedRoomingHouseRecommendationScorer>();
services.AddScoped<IRoomingHouseRecommendationReranker, GeminiRoomingHouseRecommendationReranker>();
```

Neu parser chua duoc register:

```csharp
services.AddScoped<IRoomingHouseSearchParser, RoomingHouseSearchParser>();
```

Check:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Pass condition:

- Build pass.
- App start khong loi DI.

## 6. Frontend merge plan

### Step 9: Port local behavior storage

Port file:

- `client/src/features/rooming-houses/rentalBehaviorStorage.ts`

Logic can giu:

- LocalStorage key cho guest behavior.
- Luu recent queries.
- Luu rooming house da xem/click.
- Luu preferred amenities.
- Luu province/ward.
- Luu price/area preference.
- TTL 30 ngay.
- Convert sang request recommendation.

Check:

```bash
cd client
npm run build
```

Pass condition:

- Build pass.
- Khong loi type import/export.

### Step 10: Patch frontend API va types

Patch:

- `client/src/features/rooming-houses/api.ts`
- `client/src/features/rooming-houses/types.ts`
- Neu PR20 co endpoint constants, patch them vao `client/src/shared/api/endpoints.ts`.

Them API:

- `searchPublicRoomingHouses`
- `getGuestRoomingHouseRecommendations`

Them types:

- `RoomingHouseSearchRequest`
- `RoomingHouseSearchMetadata`
- `GuestRoomingHouseRecommendationRequest`
- `RoomingHouseRecommendationResponse`

Luu y:

- Doi `LeasePolicy` thanh `RentalPolicy`.
- Doi `/lease-policy` thanh `/rental-policy` neu co.
- Uu tien endpoint constants cua PR20 thay vi hard-code path moi.

Check:

```bash
cd client
npm run build
```

Pass condition:

- Build pass.
- Khong loi type `LeasePolicy` missing.
- Khong pha API hien tai cua PR20.

### Step 11: Patch search page

Patch:

- `SearchRoomingHousesPage.tsx`

Can them:

- Goi `searchPublicRoomingHouses`.
- Luu search behavior bang `saveSearchBehavior`.
- Hien thi metadata AI neu UI phu hop.
- Fallback neu API search loi.

Khong lam:

- Khong replace layout neu PR20 da refactor.
- Khong xoa filter/sort san co cua PR20.

Check:

```bash
cd client
npm run build
```

Manual test:

- Search query tieng Viet.
- Doi filter gia/dien tich.
- Sort theo gia.
- Click item va qua detail.

### Step 12: Patch public detail pages

Patch:

- `PublicRoomingHouseDetailPage.tsx`
- `PublicRoomDetailPage.tsx`

Can them:

- Khi user xem/click nha tro, goi `saveRoomingHouseView(roomingHouseId)`.

Check:

```bash
cd client
npm run build
```

Manual test:

- Vao detail nha tro.
- Kiem tra localStorage co behavior.
- Back ve home/search, recommendation co the dung behavior do.

### Step 13: Patch home/me page

Patch:

- `MePage.tsx`
- `MePage.css` neu can.

Can them:

- Neu co usable behavior, goi `getGuestRoomingHouseRecommendations`.
- Neu recommendation co item, hien thi personalized listings.
- Neu recommendation empty/error, fallback public listings PR20 dang co.
- Khi click listing, save view behavior.

Luu y:

- PR20 co the da refactor account/profile/wallet routes.
- Khong lay nguyen `MePage.tsx` tu backup.
- Neu backup dung route cu nhu `ROUTE_PATHS.ME.PROFILE`, can doi theo route hien tai cua PR20.

Check:

```bash
cd client
npm run build
```

Manual test:

- Clear localStorage.
- Vao home: thay public listing fallback.
- Search/click vai item.
- Quay lai home: thay personalized listing neu backend co data.

## 7. Full backend regression

Chay:

```bash
dotnet build server/src/SmartRentalPlatform.sln
```

Neu co test project:

```bash
dotnet test server/src/SmartRentalPlatform.sln
```

Manual test backend can co:

- Public rooming house list cu.
- Public rooming house detail cu.
- Public search moi.
- Guest recommendation moi.
- Rental request flow cua PR20.
- Deposit flow cua PR20.
- Wallet payment flow cua PR20.
- Contract preview/sign OTP flow cua PR20.
- Invoice flow cua PR20.
- Appendix flow cua PR20.
- Background worker khong loi start.

## 8. Full frontend regression

Chay:

```bash
cd client
npm run build
```

Neu co e2e:

```bash
cd client
npm run test:e2e
```

Manual test frontend can co:

- Home page.
- Search page.
- Detail nha tro.
- Detail phong.
- Login tenant.
- Rental request.
- Deposit.
- Wallet payment.
- Contract page.
- Sign contract.
- Appendix page.
- Account/profile/wallet/invoice pages.

## 9. Security check truoc commit

Chay:

```bash
rg "<<<<<<<|=======|>>>>>>>" .
rg "LeasePolicy|lease-policy" client server
rg "Gemini.*ApiKey|ApiKey" server/src/SmartRentalPlatform.Api
git diff --check
```

Can dam bao:

- Khong con conflict marker.
- Khong co API key that.
- Khong keo lai `LeasePolicy` o noi PR20 da doi sang `RentalPolicy`.
- Khong co whitespace error.

## 10. Diff review truoc commit

Chay:

```bash
git diff --stat
git diff --name-status
git diff
```

Review dac biet cac file:

- `RoomingHouseQueryService.cs`
- `IRoomingHouseQueryService.cs`
- `PublicRoomingHousesController.cs`
- `Application.DependencyInjection`
- `Infrastructure.DependencyInjection`
- `appsettings.json`
- `client/src/features/rooming-houses/api.ts`
- `client/src/features/rooming-houses/types.ts`
- `MePage.tsx`
- `SearchRoomingHousesPage.tsx`

## 11. Commit

Chi commit sau khi backend va frontend build pass.

```bash
git status --short
git add .
git commit -m "Merge AI rooming house search into PR20 flow"
```

## 12. Thu tu lam khuyen nghi

Thu tu it rui ro nhat:

1. Tao branch merge tu PR20.
2. Port backend DTO.
3. Port parser.
4. Port recommendation/search interfaces.
5. Port Gemini infra.
6. Patch `RoomingHouseQueryService`.
7. Patch interface va controller.
8. Patch DI.
9. Build backend.
10. Port frontend behavior storage.
11. Patch frontend API/types.
12. Patch search page.
13. Patch detail pages.
14. Patch home/me page.
15. Build frontend.
16. Manual regression.
17. Security check.
18. Commit.

## 13. Definition of done

Merge duoc xem la xong khi:

- `dotnet build server/src/SmartRentalPlatform.sln` pass.
- `npm run build` trong `client` pass.
- Public search moi tra 200.
- Guest recommendation moi tra 200.
- Rental contract/deposit/wallet/appendix flow cua PR20 van chay.
- Khong co API key that trong diff.
- Khong co conflict marker.
- Khong co migration ngoai y muon.
- Khong co rollback tu `RentalPolicy` ve `LeasePolicy`.

