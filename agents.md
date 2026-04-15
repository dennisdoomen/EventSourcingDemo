# ClubAdmin – AI Agent Instructions

This file provides context for AI coding agents (GitHub Copilot, Claude, etc.) working in this repository.

## Project Overview

**ClubAdmin** is a modular administration system for sports clubs. It is built with two independently
deployable backend modules:

| Module | Technology |
|---|---|
| Member Management | Azure Functions v4 · Eventuous · Azure SQL |
| Financial Transactions | Azure Functions v4 · Wolverine · EventStoreDB · Liquid Projections · Dapper |

Frontend: **Next.js 15** (App Router, SSR) · React 19 · TypeScript · MUI v6 · Ant Design v5 · pnpm

## Repository Structure

```
ClubAdmin/
├── agents.md                          ← this file
├── .github/
│   ├── workflows/build.yml            ← thin Nuke wrapper
│   └── skills/                        ← AI agent skills
│       ├── coding-conventions.md
│       ├── code-review.md
│       └── check-rework.md
├── Build/                             ← Nuke build project
├── src/
│   ├── Shared/ClubAdmin.Shared/       ← common value objects + interfaces
│   ├── Members/
│   │   ├── ClubAdmin.Members/         ← domain + Eventuous application layer
│   │   ├── ClubAdmin.Members.Api/     ← Azure Functions v4
│   │   └── ClubAdmin.Members.Specs/   ← xUnit tests
│   ├── Finances/
│   │   ├── ClubAdmin.Finances/        ← domain + Wolverine handlers + Dapper projections
│   │   ├── ClubAdmin.Finances.Api/    ← Azure Functions v4
│   │   └── ClubAdmin.Finances.Specs/  ← xUnit tests
│   └── AppHost/
│       └── ClubAdmin.AppHost/         ← .NET Aspire (local dev only)
├── frontend/
│   └── club-admin-ui/                 ← Next.js SSR frontend
└── infra/
    └── ClubAdmin.Infra/               ← Pulumi IaC (Azure only)
```

## Build Instructions

```powershell
# Windows
./build.ps1 Compile
./build.ps1 Test
./build.ps1 Publish

# Linux / macOS
./build.sh Compile
./build.sh Test
./build.sh Publish
```

Available Nuke targets: `CalculateVersion`, `Restore`, `Compile`, `Test`, `GenerateCodeCoverageReport`, `Publish`, `Default`

## Local Development

Start all services and the frontend via .NET Aspire:

```bash
dotnet run --project src/AppHost/ClubAdmin.AppHost
```

This starts:
- SQL Server container (Members event store + read models; Finances read models)
- EventStoreDB container (Finances event store)
- Members API Azure Function
- Finances API Azure Function
- Next.js frontend (`pnpm dev`)
- Aspire Dashboard (observability)

**Note:** Aspire is for local development only. Azure provisioning is handled by Pulumi in `infra/`.

## Testing

```bash
dotnet test                            # run all tests
dotnet test --filter "Category=Unit"  # run unit tests only
```

Test projects use the `.Specs` suffix (both project name and class name). Tests are treated as executable
specifications using xUnit, FluentAssertions, and FakeItEasy.

## Architecture Decisions

### Domain-Driven Design + Vertical Slice Architecture

Each module owns its domain model, commands, events, projections, and API. There is no shared domain
logic — only shared infrastructure types live in `ClubAdmin.Shared`.

### Event Sourcing

- **Module 1 (Members):** Eventuous `Aggregate<MemberState>` stored in Azure SQL via `Eventuous.SqlServer`.
  Command handling via `CommandService<Member, MemberState, MemberId>`.
- **Module 2 (Finances):** Wolverine static handlers dispatch commands; events are stored in EventStoreDB.
  Liquid Projections subscribes to the event stream and projects to Azure SQL via Dapper.

### Why Two Different Event-Sourcing Frameworks?

This is an explicit architectural demonstration — the system intentionally uses both Eventuous and
Wolverine to showcase different approaches to event sourcing in the same solution.

### Coding Conventions

See `.github/skills/coding-conventions.md` for the full C# coding guidelines.

## Key Packages

| Package | Usage |
|---|---|
| `Eventuous` | Member aggregate + command service |
| `Eventuous.SqlServer` | SQL-backed event store for Members |
| `WolverineFx` | Finance command handlers + message bus |
| `LiquidProjections` | Finance event projections |
| `EventStore.Client.Grpc.Streams` | EventStoreDB gRPC client |
| `Dapper` | Finances read-model SQL queries |
| `Microsoft.Data.SqlClient` | Azure SQL connectivity |
| `Microsoft.Azure.Functions.Worker` | Azure Functions isolated worker host |
| `Microsoft.Azure.WebJobs.Extensions.OpenApi` | OpenAPI/Swagger for Function endpoints |
| `Aspire.Hosting.AppHost` | Local orchestration |
| `FluentAssertions` | Test assertions |
| `FakeItEasy` | Test mocking |
| `Mockly` | HTTP mocking in tests |

## Frontend

```bash
cd frontend/club-admin-ui
pnpm install
pnpm dev         # start Next.js dev server
pnpm build       # production build
pnpm lint        # ESLint
pnpm format      # Prettier
```

Module structure: `src/modules/members/` and `src/modules/finances/`.  
App Router pages in `src/app/`.

## Do NOT

- Commit secrets or connection strings — use Aspire service discovery locally, Azure Key Vault in prod.
- Generate code that violates `TreatWarningsAsErrors=true` — all warnings are build errors.
- Add Marten — use Dapper + `Microsoft.Data.SqlClient` for Finances read-model persistence.
- Add package versions to `.csproj` files — all versions are centrally managed in `Directory.Packages.props`.
- Use .NET versions below net10.0 or C# versions below 14.0.
- Write test classes without the `.Specs` suffix.
