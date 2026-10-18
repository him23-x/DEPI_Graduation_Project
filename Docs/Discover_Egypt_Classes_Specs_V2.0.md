# Discover Egypt - Classes Specs V2.0

مبني على DEPI GP Context V2.0 و ERD V2.0.

## قواعد عامة

- كل Id من نوع `Guid`. الـ Id بيجي من `BaseEntity` (Sequential Guid / Guid v7). الـ 8 Join Classes معندهمش Id، الـ Key بتاعهم Composite من FKين.
- كل التواريخ `DateTime` بتتخزن UTC. `LicenseExpiryDate` من نوع `DateOnly`.
- الـ Enums بتتخزن كـ string في الـ DB (مفيش جدول لكل Enum).
- `?` بعد النوع معناها Nullable.
- الـ Domain project مفيهوش Reference لـ Identity، فأي ربط بـ User بيبقى `Guid` بس من غير Navigation Property (`UserId`, `TouristUserId`, `ProviderUserId`, `*AdminId`, ...).
- Collection Navigation من نوع `ICollection<T>` وبتتعمل لها Initialize بـ `new List<T>()`.
- الـ Navigation في الجدول مكتوبة مع الـ properties العادية وعليها علامة **Nav**.

---

## Enums

| Enum | Values |
|---|---|
| HostType | Family, Individual, Artisan, Farmer |
| VerificationStatus | Pending, Approved, Rejected, Expired |
| VerificationDocType | NationalId, License, Language, Residence, Permission |
| Budget | Under500, From500To800, From800To1500, From1500To3000, Over3000 |
| DurationPreference | Under2h, From2To4h, From4To6h, FullDay |
| GuidePreference | Preferred, NoGuide, Either |
| ExperienceStatus | Draft, PendingApproval, Published, Rejected, Unpublished |
| MediaType | Image, Video |
| EvidenceKind | MenuAndAllergenInfo, FoodAreaPhoto, PortfolioPhoto, WorkshopPhoto, RouteDetails, VenueProof, VenuePermission |
| DeclarationType | FoodSafety, ToolsMaterialsSafety, PrivacyVenuePermission |
| BookingType | Experience, Guide |
| BookingStatus | Pending, Confirmed, Rejected, Cancelled, Completed |
| PaymentStatus | Pending, Paid, Failed, Refunded, PartiallyRefunded |
| PayoutStatus | Pending, Paid, Cancelled |
| DisputeStatus | Open, UnderReview, Resolved, Closed |
| DisputeOutcome | ProviderFault, TouristFault, NoFault |
| SanctionType | ProviderSuspension, TouristBan |
| NotificationType | BookingConfirmation, BookingReminder, PaymentUpdate, DisputeUpdate, GuideSuggestion, NewBooking, BookingStatusUpdate, ExperienceApprovalUpdate, VerificationUpdate |

---

## 0. ApplicationUser (Infrastructure)

بتورث من `IdentityUser<Guid>`. مفيهاش Navigation للـ Domain.

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK (من Identity) |
| Email | string | UK (من Identity) |
| PasswordHash | string | من Identity |
| FullName | string | |
| IsActive | bool | |
| CreatedAt | DateTime | |

الـ Role بييجي من Identity Roles (Tourist, Host, Guide, Admin).

---

## 1. Lookups

### Language

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| Name | string | |
| GuideLanguages | ICollection\<GuideLanguage\> | Nav 1:N |
| VerificationDocuments | ICollection\<VerificationDocument\> | Nav 1:N |
| PreferenceLanguages | ICollection\<PreferenceLanguage\> | Nav 1:N |
| ExperienceLanguages | ICollection\<ExperienceLanguage\> | Nav 1:N |

### CulturalArea

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| Name | string | |
| GuideCulturalAreas | ICollection\<GuideCulturalArea\> | Nav 1:N |
| VerificationDocuments | ICollection\<VerificationDocument\> | Nav 1:N |
| PreferenceCulturalAreas | ICollection\<PreferenceCulturalArea\> | Nav 1:N |
| Experiences | ICollection\<Experience\> | Nav 1:N |

### Interest

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| Name | string | |
| PreferenceInterests | ICollection\<PreferenceInterest\> | Nav 1:N |
| ExperienceInterests | ICollection\<ExperienceInterest\> | Nav 1:N |

### ExperienceType

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| Name | string | |
| PreferenceExperienceTypes | ICollection\<PreferenceExperienceType\> | Nav 1:N |
| Experiences | ICollection\<Experience\> | Nav 1:N |

---

## 2. Profiles and Guide verification

### HostProfile

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| UserId | Guid | FK (ApplicationUser), UK |
| HostType | HostType | |
| Bio | string | |
| ProfileImageKey | string? | |
| CreatedAt | DateTime | |
| Experiences | ICollection\<Experience\> | Nav 1:N |

### GuideProfile

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| UserId | Guid | FK (ApplicationUser), UK |
| Bio | string | |
| HourlyRate | decimal | EGP |
| ProfileImageKey | string? | |
| CreatedAt | DateTime | |
| VerificationRequests | ICollection\<VerificationRequest\> | Nav 1:N |
| GuideLanguages | ICollection\<GuideLanguage\> | Nav 1:N |
| GuideCulturalAreas | ICollection\<GuideCulturalArea\> | Nav 1:N |
| Bookings | ICollection\<Booking\> | Nav 1:N |

### VerificationRequest

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| GuideProfileId | Guid | FK |
| GuideProfile | GuideProfile | Nav N:1 |
| Status | VerificationStatus | |
| LicenseExpiryDate | DateOnly | |
| RejectionReason | string? | |
| ReviewedByAdminId | Guid? | Admin user، null لحد المراجعة |
| SubmittedAt | DateTime | |
| ReviewedAt | DateTime? | |
| Documents | ICollection\<VerificationDocument\> | Nav 1:N |

### VerificationDocument

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| VerificationRequestId | Guid | FK |
| VerificationRequest | VerificationRequest | Nav N:1 |
| DocType | VerificationDocType | |
| LanguageId | Guid? | FK، لـ DocType = Language |
| Language | Language? | Nav N:1 |
| CulturalAreaId | Guid? | FK، لـ DocType = Residence |
| CulturalArea | CulturalArea? | Nav N:1 |
| StorageKey | string | private blob |
| UploadedAt | DateTime | |
| DeletedAt | DateTime? | |

### GuideLanguage (Join)

| Property | Type | Note |
|---|---|---|
| GuideProfileId | Guid | PK, FK |
| GuideProfile | GuideProfile | Nav N:1 |
| LanguageId | Guid | PK, FK |
| Language | Language | Nav N:1 |
| IsNative | bool | |

### GuideCulturalArea (Join)

| Property | Type | Note |
|---|---|---|
| GuideProfileId | Guid | PK, FK |
| GuideProfile | GuideProfile | Nav N:1 |
| CulturalAreaId | Guid | PK, FK |
| CulturalArea | CulturalArea | Nav N:1 |

---

## 3. Tourist preferences

### TouristPreference

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| UserId | Guid | FK (ApplicationUser), UK |
| Budget | Budget | |
| Duration | DurationPreference | |
| GuidePreference | GuidePreference | |
| PreferenceInterests | ICollection\<PreferenceInterest\> | Nav 1:N |
| PreferenceExperienceTypes | ICollection\<PreferenceExperienceType\> | Nav 1:N |
| PreferenceCulturalAreas | ICollection\<PreferenceCulturalArea\> | Nav 1:N |
| PreferenceLanguages | ICollection\<PreferenceLanguage\> | Nav 1:N |

### PreferenceInterest (Join)

| Property | Type | Note |
|---|---|---|
| TouristPreferenceId | Guid | PK, FK |
| TouristPreference | TouristPreference | Nav N:1 |
| InterestId | Guid | PK, FK |
| Interest | Interest | Nav N:1 |

### PreferenceExperienceType (Join)

| Property | Type | Note |
|---|---|---|
| TouristPreferenceId | Guid | PK, FK |
| TouristPreference | TouristPreference | Nav N:1 |
| ExperienceTypeId | Guid | PK, FK |
| ExperienceType | ExperienceType | Nav N:1 |

### PreferenceCulturalArea (Join)

| Property | Type | Note |
|---|---|---|
| TouristPreferenceId | Guid | PK, FK |
| TouristPreference | TouristPreference | Nav N:1 |
| CulturalAreaId | Guid | PK, FK |
| CulturalArea | CulturalArea | Nav N:1 |

### PreferenceLanguage (Join)

| Property | Type | Note |
|---|---|---|
| TouristPreferenceId | Guid | PK, FK |
| TouristPreference | TouristPreference | Nav N:1 |
| LanguageId | Guid | PK, FK |
| Language | Language | Nav N:1 |

---

## 4. Experiences, media and evidence

### Experience

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| HostProfileId | Guid | FK |
| HostProfile | HostProfile | Nav N:1 |
| ExperienceTypeId | Guid | FK |
| ExperienceType | ExperienceType | Nav N:1 |
| CulturalAreaId | Guid | FK |
| CulturalArea | CulturalArea | Nav N:1 |
| Title | string | |
| Description | string | |
| CulturalStory | string | mandatory |
| PricePerPerson | decimal | EGP |
| DurationMinutes | int | |
| MaxGroupSize | int | |
| MeetingPointArea | string | public label |
| ExactMeetingPoint | string | private، بعد Confirmed بس |
| Status | ExperienceStatus | |
| RejectionReason | string? | بتتمسح عند إعادة الإرسال |
| ReviewedByAdminId | Guid? | Admin user |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |
| Media | ICollection\<ExperienceMedia\> | Nav 1:N (1 إلى 8 صور، 0 إلى 2 فيديو) |
| Evidence | ICollection\<ExperienceEvidence\> | Nav 1:N |
| Declarations | ICollection\<ExperienceDeclaration\> | Nav 1:N |
| ExperienceInterests | ICollection\<ExperienceInterest\> | Nav 1:N (واحد على الأقل) |
| ExperienceLanguages | ICollection\<ExperienceLanguage\> | Nav 1:N (واحد على الأقل) |
| Bookings | ICollection\<Booking\> | Nav 1:N |

### ExperienceMedia

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| ExperienceId | Guid | FK |
| Experience | Experience | Nav N:1 |
| MediaType | MediaType | |
| StorageKey | string | |
| DisplayOrder | int | |
| IsCover | bool | صورة واحدة بس، ومش فيديو |
| CreatedAt | DateTime | |

### ExperienceEvidence

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| ExperienceId | Guid | FK |
| Experience | Experience | Nav N:1 |
| EvidenceKind | EvidenceKind | حسب Experience Type |
| TextContent | string? | للـ in-platform form |
| StorageKey | string? | private blob |
| UploadedAt | DateTime | |
| DeletedAt | DateTime? | |

### ExperienceDeclaration

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| ExperienceId | Guid | FK |
| Experience | Experience | Nav N:1 |
| HostUserId | Guid | FK (ApplicationUser) |
| DeclarationType | DeclarationType | |
| DeclarationText | string | النص أو الإصدار اللي اتوافق عليه |
| AcceptedAt | DateTime | |

### ExperienceInterest (Join)

| Property | Type | Note |
|---|---|---|
| ExperienceId | Guid | PK, FK |
| Experience | Experience | Nav N:1 |
| InterestId | Guid | PK, FK |
| Interest | Interest | Nav N:1 |

### ExperienceLanguage (Join)

| Property | Type | Note |
|---|---|---|
| ExperienceId | Guid | PK, FK |
| Experience | Experience | Nav N:1 |
| LanguageId | Guid | PK, FK |
| Language | Language | Nav N:1 |

---

## 5. Bookings, payments, disputes and notifications

### Booking

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| TouristUserId | Guid | FK (ApplicationUser) |
| ProviderUserId | Guid | FK (ApplicationUser)، Host أو Guide |
| BookingType | BookingType | |
| ExperienceId | Guid? | FK، لـ Experience booking بس |
| Experience | Experience? | Nav N:1 |
| GuideProfileId | Guid? | FK، لـ Guide booking بس |
| GuideProfile | GuideProfile? | Nav N:1 |
| ScheduledStart | DateTime | UTC |
| ScheduledEnd | DateTime | UTC |
| NumberOfGuests | int | Experience بس |
| GuideHours | int? | Guide بس |
| Destination | string? | Guide بس |
| PricePerPersonSnapshot | decimal? | Experience بس |
| HourlyRateSnapshot | decimal? | Guide بس |
| ServiceAmount | decimal | |
| PlatformFeeRate | decimal | |
| PlatformFeeAmount | decimal | |
| TotalAmount | decimal | |
| Status | BookingStatus | |
| CreatedAt | DateTime | |
| Payment | Payment | Nav 1:1 |
| Payout | Payout? | Nav 1:0..1 |
| Refunds | ICollection\<Refund\> | Nav 1:N |
| Review | Review? | Nav 1:0..1 |
| Disputes | ICollection\<Dispute\> | Nav 1:N |
| Shares | ICollection\<BookingShare\> | Nav 1:N |
| Notifications | ICollection\<Notification\> | Nav 1:N |

### Payment

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| BookingId | Guid | FK, UK |
| Booking | Booking | Nav 1:1 |
| Amount | decimal | = TotalAmount |
| Status | PaymentStatus | |
| PaidAt | DateTime? | |
| CreatedAt | DateTime | |

### Payout

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| BookingId | Guid | FK, UK |
| Booking | Booking | Nav 1:1 |
| Amount | decimal | |
| EligibleAt | DateTime | ScheduledEnd + 24h |
| Status | PayoutStatus | |
| PaidAt | DateTime? | |

### Refund

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| BookingId | Guid | FK |
| Booking | Booking | Nav N:1 |
| DisputeId | Guid? | FK |
| Dispute | Dispute? | Nav N:1 |
| Amount | decimal | |
| Reason | string | |
| DecidedByAdminId | Guid? | Admin user |
| CreatedAt | DateTime | |

### Review

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| BookingId | Guid | FK, UK |
| Booking | Booking | Nav 1:1 |
| Rating | int | 1 إلى 5 |
| Comment | string? | |
| CreatedAt | DateTime | |

### Dispute

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| BookingId | Guid | FK |
| Booking | Booking | Nav N:1 |
| OpenedByUserId | Guid | FK (ApplicationUser) |
| Description | string | |
| Status | DisputeStatus | |
| Outcome | DisputeOutcome? | بتتحدد عند Resolved |
| ResolutionNote | string? | |
| ResolvedByAdminId | Guid? | Admin user |
| CreatedAt | DateTime | |
| ResolvedAt | DateTime? | |
| Refunds | ICollection\<Refund\> | Nav 1:N |
| Sanctions | ICollection\<Sanction\> | Nav 1:N |

### Sanction

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| UserId | Guid | FK (ApplicationUser)، المحظور أو الموقوف |
| DisputeId | Guid | FK |
| Dispute | Dispute | Nav N:1 |
| SanctionType | SanctionType | |
| Days | int | |
| StartsAt | DateTime | |
| EndsAt | DateTime | |
| DecidedByAdminId | Guid | Admin user |

### Notification

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| UserId | Guid | FK (ApplicationUser) |
| BookingId | Guid? | FK |
| Booking | Booking? | Nav N:1 |
| Type | NotificationType | |
| Message | string | |
| IsRead | bool | |
| CreatedAt | DateTime | |

### BookingShare

| Property | Type | Note |
|---|---|---|
| Id | Guid | PK |
| BookingId | Guid | FK |
| Booking | Booking | Nav N:1 |
| Token | string | UK، read-only link |
| ExpiresAt | DateTime | |
| CreatedAt | DateTime | |

---

## ملاحظات على الـ ERD (لازم تتعدل عشان تتطابق مع الـ Specs)

- `VerificationRequest.ReviewedByAdminId` و `Experience.ReviewedByAdminId` لازم يبقوا Nullable، لأن مفيش Admin قبل المراجعة.
- `Dispute.Outcome` لازم يبقى Nullable لحد ما الـ Dispute يتحل.
- `Booking`: مطلوب Check Constraint إن واحد بس من `ExperienceId` / `GuideProfileId` يكون ليه قيمة حسب `BookingType`.
- الحقول الخاصة بنوع واحد من الـ Booking (`NumberOfGuests`، `GuideHours`، `Destination`، الـ Snapshots) كل واحد منها فاضي في النوع التاني.
