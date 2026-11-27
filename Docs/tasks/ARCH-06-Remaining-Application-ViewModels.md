# ARCH-06: Remaining Application ViewModels

## What to respect
- `Docs/AGENT.md`: application page data belongs in feature DTOs, account form ViewModels stay in Presentation, and DTO-to-ViewModel mappings are prohibited.
- `Docs/AGENT.md`: Application must not depend on Presentation; dependencies point inward.
- Current implementation: preserve existing Host Experience, Host Dispute, Host Profile, and account behavior.

## Tasks
1. Move Host Experience, Host Dispute, and Host Profile application data types from `Application/ViewModels` into `Application/DTOs/<Feature>` and update references without changing behavior.
2. Keep Login/Register forms under `Presentation/ViewModels/Account`; make account commands and validators independent of Presentation.
3. Remove obsolete ViewModels references and rename validation files to match their targets, then build and run the existing test suite.

## Acceptance criteria
- No types remain under `DiscoverEgypt.Application.ViewModels`.
- Application has no dependency on Presentation types.
- DTO and Response distinctions follow `Docs/AGENT.md`.
- Build and existing tests pass.

## Execution Report
Completed. Build succeeded (0 errors); test suite passed (115/115). Existing AutoMapper vulnerability warnings remain.
