# Discover Egypt - ERD

Based on DEPI GP Context V2.0. The diagrams use Mermaid `erDiagram` (rendered natively on GitHub and in VS Code with a Mermaid extension).

Notes:
- All Ids are Guid.
- Any `*AdminId` column is an FK to `ApplicationUser` (Admin role).
- Enums are columns, not tables.
- `Role` on `ApplicationUser` comes from ASP.NET Core Identity roles.
- A table that appears in more than one diagram is shown with its `Id` only in the secondary diagrams.
- Join tables (8 total) have a composite PK made of two FKs.

---

## 1. Accounts, profiles and Guide verification

```mermaid
erDiagram
  ApplicationUser ||--o| HostProfile : "has"
  ApplicationUser ||--o| GuideProfile : "has"
  GuideProfile ||--o{ VerificationRequest : "submits"
  VerificationRequest ||--|{ VerificationDocument : "contains"
  GuideProfile ||--|{ GuideLanguage : "speaks"
  GuideProfile ||--|{ GuideCulturalArea : "works in"
  Language ||--o{ GuideLanguage : "used in"
  CulturalArea ||--o{ GuideCulturalArea : "used in"
  Language |o--o{ VerificationDocument : "proof of"
  CulturalArea |o--o{ VerificationDocument : "residence in"

  ApplicationUser {
    uuid Id PK
    string Email UK
    string PasswordHash
    string FullName
    string Role "Identity role"
    bool IsActive
    datetime CreatedAt
  }
  HostProfile {
    uuid Id PK
    uuid UserId FK, UK
    string HostType "Family, Individual, Artisan, Farmer"
    string Bio
    string ProfileImageKey "nullable"
    datetime CreatedAt
  }
  GuideProfile {
    uuid Id PK
    uuid UserId FK, UK
    string Bio
    decimal HourlyRate
    string ProfileImageKey "nullable"
    datetime CreatedAt
  }
  VerificationRequest {
    uuid Id PK
    uuid GuideProfileId FK
    string Status "Pending, Approved, Rejected, Expired"
    date LicenseExpiryDate
    string RejectionReason "nullable"
    uuid ReviewedByAdminId FK
    datetime SubmittedAt
    datetime ReviewedAt "nullable"
  }
  VerificationDocument {
    uuid Id PK
    uuid VerificationRequestId FK
    string DocType "NationalId, License, Language, Residence, Permission"
    uuid LanguageId FK "nullable"
    uuid CulturalAreaId FK "nullable"
    string StorageKey "private blob"
    datetime UploadedAt
    datetime DeletedAt "nullable"
  }
  GuideLanguage {
    uuid GuideProfileId PK, FK
    uuid LanguageId PK, FK
    bool IsNative
  }
  GuideCulturalArea {
    uuid GuideProfileId PK, FK
    uuid CulturalAreaId PK, FK
  }
  Language {
    uuid Id PK
    string Name
  }
  CulturalArea {
    uuid Id PK
    string Name
  }
```

---

## 2. Tourist preferences

```mermaid
erDiagram
  ApplicationUser ||--o| TouristPreference : "sets"
  TouristPreference ||--o{ PreferenceInterest : "has"
  TouristPreference ||--o{ PreferenceExperienceType : "has"
  TouristPreference ||--o{ PreferenceCulturalArea : "has"
  TouristPreference ||--o{ PreferenceLanguage : "has"
  Interest ||--o{ PreferenceInterest : "used in"
  ExperienceType ||--o{ PreferenceExperienceType : "used in"
  CulturalArea ||--o{ PreferenceCulturalArea : "used in"
  Language ||--o{ PreferenceLanguage : "used in"

  ApplicationUser {
    uuid Id PK
  }
  TouristPreference {
    uuid Id PK
    uuid UserId FK, UK
    string Budget "5 buckets"
    string Duration "4 buckets"
    string GuidePreference "Preferred, NoGuide, Either"
  }
  PreferenceInterest {
    uuid TouristPreferenceId PK, FK
    uuid InterestId PK, FK
  }
  PreferenceExperienceType {
    uuid TouristPreferenceId PK, FK
    uuid ExperienceTypeId PK, FK
  }
  PreferenceCulturalArea {
    uuid TouristPreferenceId PK, FK
    uuid CulturalAreaId PK, FK
  }
  PreferenceLanguage {
    uuid TouristPreferenceId PK, FK
    uuid LanguageId PK, FK
  }
  Interest {
    uuid Id PK
    string Name
  }
  ExperienceType {
    uuid Id PK
    string Name
  }
  CulturalArea {
    uuid Id PK
    string Name
  }
  Language {
    uuid Id PK
    string Name
  }
```

---

## 3. Experiences, media and evidence

```mermaid
erDiagram
  HostProfile ||--o{ Experience : "owns"
  ExperienceType ||--o{ Experience : "classifies"
  CulturalArea ||--o{ Experience : "located in"
  Experience ||--|{ ExperienceMedia : "has"
  Experience ||--o{ ExperienceEvidence : "has"
  Experience ||--o{ ExperienceDeclaration : "has"
  Experience ||--|{ ExperienceInterest : "tagged"
  Experience ||--|{ ExperienceLanguage : "offered in"
  Interest ||--o{ ExperienceInterest : "used in"
  Language ||--o{ ExperienceLanguage : "used in"

  HostProfile {
    uuid Id PK
  }
  Experience {
    uuid Id PK
    uuid HostProfileId FK
    uuid ExperienceTypeId FK
    uuid CulturalAreaId FK
    string Title
    string Description
    string CulturalStory "mandatory"
    decimal PricePerPerson "EGP"
    int DurationMinutes
    int MaxGroupSize
    string MeetingPointArea "public label"
    string ExactMeetingPoint "private"
    string Status "Draft, PendingApproval, Published, Rejected, Unpublished"
    string RejectionReason "nullable"
    uuid ReviewedByAdminId FK
    datetime CreatedAt
    datetime UpdatedAt
  }
  ExperienceMedia {
    uuid Id PK
    uuid ExperienceId FK
    string MediaType "Image, Video"
    string StorageKey
    int DisplayOrder
    bool IsCover "image only"
    datetime CreatedAt
  }
  ExperienceEvidence {
    uuid Id PK
    uuid ExperienceId FK
    string EvidenceKind "by Experience Type"
    string TextContent "in-platform form, nullable"
    string StorageKey "private blob, nullable"
    datetime UploadedAt
    datetime DeletedAt "nullable"
  }
  ExperienceDeclaration {
    uuid Id PK
    uuid ExperienceId FK
    uuid HostUserId FK
    string DeclarationType
    string DeclarationText "text or version"
    datetime AcceptedAt
  }
  ExperienceInterest {
    uuid ExperienceId PK, FK
    uuid InterestId PK, FK
  }
  ExperienceLanguage {
    uuid ExperienceId PK, FK
    uuid LanguageId PK, FK
  }
  ExperienceType {
    uuid Id PK
    string Name
  }
  CulturalArea {
    uuid Id PK
    string Name
  }
  Interest {
    uuid Id PK
    string Name
  }
  Language {
    uuid Id PK
    string Name
  }
```

---

## 4. Bookings, payments, disputes and notifications

```mermaid
erDiagram
  ApplicationUser ||--o{ Booking : "tourist"
  ApplicationUser ||--o{ Booking : "provider"
  Experience |o--o{ Booking : "booked as"
  GuideProfile |o--o{ Booking : "booked as"
  Booking ||--|| Payment : "paid by"
  Booking ||--o| Payout : "pays out"
  Booking ||--o{ Refund : "refunded by"
  Booking ||--o| Review : "reviewed by"
  Booking ||--o{ Dispute : "disputed by"
  Booking ||--o{ BookingShare : "shared via"
  Booking |o--o{ Notification : "about"
  Dispute |o--o{ Refund : "causes"
  Dispute ||--o{ Sanction : "causes"
  ApplicationUser ||--o{ Sanction : "restricted by"
  ApplicationUser ||--o{ Notification : "receives"

  ApplicationUser {
    uuid Id PK
  }
  Experience {
    uuid Id PK
  }
  GuideProfile {
    uuid Id PK
  }
  Booking {
    uuid Id PK
    uuid TouristUserId FK
    uuid ProviderUserId FK "Host or Guide"
    string BookingType "Experience, Guide"
    uuid ExperienceId FK "nullable"
    uuid GuideProfileId FK "nullable"
    datetime ScheduledStart "UTC"
    datetime ScheduledEnd "UTC"
    int NumberOfGuests
    int GuideHours "Guide only"
    string Destination "Guide only"
    decimal PricePerPersonSnapshot "Experience only"
    decimal HourlyRateSnapshot "Guide only"
    decimal ServiceAmount
    decimal PlatformFeeRate
    decimal PlatformFeeAmount
    decimal TotalAmount
    string Status "Pending, Confirmed, Rejected, Cancelled, Completed"
    datetime CreatedAt
  }
  Payment {
    uuid Id PK
    uuid BookingId FK, UK
    decimal Amount
    string Status "Pending, Paid, Failed, Refunded, PartiallyRefunded"
    datetime PaidAt "nullable"
    datetime CreatedAt
  }
  Payout {
    uuid Id PK
    uuid BookingId FK, UK
    decimal Amount
    datetime EligibleAt "ScheduledEnd + 24h"
    string Status "Pending, Paid, Cancelled"
    datetime PaidAt "nullable"
  }
  Refund {
    uuid Id PK
    uuid BookingId FK
    uuid DisputeId FK "nullable"
    decimal Amount
    string Reason
    uuid DecidedByAdminId FK "Admin user, nullable"
    datetime CreatedAt
  }
  Review {
    uuid Id PK
    uuid BookingId FK, UK
    int Rating "1 to 5"
    string Comment "nullable"
    datetime CreatedAt
  }
  Dispute {
    uuid Id PK
    uuid BookingId FK
    uuid OpenedByUserId FK
    string Description
    string Status "Open, UnderReview, Resolved, Closed"
    string Outcome "ProviderFault, TouristFault, NoFault"
    string ResolutionNote "nullable"
    uuid ResolvedByAdminId FK "Admin user, nullable"
    datetime CreatedAt
    datetime ResolvedAt "nullable"
  }
  Sanction {
    uuid Id PK
    uuid UserId FK
    uuid DisputeId FK
    string SanctionType "ProviderSuspension, TouristBan"
    int Days
    datetime StartsAt
    datetime EndsAt
    uuid DecidedByAdminId FK "Admin user"
  }
  Notification {
    uuid Id PK
    uuid UserId FK
    uuid BookingId FK "nullable"
    string Type
    string Message
    bool IsRead
    datetime CreatedAt
  }
  BookingShare {
    uuid Id PK
    uuid BookingId FK
    string Token UK "read-only link"
    datetime ExpiresAt
    datetime CreatedAt
  }
```
