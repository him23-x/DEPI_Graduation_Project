# Discover Egypt: صفحات الـ Host وعمليات الـ Database

مبني على DEPI GP Context V2.0 وClasses Specs V2.0 وERD V2.0.

## ملاحظة قبل البدء

ملف الـ Naming Convention متعارض مع الـ Context V2.0، والـ Context هو المرجع:

| Naming Convention | Context V2.0 (الأحدث) |
| --- | --- |
| `int` Ids و `IdentityUser<int>` | `Guid` Ids و `IdentityUser<Guid>` |
| Web API Controllers (`api/v1/...`) | ASP.NET Core MVC مع Razor Views |
| `CreatedAtUtc` | `CreatedAt` |

عدّل الـ Convention قبل ما تبدأ، وخلّي بقية القواعد (PascalCase، `Async`، `Request`/`Dto`، إلخ) زي ما هي.

## 1) الصفحات المطلوبة للـ Host

### Auth و Onboarding

| # | الصفحة | الغرض |
| --- | --- | --- |
| 1 | Register | الـ Role ثابت Host (مش Dropdown) |
| 2 | Login / Logout | بالإضافة لـ Access Denied |
| 3 | Create Host Profile | HostType, Bio, Profile Image (اختياري). لازم تتكمل قبل أي Dashboard feature |
| 4 | Edit Host Profile | نفس الفورم |

### Dashboard و Experiences

| # | الصفحة | الغرض |
| --- | --- | --- |
| 5 | Dashboard | عدّاد Experiences حسب الـ Status، Pending requests، أقرب Confirmed bookings، Payouts، Unread notifications، متوسط التقييم |
| 6 | My Experiences | قائمة بـ Filter بالـ Status، فيها Cover وBadge للحالة وسبب الرفض لو Rejected |
| 7 | Experience Wizard (Create/Edit) | Stepper من 4 خطوات (تحت) |
| 8 | Experience Details | عرض للقراءة: الحالة، Rejection Reason، الـ Evidence، الـ Declarations، أزرار Submit/Resubmit/Edit |

خطوات الـ Wizard (كل خطوة Action مستقلة، والـ Draft يتحفظ من الخطوة 1):

- **Step 1 - Basic info:** Type, Cultural Area, Title, Description, Cultural Story, Price, Duration, MaxGroupSize, MeetingPointArea, ExactMeetingPoint، و Interests (واحد على الأقل) وLanguages (واحدة على الأقل).
- **Step 2 - Media:** رفع، حذف، ترتيب، تحديد Cover. (1 إلى 8 صور، 0 إلى 2 فيديو.)
- **Step 3 - Evidence:** الفورم بيتغير حسب الـ Type:
  - Food: Menu وAllergen (نص)، و3 إلى 5 صور.
  - Arts: 3 إلى 10 Portfolio، و1 إلى 5 Workshop.
  - Local Traditions: Route Details (نص)، وإثبات المكان (لو بيت أو مكان خاص)، وVenue Permission (لو مكان طرف ثالث).
- **Step 4 - Declaration و Submit:** النص المكتوب مع Checkbox "I agree" حسب الـ Type، وبعدها Review وSubmit.

### Bookings والباقي

| # | الصفحة | الغرض |
| --- | --- | --- |
| 9 | Booking Requests / My Bookings | Tabs: Pending، Confirmed، Completed، Rejected/Cancelled. الـ Pending فيها عدّاد للوقت المتبقي للرد (12 ساعة) |
| 10 | Booking Details | بيانات السائح والتاريخ وعدد الأشخاص والـ Exact Meeting Point والـ ServiceAmount وحالة الـ Payout. أزرار: Accept، Reject، Mark Completed |
| 11 | Earnings (Payouts) | قراءة بس: الـ Amount والـ EligibleAt والـ Status وإجماليات |
| 12 | Reviews | تقييمات الـ Experiences بتاعته، متوسط وعدد (من غير نجوم) |
| 13 | Disputes (List + Details) | قراءة بس، الـ Dispute على الـ Bookings بتاعته |
| 14 | Notifications | قائمة، Mark as read، Mark all |

### Shared Components

- Navbar فيها Bell بعدّاد الـ Unread.
- Suspension Banner لو عليه Sanction فعّال.
- Status Badges.
- Filter بيحوّل لصفحة Create Profile لو البروفايل ناقص.

## 2) الـ Operations على الـ Database

### أ) Account و Profile

| Operation | النوع | Tables | ملاحظات |
| --- | --- | --- | --- |
| Register + Assign Role Host | Write | ApplicationUser, Identity Roles | عن طريق UserManager |
| Get HostProfile by UserId | Read | HostProfile | للـ Gate بعد Login |
| Create HostProfile | Write | HostProfile | UserId فريد |
| Update HostProfile | Write | HostProfile | لو الصورة اتغيرت امسح القديمة من الـ Blob |
| Get active suspension | Read | Sanction | `UserId` مع `StartsAt <= now <= EndsAt` و`ProviderSuspension` |

### ب) Experiences

| Operation | النوع | Tables | ملاحظات |
| --- | --- | --- | --- |
| Load lookups | Read | ExperienceType, CulturalArea, Interest, Language | للـ Dropdowns، تقدر تعمل لها Cache |
| Create Experience (Draft) | Write (Transaction) | Experience, ExperienceInterest, ExperienceLanguage | `Status=Draft`, `HostProfileId` من الـ Claims |
| Update Draft/Rejected | Write | Experience + Join tables | امسح الـ Join القديم وأدخل الجديد |
| Update Published | Write | Experience | Price و MaxGroupSize مباشرة. Title/Description/CulturalStory/MeetingPoint/Duration/Type ترجّعها `PendingApproval` |
| List my experiences | Read | Experience + Cover من ExperienceMedia | Paging + Filter بالـ Status |
| Get details | Read | Experience + Media, Evidence, Declarations, Interests, Languages | تأكد إنها بتاعته |
| Submit / Resubmit | Write | Experience | تأكد من اكتمال الكل، `Status=PendingApproval`، `RejectionReason=null`، `UpdatedAt` |

**Media:**

| Operation | ملاحظات |
| --- | --- |
| Add media | تحقق من النوع (JPEG/PNG/WebP أو MP4)، الحجم (10MB / 100MB)، العدد (8 صور / 2 فيديو). ارفع للـ Blob وبعدها سجّل `StorageKey` |
| Delete media | لو Published: ممنوع تحذف آخر صورة أو الـ Cover من غير بديل |
| Reorder | Update لـ `DisplayOrder` |
| Set cover | Transaction: شيل `IsCover` من القديمة وحطها على الجديدة. لازم تكون صورة |

**Evidence و Declarations:**

| Operation | ملاحظات |
| --- | --- |
| Save text evidence | `ExperienceEvidence.TextContent` (Menu/Allergen، Route Details) |
| Upload file evidence | ملف في Private Blob، PDF/JPEG/PNG، 10MB، والعدد حسب الـ Type |
| Replace evidence | Soft delete (`DeletedAt`) للقديم + Insert للجديد. لو Published ترجع `PendingApproval` |
| Download evidence | بيعدّي على Controller فيه Authorization (الـ Host صاحبها أو Admin). مفيش URL عام |
| Accept declaration | Insert في `ExperienceDeclaration`: HostUserId, Type, النص أو الإصدار, AcceptedAt |

### ج) Bookings

شرط عام: الـ Host مش بيشوف الـ Booking غير لو `Payment.Status = Paid`.

| Operation | النوع | ملاحظات |
| --- | --- | --- |
| List bookings | Read | `ProviderUserId` + `BookingType=Experience` + `Payment.Paid`، Include Experience، واسم السائح من UserManager (مفيش Navigation) |
| Booking details | Read | Include Payment, Payout, Experience |
| **Accept** | Write (Transaction) | الشروط: `Pending`، Payment Paid، مش Suspended، لسه جوه الـ 12 ساعة، ومفيش Overlap. بعدها: `Status=Confirmed`، إنشاء **Payout** (`Amount=ServiceAmount`, `EligibleAt=ScheduledEnd+24h`, `Pending`)، وإشعار للسائح |
| **Reject** | Write (Transaction) | `Pending` بس. `Status=Rejected`، إنشاء **Refund** (ServiceAmount بس، الـ Fee مش بيرجع)، `Payment.Status=Refunded`، مفيش Payout، وإشعار للسائح |
| **Mark Completed** | Write | الشروط: `Confirmed`، `UtcNow >= ScheduledEnd`، ومفيش Dispute مفتوح. **مبيفرجش الـ Payout** |

**Overlap check** (على مستوى الـ Host كله، مش الـ Experience بس): دور على Booking بنفس `ProviderUserId` ونوعها Experience وحالتها Confirmed، وفيها `ScheduledStart < newEnd` و`ScheduledEnd > newStart`. عشان تضمن الصحة لو اتنين عملوا Accept في نفس اللحظة، استخدم Transaction مع `sp_getapplock` على الـ HostProfileId، أو Serializable isolation، واعمل Test لسيناريو الـ Concurrency ده.

### د) Read-only و Notifications

| Operation | Tables |
| --- | --- |
| Payouts list + Totals | Payout JOIN Booking (+ Experience) بـ `ProviderUserId` |
| Reviews + Avg + Count | Review JOIN Booking JOIN Experience بـ `HostProfileId` (مش بتتخزن، بتتحسب) |
| Disputes list/details | Dispute JOIN Booking بـ `ProviderUserId` |
| Notifications list (Paged) | Notification بـ `UserId` |
| Unread count | Notification `IsRead=false` |
| Mark read / all | Update `IsRead` |
| Dashboard aggregates | Counts و Sums من الجداول اللي فوق |

## 3) قواعد لازم تفضل في بالك

- **Authorization على كل Query:** دايمًا فلتر بالـ HostProfileId/UserId اللي من الـ Claims، مش من الـ Route أو الـ Form. ده بيمنع إن Host يفتح Experience أو Booking لحد تاني.
- **الـ Rules في الـ Application layer:** حد الـ 8 صور، وشرط الـ Cover، وانتقالات الـ Status، وفحص الـ Overlap. الـ Controller ميعملش Business logic.
- **كل حاجة متعددة الجداول في Transaction واحدة** (Accept وReject وSet Cover وCreate Experience).
- **الـ Blob والـ DB:** ارفع الأول وسجّل بعدين، ولو الـ DB فشل امسح الملف.
- **التواريخ UTC في الـ DB**، وبتتعرض بتوقيت Africa/Cairo.
- أي حاجة تحتاج Background Service (Timeout 12 ساعة، Auto-complete، Payout release، Unpublish لما الـ Host يتعطل) مش صفحات Host. اعملها في مرحلة منفصلة، بس الصفحات فوق بتعتمد عليها.

## 4) ترتيب الشغل المقترح

1. **Auth + Host Profile** (الصفحات 1 إلى 4 وفلتر Profile Gate).
2. **Experiences:** القائمة، ثم Step 1، ثم Media، ثم Evidence، ثم Declaration/Submit. ده أكبر جزء.
3. **Bookings:** Accept وReject وComplete بعد ما تعمل Overlap check.
4. **Read-only:** Payouts وReviews وDisputes وNotifications.
5. **Dashboard** آخر حاجة لأنه بيجمع من كل اللي قبله.

علشان تقدر تختبر، محتاج حاجتين من برّه الـ Host: Admin يعمل Approve للـ Experience (أو Seed لـ Experience Published)، وTourist يعمل Booking (أو Seed لـ Bookings بحالات مختلفة).

## 5) نقاط الـ Context مش حاسمها

1. **الـ Host يفتح Dispute؟** الـ Context بيقول tourist (or a relevant participant) بس مش بيذكر الـ Host. الفرض هنا إنها قراءة بس.
2. **مصدر الـ 12 ساعة:** بتتحسب من `Booking.CreatedAt` ولا `Payment.PaidAt`؟ الأنسب `PaidAt` لأن الـ Host مبيشوفش الـ Booking قبل الدفع.
3. **حذف Experience:** مش مذكور. الاقتراح إنك تسمح بحذف الـ Draft بس لو معندوش Bookings.
4. **تعديل Experience وهي PendingApproval:** مش مذكور. الاقتراح إنك تقفلها لحد قرار الـ Admin.
5. **تعديل Experience وهي Unpublished:** مش مذكور، وده بيأثر على Wizard الـ Edit.
