# Discover Egypt project guide

This guide records the project structure and conventions agreed by the team. Follow the current code and approved design when working in the repository.

## Project structure

```text
DiscoverEgypt/
├── DiscoverEgypt.Domain/
│   ├── Entities/
│   └── Enums/<Feature>/
├── DiscoverEgypt.Application/
│   ├── Common/Behaviors/
│   ├── Contracts/
│   │   ├── HostExperiences/
│   │   ├── Notifications/
│   │   ├── Persistence/
│   │   └── Storage/
│   ├── DTOs/<Feature>/
│   ├── Enums/<Feature>/
│   ├── Features/<Feature>/
│   │   ├── Handlers/{Commands,Queries}/
│   │   ├── Requests/{Commands,Queries}/
│   │   ├── Validators/
│   │   └── Policies/, Services/, Options/  # where present for the feature
│   ├── Mapping/
│   ├── Models/                         # settings only
│   └── Responses/<Feature>/            # agreed location when needed
├── DiscoverEgypt.Persistence/
│   ├── Configurations/Entities/
│   ├── Identity/
│   ├── Migrations/
│   ├── Repositories/Host/<Feature>/
│   ├── Seeding/
│   └── Services/<Feature>/
├── DiscoverEgypt.Infrastructure/
│   └── Storage/
├── DiscoverEgypt.Presentation/
│   ├── BackgroundServices/
│   ├── Controllers/<Feature>/
│   ├── ViewComponents/Host/Notifications/
│   ├── ViewModels/Account/             # Login and Register forms
│   ├── Views/{Account,Host,Shared}/
│   └── wwwroot/
└── DiscoverEgypt.Tests/
```

Group files by feature and responsibility. Do not put unrelated C# types together in a folder. Every enum has its own file; do not declare an enum inside another type's file.

## Type location, name, and shape

| Type | Location | Name / suffix | Shape |
|---|---|---|---|
| Domain entity | `Domain/Entities/` | Domain name, no generic suffix | `class` |
| Enum used by Domain entities | `Domain/Enums/<Feature>/` | Meaningful enum name | `enum`, one per file |
| Application enum not used by Domain entities | `Application/Enums/<Feature>/` | Meaningful enum name; operation outcomes end in `Outcome` | `enum`, one per file |
| Application DTO | `Application/DTOs/<Feature>/` | `Dto` | Input: mutable `class` with `{ get; set; }`; output: `sealed record` |
| CQRS command | `Application/Features/<Feature>/Requests/Commands/` | `Command` | `sealed record` |
| CQRS query | `Application/Features/<Feature>/Requests/Queries/` | `Query` | `sealed record` |
| Operation response represented by a class or record | `Application/Responses/<Feature>/` | `Response` | Immutable data: `record`; mutable data: `class` |
| Storage contract data | `Application/Contracts/Storage/` | No DTO suffix | Keep the agreed `class` or `record` shape |
| Settings/configuration type | `Application/Models/` | Specific settings name | `class`; this folder is for settings only |
| MVC form ViewModel, when it has a distinct purpose | `Presentation/ViewModels/<Feature>/` | Existing account examples use `ViewModel` | Current Login and Register forms are `class` types |

Keep `Application/DTOs/<Feature>/` for DTOs only. Do not use it for enums, storage contract data, or operation response types.

Classify every `*Result` by its actual type before moving or renaming it. An operation-result enum belongs in `Application/Enums/<Feature>/` and uses the `Outcome` suffix. An operation-result class or record belongs in `Application/Responses/<Feature>/` and uses the `Response` suffix. An enum is never named `Response`.

`ExperienceMediaDownload` and `StoredExperienceMedia` belong in `Application/Contracts/Storage/` without a DTO suffix.

## Presentation models

- Use an Application DTO directly in a Razor view when it already represents the data the page needs.
- Keep a ViewModel only when it serves a distinct form or presentation purpose. Current Login and Register ViewModels are the account form models.
- Do not add mappings between Application DTOs and Presentation ViewModels.

## Layer dependencies

- `Domain` is the innermost layer and does not reference the other project layers.
- `Application` references `Domain` and owns use cases, requests, handlers, validators, DTOs, and application contracts.
- `Persistence` and `Infrastructure` implement Application contracts and reference `Application` and `Domain`.
- `Presentation` uses `Application` and `Domain`. Its startup composition registers the Persistence and Infrastructure implementations.
- Keep project references directed inward toward `Domain` and `Application`; do not introduce project-reference cycles.

## Before adding a type

Search the same feature and layer for an existing type with the same or similar responsibility. Reuse or extend it when it already represents that responsibility; add a new type only when it has a distinct role in the agreed design.
