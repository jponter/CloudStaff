# CloudStaff — Project Reference

## Overview

CloudStaff is an ASP.NET Core MVC (.NET 10) staff allocation and forecasting tool. This is a POC — no authentication implemented yet.

---

## Solution Structure

```
CloudStaff/
  CloudStaff.MVC/               # ASP.NET Core MVC — UI, controllers, viewmodels, views
  CloudStaff.DataContext/        # EF Core context and migrations (Npgsql/PostgreSQL)
  CloudStaff.EntityModels/       # Plain C# entity classes — zero third-party dependencies
  Directory.Packages.props       # Central Package Management
```

### Feature folder layout inside CloudStaff.MVC

```
Features/
  _ViewStart.cshtml              # Applies _Layout to all feature views
  _ViewImports.cshtml            # Tag helpers and usings for all feature views
  Managers/
    ManagersController.cs
    ManagerViewModel.cs
    Views/
      Index.cshtml
      Create.cshtml
      Edit.cshtml
      Delete.cshtml
      _Form.cshtml
  Allocations/
    AllocationsController.cs
    AllocationsViewModel.cs
    Views/
      Index.cshtml
  Staff/                         # TODO
  Clients/                       # TODO
    Projects/                    # TODO
Infrastructure/
  FeatureFolderViewExpander.cs   # Registers Features/ as a Razor view location
```

---

## Tech Stack

| Concern | Choice |
|---|---|
| Framework | .NET 10, ASP.NET Core MVC |
| ORM | EF Core 10.0.9 |
| Database driver | Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2 |
| Database | PostgreSQL (local Docker instance for dev) |
| UI | Bootstrap 5 |
| Package management | Central Package Management via Directory.Packages.props |

---

## NuGet Packages (Directory.Packages.props)

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.2" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.9" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.9">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageVersion>
  </ItemGroup>
</Project>
```

---

## Conventions

- `CloudStaff.EntityModels` — no third-party dependencies, no EF attributes. POCOs only.
- All EF configuration lives in `OnModelCreating` in `CloudStaffContext`.
- Feature folders contain controller, viewmodel, and views together.
- `_ViewStart.cshtml` and `_ViewImports.cshtml` live at `Features/` root and are inherited by all subfolders.
- EF migrations live in `CloudStaff.DataContext`.

---

## EF Core Migration Commands

Always run from the solution root (`S:\CloudStaff`):

```bash
# Add a migration
dotnet ef migrations add <MigrationName> \
  --project CloudStaff.DataContext \
  --startup-project CloudStaff.MVC \
  --context CloudStaffContext

# Apply migrations
dotnet ef database update \
  --project CloudStaff.DataContext \
  --startup-project CloudStaff.MVC \
  --context CloudStaffContext

# Remove last migration (dev only)
dotnet ef migrations remove \
  --project CloudStaff.DataContext \
  --startup-project CloudStaff.MVC \
  --context CloudStaffContext
```

---

## Connection String

Stored in `CloudStaff.MVC/appsettings.Development.json` (not committed to source control):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cloudstaff;Username=...;Password=..."
  }
}
```

Registered in `Program.cs`:

```csharp
builder.Services.AddDbContext<CloudStaffContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

## Domain Model

### Entities

#### Manager
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Name | string(200) | Required |
| AsNumber | string(20) | Required, unique |
| Location | string(200) | Required |
| Email | string(200) | Required |

#### Staff
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Name | string(200) | Required |
| AsNumber | string(20) | Required, unique |
| ManagerId | int FK | → managers, ON DELETE RESTRICT |
| HomePoolId | int? FK | → home_pools, ON DELETE SET NULL |
| StaffRoleId | int? FK | → staff_roles, ON DELETE SET NULL |
| GcmLevel | int? | 1–9 |
| SfiaLevel | int? | 1–7 |
| ContractJobTitle | string?(200) | Legacy free-text field |

#### StaffCategory
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Name | string(200) | Required |

Values: Strategy & Architecture, Development & Implementation, Delivery & Operations, Cross-Cutting

#### HomePool
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Code | string(10) | Required, unique (e.g. SA-01) |
| Name | string(200) | Required |
| CategoryId | int FK | → staff_categories, ON DELETE RESTRICT |

#### StaffRole
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Title | string(200) | Required |
| HomePoolId | int FK | → home_pools, ON DELETE RESTRICT |

Note: Role titles are not globally unique — the same title can exist under different home pools (e.g. "Integration Architect" appears in both Solution Architecture and Integration & API Services).

#### Platform
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Name | string(100) | Required, unique |

Values: AWS, Azure, GCP, Multi-Cloud, Dynamics, VMWare, Wintel, Linux/Unix

#### StaffPlatform (join table)
| Column | Type | Notes |
|---|---|---|
| StaffId | int FK | Composite PK |
| PlatformId | int FK | Composite PK |

No cap on platforms per staff member. Use this table for queries like "how many staff with AWS skill are available".

#### Client
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Name | string(200) | Required |
| Description | string(1000) | |
| Type | int | Enum: Internal = 0, External = 1 |

#### ClientProject
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| Name | string(200) | Required |
| Description | string(1000) | |
| ClientId | int FK | → clients, ON DELETE CASCADE |

#### StaffAllocation
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, identity |
| StaffId | int FK | → staff, ON DELETE CASCADE |
| ClientProjectId | int FK | → client_projects, ON DELETE CASCADE |
| StartDate | DateOnly | Required — inclusive start of allocation |
| EndDate | DateOnly | Required — inclusive end of allocation |
| Percentage | int | 0–100+; >100 triggers over-allocation warning |

Date ranges are the single source of truth. Both weekly and monthly timeline views derive cell values by checking overlap: `StartDate <= periodEnd AND EndDate >= periodStart`. Multiple allocations for different projects can overlap on the same staff member (additive — total shown in cell).

### Relationships Summary

```
StaffCategory → HomePool → StaffRole
                    ↓
Manager → Staff ←──┘
              ↓
         StaffPlatform → Platform

Client → ClientProject
```

- Category is **never stored on Staff** — it is derived by navigating Staff → HomePool → StaffCategory
- Deleting a Manager is **blocked** if they have Staff assigned (UI and DB enforce this)
- Deleting a HomePool or StaffRole sets the FK to NULL on affected Staff rows

---

## Migrations Applied

| Migration | Description |
|---|---|
| 20260610155949_addManagerFields | Initial schema + Manager extra fields (AsNumber, Location, Email) |
| 20260610164813_AddStaffModel | Staff extra fields, lookup tables (StaffCategory, HomePool, StaffRole, Platform, StaffPlatform) |
| 20260611084804_AddStaffAllocations | Creates `staff_allocations` table with Year/WeekNumber/Month period model |
| 20260611090020_AddAllocationDates | Adds nullable StartDate/EndDate to staff_allocations |
| 20260611092158_RefactorAllocationsToDateRange | **Breaking** — removes Year/WeekNumber/Month columns, makes StartDate/EndDate NOT NULL. Clears all existing allocation rows. Date range is now the sole truth for allocation periods. |

---

## Features Completed

- [x] Managers — full CRUD (Index, Create, Edit, Delete)
  - Delete blocked at UI and POST level if staff are assigned
  - AsNumber uniqueness validated on Create and Edit
- [x] Allocations — timeline view with weekly and monthly modes
  - "View as" manager dropdown scopes the grid to that manager's staff
  - 13-week rolling window with prev/next navigation; full 12-month view for months mode
  - Color-coded cells: None (red) → Low → Mid → Good → High → Full (blue) → Over (purple)
  - Click any cell to open modal: shows existing allocations with start/end dates, add new allocation with date pickers, delete individual allocations
  - Start/end dates default to clicked period boundaries; user can extend across multiple periods
  - Single allocation record stored per project assignment (date range, not per-period rows)
  - Both weekly and monthly views read from the same records via date-overlap query — consistent across view modes
  - AJAX endpoints: `CellData`, `SetAllocation`, `DeleteAllocation`; page reloads after any mutation so all overlapping cells refresh

## Features Planned

- [ ] Staff — CRUD + bulk CSV import page
- [ ] Seed/import page (`/Admin/Seed`) for lookup table data (StaffCategory, HomePool, StaffRole, Platform) loaded from Excel/CSV
- [ ] Clients — CRUD
- [ ] Client Projects — CRUD (nested under Clients)
- [ ] Forecasting views / utilisation reports

---

## Bulk Import Plan

A one-off seed/import page at `/Admin/Seed` will:

1. Accept CSV uploads to populate lookup tables (StaffCategory → HomePool → StaffRole, Platform)
2. Accept a staff CSV with columns: `StaffName, ManagerName, AsNumber, Location, Email` — manager looked up by name, created if not found
3. Report a summary of rows created and any failures

The staff CSV format will be:

```
StaffName,ManagerName,AsNumber,HomePool,Role,GcmLevel,SfiaLevel,ContractJobTitle
```

---

## Local Dev URLs

| Profile | URL |
|---|---|
| http | http://localhost:5217 |
| https | https://localhost:7268 |