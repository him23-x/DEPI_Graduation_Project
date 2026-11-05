# Work Order — Discover Egypt

Source: `Docs/SRS.md`, `Docs/AGENT.md`, and applicable entity definitions in `Docs/Discover_Egypt_Classes_Specs_V2.0.md`. This is a proposed dependency/value order, not an implementation status report. Open Questions from the SRS remain unresolved and are not treated as requirements.

## Phase 1 — Foundations

### FR-01 — Accounts
- **BRs:** BR-01
- **NFRs:** NFR-01, NFR-02 (cross-cutting authorization starts here)
- **needs:** none
- **Cycles**
  - **Register account**
    - DAL: persist Identity user and its single selected role.
    - BLL: registration request and validation; prevent self-registration as Admin.
    - PL: shared registration form for Tourist, Host, and Guide.
    - Test: permitted roles register; Admin registration is unavailable.
  - **Log in and out**
    - DAL: use the existing Identity store.
    - BLL: authenticate and issue/clear the session.
    - PL: shared login and logout flow for all roles.
    - Test: each role can sign in; an invalid credential is rejected.

### FR-02 — Profiles and verification
- **BRs:** BR-01, BR-05
- **NFRs:** NFR-01, NFR-02, NFR-03
- **needs:** FR-01
- **Cycles**
  - **Create or edit HostProfile**
    - DAL: persist one profile per user.
    - BLL: enforce ownership and profile rules from the class specification.
    - PL: host profile form and page.
    - Test: one HostProfile per account; another user's profile cannot be changed.
  - **Create or edit GuideProfile**
    - DAL: persist one profile per user and its language/area selections.
    - BLL: validate the profile fields against the class specification.
    - PL: guide profile form and page.
    - Test: one GuideProfile per account; selections persist.
  - **Submit guide verification**
    - DAL: persist VerificationRequest and private VerificationDocument metadata.
    - BLL: validate required identity, license, language, and work-area evidence per BR-05; store private files through the storage contract.
    - PL: submit verification and upload evidence.
    - Test: incomplete evidence cannot be submitted; unauthorized users cannot read documents.
  - **Review guide verification**
    - DAL: persist review status, admin, and review time.
    - BLL: approve or reject the submitted request and record a rejection reason when rejected.
    - PL: Admin review page.
    - Test: only Admin can review; decisions and ownership are enforced.

## Phase 2 — Core flow

### FR-03 — Experience management
- **BRs:** BR-02, BR-03, BR-04, BR-05, BR-06
- **NFRs:** NFR-01, NFR-02, NFR-03
- **needs:** FR-01, FR-02
- **Cycles**
  - **Create Experience draft**
    - DAL: persist Experience and its required lookup references.
    - BLL: validate required fields and BR-02; create in Draft status.
    - PL: host create form; use the Application DTO directly unless a distinct form model is needed.
    - Test: required story, area, interest, language, and one experience type are enforced.
  - **Edit Draft or Rejected Experience**
    - DAL: update the owned Experience and its interest/language links.
    - BLL: allow only Draft/Rejected edits and clear rejection reason on resubmission as specified.
    - PL: edit form for eligible statuses.
    - Test: only the owner can edit; PendingApproval and Unpublished remain read-only to Host.
  - **Manage ExperienceMedia**
    - DAL: persist media metadata and storage keys.
    - BLL: enforce file formats/sizes, 1–8 images, up to 2 videos, and image-only single cover (BR-04, BR-06).
    - PL: upload, order, and select cover.
    - Test: limits and private storage rules are enforced.
  - **Save ExperienceEvidence**
    - DAL: persist type-specific evidence text/file metadata.
    - BLL: enforce the applicable evidence requirements and private access (BR-05, BR-06).
    - PL: evidence forms and private uploads.
    - Test: evidence is not publicly accessible and required evidence is validated.
  - **Accept ExperienceDeclaration**
    - DAL: persist declaration type, text/version, host, and acceptance time.
    - BLL: require the declaration applicable to the experience type before submission.
    - PL: declaration confirmation control.
    - Test: submission cannot proceed without required acceptance.
  - **Submit or resubmit Experience**
    - DAL: persist the status transition and submission data.
    - BLL: enforce Draft/Rejected → PendingApproval, BR-03, and publishing prerequisites in BR-02/04/05.
    - PL: submit-for-review action and status feedback.
    - Test: missing prerequisites block submission; PendingApproval/Unpublished are not editable by Host.

### FR-04 — Experience review
- **BRs:** BR-03, BR-05, BR-06
- **NFRs:** NFR-01, NFR-02, NFR-03
- **needs:** FR-03
- **Cycles**
  - **List and inspect submitted Experience**
    - DAL: query submitted experiences and private evidence metadata.
    - BLL: restrict review data and evidence to Admin.
    - PL: Admin queue and review details.
    - Test: Admin can inspect evidence; other roles cannot.
  - **Approve or reject Experience**
    - DAL: persist decision, reviewer, and reason.
    - BLL: enforce valid status transitions and rejection reason.
    - PL: Admin decision actions.
    - Test: only Admin can decide; status and reason persist.
  - **Unpublish or republish Experience**
    - DAL: persist status changes.
    - BLL: enforce Admin-only action and republish prerequisites (BR-03).
    - PL: Admin status actions.
    - Test: republishing fails until prerequisites are met; Host cannot edit Unpublished content.

### FR-05 — Explore
- **BRs:** BR-02, BR-07, BR-08
- **NFRs:** NFR-01, NFR-02, NFR-03
- **needs:** FR-03, FR-04, FR-02
- **Cycles**
  - **Explore published Experiences**
    - DAL: query only Published experiences and public fields.
    - BLL: apply requested search, filter, and sort options.
    - PL: explore and results pages.
    - Test: unpublished data and exact meeting points are not exposed publicly.
  - **Browse and select approved Guides**
    - DAL: query verified/approved Guide profiles and public fields.
    - BLL: exclude unapproved guides from bookable results.
    - PL: guide list and details.
    - Test: only approved Guides appear as eligible for booking.

### FR-07 — Bookings
- **BRs:** BR-07, BR-08, BR-09
- **NFRs:** NFR-01, NFR-02, NFR-03, NFR-04
- **needs:** FR-05, FR-01, FR-02
- **Cycles**
  - **Create Experience Booking**
    - DAL: persist Booking with experience, provider, schedule, guest count, and price snapshots.
    - BLL: enforce eligibility, 72-hour lead time, provider overlap, pricing, and exact-location privacy.
    - PL: booking form and confirmation page.
    - Test: BR-07–09 hold for Experience bookings.
  - **Create Guide Booking**
    - DAL: persist Booking with Guide, schedule, hours, destination, and rate snapshot.
    - BLL: enforce approved Guide eligibility, 72-hour lead time, overlap, and pricing rules.
    - PL: guide booking form and confirmation page.
    - Test: BR-08/09 hold for Guide bookings.

### FR-08 — Mock payment
- **BRs:** BR-09, BR-11
- **NFRs:** NFR-01, NFR-02, NFR-04, NFR-05
- **needs:** FR-07
- **Cycles**
  - **Process mock payment**
    - DAL: persist Payment separately from Booking status.
    - BLL: simulate success/failure only; preserve booking price/fee snapshots.
    - PL: payment result and separate booking/payment statuses.
    - Test: both outcomes work; no real payment or funds movement occurs.

### FR-09 — Provider booking management
- **BRs:** BR-07, BR-08, BR-11
- **NFRs:** NFR-01, NFR-02, NFR-04, NFR-05
- **needs:** FR-07, FR-08
- **Cycles**
  - **List and inspect provider Bookings**
    - DAL: query only the provider's paid bookings.
    - BLL: enforce provider ownership and exact-location access rules.
    - PL: Host/Guide booking list and details.
    - Test: unpaid or another provider's booking is inaccessible.
  - **Accept or reject Booking**
    - DAL: persist decision and status.
    - BLL: enforce 12-hour response window and valid transitions.
    - PL: provider decision actions.
    - Test: unauthorized and late/invalid transitions are rejected.
  - **Complete Booking**
    - DAL: persist completion status.
    - BLL: allow completion only under the specified booking rules.
    - PL: completion action.
    - Test: only the assigned provider can complete an eligible booking.

### FR-10 — Cancellation and refunds
- **BRs:** BR-10, BR-11
- **NFRs:** NFR-01, NFR-02, NFR-04, NFR-05
- **needs:** FR-07, FR-08, FR-09
- **Cycles**
  - **Cancel Booking and record Refund**
    - DAL: persist cancellation, Refund, and resulting payment/payout state.
    - BLL: calculate refund by pending/confirmed status and 48-hour rule; exclude platform fee.
    - PL: Tourist cancellation and result detail.
    - Test: each BR-10 calculation and eligibility boundary is verified.

### FR-11 — Payouts
- **BRs:** BR-11, BR-15
- **NFRs:** NFR-01, NFR-02, NFR-04, NFR-05
- **needs:** FR-08, FR-09, FR-10
- **Cycles**
  - **List provider Payouts**
    - DAL: query payout data scoped to the current provider.
    - BLL: expose amount, status, and EligibleAt without leaking other providers' data.
    - PL: Host/Guide earnings and payout list.
    - Test: ownership and eligibility timestamps are correct.
  - **Release eligible Payout**
    - DAL: persist simulated release status and time.
    - BLL: require service end + 24 hours and no open/in-review dispute; apply UTC calculations.
    - PL: simulated release status.
    - Test: early release and release during a blocking dispute are rejected; no real funds move.

### FR-12 — Disputes
- **BRs:** BR-10, BR-11, BR-12, BR-15
- **NFRs:** NFR-01, NFR-02, NFR-03, NFR-04, NFR-05
- **needs:** FR-07, FR-08, FR-11
- **Cycles**
  - **Open Dispute**
    - DAL: persist Dispute and opener.
    - BLL: enforce eligible booking states and 24-hour window; leave Host eligibility unresolved per Open Questions.
    - PL: Tourist dispute form and status.
    - Test: eligible and ineligible booking cases follow BR-12.
  - **Review and resolve Dispute**
    - DAL: persist outcome, note, admin, resolution time, refunds, and sanctions.
    - BLL: enforce Admin-only decisions and BR-12 outcomes/BR-15 default sanction durations; preserve payout hold behavior.
    - PL: Admin dispute queue and details.
    - Test: each outcome applies its specified refund, sanction, and payout effect.

## Phase 3 — Secondary flows

### FR-06 — Tourist preferences and recommendations
- **BRs:** BR-14
- **NFRs:** NFR-01, NFR-02
- **needs:** FR-05
- **Cycles**
  - **Save TouristPreference**
    - DAL: persist preference and its interest/type/area/language links.
    - BLL: validate selected lookup references.
    - PL: preferences form.
    - Test: preference values and selections persist for their owner.
  - **List recommended Experiences**
    - DAL: query eligible published Experiences and required matching data.
    - BLL: apply BR-14 hard language filter, optional type filter, weights, and renormalization.
    - PL: recommendation list and match ordering.
    - Test: deterministic score and filter cases follow BR-14.

### FR-13 — Reviews
- **BRs:** BR-13
- **NFRs:** NFR-01, NFR-02
- **needs:** FR-09
- **Cycles**
  - **Create Review**
    - DAL: persist one Review per Booking.
    - BLL: enforce booking owner, Completed state, rating 1–5, and one review per booking.
    - PL: Tourist rating/comment form.
    - Test: invalid owner/status/rating and duplicate review are rejected.
  - **Display reviews and rating summary**
    - DAL: query reviews and aggregate values.
    - BLL: expose numeric/text rating without star icons.
    - PL: review list and summary.
    - Test: displayed average/count match the stored reviews.

### FR-14 — Notifications
- **BRs:** BR-01, BR-03, BR-08, BR-11, BR-12
- **NFRs:** NFR-01, NFR-02, NFR-04
- **needs:** FR-02, FR-04, FR-08, FR-09, FR-12
- **Cycles**
  - **Create and list Notification**
    - DAL: persist and query notifications by recipient.
    - BLL: create notifications for the specified verification, experience, booking, payment, and dispute events.
    - PL: notification list/component.
    - Test: recipients and event associations are correct.
  - **Mark Notification read**
    - DAL: persist IsRead for the owned notification(s).
    - BLL: enforce recipient ownership.
    - PL: mark-one and mark-all actions where supported by the design.
    - Test: another user's notification cannot be changed.

### FR-15 — Booking sharing
- **BRs:** BR-07
- **NFRs:** NFR-01, NFR-02, NFR-03
- **needs:** FR-07, FR-09
- **Cycles**
  - **Create BookingShare**
    - DAL: persist unique token and expiry.
    - BLL: issue a read-only share for an eligible owned booking; expose exact meeting point only under BR-07.
    - PL: create/copy share link.
    - Test: token uniqueness, expiry, and owner authorization hold.
  - **Read shared Booking**
    - DAL: query by valid token.
    - BLL: enforce expiry and read-only field allowlist.
    - PL: public token page.
    - Test: expired/invalid tokens fail; no edit actions or disallowed private data are exposed.

### FR-16 — Administration
- **BRs:** BR-01, BR-12, BR-15
- **NFRs:** NFR-01, NFR-02, NFR-04, NFR-05
- **needs:** FR-01, FR-02, FR-04, FR-08, FR-11, FR-12
- **Cycles**
  - **Activate or deactivate ApplicationUser**
    - DAL: persist IsActive.
    - BLL: enforce Admin-only action and the agreed account state rules.
    - PL: Admin account management.
    - Test: non-Admin cannot change account state; Admin cannot self-register.
  - **Monitor Booking, Payment, and Payout**
    - DAL: query administrative transaction and payout summaries.
    - BLL: restrict access to Admin and use stored booking/payment snapshots.
    - PL: Admin monitoring dashboard.
    - Test: totals and status breakdowns match source records; non-Admin access is denied.

## Open Questions carried forward

- File storage backend by environment (the current development decision supports local storage; production choice remains open).
- Final Experience and Guide platform fee rates.
- Production payment provider and settlement method.
- Evidence retention periods.
- Whether Hosts may open disputes.
- Handling complaints received after the 24-hour dispute window.
- Measurable performance, availability, backup/recovery, and browser support targets.
- Required UI languages.

Do not schedule or implement cycles whose behavior depends on an unresolved Open Question until the team decides it.
