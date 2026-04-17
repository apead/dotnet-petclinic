# .NET PetClinic Sample Application

A .NET 10 port of the classic [Spring PetClinic](https://github.com/spring-projects/spring-petclinic) sample application, built with **ASP.NET Blazor Web App** (SSR + Interactive Server), **Entity Framework Core**, and **SQLite**.

> This project lives in `src-dotnet/PetClinic/` alongside the original Java source. The Java code is untouched.

## Understanding the application

PetClinic is a sample application that demonstrates how to build a data-driven web app with .NET 10. It manages a veterinary practice — owners, their pets, vets, specialties, and visit history.

### Domain model

```
Owner ──< Pet >── PetType
          │
          └──< Visit

Vet >──< Specialty
```

### Key technology choices

| Concern | Technology |
|---------|-----------|
| Web framework | ASP.NET Blazor Web App (SSR + Interactive Server) |
| ORM | Entity Framework Core 10 |
| Database | SQLite (file `petclinic.db`, auto-created on first run) |
| UI | Bootstrap 5 + Font Awesome 6 (CDN) |
| Caching | `IMemoryCache` (vet list, 10-minute TTL) |

## Run PetClinic locally

**.NET 10 SDK** or later is required. You can download it from [https://dot.net](https://dot.net).

Clone the repo and navigate to the project folder:

```bash
git clone https://github.com/apead/dotnet-petclinic.git
cd dotnet-petclinic/src-dotnet/PetClinic
```

Run the application:

```bash
dotnet run
```

Then access PetClinic at <http://localhost:5119>.

The SQLite database file (`petclinic.db`) is created automatically on first startup and seeded with sample data (10 owners, 13 pets, 6 vets, 6 pet types, 4 visits).

## Building a container

Build a container image with the .NET SDK's built-in publish support:

```bash
dotnet publish -c Release /t:PublishContainer
```

Or write a `Dockerfile` using the official .NET 10 base images:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PetClinic.dll"]
```

## Database configuration

By default, PetClinic uses **SQLite** with the database file stored at the project root. The connection string is in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=petclinic.db"
}
```

The database is created automatically via `EnsureCreated()` — no migrations are needed for a fresh install.

To switch to a different database (e.g., SQL Server or PostgreSQL), install the corresponding EF Core provider package and update the connection string:

```bash
# SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

Then replace `UseSqlite(...)` with `UseSqlServer(...)` or `UseNpgsql(...)` in `Program.cs`.

## Working with PetClinic in your IDE

### Prerequisites

- [.NET 10 SDK](https://dot.net) (or newer)
- [Git](https://help.github.com/articles/set-up-git)
- Your preferred IDE:
  - [Visual Studio 2022+](https://visualstudio.microsoft.com/) (Community edition is free)
  - [VS Code](https://code.visualstudio.com) with the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension
  - [JetBrains Rider](https://www.jetbrains.com/rider/)

### Steps

1. Clone the repository:

    ```bash
    git clone https://github.com/apead/dotnet-petclinic.git
    ```

2. **In Visual Studio:** Open `src-dotnet/PetClinic/PetClinic.csproj` (or the solution root). Press **F5** to run with the debugger.

3. **In VS Code:** Open the `src-dotnet/PetClinic` folder. The C# Dev Kit will detect the project. Use the Run & Debug panel or:

    ```bash
    dotnet run
    ```

4. **In Rider:** Open `src-dotnet/PetClinic/PetClinic.csproj`. Click the Run button or use the built-in terminal to `dotnet run`.

5. Navigate to <http://localhost:5119> (HTTP) or <https://localhost:7081> (HTTPS).

> **Note on the `/oups` route:** This page intentionally throws an `InvalidOperationException` to simulate an unhandled error — the same behaviour as the original Java `CrashController`. When running under a debugger, the debugger will break here; this is expected. Run without a debugger to see the error page.

## Looking for something in particular?

| .NET / Blazor concept | File |
|-----------------------|------|
| Entry point & DI wiring | [`Program.cs`](Program.cs) |
| EF Core DbContext | [`Data/PetClinicContext.cs`](Data/PetClinicContext.cs) |
| Seed data | [`Data/DbInitializer.cs`](Data/DbInitializer.cs) |
| Domain models | [`Models/`](Models/) |
| Service layer | [`Services/`](Services/) |
| Blazor pages | [`Components/Pages/`](Components/Pages/) |
| Shared layout & navbar | [`Components/Layout/MainLayout.razor`](Components/Layout/MainLayout.razor) |
| Pagination component | [`Components/Shared/Pagination.razor`](Components/Shared/Pagination.razor) |
| App settings | [`appsettings.json`](appsettings.json) |
| Styles | [`wwwroot/app.css`](wwwroot/app.css) |
| Vets JSON API | `GET /api/vets` (registered in `Program.cs`) |

## Scaffolding convention

This project was bootstrapped exclusively with `dotnet new`:

```bash
dotnet new blazor -n PetClinic -o src-dotnet/PetClinic --framework net10.0
```

See [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md) for the reusable Copilot prompt used to generate this conversion.

## Contributing

Pull requests and issue reports are welcome. Please open an issue before submitting large changes.

## License

This project is a port of the [Spring PetClinic](https://github.com/spring-projects/spring-petclinic) sample application, which is released under version 2.0 of the [Apache License](https://www.apache.org/licenses/LICENSE-2.0). This .NET port is released under the same license.
