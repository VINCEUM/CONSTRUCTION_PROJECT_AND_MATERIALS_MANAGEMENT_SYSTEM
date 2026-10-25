# Construction Project and Materials Monitoring System (CPMMS)
IT13 · C# / .NET 9 WinForms · MySQL 8

Monitors construction **projects** and the **materials** they use. A project has a schedule of
weighted tasks and a Bill of Quantities; its materials move through one auditable paper trail:
request → purchase order → delivery → issuance → return → physical count. Every stock movement
is written to a single ledger, so what a project **planned** to use can always be compared with
what it **actually** used.

## Where things are

```
IT13/
├── database_schema.sql  25 tables, 3 reporting views, all constraints
├── database_seed.sql    demo data: 3 projects, 20 materials, a full purchase cycle
├── docs/
│   └── database-design.md   ERDs, table dictionary, business rules, normalization
├── app/                     the WinForms application (see below)
└── README.md
```

## Setting up the database

**1. Install MySQL Server.** You have Workbench, Router and Shell, but not the server itself.
Either works:

Use the **MySQL Installer** (already on this machine at
`C:\Program Files (x86)\MySQL\MySQL Installer for Windows`): open it, click **Add**, and
install **MySQL Server 8.0.46**.

Do **not** use XAMPP — recent versions ship MariaDB, which lacks the `utf8mb4_0900_ai_ci`
collation this schema uses, so the very first statement would fail. Laragon is fine only if
you pick MySQL rather than MariaDB. Also avoid anything below 8.0.16: CHECK constraints are
silently ignored before that version, which would quietly void the "stock can never go
negative" guarantee.

**2. Import the schema and data.** In MySQL Workbench: *File → Open SQL Script*, open
`database_schema.sql`, click the lightning bolt. Repeat for `database_seed.sql`.

From a terminal instead:

```bash
mysql -u root -p < "D:/RESEARCH PAPER/IT13/database_schema.sql"
```

```bash
mysql -u root -p < "D:/RESEARCH PAPER/IT13/database_seed.sql"
```

**3. Verify it.** These two must each return **zero rows** — they are the schema's self-test:

```sql
SELECT * FROM v_stock_check WHERE difference <> 0;
SELECT * FROM inventory_transactions WHERE balance_after < 0;
```

Then look at the report that the whole system exists for:

```sql
SELECT project_name, material_name, planned_qty, actual_qty, variance_percent
FROM v_project_material_variance
ORDER BY variance_percent DESC;
```

**4. Get the ERD for the paper.** In Workbench: *Database → Reverse Engineer*, point it at
`construction_mms`, and it generates an EER diagram from the live schema. Export as PNG or
PDF and it goes straight into your documentation — no hand-drawing, and it can't drift from
the real database.

## Roles and demo accounts

There are exactly two roles. All demo passwords are `password`.

| Role | Email | Can do |
|---|---|---|
| Administrator | admin@buildcorp.test | Everything: users, projects (and assigning engineers), suppliers, the materials list, approving requests, purchase orders, receiving deliveries, issuing, returns, physical counts, reports, activity log |
| Project Engineer | karla.dizon@buildcorp.test | **Only the projects assigned to them:** view the project, schedule tasks and update progress, record the Bill of Quantities, create and submit material requests, read reports |
| Project Engineer | nico.alcantara@buildcorp.test | Same, for a second project |

An engineer cannot approve requests, create purchase orders, manage suppliers, change the
materials list, issue or receive stock, or edit stock directly. These rules are enforced in the
service layer (`AccessGuard`, `ApprovalService`, `ProjectMonitoringService`, `ReportService`),
not just by hiding buttons.

> If your database was created before the two-role change, run
> `migration_phase1_two_roles.sql` once. A fresh import of `database_seed.sql` already has two roles.

## What the seed data already shows

- **PRJ-2026-001** two-storey residence — ongoing, 49% weighted progress, ₱120k of materials issued against a ₱1.45M budget
- **PRJ-2026-002** commercial renovation — ongoing, electrical rewiring under way
- **PRJ-2026-003** perimeter fence — completed, for historical reporting
- **PO-2026-0003** — a **partial delivery**: 120 of 200 bags of cement received, the rest on backorder
- **RT-2026-0001** — a **return** with 4 reusable sheets back in stock and 2 damaged ones staying charged to the project
- **ADJ-2026-0001** — a **physical count** three bags short, corrected through an adjustment rather than an edit

## The application

C# / .NET 9 WinForms desktop app, MySQL via MySqlConnector + Dapper.

```
app/
├── CPMMS.sln
├── CPMMS.Core/          class library: no UI code
│   ├── Database/        DatabaseHelper (connection + queries), DemoData (offline sample data)
│   ├── Models/          plain classes matching the tables and views
│   └── Services/
│       ├── AuthService            bcrypt login, session
│       ├── AccessGuard            the Administrator-only rule, enforced for writes
│       ├── ApprovalService        approve / reject (never the requester)
│       ├── RequestService         draft, submit, cancel, void a material request
│       ├── PurchaseOrderService   purchase orders from approved requests; cancel
│       ├── InventoryService       every stock movement: issue, receive, return, count
│       ├── StockControlService    returns and physical counts (checks + history)
│       ├── ProjectMonitoringService  tasks, weighted progress, Bill of Quantities
│       ├── ReportService          per-project reports and the activity log
│       ├── CatalogService         reads for the list screens and the dashboard
│       └── MaintenanceService     suppliers, materials, projects, users
└── CPMMS.App/           WinForms (login, main window, dialogs, and Views/)
```

**Open it in Visual Studio:** double-click `app/CPMMS.sln`, set `CPMMS.App` as the startup
project, press F5. Or from a terminal:

```bash
dotnet run --project "app/CPMMS.App"
```

Set your MySQL password in `app/CPMMS.App/appsettings.json`. The file ships with a blank
password, which is almost certainly not what your server uses. Delivery photos are copied into an
`uploads/deliveries` folder next to the executable.

### Why the layering matters

`CPMMS.Core` has no reference to WinForms. Every stock rule lives in a service, not behind a
button, so the rules hold no matter which screen calls them. `IssueMaterials` in particular does all
of this in **one database transaction**:

1. locks the request row, and rejects anything not `approved` / `partially_issued`
2. locks every material row (`SELECT … FOR UPDATE`) **before** checking any quantity
3. rejects the whole issuance if any line exceeds stock, so nothing partial is written
4. writes the issue header and lines, snapshotting `unit_cost`
5. writes one `inventory_transactions` row per line with the new `balance_after`
6. updates `materials.current_stock` in the same transaction
7. advances `qty_issued` on the request lines and moves the request status
8. posts the cost to `project_expenses` and writes an `activity_logs` entry

Step 2 is what makes two issuances of the last 10 bags at the same time safe: the second one
waits for the lock, then fails the stock check honestly instead of driving stock negative.

### Screens

| Screen | Who | What it does |
|---|---|---|
| **Dashboard** | both | Contract value, pending requests, and the **reorder list** (materials at or below their minimum, with how far short). The Administrator also sees stock value and the **ledger health check** |
| **Projects** | Admin edits, engineer reads | Budget vs material spend vs weighted progress. The Administrator adds and edits projects and assigns the Project Engineer |
| **Monitoring** | both | Pick a project: tasks with weight, schedule and progress (overall progress is weighted), and the Bill of Quantities with planned cost against the material budget |
| **Reports** | both | Per project: progress, material requests, inventory transactions, material usage, cost and quantity variance. Export to CSV |
| **Materials** | Admin edits, engineer reads | Search, low-stock filter, stock card for any material. Stock can only change through a posted transaction |
| **Material Requests** | both | Engineer drafts, submits, cancels. Administrator approves, rejects, voids, and issues materials |
| **Purchase Orders** | Admin | Create from an approved request, receive deliveries (DR number, date, photo, recipient, partial deliveries), cancel an order that has received nothing |
| **Suppliers** | Admin | Add, edit, deactivate |
| **Stock Control** | Admin | Returns (reusable go back to stock, damaged stay charged as wastage) and physical counts (differences posted as adjustments with a reason) |
| **Activity Log** | Admin | Who did what and when: approvals, rejections, cancellations, voids, adjustments, deliveries, issues, and reorder alerts |
| **Users** | Admin | Add, edit, reset password, deactivate |

### Stock movements, all in `InventoryService`

| Operation | Rule it enforces |
|---|---|
| `IssueMaterials` | locks each material before checking; rejects the whole issuance if any line exceeds stock; snapshots unit cost; posts the project expense |
| `ReceiveDelivery` | partial deliveries keep the PO open; updates last purchase price; writes one `RECEIVED` row per line |
| `ReturnMaterials` | reusable stock returns to the shelf; **damaged does not**, it stays charged to the project as wastage |
| `PostStockAdjustment` | the counted figure becomes stock, and the difference is written as an `ADJUSTMENT` with a reason; stock is never edited directly |

When an issuance or a count takes a material to or below its minimum, a `reorder` entry is written
to the activity log. **No purchase order is created automatically**: an engineer requests, the
Administrator approves and orders.

### Merged from the teammate version

The group had a second implementation (`ConstructionPMS_IT13FinalProject`). Its maintenance
screens were rebuilt against this schema; its simpler 6-table database was not carried over. See
[docs/merge-notes.md](docs/merge-notes.md).

## Status

Every functional requirement in the project requirements document is implemented. Not done:
automated tests, and PDF/print output (reports export to CSV instead). Demo mode (running without
MySQL) only covers browsing the original screens; the monitoring, reports, stock-control, and
purchasing features need the database.
