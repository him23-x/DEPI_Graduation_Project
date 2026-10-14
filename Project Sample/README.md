# Discover Egypt: مشروع تجريبي (Clean Architecture + HTML/CSS/JavaScript)

مشروع صغير بيوريك **إزاي هنشتغل** بـ Clean Architecture، و**إزاي الفرونت بيتكلم مع الباك**. فيه Feature واحدة كاملة من Discover Egypt: عرض التجارب المنشورة، وحساب سعر الحجز (سعر الخدمة + رسوم المنصة = الإجمالي)، وحفظ الحجز.

> **مهم:** الكود ده اتكتب من غير ما اتبني أو اتشغّل (الـ .NET SDK مكانش متاح وقت الكتابة). لو ظهر أي Error في أول Build على Visual Studio، ابعتلي نص الـ Error وأصلّحه.

## 1. الهيكل ومين بيعتمد على مين

```
src/
├─ DiscoverEgypt.Domain          (Booking, Experience, Enums, business rules)   -> depends on nothing
├─ DiscoverEgypt.Application     (BookingService, IBookingRepository, DTOs)     -> depends on Domain
├─ DiscoverEgypt.Infrastructure  (InMemory repositories; EF Core later)         -> depends on Application
└─ DiscoverEgypt.WebApi          (Controllers, Program.cs, wwwroot = frontend)  -> depends on Application + Infrastructure

Dependency direction:  WebApi -> Application -> Domain
                       Infrastructure -> Application -> Domain
```

القاعدة: **الاعتماد بيشاور لجوه بس**. الـ Domain مبيعرفش حد. الـ Application بيحدد الـ Interfaces (زي `IBookingRepository`)، والـ Infrastructure بينفّذها.

| الطبقة | فيها إيه في المشروع ده |
|---|---|
| Domain | `Booking.CreateForExperience(...)`: قواعد الحجز وحساب الرسوم (`PlatformFeeAmount = ServiceAmount x Rate`، تقريب لخانتين، `TotalAmount`) |
| Application | `BookingService`: بيجيب التجربة، وينده على الـ Domain، ويحفظ. وبيعرّف `IExperienceRepository` و`IBookingRepository` والـ DTOs |
| Infrastructure | `InMemoryExperienceRepository` و`InMemoryBookingRepository`: بدل قاعدة البيانات مؤقتاً |
| WebApi | Controllers رفيعة، و`Program.cs` (بيربط كل Interface بتنفيذه)، وفولدر `wwwroot` فيه الصفحة |

## 2. طريق الطلب: مثال "احسب السعر"

```
Browser (app.js)
  -> api.js : fetch("POST /api/bookings/preview")
  -> BookingsController.Preview            (WebApi)
  -> BookingService.PreviewExperienceBookingAsync   (Application)
       -> IExperienceRepository.GetByIdAsync        (Application interface -> Infrastructure class)
       -> Booking.CreateForExperience(...)          (Domain: rules + fee calculation)
  <- PriceBreakdownDto (JSON) <- Controller <- Browser shows the breakdown
```

لو قاعدة اتكسرت (عدد أفراد أكبر من `MaxGroupSize` مثلاً) الـ Domain بيرمي `DomainException`، والـ middleware في `Program.cs` بيحوّله لـ **HTTP 400** برسالة `{ "error": "..." }`، والصفحة بتعرضها. التجربة مش موجودة: `NotFoundException` يتحوّل لـ **404**.

## 3. ربط الفرونت بالباك

- كل كلام الفرونت مع الباك في ملف واحد: `wwwroot/js/api.js` (دوال بتستخدم `fetch`). لو Endpoint اتغيّر، بتغيّره هناك بس.
- الـ Web API بيرجّع **JSON** بحروف صغيرة في أول الاسم (camelCase)، يعني `ServiceAmount` في C# بيوصل `serviceAmount` في JavaScript.
- الفرونت بياخد **DTOs** (زي `ExperienceDto`) مش الـ Entities بتاعة الـ Domain.
- الصفحة متقدّمة من نفس الـ Web API (`UseDefaultFiles` و`UseStaticFiles`)، فمفيش **CORS**. لو الفرونت اتفتح على عنوان تاني (زي Live Server على بورت 5500)، بدّل `API_BASE` في `api.js` وفعّل الـ CORS (الكود متعلّق كتعليق في `Program.cs`).
- الصفحة فيها جدول "سجل الطلبات" بيوريك كل طلب بيطلع: الطريقة والمسار والحالة.

## 4. التشغيل على Visual Studio Community

**المطلوب مرة واحدة:** Visual Studio Community مع الـ Workload اسمه **ASP.NET and web development** (من Visual Studio Installer).

1. افتح الملف **`DiscoverEgypt.sln`** (دوس عليه مرتين).
2. اتأكد إن `DiscoverEgypt.WebApi` هو الـ Startup Project (اسمه بيبقى بالخط الغامق في Solution Explorer). لو مش كده: كليك يمين عليه ثم **Set as Startup Project**.
3. اضغط **F5** (أو **Ctrl+F5** من غير Debug). المتصفح هيفتح على `http://localhost:5080` وفيه الصفحة.
4. ممكن تجرّب الـ API مباشرة: `http://localhost:5080/api/experiences`.

**لو ظهر Error إن إصدار .NET مش موجود** (مثلاً `net8.0` مش متسطّب): في Solution Explorer كليك يمين على كل Project ثم **Properties** ثم **Target framework**، واختار الإصدار المتسطّب عندك. أو غيّر `TargetFramework` في الـ 4 ملفات `.csproj`. ولو عندك Visual Studio أحدث ومعاه .NET أحدث، ده طبيعي.

**الشغل اليومي:**
- عشان تشوف تعديلات الـ JavaScript والـ CSS اعمل Refresh للمتصفح (**Ctrl+F5**)، ومفيش إعادة Build لملفات `wwwroot`.
- تعديل أي ملف C# محتاج إعادة تشغيل (**Shift+F5** ثم **F5**)، أو **Hot Reload** (الزرار الأحمر) لو مفعّل.
- لتغيير البورت: الملف `src/DiscoverEgypt.WebApi/Properties/launchSettings.json`.

**بديل من غير Visual Studio (اختياري):** `dotnet run --project src/DiscoverEgypt.WebApi --urls http://localhost:5080`.

## 5. إزاي تضيف Feature جديدة (الترتيب)

1. **Domain:** الـ Entity أو الـ Enum أو القاعدة (من الـ Classes Specs).
2. **Application:** الـ Interface للـ Repository لو محتاج، والـ DTOs، والـ Service (Use Case).
3. **Infrastructure:** تنفيذ الـ Interface.
4. **WebApi:** Controller رفيع + تسجيل الـ Service والـ Repository في `Program.cs`.
5. **Frontend:** دالة في `api.js`، وبعدين استخدامها في `app.js`.

## 6. إزاي تبدّل الـ In-memory بقاعدة بيانات حقيقية

1. ضيف في Infrastructure الحزم: `Microsoft.EntityFrameworkCore.SqlServer` و`Microsoft.EntityFrameworkCore.Design`.
2. اعمل `AppDbContext` وتعريفات الجداول (Entity Framework Core) في الـ Infrastructure.
3. اكتب `ExperienceRepository : IExperienceRepository` و`BookingRepository : IBookingRepository`.
4. في `Program.cs`: سجّل `AddDbContext`، وغيّر الـ Repositories من `AddSingleton` لـ `AddScoped`.
5. **الـ Domain والـ Application والـ Controllers والفرونت مبيتغيروش.** ده بالظبط فايدة Clean Architecture.

## 7. المشروع التجريبي مقابل المشروع الحقيقي

- `Experience` و`Booking` هنا **جزء** من الـ Classes Specs (الحقول اللازمة للمثال بس).
- `TouristId` بييجي من الـ Request هنا للتبسيط. في المشروع الحقيقي بييجي من المستخدم المسجّل (ASP.NET Core Identity) مش من الفرونت.
- نسبة الرسوم `0.15` في `appsettings.json` **توضيحية وليست نهائية** (حسب الـ Context). ولو القيمة ناقصة المشروع بيفشل عند التشغيل (Fail fast).
- مفيش Payment ولا تسجيل دخول ولا Guide في المثال ده.
- الـ Context مكتوب فيه إن الـ Frontend هو Angular، وقرارك الحالي HTML وCSS وJavaScript. اتبعت القرار الجديد، فخلّي الـ Context يتحدّث.
