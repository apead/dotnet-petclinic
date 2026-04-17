# PetClinic .NET 10 Blazor — Copilot Instructions

This project is a complete conversion of the [Spring PetClinic](https://github.com/spring-projects/spring-petclinic) sample application to **.NET 10 ASP.NET Blazor Web App** with Interactive Server rendering.

---

## Project Layout

```
src-dotnet/PetClinic/
├── Components/
│   ├── App.razor                  # Root HTML shell (adds Font Awesome CDN + Bootstrap)
│   ├── Layout/MainLayout.razor    # Bootstrap navbar (green #6db33f) + body wrapper
│   ├── Pages/
│   │   ├── Home.razor                         @page "/"
│   │   ├── Error.razor                        @page "/Error"
│   │   ├── NotFound.razor                     @page "/not-found"
│   │   ├── Oups.razor                         @page "/oups"
│   │   ├── Owners/
│   │   │   ├── FindOwners.razor               @page "/owners/find"
│   │   │   ├── OwnersList.razor               @page "/owners"
│   │   │   ├── OwnerDetails.razor             @page "/owners/{OwnerId:int}"
│   │   │   └── CreateOrUpdateOwner.razor      @page "/owners/new" + @page "/owners/{OwnerId:int}/edit"
│   │   ├── Pets/
│   │   │   └── CreateOrUpdatePet.razor        @page "/owners/{OwnerId:int}/pets/new" + edit
│   │   ├── Visits/
│   │   │   └── CreateOrUpdateVisit.razor      @page "/owners/{OwnerId:int}/pets/{PetId:int}/visits/new"
│   │   └── Vets/
│   │       └── VetList.razor                  @page "/vets"
│   ├── Shared/
│   │   └── Pagination.razor       # Reusable Bootstrap pagination component
│   ├── Routes.razor
│   └── _Imports.razor             # Global @using directives
├── Data/
│   ├── PetClinicContext.cs        # EF Core DbContext (SQLite)
│   └── DbInitializer.cs           # Seeds sample data at startup
├── Models/
│   ├── BaseEntity.cs              # Id (int), IsNew bool
│   ├── NamedEntity.cs             # : BaseEntity, Name [Required, StringLength(80)]
│   ├── Person.cs                  # : BaseEntity, FirstName, LastName
│   ├── Owner.cs                   # : Person, Address, City, Telephone, Pets
│   ├── Pet.cs                     # : NamedEntity, BirthDate, TypeId, OwnerId, Visits
│   ├── PetType.cs                 # : NamedEntity
│   ├── Specialty.cs               # : NamedEntity
│   ├── Vet.cs                     # : Person, Specialties, SpecialtiesDisplay
│   └── Visit.cs                   # : BaseEntity, VisitDate, Description, PetId
├── Services/
│   ├── OwnerService.cs            # Search (paginated), GetById, Create, Update
│   ├── PetService.cs              # GetPetTypes, GetById, Create, Update
│   ├── VisitService.cs            # Create
│   └── VetService.cs              # GetAll (cached), GetPaged
├── Program.cs
├── appsettings.json
└── wwwroot/app.css
```

---

## Domain Model & Relationships

### Entity Hierarchy
```
BaseEntity (Id, IsNew)
  ├── NamedEntity (Name)
  │     ├── PetType
  │     ├── Specialty
  │     └── Pet (BirthDate, TypeId → PetType, OwnerId → Owner, Visits[])
  └── Person (FirstName, LastName)
        ├── Owner (Address, City, Telephone, Pets[])
        └── Vet (Specialties[] ← many-to-many)

Visit (BaseEntity: VisitDate, Description, PetId → Pet)
```

### Database Tables (EF Core table names)
| Entity     | Table           |
|------------|-----------------|
| Owner      | owners          |
| Pet        | pets            |
| PetType    | types           |
| Visit      | visits          |
| Vet        | vets            |
| Specialty  | specialties     |
| Vet↔Spec   | vet_specialties |

---

## Route Mapping (Java Spring → .NET Blazor)

| Java Spring MVC Route           | .NET Blazor Page Route                                    |
|---------------------------------|-----------------------------------------------------------|
| GET /                           | `/` → Home.razor                                          |
| GET /owners/find                | `/owners/find` → FindOwners.razor                         |
| GET /owners?lastName=X&page=N   | `/owners` → OwnersList.razor (query params)               |
| GET /owners/{id}                | `/owners/{OwnerId:int}` → OwnerDetails.razor              |
| GET /owners/new                 | `/owners/new` → CreateOrUpdateOwner.razor                 |
| POST /owners/new                | same (OnValidSubmit) → redirect to /owners/{id}           |
| GET /owners/{id}/edit           | `/owners/{OwnerId:int}/edit` → CreateOrUpdateOwner.razor  |
| POST /owners/{id}/edit          | same (OnValidSubmit) → redirect to /owners/{id}           |
| GET /owners/{id}/pets/new       | `/owners/{OwnerId:int}/pets/new` → CreateOrUpdatePet.razor|
| GET /owners/{id}/pets/{pid}/edit| `/owners/{OwnerId:int}/pets/{PetId:int}/edit`             |
| GET /owners/{id}/pets/{pid}/visits/new | `/owners/{OwnerId:int}/pets/{PetId:int}/visits/new` |
| GET /vets.html                  | `/vets` → VetList.razor                                   |
| GET /api/vets (JSON)            | `/api/vets` → minimal API in Program.cs                   |
| GET /oups                       | `/oups` → Oups.razor                                      |

---

## EF Core Configuration Patterns

- **Database**: SQLite via `Microsoft.EntityFrameworkCore.Sqlite`
- **Connection string**: `appsettings.json → ConnectionStrings:DefaultConnection` = `"Data Source=petclinic.db"`
- **DbContext**: `PetClinicContext(DbContextOptions<PetClinicContext>)` — primary constructor pattern
- **Seeding**: `DbInitializer.Initialize(context)` called at startup via `IServiceScope`; uses `EnsureCreated()` + guard `if (context.Vets.Any()) return`
- **Cascade delete**: Owner→Pets, Pet→Visits use `OnDelete(DeleteBehavior.Cascade)`
- **Many-to-many**: Vet↔Specialty via `UsingEntity(j => j.ToTable("vet_specialties"))` (no explicit join entity)
- **Includes**: Services always `Include` navigation properties needed for display (avoid lazy loading)

---

## Blazor Component Conventions

- **Render mode**: All interactive pages use `@rendermode InteractiveServer` at the top of the component
- **Layout**: `MainLayout.razor` is the default layout (set in `Routes.razor` or via `@layout`)
- **Shared components**: Live in `Components/Shared/` and are auto-imported via `_Imports.razor` (`@using PetClinic.Components.Shared`)
- **Parameter passing**: Route params use `[Parameter]`, query string params use `[SupplyParameterFromQuery]`
- **Navigation**: Use injected `NavigationManager.NavigateTo(url)` for programmatic navigation after form submission
- **Forms**: Use `EditForm` with `DataAnnotationsValidator`, `ValidationSummary`, `ValidationMessage`, and `InputText`/`InputSelect`/`InputTextArea`
- **Date inputs**: Use a `string` backing field + `@onchange` handler to work around Razor nested-quote limitations with `DateOnly.ToString("yyyy-MM-dd")`
- **Pagination**: Use the `<Pagination>` shared component with `CurrentPage`, `TotalPages`, and `OnPageChange` parameters
- **Loading state**: Pages use an `isLoading` bool to show "Loading..." while awaiting data

---

## Validation Rules

| Entity   | Field       | Rules                                                      |
|----------|-------------|------------------------------------------------------------|
| Person   | FirstName   | Required, MaxLength 30                                     |
| Person   | LastName    | Required, MaxLength 30                                     |
| Owner    | Address     | Required, MaxLength 255                                    |
| Owner    | City        | Required, MaxLength 80                                     |
| Owner    | Telephone   | Required, Regex `\d{10}` (10-digit number)                 |
| Pet      | Name        | Required, MaxLength 80 (via NamedEntity)                   |
| Pet      | BirthDate   | Required, DataType.Date                                    |
| Pet      | TypeId      | Required (int, must not be 0)                              |
| Visit    | VisitDate   | Required, DataType.Date, defaults to today                 |
| Visit    | Description | Required, MaxLength 255                                    |

---

## Service Registration

All services are registered as **Scoped** in `Program.cs`:

```csharp
builder.Services.AddDbContext<PetClinicContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddMemoryCache();          // for VetService caching
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<PetService>();
builder.Services.AddScoped<VisitService>();
builder.Services.AddScoped<VetService>();
```

`VetService` uses `IMemoryCache` (registered via `AddMemoryCache()`) to cache the full vet list for 10 minutes.

---

## Scaffolding New Blazor Projects

When creating new Blazor projects in this solution, always scaffold with:

```bash
dotnet new blazor --interactivity Server --name <ProjectName> --output <OutputPath>
```

This ensures:
- `.NET 10` target framework
- Interactive Server rendering mode configured by default
- Correct `Program.cs` and `App.razor` baseline

Do **not** use `dotnet new blazorserver` (legacy) or `dotnet new blazorwasm`.

---

## Key Implementation Notes

1. **`_Imports.razor`** includes `@using PetClinic.Models` and `@using PetClinic.Services` so all pages have access to domain types and services without extra `@using` directives.

2. **`App.razor`** includes Font Awesome 4.7 from CDN for navbar icons (`fa fa-home`, `fa fa-search`, etc.).

3. **`MainLayout.razor`** renders the Bootstrap navbar with the PetClinic green (`#6db33f`). Content goes inside `.container.xd-container` for consistent padding.

4. **`Pagination.razor`** is only rendered when `TotalPages > 1`. It emits `OnPageChange` events; pages handle navigation themselves via `NavigationManager`.

5. **Date handling**: `DateOnly` is used for `BirthDate` and `VisitDate`. HTML `<input type="date">` expects `yyyy-MM-dd` format. Use a `string` backing field named `*DateStr` and an `OnChange` event handler to parse.

6. **`/api/vets` endpoint**: Minimal API in `Program.cs` returning anonymous JSON objects (same contract as the original Spring `@ResponseBody` endpoint).
