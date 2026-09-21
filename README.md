# Construction Project and Materials Management System (CPMMS)
CCE 106 · C# / .NET 9 WinForms · MySQL 8

Tracks what a construction project **planned** to consume against what it **actually**
consumed, through the full paper trail: request → purchase order → delivery → issuance →
return → physical count.

## Where things are

```
CCE 106/
├── database/
│   ├── 01_schema.sql     25 tables, 3 reporting views, all constraints
│   └── 02_seed.sql       demo data: 3 projects, 20 materials, a full purchase cycle
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
`01_schema.sql`, click the lightning bolt. Repeat for `02_seed.sql`.

From a terminal instead:

```bash
mysql -u root -p < "D:/RESEARCH PAPER/CCE 106/database/01_schema.sql"
```

```bash
mysql -u root -p < "D:/RESEARCH PAPER/CCE 106/database/02_seed.sql"
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

## Demo accounts

All demo passwords are `password`.

| Role | Email | Can do |
|---|---|---|
| Admin | admin@buildcorp.test | Users, suppliers, approvals, reports, settings |
| Project Engineer | karla.dizon@buildcorp.test | Projects, tasks, estimates, requests, progress |
| Project Engineer | nico.alcantara@buildcorp.test | Same, second project |
| Storekeeper | mario.sagun@buildcorp.test | Receiving, issuance, returns, physical count |

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
├── CPMMS.Core/          class library — no UI code
│   ├── Database/
│   │   ├── DatabaseHelper.cs   connection string + query helpers
│   │   └── DemoData.cs         sample data for running without MySQL
│   ├── Models/          plain classes matching the tables and views
│   └── Services/
│       ├── AuthService.cs       bcrypt login, session, audit log
│       ├── CatalogService.cs    reads for the list screens
│       ├── InventoryService.cs  every movement of stock: issue, receive,
│       │                        return, physical count
│       └── MaintenanceService.cs suppliers, materials, projects, users
└── CPMMS.App/           WinForms
    ├── LoginForm        checks the DB is reachable before you try to sign in
    ├── MainForm         left nav + swappable views
    ├── IssueForm        release materials against an approved request
    ├── ReceiveForm      receive a delivery against a purchase order
    ├── LedgerForm       stock card for one material
    └── Views/           Dashboard, Projects, Materials, Requests,
                         Purchase Orders, Variance
```

**Open it in Visual Studio:** double-click `app/CPMMS.sln`, set `CPMMS.App` as the startup
project, press F5. Or from a terminal:

```bash
dotnet run --project "D:/RESEARCH PAPER/CCE 106/app/CPMMS.App"
```

Set your MySQL root password in `app/CPMMS.App/appsettings.json` — the file ships with a
blank password, which is almost certainly not what your server uses.

### Why the layering matters

`CPMMS.Core` has no reference to WinForms. Every stock rule lives in `InventoryService`,
not behind a button, so the rules hold no matter which screen calls them — and you can say
that in the defence. `IssueMaterials` in particular does all of this in **one database
transaction**:

1. locks the request row, and rejects anything not `approved` / `partially_issued`
2. locks every material row (`SELECT … FOR UPDATE`) **before** checking any quantity
3. rejects the whole issuance if any line exceeds stock — nothing partial is written
4. writes the issue header and lines, snapshotting `unit_cost`
5. writes one `inventory_transactions` row per line with the new `balance_after`
6. updates `materials.current_stock` in the same transaction
7. advances `qty_issued` on the request lines and moves the request status
8. posts the cost to `project_expenses` and writes an `activity_logs` entry

Step 2 is what makes two storekeepers issuing the last 10 bags at the same time safe — the
second one waits for the lock, then fails the stock check honestly instead of driving stock
negative.

### Screens so far

| Screen | Shows |
|---|---|
| **Dashboard** | KPI cards, low-stock list, and a **ledger health check** — cached stock against the ledger's own sum |
| **Projects** | Budget vs material spend vs weighted progress, from `v_project_cost_summary` |
| **Materials** | Search, low-stock filter, colour-coded stock; double-click a row for its stock card |
| **Stock card** | Every transaction behind one material's balance, with a footer proving the two agree |
| **Material Requests** | Master-detail; flags lines that cannot be filled from stock right now |
| **Purchase Orders** | Open orders with outstanding lines; opens the receiving form |
| **Receive delivery** | Editable lines, DR number, partial and over-delivery handled |
| **Issue materials** | Editable lines pre-filled with what stock can actually cover, with the stock rules enforced by the service |
| **Variance Report** | Planned vs actual per material, overruns over 10% in red — the report the system exists for |
| **Suppliers** | Add, edit, deactivate — merged in from the teammate version |
| **Users** | Add, edit, reset password, deactivate. Admin only — merged in from the teammate version |
| **Materials / Projects** | Add and Edit dialogs — merged in from the teammate version |

### Stock movements — all four are implemented in `InventoryService`

| Operation | Rule it enforces |
|---|---|
| `IssueMaterials` | locks each material before checking; rejects the whole issuance if any line exceeds stock; snapshots unit cost; posts the project expense |
| `ReceiveDelivery` | partial deliveries keep the PO open; updates last purchase price; writes one `RECEIVED` row per line |
| `ReturnMaterials` | reusable stock returns to the bodega; **damaged does not** — it stays charged to the project as wastage |
| `PostStockAdjustment` | the counted figure becomes stock, and the difference is written as an `ADJUSTMENT` with a reason — stock is never edited directly |

### Merged from the teammate version

The group had a second implementation (`ConstructionPMS_IT13FinalProject`). Its maintenance
screens have been rebuilt against this schema; its simpler 6-table database was not carried
over. See [docs/merge-notes.md](docs/merge-notes.md) for what came from where and why.

### Still to build

Forms for returns and physical count (the services are written), creating requests and
purchase orders in-app, printable reports.

## Next step

1. Install MySQL Server and import the two SQL files
2. Run the app and sign in — the dashboard should report *ledger check passed*
3. Build the issuance form on top of `InventoryService.IssueMaterials`
4. Receiving against a purchase order, then returns and physical count
5. Supplier, user and project maintenance screens
6. Printable reports

Status: database design complete and parse-checked. Application builds clean (0 warnings,
0 errors) with login, dashboard, projects, materials, stock card, requests and variance
screens. Neither has been run against a live MySQL server yet — that is step 1.
