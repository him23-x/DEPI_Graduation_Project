# DEPI GP Context V2.0 — Discover Egypt

> Last updated: 2026-10-18
> Purpose: single source of truth for the currently approved business rules and remaining decisions. This file defines business requirements; the ERD and Classes Specs are separate design deliverables and must be created or updated from this version before Entity implementation. Existing ERD and Classes Specs V1.4 are outdated references where they conflict with this context.
>
> **V2.0 status:** Baseline for the ERD. Built from V1.18 (which already contained V1.16 and the owner-approved resolutions of the V1.17 design review), plus the further owner-approved decisions of 2026-10-18, all merged into the numbered sections. Every item that blocked the ERD or the Classes Specs is closed. Section 25 is the version history; Section 26 is the glossary and ADR candidates. The only remaining open items are Post-MVP or pre-launch (Section 22).

---

## 1. Project Overview

| Item | Value |
|---|---|
| Project name | Discover Egypt |
| Type | Smart Tourism & Cultural Experience Platform |
| Platform | Web application |
| Backend | ASP.NET Core MVC / .NET, Entity Framework Core, SQL Server |
| Web layer | ASP.NET Core MVC: Controllers and server-rendered Razor Views |
| Frontend | HTML (Razor Views), CSS and JavaScript with no frontend framework, served by the MVC application (static files in `wwwroot`) |
| Authentication | ASP.NET Core Identity with `IdentityUser<Guid>` using MVC cookie authentication; secure, HttpOnly cookies and SameSite=Lax as the default baseline |
| Primary keys | `Guid` for every Id: all main domain entities, `IdentityUser<Guid>` and `IdentityRole<Guid>` (no int identity). The 8 many-to-many Join Classes have no Id of their own: their key is a composite of two Guid foreign keys |
| Architecture | Clean Architecture in 4 projects (Domain, Application, Infrastructure, Web) |
| Repository | One GitHub repository (the .NET solution and `docs/`) using GitHub Flow (feature branches and Pull Requests) |
| Current stage | Business context consolidated in V2.0 (owner-approved on 2026-10-18). Next: create/update the ERD, then create/update the Classes Specs from this context; only after review and approval should Entity implementation begin. |
| Tagline | "Don't just visit Egypt. Live it." |

Discover Egypt is NOT a generic sightseeing portal. It connects tourists (mainly foreign visitors) with Egyptian local hosts and guides for authentic cultural experiences:

- **Food & Cooking** — cooking or food-tasting with a local family or food maker.
- **Arts & Crafts** — hands-on craft-making or learning with an artisan.
- **Local Traditions & Daily Life** — focused experiences about local customs, everyday life or traditional markets, led by someone with a genuine connection to the place. General sightseeing tours and the separately-booked Guide service are excluded.

## 2. Core Problem and Main Flow

Traditional tourism focuses on famous landmarks, fixed programs, crowded buses and generic sightseeing, with limited interaction with local communities. Discover Egypt offers authentic, personalized experiences instead.

Main flow:

```
Tourist -> Preferences -> Personalized Recommendations -> Explore -> Choose Experience
 -> Book + Mock Payment -> Host accepts/rejects -> Confirmed -> Experience happens
 -> scheduled end + 24-hour complaint window -> payout eligible if no complaint -> Review -> (optional) "Book a Guide?"
 -> Feedback improves future recommendations
```

Trust mechanisms: category-specific verification evidence for each Host experience, Guide verification, Admin approval of every experience, transparent pricing, booking management, reviews, cancellation/refund rules, dispute handling, booking sharing for safety, notifications.

## 3. Roles and Accounts

- Four roles: Tourist, Host, Guide, Admin.
- One central user identity (`IdentityUser<Guid>`); roles decide access. No separate TouristUser/HostUser/GuideUser/AdminUser authentication entities.
- Decision: ONE primary role per account. A single account is not designed to be Tourist + Host + Guide at the same time.
- A Host account has a Host profile; a Guide account has a Guide profile. Admin is role-based only.
- The role is chosen at registration (Tourist, Host or Guide); Admin accounts are seeded, not self-registered. A Host or Guide must complete the profile before using dashboard features; completing the profile does not add a role, and a Tourist account cannot create a Host or Guide profile. Hosts do not receive account-level verification; each Host experience must pass its type-specific evidence review and Admin approval before it can be published or booked. Guides require Admin verification before they are bookable.

## 4. Role Capabilities

**Tourist:** register/login, manage profile, set preferences, browse/search/filter/sort experiences, view details, get personalized recommendations, book an experience, book a guide (separate booking), mock payment, share a booking with a trusted contact, view bookings, cancel per policy, report an issue / open a dispute, review completed bookings, receive notifications.

**Host** (types: Family, Individual, Artisan, Farmer): register, create a Host profile (with an optional profile picture), create experiences (each with a mandatory Cultural Story), provide private verification evidence required for that experience type, upload experience media (at least 1 image, up to 8 images, optional videos up to 2), submit the experience and evidence for Admin review, define price/duration/group size/cultural area/type/language, receive bookings for published experiences, accept or reject, conduct the experience, mark it Completed, receive payout, receive reviews. There is no Host account-level verification.

**Guide:** register, create Guide profile (with hourly rate, languages, working areas, optional profile picture), submit the required identity, license (with its expiry date), non-native language-level and work-area residence evidence for verification, wait for Admin approval, receive guide booking requests, accept or reject, accompany the tourist, view booking details, mark the booking Completed, receive payout, receive reviews.

**Admin:** manage users (activate/deactivate), review Guide verification requests and their private documents (approve/reject), review each Host experience together with its type-specific private evidence and public media, approve/reject/unpublish experiences, manage bookings, monitor transactions and payouts, manage and resolve disputes, handle refund decisions per system rules, and apply provider suspensions and tourist bans for a number of days after a Dispute outcome (Section 11).

## 5. Experiences

- An Experience belongs to a Host. Examples: Traditional Egyptian Family Cooking.
- Each Experience has exactly one required primary Experience Type (the fixed top-level category, like an app-store category), one or more required Interests for recommendation matching, one or more Languages (at least one required), and one Cultural Area.
- Experience Types (the three initial MVP categories):
  - **Food & Cooking** — cooking or food-tasting experiences with a local family or food maker.
  - **Arts & Crafts** — hands-on craft-making or learning experiences with an artisan.
  - **Local Traditions & Daily Life** — focused experiences about local customs, everyday life or traditional markets, led by someone with a genuine connection to the place; excludes general sightseeing tours and the separately-booked Guide service.
  - Workshops are an activity format, not a separate Experience Type.
- Cultural Areas: Cairo, Giza, Alexandria, Luxor, Aswan, Red Sea, Other.
- Languages: English, Arabic, French, German, Spanish (modeled consistently, not as repeated strings).
- Content: title, description, **Cultural Story (mandatory)**, price per person, duration, max group size, meeting point, media (images and optional videos, see "Experience media" below), status.
- **Meeting point privacy:** the listing shows a public area label only. The exact meeting point/address is stored separately and shown to the tourist only after the booking is Confirmed, and to the trusted contact through the booking-share link (Section 16). Editing either one follows the meeting-point re-approval rule below.
- **Cultural Story of the Experience** is a key differentiator: it explains why the experience is culturally meaningful (family cooking traditions, artisan heritage, community customs, traditional techniques), not only what the tourist will do. It is reviewed by Admin.
- **Pricing is per person** (PricePerPerson), snapshotted into the booking at booking time.
- Required fields are NOT NULL. "Draft" means the Host created/completed the experience but has not submitted/published it; UI validation ensures required fields (and at least one image) before submission; the hard image rule is enforced at publication (see below).

### Publishing workflow (Admin approval is mandatory)

```
Draft -> PendingApproval -> Published
                         -> Rejected (with rejection reason)
Rejected -> PendingApproval (the Host edits the same experience and resubmits)
Published -> Unpublished (Admin can unpublish)
Unpublished -> Published (Admin, same publishing conditions)
```

Publication requires ALL of: (1) the experience has passed Admin review, including approval of the required private verification evidence for its Experience Type, and (2) the experience has at least one image. Host verification is not a publication prerequisite. The image requirement is a hard business rule enforced by the backend/domain/application layer (not only by UI validation): the Admin approval action fails, and any later re-publish from Unpublished fails, if the experience has no image or no valid cover.


### Experience-type verification for Hosts

- There is no Host-level verification request or verified status. Host identity documents are not collected as a general Host verification step.
- Every new Experience submission includes the private verification evidence required for its selected Experience Type. The evidence is associated with that Experience and reviewed by Admin together with its content.
- An Experience cannot be Published unless its type-specific evidence and listing are approved by Admin, and it meets the media requirements above.
#### Required verification evidence by Experience Type

Each Host Experience submission includes these three evidence items. Forms and declarations are completed in the platform; evidence files are uploaded privately and are not public ExperienceMedia. File-count limits and accepted formats are in the Media and file storage rules.

**Food & Cooking**
1. Menu, ingredients and allergen information (in-platform form).
2. Three to five photos showing the food preparation, washing and storage areas.
3. Food hygiene and safe-preparation declaration (written in-platform; Host checks “I agree”).

**Arts & Crafts**
1. Three to ten photos of the Host's completed work (portfolio).
2. One to five photos of the workshop and the tools/materials used.
3. Tools and materials safety declaration (written in-platform; Host checks “I agree”).

**Local Traditions & Daily Life**
1. Experience description, Cultural Story, and route/location details (in-platform fields).
2. Evidence of the right to use the venue: for a home-based Daily Life experience, a recent utility bill matching the address; if not in the Host's name, a lease or written owner permission. For a public route, provide the route and meeting point instead.
3. Privacy and venue/participant permission declaration (written in-platform; Host checks “I agree”). A separate venue permission document is required when the experience uses a private or controlled third-party location.

A utility bill proves connection to or residence at an address, not legal ownership. Any statutory permit or certificate is required only when confirmed applicable to the specific activity and venue; no single official permit is imposed on every Host Experience.
- Verification evidence is sensitive private data, separate from public ExperienceMedia. Store it in private Azure Blob Storage containers; SQL Server stores metadata and a `StorageKey`. Access is limited to Admin reviewers and the Host who submitted it. Never enable anonymous/public container access or create permanent public URLs.
- If the Experience Type changes, the evidence must be reviewed again before publication. On a Published Experience, changing the Experience Type or replacing any required evidence file returns it to PendingApproval and hides it from Explore until Admin approves it again; existing bookings are not affected. Rejection and resubmission reuse the same Experience and require the necessary evidence for its current type.
- Host declarations (such as food-safety, tools/material-safety, or privacy/location declarations) are displayed as written text inside the Experience submission flow. The Host must actively select an “I agree” checkbox before submitting; no separate PDF declaration is required.
- Record the exact declaration text or version, the Host user, the linked Experience, and the acceptance timestamp so the Admin can see what was accepted and when.
### Rejected experiences

- The Host does NOT create a new Experience after a rejection. The Host edits the same Experience and resubmits it (Rejected -> PendingApproval).
- `RejectionReason` is cleared when the experience is resubmitted.

### Editing a published experience

- Editing Title, Description, Cultural Story, meeting point or duration sends the experience back to PendingApproval (hidden from Explore until re-approved).
- Changing the Experience Type or replacing a required evidence file also sends the experience back to PendingApproval (see the evidence rules above); existing bookings are not affected.
- Price and max group size can be changed directly; existing bookings are protected by snapshots.
- If a Host is deactivated, all of their experiences become Unpublished.
- Adding or replacing media on a Published experience does not send it back to PendingApproval (media is not in the list above), but the last image and the cover image cannot be removed without a replacement: a published experience must always keep at least one image and a valid cover.

### Experience media (FINAL business rules)

- Media lives in a single `ExperienceMedia` entity (images and videos together). There is NO separate image table and NO separate video table.
- **Images:** at least 1 image is required for an Experience to be published; maximum 8 images per Experience.
- **Videos:** optional (zero is valid); maximum 2 videos per Experience. A video is never required for publishing.
- **Combined:** minimum 1 image + 0 videos; maximum 8 images + 2 videos.
- **Publishing is blocked when an Experience has no images.** This is a hard business rule enforced by the backend/domain/application layer, not only by UI validation.
- **Cover:** at most one image can be the cover; the cover must be an image (never a video). Because every published Experience has at least one image, every published Experience has a valid cover.
- Each media item stores: its experience, media type (Image or Video), storage key, display order, a cover flag and its creation date.

### Profile pictures

- Host profile picture and Guide profile picture: each profile has an optional profile image reference (`ProfileImageKey`) using the same external storage approach.
- Profile pictures accept JPEG, PNG or WebP, maximum 10 MB per image.
- There is no separate profile media entity in the MVP.

### Media and file storage

- Actual image/video files are NOT stored in SQL Server as binary data. Files live in private Azure Blob Storage; SQL Server stores only metadata and a `StorageKey`.
- `StorageKey` is used instead of permanent public URLs. Blob access is authorized by the application; if direct temporary access is needed, issue short-lived, narrowly scoped user-delegation SAS. Use Microsoft Entra ID/managed identity for the app-to-storage connection when hosted on Azure.
- Azure Blob Storage is the selected MVP storage provider. Keep storage behind an application abstraction so the domain model does not depend on the Azure SDK.
- Containers are private, public/anonymous access is disabled, and the application uses the narrowest practical storage permissions. On Azure, authorize the application with Microsoft Entra ID and a managed identity; do not store long-lived account keys in source code.
- Public Experience images accept JPEG, PNG or WebP, maximum 10 MB each. Experience videos accept MP4 encoded with H.264, maximum 100 MB each. Reject SVG, archives and executable formats.
- Private evidence documents accept PDF, JPEG or PNG, maximum 10 MB per file. A multi-page document should be uploaded as one PDF where possible. An ID card may use two image files for its front and back.
- Evidence upload counts: Food & Cooking requires 3–5 area photos; Arts & Crafts requires 3–10 portfolio photos and 1–5 workshop/tools photos; Local Traditions & Daily Life requires one location proof when the experience uses a private/home venue, plus one separate permission file when a controlled third-party venue requires it. A public route uses the in-platform route/meeting-point fields without an address document. Guide evidence is capped at two National ID images, one license document (one PDF or up to two images), one language document per declared non-native language, and one residence proof per selected work area; written permission from a document holder may add one file per applicable proof.
- Retention: keep evidence only while needed to review or support the associated active Experience or Guide verification. After rejection, withdrawal, or deletion of the associated Experience/profile, delete its private evidence within 90 days, unless an active dispute, legal hold, or applicable law requires longer retention. Apply the same retention and deletion behavior to backup copies as operationally feasible. Confirm the production retention schedule with the project’s privacy/legal reviewer before launch.
- Rejected evidence: for a rejected Experience, the 90-day deletion period starts at the rejection decision. If the Host resubmits before it ends, the deletion is cancelled, evidence files that are still valid are kept, and files replaced during resubmission are deleted when replaced. If the Host does not resubmit, all of its evidence is deleted on day 90. The active-dispute, legal-hold and applicable-law exceptions above still apply.
- Guide identity documents: after Admin approval, delete the National ID images and residence proofs within a configurable number of days (initial value 30, see Section 17). Keep the decision record (reviewer, date, document types received, and the license expiry date). Re-verification requires new uploads. The 90-day rule above still applies after rejection or withdrawal. Confirm with the privacy/legal reviewer before launch.
- Verification documents use private Azure Blob Storage (see section 13) and are never public media.

## 6. Guides (independent paid service)

- The Guide is an independent service provider, NOT a free accessory and NOT an attribute of an Experience.
- Host owns the cultural experience; Guide explains cultural/historical context and accompanies the tourist.
- Guide pricing is **hourly** (HourlyRate on the Guide profile). The rate is snapshotted into each guide booking, so later rate changes do not affect history.
- Guide must be Admin-approved before being bookable. A Guide whose license has expired (status Expired) is not bookable until re-verified (Section 13).
- A Guide has an optional profile picture (a storage key, same approach as other media). Guide verification requires private images of the National ID (front and back), the Tourist Guide License, language-level evidence for every declared non-native language, and residence evidence for every selected work area (see section 13).
- Guides declare spoken languages and working cultural areas; at least one language and one work area are required. Each declared non-native language and each selected work area must have supporting verification evidence (see Section 13). A language declared Native needs no language certificate. Tourists choose a specific approved Guide (no automatic assignment), filtering by area, language, price, and requested time availability.
- Guide hours are whole hours (minimum number of hours is a configurable value; initial value 2, see Section 17).
- In a guide booking the destination/location is free text (not tied to a Cultural Area).
- After an Experience booking becomes Completed, the app asks the tourist "Do you want to book a guide to go somewhere?" (unless their guide preference is "No guide needed"). A guide booking can also be made independently from the Guides page at any time.

## 7. Tourist Preferences and Recommendations

Preferences (all selectable by the tourist):

- Interests: Egyptian Food, Local Culture, History, Arts & Crafts, Traditional Markets, Local Communities, Nature, Family Experiences, Workshops, Egyptian Traditions
- Experience types (multi-select; use the three categories in Section 5), Cultural areas (multi-select), Languages (multi-select)
- Budget (one): Under EGP 500 / 500-800 / 800-1,500 / 1,500-3,000 / 3,000+
- Duration (one): Under 2 hours / 2-4 hours / 4-6 hours / Full day
- Guide preference: Guide preferred / No guide needed / Either is fine

Recommendation engine: **rule-based in the MVP, no AI/ML**. Match score weights:

| Factor | Weight |
|---|---|
| Interest match | 40% |
| Budget match | 25% |
| Time/duration match | 15% |
| Cultural area match | 10% |
| Previous ratings | 10% |

- UI shows textual match, e.g. "95% Match for You". No stars.
- Feedback loop: after a completed experience the tourist reviews it; the previous-ratings factor gives more weight to experience types and cultural areas the tourist rated well. Keep it simple and rule-based.
- Bucket boundaries (budget/duration) live in configuration, not hardcoded in many places.
- Recommendations only include Published experiences, so every recommended experience has at least one image and a cover image to display.
- **Eligibility filters:** Language is a hard filter: if the tourist selected languages, only Experiences that offer at least one of them are recommended. Experience Type is an optional filter: if the tourist selected types, only those types are recommended; if none were selected it has no effect. Neither is part of the score, so the weights above still total 100%.
- **Factor scoring:** each factor gets a value from 0 to 1. Interest = the overlap ratio between the tourist's interests and the Experience's interests. Budget and Duration = 1 when the Experience falls in the tourist's selected bucket, 0.5 for an adjacent bucket, 0 otherwise. Cultural area = 1 on a match, 0 otherwise. Previous ratings = derived from the tourist's own past ratings of Experiences with the same Experience Type and Cultural Area, scaled to the 0 to 1 range (the exact mapping is an implementation detail for the Classes Specs).
- **Match score:** Match = sum of (weight x factor) divided by the sum of the weights of the factors that apply, shown as a percentage. If the tourist has no previous ratings, that factor is left out of both sums, so scores can still reach 100%.
- **No preferences:** if the tourist has not set Preferences, no Match is calculated; the Recommended sort falls back to Newest and the UI prompts the tourist to set Preferences.

## 8. Search, Filter, Sort

- Search: experience title, places/areas, activities.
- Experience Explore filters: cultural area, experience type (the three required categories in Section 5), budget, duration, host type, and language. Guide availability is checked separately on the Guide booking flow after the tourist selects a requested date, start time and duration; Guides are not bundled with Experience bookings.
- Sort: Recommended, Price low to high, Price high to low, Newest.
- Filters operate on existing experience properties; no extra concepts needed.
- Explore, search and recommendation results only contain Published experiences, so every result has at least one image and a cover image to display.
- Experiences of a suspended Host, and Guides who are suspended or Expired, are filtered out of Explore, search, recommendations and Guide search while the restriction lasts; a suspension does not change the Experience status (Section 11).

## 9. Bookings

A booking is a general reservation with exactly ONE provider. There are two booking types:

1. **Experience booking** — approved by the Host. No guide inside it.
2. **Guide booking** — approved by the Guide. No experience inside it. Requires destination/location, date, start time and number of hours.

(Decision: experience and guide are separate bookings, each paid separately. The earlier combined "Experience + Guide" booking was dropped.)

- Availability is **request-based**. NO session/slot/calendar system in the MVP. The tourist requests date, start time and number of guests (and hours/destination for guides); the provider accepts or rejects.
- Booking status (separate from payment status): Pending, Confirmed, Rejected, Cancelled, Completed.
- Only Published experiences can be booked (so every bookable experience has at least one image and a cover). Unpublishing an experience later does not affect bookings that already exist.
- The booking stores snapshots so history stays stable: price per person (experience), guide hourly rate and hours (guide), the service amount, the platform fee rate and amount, and the total.
- Confirmed requires the provider's acceptance. Rejected means the provider rejected it. Providers only see a booking once payment is Paid.
- Providers can reject while Pending; a provider cannot cancel a Confirmed booking in the MVP (problems go through the dispute process).
- A booking becomes Completed when the provider marks it, and not before the scheduled end time; if the provider does not, the system marks it Completed automatically at ScheduledEnd + 24 hours when no Dispute is Open or UnderReview (Section 11).
- Time blocking: a Confirmed booking blocks its interval through the scheduled end. A Pending booking whose payment is Paid blocks it until accepted, rejected or timed out. A Pending booking whose payment is still in progress blocks it only until the failed/abandoned-payment timeout, and a booking whose payment Failed never blocks.
- A Host cannot have two Confirmed Experience bookings with overlapping time intervals; accepting fails if an overlap exists, and the check must stay correct when two accepts happen at the same moment (concurrency control at the database level). Max group size applies per booking.
- A tourist cannot book their own experience (not possible anyway with one role per account).
- Minimum lead time: a booking request must have a scheduled start at least the configured lead time (initial 72 hours, Section 17) after the request is created.
- If a booking's provider does not respond within the configured timeout, it is cancelled automatically; refund 100% of ServiceAmount and retain PlatformFeeAmount.

## 10. Pricing and Platform Fee

Per booking:

```
Experience booking:  ServiceAmount = PricePerPerson x NumberOfGuests
Guide booking:       ServiceAmount = GuideHourlyRate x GuideHours
PlatformFeeAmount  = ServiceAmount x PlatformFeeRate   (rounded to 2 decimals, away from zero)
TotalAmount        = ServiceAmount + PlatformFeeAmount
```

- Decision: the platform earns **from the platform fee only**, charged on top of the provider price. NO commission is deducted from the Host or the Guide (providers receive 100% of the service amount).
- Experience and guide bookings have separate fee rates (separate configuration values), so the Guide fee stays flexible and is NOT assumed to be 15%.
- The fee rate is snapshotted in every booking.
- The transparency badge ("X% directly supports the local host") is now computed from the booking, not a fixed 85%.
- Examples (rates are illustrative only, not final):
  - Experience: 800 x 2 guests = 1,600 + 15% fee (240) = **1,840**
  - Guide: 500/hour x 3 hours = 1,500 + 15% fee (225) = **1,725**

## 11. Payment, Payout, Cancellation, Refund

Definitions: **ServiceAmount** = the Host/Guide service price. **PlatformFeeAmount** = the platform fee added on top of the service price. **TotalAmount** = ServiceAmount + PlatformFeeAmount. **Payout** = money released to the Host/Guide. During the MVP mock, amounts and holds are simulated in the app; no real funds are collected or held.

The MVP uses a Mock Payment flow that simulates gateway steps and status updates but moves no real money. Payment status: Pending, Paid, Failed, Refunded, PartiallyRefunded. Refunded means 100% of ServiceAmount was returned (the fee is retained); PartiallyRefunded means the 50% case. Every refund is also recorded as a Refund record (booking, amount, reason, date, deciding Admin when applicable). Payout status: Pending, Paid, Cancelled; a Payout has an Amount and an EligibleAt timestamp. A production integration must use a payment provider whose contract and settlement setup support keeping provider proceeds unpaid until the service and complaint window are complete; the app database records the ledger/status but is not itself a place where money is held.

The mock gateway has exactly two outcomes, success or failure, chosen with two buttons on the mock payment page; no other scenarios are modeled in the MVP.

### Step-by-step cycle (same for both booking types)

1. Tourist prepares the booking; the UI shows ServiceAmount + PlatformFeeAmount = TotalAmount. Nothing is saved yet.
2. Tourist presses "Pay and book": the booking and payment are created together (Booking = Pending, Payment = Pending). Amounts and fee rate are snapshotted.
3. Mock gateway flow returns success or failure. On success: Payment = Paid, Booking stays Pending, and the provider is notified. On failure: Payment = Failed, the provider is not notified, and the booking closes after the configured timeout. Retry reuses the same payment record.
4. Provider accepts: Booking = Confirmed, and Payout is created as Pending for 100% of ServiceAmount with EligibleAt = ScheduledEnd + 24 hours (the Amount is recalculated if the tourist cancels, see Cancellation and refund). If the provider rejects: Booking = Rejected, ServiceAmount is refunded, PlatformFeeAmount is not refunded, and no payout is created.
5. The booking's scheduled end time starts a 24-hour complaint window. The Host/Guide cannot mark the service Completed before the scheduled end. The tourist is notified and can report a problem during the window. A provider completion click does not release funds early, and payout eligibility never depends on that click. If the provider has not marked the booking Completed by ScheduledEnd + 24 hours, the system marks it Completed automatically when no Dispute is Open or UnderReview.
6. When EligibleAt has passed and no Dispute on the booking is Open or UnderReview, the payout is releasable. In the mock, the release is simulated. In production, the app requests payout from the contracted payment provider and marks it Paid only after the provider confirms success.
7. If a dispute is opened during the window, payout remains Pending until Admin resolves it. The dispute outcome rules below then determine refund, payout and account action.
8. Tourist can write a review. After an Experience booking, the app offers "Book a guide?".

### Cancellation and refund (platform-wide)

- The PlatformFeeAmount is non-refundable in every outcome, including tourist cancellation, provider rejection/non-response, and post-service disputes. The refund rules below apply to ServiceAmount; the 15% fee example is illustrative and the final Experience/Guide fee rates remain separate configuration values.
- Tourist cancels a **Pending** booking: refund 100% of ServiceAmount because the provider has not accepted it; do not create/pay a provider payout. PlatformFeeAmount is not refunded.
- For an accepted/Confirmed booking, tourist cancels **48 hours or more before the scheduled start**: refund 100% of ServiceAmount; the tourist does not get PlatformFeeAmount back; no Host/Guide payout is made.
- For an accepted/Confirmed booking, tourist cancels **less than 48 hours before the scheduled start**: refund 50% of ServiceAmount; the remaining 50% of ServiceAmount goes to the Host/Guide; PlatformFeeAmount remains with the platform.
- Example: ServiceAmount = 1,000 and PlatformFeeAmount = 150, so TotalAmount = 1,150. At least 48 hours before start on an accepted booking: tourist receives 1,000 back and the platform keeps 150. Less than 48 hours before start on an accepted booking: tourist receives 500, the provider receives 500, and the platform keeps 150.
- Cancellation decisions are based on the scheduled start time; cancellation/refund logic is kept separate from booking status and payment status.
- Tourist cancellation is available until the scheduled start. After ScheduledStart cancellation is closed, and any problem is handled through a Dispute (Section 14).
- Refund rules apply only to money actually collected. If the payment is Pending or Failed when the tourist cancels, the booking closes with no Refund record, and PlatformFeeAmount is retained only if it was collected.
- Amounts are in EGP with 2 decimals. In the 50/50 split, the provider share is 50% of ServiceAmount rounded toward zero and the tourist refund is ServiceAmount minus the provider share, so the two always add up exactly to ServiceAmount. Fee rounding is unchanged.
- Late cancellation (less than 48 hours): the Payout Amount becomes the 50% provider share and EligibleAt stays ScheduledEnd + 24 hours. Cancellation 48 hours or more before start: the Payout is Cancelled.
- Provider rejection or provider non-response: refund 100% of ServiceAmount, do not refund PlatformFeeAmount, and make no provider payout. Guide rejection follows the same refund rule; the tourist can create a new booking with another Guide (no guide swapping on an existing booking).

### Complaints after the service and payout

- "Dispute" is the single name for any complaint, issue or report about a booking. A tourist may open a Dispute during the 24 hours after the scheduled end time (the complaint window), and also earlier on a Confirmed booking (Section 14). Any Dispute that is Open or UnderReview pauses payout release. A Dispute can only be opened until ScheduledEnd + 24 hours; after the complaint window closes no Dispute can be opened (Section 14).
- If Admin determines the Host/Guide was at fault: refund 100% of ServiceAmount, do not refund PlatformFeeAmount, and suspend the provider for a number of days entered by the Admin (default 14, see Section 17). The platform fee remains non-refundable.
- If Admin determines the tourist was at fault: no refund is made, the provider receives 100% of ServiceAmount, the PlatformFeeAmount remains with the platform, and the tourist is banned for a number of days entered by the Admin (default 30, see Section 17).
- If Admin finds no fault on either side (NoFault): no refund, no sanction, and the payout is released normally.
- Record the Admin decision, evidence/reason, refund and payout amounts, and any suspension/ban action.
- Release of a provider payout is prohibited before the 24-hour complaint window ends. After a dispute is opened, do not release it until the Admin decision is applied.
- Production payouts depend on the payment provider's actual delayed-settlement/disbursement capability and contract. The MVP only simulates this release; it does not imply the platform possesses customer funds.

### Suspension, ban and unavailability

- **Duration:** the Admin enters the number of days when applying a suspension (provider fault) or a ban (tourist fault); the defaults are 14 days for a provider suspension and 30 days for a tourist ban (Section 17). A restriction ends automatically when its period passes. The record keeps the days, start and end, the deciding Admin and the Dispute that caused it.
- **Access:** a suspended provider or banned tourist can still log in, see bookings and notifications, and open or follow a Dispute during the restriction.
- **Suspended provider:** cannot receive new bookings or accept Pending ones. Published Experiences (or the Guide profile) are filtered out of Explore, search, recommendations and Guide search for the period, without changing their status. Pending bookings of the suspended provider are cancelled automatically under the provider non-response refund rule (100% of ServiceAmount refunded, PlatformFeeAmount retained, no payout). Confirmed bookings continue normally.
- **Banned tourist:** cannot create new bookings. Existing Pending and Confirmed bookings continue normally.
- **Expired Guide license (Section 13):** treated like a suspended provider for new and Pending bookings: Pending bookings are cancelled automatically under the same non-response refund rule, and Confirmed bookings continue.

## 12. Reviews and Ratings

- A review belongs to a real, Completed booking (not directly "Tourist -> Experience"). One review per booking, written by the booking's tourist.
- Because experience and guide are separate bookings, ratings are naturally separate: a review of an Experience booking rates the experience and its Host together; a review of a Guide booking rates the Guide.
- Numerical rating 1 to 5 with one optional comment. A Host's rating is computed from the reviews of their experiences (not stored separately).
- Display textually/numerically, e.g. "4.9 · 128 reviews".
- **No star icons anywhere in the product** (no star characters, no star-based fields or UI assumptions).

## 13. Guide verification and Host experience-type verification

- Guides must be verified by Admin before tourists can book them. Hosts do not have account-level verification; each Host Experience is reviewed and approved with its type-specific evidence before tourists can book it. A Guide is available for a requested interval only if there is no overlapping Pending or Confirmed Guide booking. Pending requests whose payment is Paid block the interval until accepted, rejected or automatically cancelled at timeout (a Pending booking whose payment is still in progress blocks only until the failed/abandoned-payment timeout, and a booking whose payment Failed never blocks); after acceptance, the confirmed booking blocks the interval through its scheduled end time.
- Guide flow: register -> create Guide profile -> submit Guide verification (Pending) -> Admin reviews -> Approved or Rejected (with reason). Host experience flow: create an Experience -> submit type-specific private evidence and listing for Admin review -> approve or reject the Experience.
- Guide verification status comes from the Guide VerificationRequest. No Host-level verification process or Host verification flag exists.
- Keep Guide verification simple: one pending Guide verification request at a time per user; no complex KYC. Host evidence is scoped to an Experience, not a user-level verification request.
- The user-level VerificationRequest is for Guide verification only and must match the account's Guide role. Host experience evidence is associated with an Experience.
- License expiry: the Guide enters the Tourist Guide License expiry date as a required field when submitting verification. When that date passes, the Guide's verification status becomes Expired, set by the scheduled background service (Section 21). An Expired Guide cannot receive new bookings and is not shown as bookable until the Guide submits a new verification request with new uploads and Admin approves it. Confirmed bookings made before the expiry continue; Pending bookings are cancelled automatically (Section 11). A reminder before expiry is not part of the MVP.

### Guide verification documents and Host experience evidence

- Guide verification requires two private images of the National ID (front and back), one Tourist Guide License document (PDF or up to two images), one supporting document per declared non-native language proving at least B1 (CEFR or equivalent), and one residence document per selected work area. A language declared Native requires no certificate. A language proof may be a recognized certificate or test result. Residence proof may be the National ID address, a utility bill dated within 3 months, or a current lease. If the residence document is not in the Guide's name, include written permission from the named holder. These proofs remain private. Hosts submit the three private evidence items listed in Section 5 with each Experience, not with a Host verification request.
- A Guide verification request can have multiple private documents. Each Host Experience submission includes the three evidence items required for its type, with extra venue permission evidence where applicable.
- Documents are private and sensitive. They are NOT public media and are never part of `ExperienceMedia`.
- Files are stored in private Azure Blob Storage; SQL Server stores only metadata and a `StorageKey`. Upload limits, retention and deletion rules are defined in Section 5.
- Access requires backend authorization (Admin reviewers; the Guide verification-request owner for Guide documents; the submitting Host for their own Experience evidence). No permanent public URLs are ever created for verification documents or evidence.
- Guide verification status comes only from the Guide VerificationRequest. There is no Host verification status or `IsVerified` flag on HostProfile; Host Experience approval is governed by that Experience's Admin approval workflow.

## 14. Disputes

- A tourist (or a relevant participant of the booking) reports an issue linked to a specific booking. A Dispute can only be opened on a booking that is Confirmed or Completed, or Cancelled when a provider share exists (a late tourist cancellation, Section 11); not on a Pending or Rejected booking, and not on a Cancelled booking with no provider share.
- A Dispute can only be opened from the moment the booking is Confirmed until ScheduledEnd + 24 hours (the complaint window). After that no Dispute can be opened in the MVP, and a late complaint is handled by the Admin outside the system.
- A user can have only one active Dispute (Open or UnderReview) on the same booking.
- Flow: Open -> UnderReview -> Resolved -> Closed. A resolved Dispute records an Outcome: ProviderFault, TouristFault or NoFault. Refunds are Refund records and payment status, not a Dispute status. The "complaint window" is the 24 hours after ScheduledEnd; a Dispute Open or UnderReview at any time pauses payout release.
- Admin reviews and resolves. Any Admin decision records which Admin made it and the resolution note/reason. No conversation/messaging system in the MVP (a dispute message feature is only added if later required).

## 15. Notifications

- Tourist: booking confirmation, booking reminder, payment update, dispute update, optional `GuideSuggestion` notification after an experience.
- Host: new booking, booking status update, experience approval update.
- Guide: new guide booking request, booking status update.
- All users: verification updates.

## 16. Booking Sharing (safety)

- A tourist can share booking details with a trusted contact for safety, through a secure read-only link.
- Shared information: experience or guide service, date/time, host or guide, location/meeting point. Payment and pricing information is NOT shared.
- Do not over-engineer: no dedicated trusted-contact management in the MVP.

## 17. Configurable Business Values (not hardcoded)

| Value | Status |
|---|---|
| Experience fee rate | Illustrative 15%, not final |
| Guide fee rate | Not finalized; no default value, and the app fails fast if it is missing. A 15% placeholder is seeded in the development configuration only, marked as not final |
| Tourist cancellation threshold | 48 hours before start: refund 100% of ServiceAmount; under 48 hours: refund 50% of ServiceAmount and pay the other 50% to the provider |
| Platform fee refundability | Never refundable in any outcome |
| Post-service complaint window | 24 hours after scheduled end |
| Provider response timeout | Initial value 12 hours (configurable) |
| Failed/abandoned payment timeout | Initial value 15 minutes (configurable) |
| Minimum guide hours | Initial value 2 (configurable) |
| Minimum booking lead time | Initial value 72 hours (configurable) |
| Guide identity document deletion after approval | Initial value 30 days (configurable) |
| Provider suspension default duration | Initial value 14 days (the Admin enters the number of days per case) |
| Tourist ban default duration | Initial value 30 days (the Admin enters the number of days per case) |
| Object storage provider | Azure Blob Storage |
| Allowed image and video file types | Images: JPEG, PNG, WebP; videos: MP4 (H.264) |
| Maximum image size / maximum video size | 10 MB per image; 100 MB per video |
| Budget and duration bucket boundaries | As listed in section 7 |

**Why 72 and 12 hours:** with a 48-hour threshold for the full ServiceAmount refund, a booking made at the minimum lead time of 72 hours and accepted after the full 12-hour response timeout still leaves the tourist at least 12 hours (72 - 12 - 48) in which a cancellation returns 100% of ServiceAmount.

## 18. MVP Scope

In scope: authentication/authorization, the four roles, experiences with admin approval and media (images/videos), profile pictures, verification document uploads, preferences, rule-based recommendations, experience bookings, guide bookings, mock payment, payouts, reviews, verification, cancellation/refund, disputes, notifications, booking sharing.

Out of scope unless explicitly added later: real payment gateway, machine learning recommendations, real-time chat, complex KYC, loyalty system, advanced analytics, AI chatbot, multi-vendor accounting, complex tax system, session/slot/calendar availability system.

## 19. Global Product Rules

- No star icons anywhere (ratings and match are textual/numeric).
- Match is displayed as "95% Match for You".
- Payment is mock/sandbox only.
- Recommendations are rule-based only.
- Booking status and payment status are always separate.
- Never silently change an established business rule.

---

## 20. Current Decision Register

The following are the currently approved business decisions. Older alternatives and superseded rules are intentionally omitted so they cannot be mistaken for current requirements. When a rule is not listed here or in the relevant product section, treat it as open rather than infer approval.

- **Experience taxonomy:** exactly three initial categories: Food & Cooking, Arts & Crafts, and Local Traditions & Daily Life. Workshops are a format, not a category. Every Experience has exactly one primary category.
- **Host verification:** no account-level Host verification. Verification evidence is submitted and reviewed for each Experience, according to its category. Required declarations are shown in the platform and accepted by the Host with an auditable checkbox record.
- **Guide verification:** account/profile-level Admin verification. Required evidence covers identity, guide license, each declared non-native language, and each selected work area; the detailed rules are in Section 13.
- **Guide availability:** an overlapping Pending or Confirmed Guide booking blocks the requested interval; Pending (payment Paid) holds the interval until accepted, rejected, or timed out (payment in progress holds it only until the payment timeout; Failed never holds it), and Confirmed holds it through the scheduled end.
- **Booking types:** Experience and Guide services are separate bookings with one provider per booking. Guide bookings are independently paid; there is no combined Experience-plus-Guide booking.
- **Pricing:** provider service price plus a platform fee added on top. The provider is entitled to 100% of ServiceAmount under the normal completed-service flow. Experience and Guide fee rates are separate configuration values; 15% is illustrative, not final.
- **Platform fee:** PlatformFeeAmount is non-refundable in every outcome, including cancellation, provider rejection/non-response, and disputes.
- **Tourist cancellation:** a Pending booking can be cancelled with 100% of ServiceAmount refunded because the provider has not accepted it; no provider payout is made. For an accepted/Confirmed booking, cancellation at least 48 hours before start returns 100% of ServiceAmount; less than 48 hours returns 50% of ServiceAmount and allocates the other 50% to the provider. The PlatformFeeAmount is never refunded. Cancellation is available until the scheduled start; after that a Dispute is used.
- **Post-service complaints:** a 24-hour complaint window starts at scheduled end. Payout cannot be released before the window ends; an open complaint holds payout for Admin resolution. Provider fault means full ServiceAmount refund, fee retained, and provider suspension measured in days. Tourist fault means no ServiceAmount refund, provider receives full ServiceAmount, fee retained, and tourist is banned. Sanction durations are entered by the Admin in days (defaults 14 for providers, 30 for tourists); their scope is in Section 11.
- **Mock payment:** MVP simulates payment and payout states and moves no real money. A production payment provider and its settlement capabilities must be confirmed before live payments.
- **Dispute window:** a Dispute can be opened only from Confirmed until ScheduledEnd + 24 hours; Cancelled bookings are disputable only when a provider share exists. Later complaints are outside the MVP.
- **Booking timing:** minimum lead time 72 hours and provider response timeout 12 hours, so the tourist keeps at least 12 hours of full-refund window.
- **Experience changes:** changing the Experience Type or replacing a required evidence file on a Published Experience returns it to PendingApproval and hides it; existing bookings are unaffected.
- **Guide license expiry:** the expiry date is stored; an expired license makes the Guide Expired and not bookable until re-verified.
- **Sanctions:** suspended or banned users keep login and Dispute access; Confirmed bookings continue; Pending bookings of a suspended provider or Expired Guide are cancelled under the non-response refund rule.
- **Recommendations:** Language is a hard filter, Experience Type an optional filter, and the Match score is the weighted sum divided by the weights that apply (Section 7). Explore also filters by language.
- **Mock payment:** two outcomes only, success or failure.
- **Media, storage, stack, identity and architecture:** use the requirements in Sections 5, 17–19. These are not reopened by the ERD/Classes Specs work unless the owner approves a change.

## 21. Implementation Assumptions

These are operational interpretations for the design work, not newly approved business rules. The ERD and Classes Specs should make the dependencies visible and flag them where a decision is still needed.

- Booking amounts and applicable fee rate are snapshotted when the booking is created.
- Payout is represented separately from Payment. A Pending payout is not a claim that real money is held by the platform; the MVP only simulates it.
- Completion by the provider does not release payout early. Payout eligibility follows the scheduled end and complaint-window rules in Section 11.
- A pending/confirmed booking and its service-time information must remain historically stable if the Experience or Guide profile later changes.
- Evidence retention and deletion follow the limits in Sections 5 and 13, with any active dispute, legal hold, or applicable law taking precedence.

### Approved technical rules (approved in V1.18)

- All instants (booking start and end, timeouts, EligibleAt) are stored in UTC and displayed in Africa/Cairo with the zone label; the 48-hour and 24-hour rules compare UTC instants.
- The Domain project has no reference to Identity, EF Core or Azure packages. The application user class (based on IdentityUser of Guid) lives in Infrastructure; Host and Guide profiles and other entities store only the UserId (Guid).
- Ids are not assigned with a random Guid in entity constructors. Use EF Core's sequential Guid generator, or version 7 Guids on .NET 9 or later, applied consistently to every entity.
- Timeouts, reminders, automatic completion, payout eligibility, evidence cleanup, Guide license expiry, and the end of suspensions and bans run in one background service inside the web application (single application instance in the MVP). Rules read stored timestamps such as EligibleAt, so a missed run catches up on the next run. Revisit if more than one instance is deployed.

## 22. Open Items

Every item that blocked the ERD or the Classes Specs was closed in V2.0 and merged into the numbered sections (see the Decision Register in Section 20). What remains is Post-MVP or pre-launch work and does not block the design or implementation of the MVP.

**Production payments (Post-MVP)**
- Final Experience and Guide platform-fee rates. The platform fee remains non-refundable in every outcome. The development configuration seeds 15% placeholders (Section 17).
- Production payment provider/contract and the supported delayed-disbursement flow. Mock Payment does not custody or transfer real funds.

**Before production launch (legal and privacy review)**
- Owner-confirmed rule to revisit with the legal reviewer: the platform fee stays non-refundable in every outcome, including provider rejection, non-response and provider fault.
- Evidence retention schedule (90 days after rejection or withdrawal; Guide identity documents 30 days after approval): confirm with the privacy/legal reviewer.

**Deferred beyond the MVP**
- Reminder notification before a Guide license expires.
- Handling of complaints raised after the complaint window closes (handled by the Admin outside the system for now).

**Resolved infrastructure decisions**

Azure Blob Storage with private containers, accepted upload types/limits, authentication, Clean Architecture, GitHub Flow, and lookup deletion behavior are specified in Sections 5, 17-19. They are not open items.

## 23. Design Process Status

Workflow: approved business context -> ERD -> Classes Specs -> C# Entity classes -> EF Core configurations -> DbContext -> migrations -> application workflows and UI.

**Current position:** the business context is consolidated in V2.0 (V1.18 plus the owner-approved decisions of 2026-10-18, with every MVP-blocking open item closed). The next project deliverables are the updated ERD and Classes Specs, in that order, based on this context. The existing ERD and Classes Specs V1.4 are outdated references and must not be treated as the implementation source where they conflict with this context. Begin C# Entity implementation only after the ERD and Classes Specs have been reviewed and approved.

## 24. AI Working Instructions

- Treat this V2.0 context as the current business source of truth. Do not treat old change-log entries, the existing ERD, or Classes Specs V1.4 as current when they conflict with this context.
- The next step is to create/update the ERD and then create/update the Classes Specs from this context. Do not jump ahead to Entity implementation before those deliverables are reviewed and approved.
- Preserve the business concept; do not invent unrelated features or alter approved rules.
- No machine learning and no real payment integration in the MVP; payment and payout are simulated.
- Keep Host and Guide conceptually distinct. Each booking has one provider.
- Prefer a normalized relational design; avoid unnecessary entities and inheritance; do not create a table for every enum.
- C# Entity classes must follow the approved ERD and Classes Specs, including explicit Join Classes for the many-to-many relationships and the agreed navigation policy.
- Stack: ASP.NET Core MVC (Razor Views) with HTML, CSS and JavaScript, no frontend framework. All IDs are Guid.
- Never silently change an established business rule. For a proposed change, explain its reason, the requirement it supports, impact on the ERD and .NET entity model, complexity, and whether it is required for MVP or optional. Clearly label unresolved proposals as proposals.
- The media rules (minimum 1 image / maximum 8 images, optional videos up to 2, at most one image cover, external storage with StorageKey, private verification evidence) are final unless the project owner approves a change.
- Section 25 is the version history and Section 26 holds the glossary and ADR candidates. Where wording differs, the numbered Sections 1 to 24 are authoritative.
- Respond in Arabic when talking to the project owner unless asked otherwise.

---

## 25. Version History

| Version | Date | Summary |
|---|---|---|
| V1.16 | 2026-10-17 | Consolidated business context: roles, experiences with evidence-based verification, separate Experience and Guide bookings, platform fee, payout and refund rules, disputes. |
| V1.18 | 2026-10-18 | V1.16 plus the owner-approved resolutions of the V1.17 design review: Completed timing and automatic completion, single Dispute concept, Refund records and Dispute outcomes, payout amount and EligibleAt, time-blocking and overlap rules, UTC storage, Identity outside Domain, one background service, exact meeting point privacy, Guide document deletion after approval. |
| V2.0 | 2026-10-18 | Baseline for the ERD: V1.18 plus the owner-approved closing decisions: wording fixes (Sections 6, 14, 22 to 24); lead time 72 hours and provider response 12 hours (9, 17); Dispute window and cancellation cutoff (11, 14); Type and evidence change re-approval (5); rejected-evidence retention (5); Guide license expiry (13); suspension, ban and unavailability rules (11, 17); recommendation filters and Match formula (7); Language filter in Explore (8); Mock Payment outcomes and Guide fee placeholder (11, 17). All MVP-blocking open items closed. |

---

## 26. Glossary and ADR Candidates

### 26.1 Glossary

| Term | Meaning |
|---|---|
| Provider | The Host or Guide who delivers the service in a booking. Each booking has exactly one. |
| Booking | One reservation with one provider: an Experience booking or a Guide booking. |
| ServiceAmount | The provider's price for the booking. The provider receives 100% of it in the normal flow. |
| PlatformFeeAmount | The fee added on top of ServiceAmount. Never refundable. |
| TotalAmount | ServiceAmount + PlatformFeeAmount. |
| Dispute | The single record for a tourist's complaint about a booking. |
| Complaint window | The 24 hours after ScheduledEnd. A Dispute can be opened from Confirmed until the window closes, and not after it. |
| Completed | The booking status showing the service took place. Set at or after ScheduledEnd (Section 9). |
| Payout | The provider's money released after the complaint window with no open Dispute. |
| EligibleAt | The moment a Payout becomes releasable: ScheduledEnd + 24 hours. |
| Experience Type | The one required top-level category of an Experience (three fixed values). |
| Interest | A tag used for recommendation matching. An Experience has one or more. |
| Host Type | The kind of Host: Family, Individual, Artisan or Farmer. Independent of Experience Type. |
| Cultural Area | The Egyptian region of an Experience (Cairo, Giza, and so on). |
| Suspension | A temporary restriction on a provider after a ProviderFault outcome: no new or accepted bookings, hidden from Explore, for a number of days set by the Admin. |
| Ban | A temporary restriction on a tourist after a TouristFault outcome: no new bookings, for a number of days set by the Admin. |
| Expired | The verification status of a Guide whose license expiry date has passed; the Guide is not bookable until re-verified. |

### 26.2 ADR candidates

| ADR | Decision | Related |
|---|---|---|
| ADR-1 | One primary role per account, chosen at registration | Section 3 |
| ADR-2 | Platform fee is never refunded | Section 11 |
| ADR-3 | Experience and Guide are separate bookings, each with one provider | Section 9 |
| ADR-4 | Host evidence belongs to an Experience, not to the Host | Sections 5 and 13 |
| ADR-5 | Identity types live in Infrastructure, not in Domain | Section 21 |
| ADR-6 | All instants stored in UTC, displayed in Africa/Cairo | Section 21 |
| ADR-7 | Payout eligibility is time-based (EligibleAt) and independent of the provider's click | Section 11 |
| ADR-8 | A Dispute can only be opened from Confirmed until ScheduledEnd + 24 hours | Sections 11 and 14 |
| ADR-9 | Suspensions and bans block new activity but never change existing Confirmed bookings | Section 11 |
