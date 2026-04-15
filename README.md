# ClubAdmin

A modular administration system for sports clubs, built as an architectural demonstration of two distinct event-sourcing approaches side-by-side in a single solution.

## Modules

| Module | Technology |
|---|---|
| **Member Management** | Azure Functions v4 · Eventuous · Azure SQL |
| **Financial Transactions** | Azure Functions v4 · Wolverine · EventStoreDB · Liquid Projections · Dapper |

**Frontend:** Next.js 15 (App Router, SSR) · React 19 · TypeScript · MUI v6 · Ant Design v5 · pnpm

---

## Prerequisites

### .NET Backend

| Tool | Version | Notes |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0.100 or later | See `global.json` |
| [Azure Functions Core Tools](https://learn.microsoft.com/azure/azure-functions/functions-run-local) | v4 | Required to run Function Apps locally |
| [Docker Desktop](https://www.docker.com/products/docker-desktop) | Latest | Required for SQL Server and EventStoreDB containers via Aspire |

### Frontend

| Tool | Version | Notes |
|---|---|---|
| [Node.js](https://nodejs.org/) | 20.x LTS | See `.nvmrc` in `frontend/club-admin-ui/` |
| [pnpm](https://pnpm.io/installation) | 10.x | `npm install -g pnpm` |

### Infrastructure (optional, Azure deployment only)

| Tool | Version | Notes |
|---|---|---|
| [Pulumi CLI](https://www.pulumi.com/docs/install/) | Latest | Only needed for Azure provisioning |
| [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) | Latest | Authentication for Pulumi |

---

## Repository Structure

```
ClubAdmin/
├── agents.md                          ← AI coding agent instructions
├── Build/                             ← Nuke build automation project
├── src/
│   ├── Shared/ClubAdmin.Shared/       ← Common value objects and interfaces
│   ├── Members/
│   │   ├── ClubAdmin.Members/         ← Domain + Eventuous application layer
│   │   ├── ClubAdmin.Members.Api/     ← Azure Functions v4 HTTP API
│   │   └── ClubAdmin.Members.Specs/   ← xUnit specification tests
│   ├── Finances/
│   │   ├── ClubAdmin.Finances/        ← Domain + Wolverine handlers + Dapper projections
│   │   ├── ClubAdmin.Finances.Api/    ← Azure Functions v4 HTTP API
│   │   └── ClubAdmin.Finances.Specs/  ← xUnit specification tests
│   └── AppHost/
│       └── ClubAdmin.AppHost/         ← .NET Aspire (local dev orchestration only)
├── frontend/
│   └── club-admin-ui/                 ← Next.js SSR frontend
└── infra/
    └── ClubAdmin.Infra/               ← Pulumi IaC for Azure provisioning
```

---

## Building

Build automation uses [Nuke](https://nuke.build/). All build logic lives in `Build/Build.cs` — the GitHub Actions workflow is a thin wrapper.

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

### Available Nuke Targets

| Target | Description |
|---|---|
| `CalculateVersion` | Calculates semantic version via GitVersion |
| `Restore` | Restores all NuGet packages |
| `Compile` | Builds the entire solution |
| `Test` | Runs all `*.Specs` test projects |
| `GenerateCodeCoverageReport` | Produces an HTML coverage report under `Build/TestResults/` |
| `Publish` | Publishes all `*.Api` Function App projects |
| `Default` | Runs `GenerateCodeCoverageReport` + `Publish` |

---

## Running Locally

The easiest way to run everything together is via [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/):

```bash
dotnet run --project src/AppHost/ClubAdmin.AppHost
```

This starts the following services automatically:

| Service | Description |
|---|---|
| **SQL Server** | Docker container — event store for Members + read models for both modules |
| **EventStoreDB** | Docker container — event store for Finances module |
| **Members API** | Azure Function App on a dynamic port |
| **Finances API** | Azure Function App on a dynamic port |
| **Frontend** | Next.js dev server (`pnpm dev`) |
| **Aspire Dashboard** | Observability dashboard (traces, logs, metrics) |

The Aspire dashboard URL is printed to the console on startup. Open it to see all services, their endpoints, structured logs, and distributed traces.

> **Note:** Aspire is for **local development only**. Azure provisioning is handled by Pulumi.

### Running Services Individually

**Members API:**
```bash
cd src/Members/ClubAdmin.Members.Api
func start
```

**Finances API:**
```bash
cd src/Finances/ClubAdmin.Finances.Api
func start
```

**Frontend:**
```bash
cd frontend/club-admin-ui
pnpm install
pnpm dev
```

---

## Testing

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=normal"

# Run a specific project
dotnet test src/Members/ClubAdmin.Members.Specs/ClubAdmin.Members.Specs.csproj
```

Tests are written as executable specifications using xUnit, FluentAssertions, and FakeItEasy. Test class names end with `Specs`.

---

## Frontend Development

```bash
cd frontend/club-admin-ui

# Install dependencies
pnpm install

# Start dev server
pnpm dev

# Production build
pnpm build

# Lint
pnpm lint

# Format with Prettier
pnpm format
```

The frontend runs on `http://localhost:3000` by default.

---

## API Documentation

Both Function Apps expose OpenAPI (Swagger) documentation. When running locally, navigate to:

- **Members API:** `http://localhost:<port>/api/swagger/ui`
- **Finances API:** `http://localhost:<port>/api/swagger/ui`

The exact ports are shown in the Aspire dashboard or in the Function App startup output.

---

## Azure Deployment

Infrastructure is managed with Pulumi in `infra/ClubAdmin.Infra/`.

```bash
cd infra/ClubAdmin.Infra

# Log in to Azure
az login

# Log in to Pulumi (or use local backend)
pulumi login

# Create a new stack (first time)
pulumi stack init dev

# Set required config
pulumi config set clubadmin:location westeurope
pulumi config set clubadmin:environment dev
pulumi config set clubadmin:sqlAdminLogin clubadmin_admin
pulumi config set --secret clubadmin:sqlAdminPassword <your-password>

# Preview changes
pulumi preview

# Deploy
pulumi up
```

Resources provisioned:
- Azure Resource Group
- Azure SQL Server + Database
- Azure Container Instance (EventStoreDB)
- Two Azure Function Apps (Members + Finances)
- Azure Static Web App (Next.js frontend with SSR)

---

## Architecture

### Domain-Driven Design + Vertical Slice Architecture

Each module owns its domain model, commands, events, projections, and API. No shared domain logic — only infrastructure types live in `ClubAdmin.Shared`.

### Event Sourcing

```
Members module                    Finances module
─────────────────────────────     ─────────────────────────────────────
Command                           Command
  │                                 │
  ▼                                 ▼
CommandService (Eventuous)        Static Handler (Wolverine)
  │                                 │
  ▼                                 ▼
Aggregate<MemberState>            Domain object + raised event
  │                                 │
  ▼                                 ▼
Azure SQL (event store)           EventStoreDB (event store)
  │                                 │
  ▼                                 ▼
SqlServerProjection               Liquid Projections subscriber
  │                                 │
  ▼                                 ▼
Azure SQL (read model)            Azure SQL via Dapper (read model)
```

The two modules intentionally use different event-sourcing frameworks to demonstrate both Eventuous and Wolverine approaches.

---

## Package Management

All NuGet package versions are centrally managed in `Directory.Packages.props` (Central Package Management). **Do not add `Version` attributes to `.csproj` files.**

---

## Contributing

See `.github/skills/coding-conventions.md` for the full C# coding guidelines and `.github/skills/code-review.md` for the pull request review checklist.
