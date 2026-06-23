**\# Smart Rental Platform \- ERD chính Interval 2**

Ngày tạo: 01/06/2026    
Phạm vi: Tổng hợp Interval 2 dựa trên project structure, ERD tổng quát Interval 1 và business rules của 5 người.

**\#\# 1\. Chốt phạm vi Interval 2 theo 5 người**

| Người | Module Interval 2 | Bảng chính |  
|---|---|---|  
| Người 1 | Public Search, Filter, AI Search Parser, Google Map Detail | \`rooming\_houses\`, \`rooms\`, \`room\_price\_tiers\`, \`amenities\`, \`property\_images\`, \`rental\_policies\` |  
| Người 2 | Viewing Appointment | \`viewing\_appointments\`, \`rooms\`, \`rooming\_houses\`, \`users\`, \`notifications\` |  
| Người 3 | Rental Request, Deposit, Contract Form, OTP Signing | \`rental\_requests\`, \`room\_deposits\`, \`contracts\`, \`contract\_occupants\`, \`contract\_occupant\_documents\`, \`contract\_files\`, \`contract\_signatures\`, \`user\_tokens\` |  
| Người 4 | Billing, Meter Reading, Invoice | \`billing\_service\_types\`, \`rooming\_house\_service\_prices\`, \`meter\_readings\`, \`invoices\`, \`invoice\_items\`, \`invoice\_payments\` |  
| Người 5 | Wallet, PayOS Top-up, Mock Payment, Wallet Payment Core | \`wallet\_accounts\`, \`wallet\_transactions\`, \`payment\_transactions\`, \`payment\_webhook\_logs\` |

**\#\# 2\. Nguyên tắc thiết kế chính**

\- Public search chỉ đọc dữ liệu đã được admin duyệt: khu trọ \`Approved\`, \`Visible\`, phòng \`Available\`.  
\- Viewing appointment chỉ xử lý lịch xem phòng; không xử lý cọc, hợp đồng, hóa đơn.  
\- Rental request sau khi landlord approve sẽ tạo khoản cọc có hạn thanh toán 2 tiếng.  
\- Cọc và hóa đơn đều thanh toán bằng ví nội bộ; PayOS chỉ dùng để nạp tiền vào ví.  
\- Chỉ tài khoản đã KYC/eKYC \`Approved\` mới được tạo yêu cầu nạp ví PayOS.  
\- Hóa đơn lấy giá dịch vụ hiện hành của khu trọ tại \`billing\_period\_end\`, sau đó snapshot đơn giá vào \`invoice\_items.unit\_price\`.  
\- Mọi biến động tiền phải ghi vào \`wallet\_transactions\`, chạy trong database transaction và chống double payment bằng \`idempotency\_key\`, \`transfer\_group\_id\`, trạng thái giao dịch.

**\#\# 3\. Mermaid ERD chính**

\`\`\`mermaid  
erDiagram  
    users {  
        uuid id PK  
        string email UK  
        string normalized\_email UK  
        string phone\_number  
        text password\_hash  
        string display\_name  
        text avatar\_url  
        string status  
        string onboarding\_status  
        boolean email\_confirmed  
        boolean phone\_confirmed  
        int access\_failed\_count  
        datetime lockout\_end\_at  
        datetime last\_login\_at  
        datetime created\_at  
        datetime updated\_at  
        datetime deleted\_at  
    }

    user\_profiles {  
        uuid user\_id PK,FK  
        string full\_name  
        date date\_of\_birth  
        string gender  
        text address\_line  
        string verified\_citizen\_id\_masked  
        string emergency\_contact\_name  
        string emergency\_contact\_phone  
        datetime created\_at  
        datetime updated\_at  
    }

    roles {  
        int id PK  
        string name UK  
        text description  
        datetime created\_at  
    }

    user\_roles {  
        uuid user\_id PK,FK  
        int role\_id PK,FK  
        datetime created\_at  
    }

    external\_logins {  
        uuid id PK  
        uuid user\_id FK  
        string provider  
        string provider\_user\_id  
        string provider\_email  
        string provider\_display\_name  
        text provider\_avatar\_url  
        datetime created\_at  
        datetime last\_login\_at  
    }

    user\_tokens {  
        uuid id PK  
        uuid user\_id FK  
        string token\_type  
        text token\_hash UK  
        uuid token\_family\_id  
        uuid replaced\_by\_token\_id FK  
        string related\_entity\_type  
        uuid related\_entity\_id  
        datetime expires\_at  
        datetime used\_at  
        datetime revoked\_at  
        string revoked\_reason  
        string created\_by\_ip  
        text user\_agent  
        datetime created\_at  
    }

    login\_logs {  
        uuid id PK  
        uuid user\_id FK  
        string login\_provider  
        string email\_attempted  
        string ip\_address  
        text user\_agent  
        boolean is\_success  
        string failure\_reason  
        datetime created\_at  
    }

    kyc\_verifications {  
        uuid id PK  
        uuid user\_id FK  
        string document\_type  
        string ekyc\_provider  
        string ekyc\_session\_id  
        text front\_image\_object\_key  
        text back\_image\_object\_key  
        text selfie\_image\_object\_key  
        string ocr\_full\_name  
        string ocr\_citizen\_id\_masked  
        text citizen\_id\_hash  
        date ocr\_date\_of\_birth  
        string ocr\_gender  
        text ocr\_address  
        decimal ocr\_confidence  
        decimal face\_match\_score  
        string liveness\_result  
        string risk\_level  
        string status  
        uuid reviewed\_by\_admin\_id FK  
        text rejected\_reason  
        datetime submitted\_at  
        datetime reviewed\_at  
        datetime created\_at  
        datetime updated\_at  
    }

    provinces {  
        string code PK  
        string name  
        string type  
        boolean is\_active  
        datetime created\_at  
        datetime updated\_at  
    }

    wards {  
        string code PK  
        string province\_code FK  
        string name  
        string type  
        boolean is\_active  
        datetime created\_at  
        datetime updated\_at  
    }

    notifications {  
        uuid id PK  
        uuid recipient\_user\_id FK  
        uuid actor\_user\_id FK  
        string type  
        string title  
        text message  
        string entity\_type  
        uuid entity\_id  
        text action\_url  
        string priority  
        string status  
        datetime read\_at  
        datetime created\_at  
    }

    approval\_audit\_logs {  
        uuid id PK  
        uuid admin\_id FK  
        string approval\_type  
        uuid entity\_id  
        string action  
        text reason  
        text additional\_info  
        datetime created\_at  
    }

    rooming\_houses {  
        uuid id PK  
        uuid landlord\_user\_id FK  
        string name  
        text description  
        text address\_line  
        string ward\_code FK  
        string province\_code FK  
        text address\_display  
        decimal latitude  
        decimal longitude  
        string approval\_status  
        string visibility\_status  
        text rejected\_reason  
        uuid reviewed\_by\_admin\_id FK  
        datetime reviewed\_at  
        datetime created\_at  
        datetime updated\_at  
        datetime deleted\_at  
    }

    rooming\_house\_legal\_documents {  
        uuid id PK  
        uuid rooming\_house\_id FK  
        string document\_type  
        text front\_image\_object\_key  
        text back\_image\_object\_key  
        text extra\_image\_object\_key  
        string document\_number\_masked  
        text document\_number\_hash  
        string status  
        uuid reviewed\_by\_admin\_id FK  
        datetime reviewed\_at  
        text rejected\_reason  
        datetime uploaded\_at  
        datetime created\_at  
        datetime updated\_at  
    }

    rooms {  
        uuid id PK  
        uuid rooming\_house\_id FK  
        string room\_number  
        int floor  
        decimal area\_m2  
        int max\_occupants  
        boolean is\_tiered\_pricing  
        string status  
        text description  
        datetime created\_at  
        datetime updated\_at  
        datetime deleted\_at  
    }

    room\_price\_tiers {  
        uuid id PK  
        uuid room\_id FK  
        int occupant\_count  
        decimal monthly\_rent  
        boolean is\_active  
        datetime created\_at  
        datetime updated\_at  
    }

    amenities {  
        uuid id PK  
        string name  
        string scope  
        string icon\_code  
        boolean is\_active  
        datetime created\_at  
    }

    room\_amenities {  
        uuid room\_id PK,FK  
        uuid amenity\_id PK,FK  
    }

    rooming\_house\_amenities {  
        uuid rooming\_house\_id PK,FK  
        uuid amenity\_id PK,FK  
    }

    property\_images {  
        uuid id PK  
        uuid rooming\_house\_id FK  
        uuid room\_id FK  
        text object\_key  
        text image\_url  
        string caption  
        boolean is\_cover  
        int sort\_order  
        datetime created\_at  
    }

    rental\_policies {  
        uuid id PK  
        uuid rooming\_house\_id FK  
        int min\_rental\_months  
        int max\_rental\_months  
        boolean allow\_short\_term\_renewal  
        int renewal\_notice\_days  
        decimal deposit\_months  
        boolean is\_active  
        datetime created\_at  
        datetime updated\_at  
    }

    viewing\_appointments {  
        uuid id PK  
        uuid room\_id FK  
        uuid tenant\_user\_id FK  
        uuid created\_by\_user\_id FK  
        datetime scheduled\_at  
        int duration\_minutes  
        string status  
        text tenant\_note  
        text landlord\_note  
        text cancel\_reason  
        datetime responded\_at  
        datetime created\_at  
        datetime updated\_at  
    }

    rental\_requests {  
        uuid id PK  
        uuid room\_id FK  
        uuid tenant\_user\_id FK  
        uuid approved\_by\_landlord\_id FK  
        date desired\_start\_date  
        date expected\_end\_date  
        int expected\_occupant\_count  
        decimal monthly\_rent\_snapshot  
        decimal deposit\_amount\_snapshot  
        text tenant\_note  
        string status  
        datetime responded\_at  
        text rejected\_reason  
        datetime created\_at  
        datetime updated\_at  
    }

    room\_deposits {  
        uuid id PK  
        uuid rental\_request\_id FK  
        uuid room\_id FK  
        uuid tenant\_user\_id FK  
        uuid landlord\_user\_id FK  
        decimal deposit\_amount  
        string currency  
        string status  
        datetime payment\_deadline\_at  
        datetime paid\_at  
        datetime refunded\_at  
        datetime forfeited\_at  
        decimal refund\_amount  
        decimal forfeited\_amount  
        uuid deposit\_decision\_by\_landlord\_id FK  
        datetime deposit\_decision\_at  
        text deposit\_decision\_reason  
        text note  
        datetime created\_at  
        datetime updated\_at  
    }

    contracts {  
        uuid id PK  
        uuid rental\_request\_id FK  
        uuid room\_deposit\_id FK  
        uuid room\_id FK  
        uuid main\_tenant\_user\_id FK  
        string contract\_number UK  
        date start\_date  
        date end\_date  
        decimal monthly\_rent  
        decimal deposit\_amount  
        int payment\_day  
        string status  
        json room\_snapshot  
        datetime draft\_completed\_at  
        datetime main\_tenant\_signature\_deadline\_at  
        datetime main\_tenant\_signed\_at  
        datetime landlord\_signature\_deadline\_at  
        datetime landlord\_signed\_at  
        datetime activated\_at  
        datetime signature\_expired\_at  
        string signature\_expired\_reason  
        text rejected\_reason  
        datetime created\_at  
        datetime updated\_at  
        datetime deleted\_at  
    }

    contract\_occupants {  
        uuid id PK  
        uuid contract\_id FK  
        uuid user\_id FK  
        uuid guardian\_occupant\_id FK  
        uuid kyc\_verification\_id FK  
        string email  
        string full\_name  
        string phone\_number  
        date date\_of\_birth  
        string gender  
        string residence\_role  
        string relationship\_to\_main\_tenant  
        boolean is\_ekyc\_verified  
        string document\_type  
        string document\_number\_masked  
        text document\_number\_hash  
        string document\_verification\_status  
        date move\_in\_date  
        date move\_out\_date  
        string status  
        uuid entered\_by\_user\_id FK  
        datetime entered\_at  
        uuid reviewed\_by\_landlord\_id FK  
        datetime reviewed\_at  
        text rejected\_reason  
        datetime created\_at  
        datetime updated\_at  
    }

    contract\_occupant\_documents {  
        uuid id PK  
        uuid contract\_occupant\_id FK  
        string document\_type  
        string document\_file\_type  
        text file\_object\_key  
        uuid uploaded\_by\_user\_id FK  
        string status  
        uuid reviewed\_by\_landlord\_id FK  
        datetime reviewed\_at  
        text rejected\_reason  
        datetime uploaded\_at  
    }

    contract\_files {  
        uuid id PK  
        uuid contract\_id FK  
        string file\_type  
        int file\_version  
        text storage\_object\_key  
        text file\_url  
        text file\_hash  
        datetime generated\_at  
        datetime created\_at  
    }

    contract\_signatures {  
        uuid id PK  
        uuid contract\_id FK  
        uuid signer\_user\_id FK  
        string signer\_role  
        string signature\_method  
        uuid otp\_token\_id FK  
        text contract\_snapshot\_hash  
        datetime signed\_at  
        datetime created\_at  
    }

    billing\_service\_types {  
        uuid id PK  
        string code UK  
        string name  
        boolean is\_metered  
        boolean is\_active  
        datetime created\_at  
    }

    rooming\_house\_service\_prices {  
        uuid id PK  
        uuid rooming\_house\_id FK  
        uuid service\_type\_id FK  
        string billing\_method  
        string unit\_name  
        decimal unit\_price  
        date effective\_from  
        date effective\_to  
        boolean is\_active  
        text note  
        datetime created\_at  
        datetime updated\_at  
    }

    meter\_readings {  
        uuid id PK  
        uuid room\_id FK  
        uuid contract\_id FK  
        uuid service\_type\_id FK  
        date billing\_period\_start  
        date billing\_period\_end  
        decimal previous\_reading  
        decimal current\_reading  
        decimal consumption  
        text proof\_image\_object\_key  
        string status  
        uuid recorded\_by\_landlord\_user\_id FK  
        datetime reading\_at  
        datetime created\_at  
        datetime updated\_at  
    }

    invoices {  
        uuid id PK  
        uuid contract\_id FK  
        uuid room\_id FK  
        uuid tenant\_user\_id FK  
        uuid landlord\_user\_id FK  
        string invoice\_no UK  
        date billing\_period\_start  
        date billing\_period\_end  
        date issue\_date  
        date due\_date  
        decimal rent\_amount  
        decimal utility\_amount  
        decimal service\_amount  
        decimal discount\_amount  
        decimal total\_amount  
        decimal paid\_amount  
        decimal remaining\_amount  
        string status  
        text note  
        datetime sent\_at  
        datetime paid\_at  
        datetime cancelled\_at  
        text cancel\_reason  
        datetime created\_at  
        datetime updated\_at  
    }

    invoice\_items {  
        uuid id PK  
        uuid invoice\_id FK  
        uuid service\_type\_id FK  
        uuid meter\_reading\_id FK  
        string item\_type  
        text description  
        decimal quantity  
        decimal unit\_price  
        decimal amount  
        datetime created\_at  
    }

    invoice\_payments {  
        uuid id PK  
        uuid invoice\_id FK  
        uuid tenant\_user\_id FK  
        uuid landlord\_user\_id FK  
        decimal amount  
        uuid wallet\_transfer\_group\_id  
        string status  
        datetime paid\_at  
        datetime created\_at  
    }

    wallet\_accounts {  
        uuid id PK  
        uuid user\_id FK  
        decimal balance  
        decimal reserved\_balance  
        string currency  
        string status  
        datetime created\_at  
        datetime updated\_at  
    }

    wallet\_transactions {  
        uuid id PK  
        uuid wallet\_account\_id FK  
        uuid user\_id FK  
        uuid transfer\_group\_id  
        string transaction\_type  
        string direction  
        decimal amount  
        decimal balance\_before  
        decimal balance\_after  
        decimal reserved\_balance\_before  
        decimal reserved\_balance\_after  
        string related\_entity\_type  
        uuid related\_entity\_id  
        text description  
        string status  
        datetime created\_at  
    }

    payment\_transactions {  
        uuid id PK  
        uuid wallet\_account\_id FK  
        uuid payer\_user\_id FK  
        string idempotency\_key UK  
        decimal amount  
        string currency  
        string payment\_purpose  
        string payment\_method  
        string provider\_order\_code UK  
        string provider\_transaction\_code  
        text provider\_checkout\_url  
        text provider\_qr\_code  
        string gateway\_response\_code  
        text gateway\_response\_message  
        string status  
        datetime expires\_at  
        datetime paid\_at  
        datetime failed\_at  
        datetime confirmed\_at  
        datetime created\_at  
        datetime updated\_at  
    }

    payment\_webhook\_logs {  
        uuid id PK  
        uuid payment\_transaction\_id FK  
        string payment\_method  
        string provider\_event\_id  
        string provider\_order\_code  
        string provider\_transaction\_code  
        string idempotency\_key  
        text raw\_payload  
        text raw\_payload\_hash UK  
        string signature\_status  
        string processing\_status  
        text error\_message  
        int retry\_count  
        datetime received\_at  
        datetime processed\_at  
        datetime created\_at  
    }

    users ||--o| user\_profiles : has\_profile  
    users ||--o{ user\_roles : has\_roles  
    roles ||--o{ user\_roles : assigned\_to\_users  
    users ||--o{ external\_logins : has\_external\_logins  
    users ||--o{ user\_tokens : owns\_tokens  
    user\_tokens ||--o| user\_tokens : replaced\_by  
    users ||--o{ login\_logs : has\_login\_logs  
    users ||--o{ kyc\_verifications : submits\_kyc  
    users ||--o{ kyc\_verifications : reviews\_kyc  
    users ||--o{ notifications : receives\_notifications  
    users ||--o{ notifications : triggers\_notifications  
    users ||--o{ approval\_audit\_logs : performs\_admin\_action

    provinces ||--o{ wards : has\_wards  
    provinces ||--o{ rooming\_houses : located\_in\_province  
    wards ||--o{ rooming\_houses : located\_in\_ward

    users ||--o{ rooming\_houses : owns\_houses  
    users ||--o{ rooming\_houses : reviews\_houses  
    rooming\_houses ||--o{ rooming\_house\_legal\_documents : has\_legal\_documents  
    users ||--o{ rooming\_house\_legal\_documents : reviews\_documents  
    rooming\_houses ||--o{ rooms : contains\_rooms  
    rooms ||--o{ room\_price\_tiers : has\_price\_tiers  
    amenities ||--o{ room\_amenities : used\_by\_rooms  
    rooms ||--o{ room\_amenities : has\_room\_amenities  
    amenities ||--o{ rooming\_house\_amenities : used\_by\_houses  
    rooming\_houses ||--o{ rooming\_house\_amenities : has\_house\_amenities  
    rooming\_houses ||--o{ property\_images : has\_house\_images  
    rooms ||--o{ property\_images : has\_room\_images  
    rooming\_houses ||--o{ rental\_policies : has\_rental\_policies

    users ||--o{ viewing\_appointments : creates\_viewings  
    users ||--o{ viewing\_appointments : books\_viewings  
    rooms ||--o{ viewing\_appointments : has\_viewings

    users ||--o{ rental\_requests : sends\_rental\_requests  
    users ||--o{ rental\_requests : approves\_rental\_requests  
    rooms ||--o{ rental\_requests : receives\_rental\_requests  
    rental\_requests ||--o| room\_deposits : creates\_deposit  
    rooms ||--o{ room\_deposits : reserved\_by\_deposits  
    users ||--o{ room\_deposits : pays\_deposits  
    users ||--o{ room\_deposits : receives\_deposits  
    users ||--o{ room\_deposits : decides\_deposit\_outcome

    rental\_requests ||--o| contracts : creates\_contract  
    room\_deposits ||--o| contracts : secures\_contract  
    rooms ||--o{ contracts : has\_contracts  
    users ||--o{ contracts : main\_tenant  
    contracts ||--o{ contract\_occupants : has\_occupants  
    users ||--o{ contract\_occupants : may\_have\_account  
    kyc\_verifications ||--o{ contract\_occupants : used\_for\_auto\_fill  
    contract\_occupants ||--o{ contract\_occupants : guardian\_of  
    users ||--o{ contract\_occupants : enters\_occupants  
    users ||--o{ contract\_occupants : reviews\_occupants  
    contract\_occupants ||--o{ contract\_occupant\_documents : has\_documents  
    users ||--o{ contract\_occupant\_documents : uploads\_documents  
    users ||--o{ contract\_occupant\_documents : reviews\_occupant\_documents  
    contracts ||--o{ contract\_files : has\_contract\_files  
    contracts ||--o{ contract\_signatures : has\_contract\_signatures  
    users ||--o{ contract\_signatures : signs  
    user\_tokens ||--o{ contract\_signatures : verifies\_signature

    rooming\_houses ||--o{ rooming\_house\_service\_prices : has\_service\_prices  
    billing\_service\_types ||--o{ rooming\_house\_service\_prices : defines\_price\_type  
    rooms ||--o{ meter\_readings : has\_meter\_readings  
    contracts ||--o{ meter\_readings : records\_usage  
    billing\_service\_types ||--o{ meter\_readings : measured\_by  
    users ||--o{ meter\_readings : records\_as\_landlord  
    contracts ||--o{ invoices : has\_invoices  
    rooms ||--o{ invoices : billed\_for\_room  
    users ||--o{ invoices : receives\_invoices  
    users ||--o{ invoices : issues\_invoices  
    invoices ||--o{ invoice\_items : contains\_items  
    billing\_service\_types ||--o{ invoice\_items : item\_service\_type  
    meter\_readings ||--o{ invoice\_items : item\_meter\_reading  
    invoices ||--o{ invoice\_payments : has\_payments  
    users ||--o{ invoice\_payments : pays\_invoice  
    users ||--o{ invoice\_payments : receives\_invoice\_payment

    users ||--o| wallet\_accounts : owns\_wallet  
    wallet\_accounts ||--o{ wallet\_transactions : has\_wallet\_transactions  
    users ||--o{ wallet\_transactions : owns\_wallet\_transactions  
    wallet\_accounts ||--o{ payment\_transactions : topup\_by\_gateway  
    users ||--o{ payment\_transactions : creates\_gateway\_payment  
    payment\_transactions ||--o{ payment\_webhook\_logs : receives\_gateway\_callbacks  
\`\`\`

\#\# 4\. Luồng tổng thể Interval 2

\`\`\`mermaid  
flowchart TD  
    A\[Tenant search/filter phòng public\] \--\> B\[Xem chi tiết phòng và Google Map\]  
    B \--\> C\[Đặt lịch xem phòng\]  
    C \--\> D{Landlord confirm lịch?}  
    D \--\>|Reject/Cancel| E\[Kết thúc lịch\]  
    D \--\>|Confirmed hoặc Completed| F\[Tenant gửi yêu cầu thuê\]  
    F \--\> G{Landlord approve yêu cầu thuê?}  
    G \--\>|Reject| H\[Yêu cầu thuê bị từ chối\]  
    G \--\>|Approve| I\[Tạo room\_deposit WaitingPayment, deadline 2h\]  
    I \--\> J{Ví tenant đủ tiền?}  
    J \--\>|Không| K\[Nạp ví PayOS/Mock\]  
    K \--\> L\[Webhook success, cộng ví\]  
    L \--\> M\[Thanh toán cọc bằng ví\]  
    J \--\>|Có| M  
    M \--\> N\[Room Reserved, Contract Draft\]  
    N \--\> O\[MainTenant điền hợp đồng và người ở\]  
    O \--\> P\[MainTenant hoàn thành draft\]  
    P \--\> Q\[MainTenant ký OTP\]  
    Q \--\> R\[Landlord ký OTP\]  
    R \--\> S\[Contract Active, Room Occupied\]  
    S \--\> T\[Landlord nhập giá dịch vụ và meter readings\]  
    T \--\> U\[Tạo invoice Draft\]  
    U \--\> V\[Issue invoice\]  
    V \--\> W\[Tenant thanh toán invoice bằng ví\]  
    W \--\> X\[Invoice Paid, landlord nhận tiền\]  
\`\`\`

\#\# 5\. State flow chính

\#\#\# 5.1 Viewing Appointment

\`\`\`mermaid  
stateDiagram-v2  
    \[\*\] \--\> Pending  
    Pending \--\> Confirmed: Landlord confirm  
    Pending \--\> Rejected: Landlord reject  
    Pending \--\> CancelledByTenant: Tenant cancel  
    Confirmed \--\> CancelledByTenant: Tenant cancel  
    Confirmed \--\> CancelledByLandlord: Landlord cancel  
    Confirmed \--\> Completed: Landlord complete  
\`\`\`

\#\#\# 5.2 Rental Request \- Deposit \- Contract

\`\`\`mermaid  
stateDiagram-v2  
    \[\*\] \--\> PendingLandlordApproval  
    PendingLandlordApproval \--\> RejectedByLandlord  
    PendingLandlordApproval \--\> WaitingDepositPayment  
    WaitingDepositPayment \--\> DepositExpired  
    WaitingDepositPayment \--\> DepositPaid  
    DepositPaid \--\> ContractDrafting  
    ContractDrafting \--\> ContractSigning  
    ContractSigning \--\> Completed  
    ContractSigning \--\> Cancelled  
\`\`\`

\#\#\# 5.3 Contract

\`\`\`mermaid  
stateDiagram-v2  
    \[\*\] \--\> Draft  
    Draft \--\> PendingMainTenantSignature: Complete draft  
    PendingMainTenantSignature \--\> Draft: MainTenant sửa lại trước khi ký  
    PendingMainTenantSignature \--\> PendingLandlordSignature: MainTenant ký OTP  
    PendingMainTenantSignature \--\> SignatureExpired: MainTenant quá hạn ký  
    PendingLandlordSignature \--\> Active: Landlord ký OTP  
    PendingLandlordSignature \--\> SignatureExpired: Landlord quá hạn ký  
    Draft \--\> Cancelled  
    PendingLandlordSignature \--\> Rejected  
\`\`\`

\#\#\# 5.4 Invoice

\`\`\`mermaid  
stateDiagram-v2  
    \[\*\] \--\> Draft  
    Draft \--\> Issued: Landlord issue  
    Issued \--\> Overdue: Job quá hạn  
    Issued \--\> Paid: Tenant thanh toán ví  
    Overdue \--\> Paid: Tenant thanh toán ví  
    Issued \--\> Cancelled: Landlord cancel trước khi paid  
    Overdue \--\> Cancelled: Landlord cancel trước khi paid  
\`\`\`

\#\#\# 5.5 Wallet top-up PayOS

\`\`\`mermaid  
stateDiagram-v2  
    \[\*\] \--\> Pending  
    Pending \--\> Succeeded: PayOS/Mock webhook success hợp lệ  
    Pending \--\> Failed: Provider failed hoặc webhook sai amount/signature  
    Pending \--\> Cancelled  
    Pending \--\> Expired  
\`\`\`

