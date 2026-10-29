# DiscoverEgypt project conventions

Use this guide when adding or reorganizing code. Preserve current behavior unless a user explicitly approves a behavior change.

## Project tree

```text
DiscoverEgypt/
├── DiscoverEgypt.Domain/
│   ├── Entities/
│   └── Enums/
│       ├── Bookings/
│       ├── Disputes/
│       ├── Experiences/
│       ├── Hosts/
│       ├── Notifications/
│       ├── Payments/
│       ├── Preferences/
│       ├── Sanctions/
│       └── Verification/
├── DiscoverEgypt.Application/
│   ├── Contracts/
│   │   ├── Notifications/
│   │   ├── Persistence/
│   │   └── Storage/
│   ├── DTOs/
│   │   ├── Bookings/
│   │   ├── HostExperiences/
│   │   └── Notifications/
│   ├── Enums/
│   │   ├── Bookings/
│   │   ├── HostExperiences/
│   │   └── Notifications/
│   ├── Features/
│   │   ├── Bookings/
│   │   │   ├── Handlers/{Commands,Queries}/
│   │   │   ├── Options/
│   │   │   ├── Policies/
│   │   │   ├── Requests/{Commands,Queries}/
│   │   │   └── Validators/{Commands,DTOs}/
│   │   └── HostExperiences/
│   │       ├── Handlers/{Commands,Queries}/
│   │       ├── Policies/
│   │       ├── Requests/{Commands,Queries}/
│   │       └── Validators/{Commands,DTOs}/
│   ├── Mapping/
│   ├── Models/                 # Settings types only; do not use as a general model folder.
│   └── Responses/<Feature>/    # For operation-result classes/records; create when needed.
├── DiscoverEgypt.Persistence/
│   ├── Configurations/Entities/
│   ├── Migrations/
│   ├── Repositories/Host/{Bookings,Experiences}/
│   └── Services/{Bookings,Notifications}/
├── DiscoverEgypt.Infrastructure/
│   └── Storage/
└── DiscoverEgypt.Presentation/
    ├── Controllers/{Booking,Experience,Notifications}/
    ├── Mapping/
    ├── ViewComponents/Host/Notifications/
    ├── ViewModels/{Bookings,Experience,Notifications}/
    └── Views/
```

Group files by feature and responsibility. Keep each enum in its own file. Do not place unrelated types together merely to reduce the number of files.

## Type placement and shape

| Type | Location | Suffix | Shape |
|---|---|---|---|
| Domain entity | `DiscoverEgypt.Domain/Entities` | Entity name | `class` |
| Domain enum used by entities | `DiscoverEgypt.Domain/Enums/<Feature>` | Enum name | `enum`, one per file |
| Application enum not used by domain entities | `DiscoverEgypt.Application/Enums/<Feature>` | Enum name; use `Outcome` for operation-result enums | `enum`, one per file |
| Application input DTO | `DiscoverEgypt.Application/DTOs/<Feature>` | `Dto` | `class` with `{ get; set; }` properties |
| Application output DTO | `DiscoverEgypt.Application/DTOs/<Feature>` | `Dto` | `sealed record` |
| CQRS command | `DiscoverEgypt.Application/Features/<Feature>/Requests/Commands` | `Command` | Follow the existing command style (`sealed record`) |
| CQRS query | `DiscoverEgypt.Application/Features/<Feature>/Requests/Queries` | `Query` | Follow the existing query style (`sealed record`) |
| Operation response represented by a class/record | `DiscoverEgypt.Application/Responses/<Feature>` | `Response` | Use a record for immutable data; use a class only when mutability is required |
| Storage contract data such as `StoredExperienceMedia` or `ExperienceMediaDownload` | `DiscoverEgypt.Application/Contracts/Storage` | No DTO suffix | Keep the contract's existing `class` or `record` shape |
| MVC view model | `DiscoverEgypt.Presentation/ViewModels/<Feature>` | `ViewModel` | Use a mutable class for form input; otherwise follow the nearest equivalent |
| Settings/configuration model | `DiscoverEgypt.Application/Models` | A specific settings name, such as `EmailSettings` | `class` |

Enums that represent operation outcomes are enums, not response DTOs: place them under `Application/Enums/<Feature>` and name them `*Outcome`. Only class/record operation-result types belong under `Application/Responses/<Feature>` and use the `Response` suffix. Do not name an enum `*Response`.

Commands and queries are the CQRS request types. Do not add a duplicate input DTO when an existing command/query already represents that application request. MVC form binding types belong in `Presentation/ViewModels`; application input DTOs remain in `Application/DTOs` when the application boundary needs a separate contract.

## Dependency direction

- `Domain` is the innermost layer and must not reference `Application`, `Persistence`, `Infrastructure`, or `Presentation`.
- `Application` may reference `Domain`; it owns use cases, CQRS requests/handlers, validators, DTOs, and interfaces in `Contracts`.
- `Persistence` and `Infrastructure` implement application contracts and may reference `Application` and `Domain`.
- `Presentation` may reference `Application` and `Domain`. It may reference `Persistence` and `Infrastructure` only to compose/register implementations at startup. Controllers and view models must use application contracts rather than calling persistence or infrastructure directly.
- Keep dependencies pointing inward toward `Domain` and `Application`; do not create project-reference cycles.

## Naming and change rules

- Use the suffixes `Dto`, `Command`, `Query`, and `Response` for those respective types. Use `Outcome` for application outcome enums. Keep feature enums named for their meaning rather than adding a generic suffix.
- Keep `Application/DTOs` for DTOs only. Put storage-bound transfer types in `Contracts/Storage`, enums in the appropriate `Enums/<Feature>` folder, and operation result classes/records in `Responses/<Feature>`.
- `Application/Models` is only for settings/configuration models. Do not use `Model` as a generic suffix or create a general-purpose Models bucket.
- Before creating a type, search for an equivalent or similar type in the same feature and layer. Reuse or extend the existing type when it has the same responsibility; do not create a duplicate shape under a new name without a clear boundary reason.
- Keep DTO input classes mutable with `{ get; set; }`. Keep DTO output records as `sealed record`. Do not change DTO shape without updating its mapping, validator, handler, and MVC usage.
- Keep enums in individual files under their feature folder; never declare an enum inside a DTO, command, entity, or a mixed-purpose file.
- Moving or renaming a type must update its namespace, file name, references, mappings, validators, and Razor imports. It must not change application behavior.
- Do not relax the existing no-edit rules for experiences in `PendingApproval` or `Unpublished` as part of structural refactoring.

Before completing a structural change, build `DiscoverEgypt/DiscoverEgypt.slnx`, run available tests, and inspect `git diff --check` and the changed-file list.
