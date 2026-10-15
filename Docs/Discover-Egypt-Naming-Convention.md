# Discover Egypt — C# Naming Convention

**Applies to:** the Discover Egypt ASP.NET Core + Entity Framework Core project, using Clean Architecture.

Use these rules for new code and when touching existing code. Keep names consistent across the domain, application, infrastructure, API, and database layers. Prefer clear domain names over abbreviations.

## 1. General C# style

- Use **PascalCase** for namespaces, types, methods, and public members: `ExperienceService`, `GetByIdAsync`.
- Use **camelCase** for parameters and local variables: `experienceId`, `bookingRequest`.
- Use **`_camelCase`** for private instance fields: `_experienceRepository`.
- Use **PascalCase** for constants: `MaximumPageSize`.
- Spell out words. Avoid unclear abbreviations such as `Exp`, `Usr`, or `Req`; common terms such as `Id`, `DTO`, and `API` are acceptable where they improve clarity.
- Name by responsibility and domain meaning. Avoid generic names such as `Manager`, `Helper`, `Processor`, or `Data` when a precise name is available.
- Keep one primary public type per file, and name the file after that type.

```csharp
// Correct
public sealed class ExperienceService { }
private readonly IExperienceRepository _experienceRepository;

// Incorrect
public sealed class ExpMgr { }
private readonly IExperienceRepository experienceRepo;
```

## 2. Entities, IDs, foreign keys, and navigations

- Use `int` for entity primary keys and foreign keys throughout the project. Do not introduce `Guid` identifiers.
- Name a primary key `Id` for ordinary entities. Use `<EntityName>Id` for foreign keys: `ExperienceId`, `BookingId`.
- Name entity types with a singular domain noun in PascalCase: `Experience`, `Booking`, `Review`.
- Name scalar properties in PascalCase and use specific nouns: `Title`, `CreatedAtUtc` (only when that field is actually part of the agreed model).
- Name a reference navigation after the related entity: `Experience`, `Tourist`.
- Name a collection navigation with the plural related entity name: `Bookings`, `Reviews`.
- Make a foreign key and its reference navigation consistent: `ExperienceId` pairs with `Experience`.
- Represent optional relationships with nullable foreign keys (for example `int?`) and nullable reference navigations where appropriate. Required relationships use non-nullable foreign keys and must be configured consistently in EF Core.
- Do not add reverse collection navigations to lookup entities or to `ApplicationUser`. Add a navigation only when the agreed model needs it; configure relationships from the owning entity or explicitly in EF Core.
- `ApplicationUser` derives from `IdentityUser<int>`. Do not add reverse collection navigations to it. Use `int` for references to the user and choose an explicit FK name that reflects its role (for example, `TouristId` or `HostId`) only if that role is part of the agreed model.

### Explicit many-to-many join entities

- Model many-to-many relationships with an explicit join entity class. Do not rely on an implicit EF Core many-to-many join.
- Name the join entity by combining the two entity names in a consistent order, typically alphabetical: `ExperienceCategory`.
- Give it the two `int` foreign keys, matching reference navigations if useful, and **no standalone `Id` property**.
- Configure the two foreign keys as a composite primary key in EF Core. The composite key is the identity of the join row.
- If the relationship later has its own domain data or lifecycle, discuss the model change with the team before changing its key design.

```csharp
// Illustrative example only; this does not assert that these are agreed project fields.
public sealed class ExperienceCategory
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}

// In the EF Core configuration:
builder.HasKey(x => new { x.ExperienceId, x.CategoryId });
```

```csharp
// Incorrect: no standalone Id on an explicit join entity
public int Id { get; set; }
```

## 3. Enums and enum values

- Name enum types with a singular domain concept in PascalCase: `BookingStatus`.
- Name enum members in PascalCase, without numeric prefixes or all-caps: `Pending`, `Confirmed`, `Cancelled`.
- Use enums for a fixed, code-owned set of values. Use lookup entities when values need database-managed records or other agreed behavior.
- Do not assign numeric values unless persistence or an external contract requires stable values. If explicit values are needed, assign and document them deliberately; never reorder or reuse persisted values.
- Store enums as integers by default unless the team explicitly configures string conversion for the database or API contract.

```csharp
// Illustrative values only; confirm the actual domain states with the team.
public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled
}
```

## 4. DTOs, requests, and responses

- Keep transport models out of the Domain project. Place them in the API/Application boundary agreed for the solution.
- Suffix data transfer types with `Dto`, such as `ExperienceDto` or `BookingSummaryDto`.
- Suffix inbound command/query models with `Request`: `CreateExperienceRequest`, `UpdatePreferencesRequest`.
- Suffix outbound API models with `Response`: `ExperienceDetailsResponse`, `BookingListResponse`.
- Use a specific operation or purpose in the type name; avoid vague names such as `ExperienceModel`, `DataRequest`, or `Result`.
- Do not expose EF entities directly from controllers. Map to response/DTO types and validate request types at the application boundary.
- Keep request and response names aligned with the feature and endpoint. Do not create a type for a single scalar value unless it makes the contract clearer or is required by the framework.

```csharp
// Correct
public sealed record CreateExperienceRequest(string Title);
public sealed record ExperienceDetailsResponse(int Id, string Title);

// Incorrect
public sealed class ExperienceModel { }
public sealed class DataRequest { }
```

These snippets are naming examples only; the illustrated properties are not a declaration of the project schema.

## 5. Interfaces, services, repositories, and controllers

- Prefix interfaces with `I`: `IExperienceService`, `IBookingRepository`.
- Name application services `<Feature>Service`; name their interfaces `I<Feature>Service`.
- Name repository interfaces `<Entity>Repository`; name implementations the same without `I`.
- Keep repository abstractions in the layer that owns the application/domain contract; put EF Core implementations in Infrastructure.
- Name ASP.NET Core controllers `<Resource>Controller` in singular PascalCase: `ExperiencesController` is acceptable if the team consistently uses plural resource names; for this project, use **plural route resource + singular controller type** (`ExperienceController`, route `/api/v1/experiences`).
- Controllers should handle HTTP concerns and delegate application behavior. Do not put business rules or EF queries in controllers.
- Avoid suffixes like `Impl`, `Concrete`, or `Default` unless multiple implementations make the distinction necessary.

```csharp
public interface IExperienceService { }
public sealed class ExperienceService : IExperienceService { }

public interface IExperienceRepository { }
public sealed class ExperienceRepository : IExperienceRepository { }

[ApiController]
[Route("api/v1/experiences")]
public sealed class ExperienceController : ControllerBase { }
```

## 6. API routes and actions

- Use lowercase, plural resource names in routes, with kebab-case for multiword segments: `/api/v1/experiences`, `/api/v1/booking-requests`.
- Use nouns for resources. Let the HTTP method express the action; avoid routes such as `/getExperiences` or `/createBooking`.
- Use `/{id:int}` for an individual resource identifier. Keep route parameter names lowercase: `{experienceId}` when the route needs a named non-generic ID.
- Nest a resource only when the relationship is part of the public API, for example `/api/v1/experiences/{experienceId}/reviews`.
- Use query parameters for filtering, sorting, and pagination rather than encoding them in route names.
- Keep API versioning consistent; use the configured versioning approach. The route examples here use `/api/v1` as the team convention.

| Purpose | Example |
|---|---|
| List resources | `GET /api/v1/experiences` |
| Read one | `GET /api/v1/experiences/{id}` |
| Create | `POST /api/v1/experiences` |
| Replace/update | `PUT /api/v1/experiences/{id}` |
| Partial update, if supported | `PATCH /api/v1/experiences/{id}` |
| Delete | `DELETE /api/v1/experiences/{id}` |

```text
Correct:   GET /api/v1/experiences?search=...
Incorrect: GET /api/v1/getExperiences
```

## 7. DbContext, EF Core configuration, and migrations

- Name the context `<ProjectName>DbContext` or `<Purpose>DbContext`; use one agreed context name consistently. Do not use the generic name `Context`.
- Name `DbSet` properties with plural entity names: `Experiences`, `Bookings`.
- Put entity mapping classes in Infrastructure under a `Configurations` folder. Name each `<Entity>Configuration`, implementing `IEntityTypeConfiguration<TEntity>`.
- Keep table, key, relationship, index, and column configuration in the entity configuration where practical. Avoid scattering EF configuration across startup code and entities.
- Configure explicit join composite keys in the join entity configuration with `HasKey(x => new { ... })`.
- Use one consistent database table naming policy. For this project, explicitly map table names to singular PascalCase entity names (for example, `ToTable("Experience")`) so migrations do not depend on incidental `DbSet` naming.
- Use clear, imperative migration names in PascalCase, describing the schema change: `AddExperienceTables`, `AddBookingStatusIndex`. Do not use dates, initials, or vague names such as `Update`.
- Review generated migrations before committing. A migration should reflect the intended model change; do not edit an already-applied migration to change deployed schema history.
- Keep migration files with the Infrastructure/database project and use the solution's configured startup project and context when generating them.

```csharp
// Illustrative configuration
public sealed class ExperienceConfiguration
    : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.ToTable("Experience");
        builder.HasKey(x => x.Id);
    }
}
```

## 8. Async methods

- Add the `Async` suffix to every method that performs asynchronous work or returns a `Task`/`Task<T>`: `GetByIdAsync`, `SaveChangesAsync`.
- Propagate async calls through controller, application, repository, and EF Core layers. Use `await`; do not block with `.Result` or `.Wait()`.
- Use `Task` for asynchronous operations without a return value and `Task<T>` when returning a value. Use `CancellationToken cancellationToken` on I/O-bound application and infrastructure methods, and pass it through to EF Core where supported.
- Do not mark a method `async` if it has no asynchronous work and simply returns an existing task.

```csharp
// Correct
Task<Experience?> GetByIdAsync(int id, CancellationToken cancellationToken);

// Incorrect
Task<Experience?> GetById(int id);
var experience = repository.GetByIdAsync(id).Result;
```

## 9. Nullability, booleans, dates, and money

- Enable nullable reference types and express optional values explicitly with `?`. Do not use `null!` to silence warnings except for framework-required EF navigation initialization; prefer required constructors or `required` properties where compatible with the project.
- Use `bool` for true/false state. Name boolean properties and variables with `Is`, `Has`, `Can`, or `Should` where that reads naturally: `IsActive`, `HasAvailability`, `CanCancel`.
- Avoid ambiguous booleans such as `active`, `flag`, or `status` when a precise state name is possible. Do not use a boolean when the domain has more than two meaningful states; use an enum or an agreed model.
- Use `DateTimeOffset` for instants that cross system or timezone boundaries. Persist and compare instants in UTC; name UTC-only values with a `Utc` suffix when it prevents ambiguity (for example, `CreatedAtUtc`).
- Use `DateOnly` for a calendar date without a time and `TimeOnly` for a local clock time when supported by the database provider and domain model.
- Do not store dates as strings or treat a local time as UTC without an explicit conversion.
- Use `decimal` for monetary amounts, never `float` or `double`. Define precision and scale in EF Core configuration. Pair an amount with a currency code when the model needs to support or communicate currency; do not assume a currency or add fields without agreement.
- Use descriptive names for amounts and rates (for example, `Amount`, `FeePercentage`) only when those concepts exist in the agreed model.

## 10. Namespaces and folders (Clean Architecture)

- Use the solution's actual root namespace, followed by layer and feature. Namespace segments use PascalCase and mirror folders.
- Keep dependencies pointing inward: Domain has no dependency on Application, Infrastructure, or API; Application depends on Domain; Infrastructure implements Application/Domain abstractions; API is the presentation boundary.
- Organize feature code by responsibility, consistently across the solution. Example layout:

```text
src/
  DiscoverEgypt.Domain/
    Entities/
    Enums/
    Common/
  DiscoverEgypt.Application/
    Features/
      Experiences/
        DTOs/
        Requests/
        Responses/
        Services/
    Interfaces/
  DiscoverEgypt.Infrastructure/
    Persistence/
      Configurations/
      Migrations/
    Repositories/
  DiscoverEgypt.Api/
    Controllers/
```

This is a folder and namespace example, not a requirement to create every folder or layer shown. Follow the existing solution structure if it has already been agreed. Use the actual project/assembly names in namespaces; `DiscoverEgypt` above is illustrative.

```csharp
namespace DiscoverEgypt.Application.Features.Experiences.Requests;
```

## 11. Git branches, commits, and pull requests

- Use lowercase kebab-case branch names with a type prefix and short feature description:
  - `feature/experience-search`
  - `fix/booking-validation`
  - `refactor/experience-repository`
  - `docs/naming-convention`
- Include an issue or ticket number when the team uses one: `feature/123-experience-search`.
- Write commits in the Conventional Commits style: `type(scope): imperative summary`.
- Use these common types: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`.
- Keep each commit focused. Do not combine unrelated features or reformat the entire project in a functional change.
- Open pull requests with a short, descriptive title using the same style as a commit where practical. The description should state what changed, why, and any relevant migration/API impact. Link the related issue when available.

```text
Correct branch: feature/booking-confirmation
Correct commit: feat(bookings): add confirmation response
Correct PR:     feat(bookings): add confirmation response

Incorrect branch: AhmedWork
Incorrect commit: changes
```

## 12. Team review checklist

Before requesting review, confirm:

- [ ] C# types and members follow PascalCase; locals and parameters follow camelCase.
- [ ] Entity IDs and FKs use `int`; the user entity derives from `IdentityUser<int>`.
- [ ] Explicit many-to-many joins have two FK columns, a composite key, and no standalone `Id`.
- [ ] Lookup entities and `ApplicationUser` have no reverse collection navigations.
- [ ] Entities, API contracts, services, repositories, and controllers have distinct, descriptive names.
- [ ] Async methods end in `Async`, are awaited, and pass cancellation where applicable.
- [ ] Nullability, boolean, date/time, and money types match the meanings they represent.
- [ ] EF mappings and migration names are clear and consistent.
- [ ] Routes use lowercase plural resource nouns; branch, commit, and PR names are descriptive.
- [ ] Examples in this document were not mistaken for approved project fields or domain states; confirm schema and business rules with the team.
