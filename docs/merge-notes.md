# Merging the two versions

Two versions of this system existed:

| | Location | Shape |
|---|---|---|
| **Ours** | `IT13/app` | .NET 9, two projects (Core + App), MySQL `construction_mms` — 25 tables, ledger, 3 reporting views |
| **Teammate's** | `source/repos/…/ConstructionPMS_IT13FinalProject` | .NET 8, one project, MySQL `construction_pms` — 6 tables |

Ours is the base; theirs contributed the screens we were missing. Their original
repository is untouched and still committed in its own Git history.

## Why ours is the base

Their schema could not answer the question the system exists to answer.

- `material_requests` holds **one material per row** — a request for five materials is five
  unrelated records, with no document to approve or issue as a unit.
- `inventory_transactions` has no `balance_after`, no unit cost and no reference to a source
  document, so a stock figure cannot be traced to the movements that produced it.
- There is no Bill of Quantities, so **planned vs actual cannot be computed at all** — the
  variance report has nothing to compare against.
- No purchase orders or deliveries, so there is no three-way match and no way to record a
  partial delivery.
- No returns or physical counts.
- Passwords are **unsalted SHA-256**. Two users with the same password get the same hash, and
  the hashes are trivially reversible from a rainbow table. Ours uses bcrypt with a work
  factor of 11.
- Stock is a plain column updated by whichever form touched it last.

## What their version contributed

Their CRUD screens, which ours genuinely lacked. Rebuilt against our schema and services:

| From their form | Now in ours |
|---|---|
| `SuppliersForm` | **Suppliers** — add, edit, deactivate |
| `UsersForm` | **Users** — add, edit, reset password, deactivate (admin only) |
| `MaterialsForm` | **Add / Edit** on the Materials screen |
| `ProjectsForm` | **Add project / Edit** on the Projects screen |
| `ReportsForm` | Already covered by our Dashboard and Variance Report |
| `InventoryForm` | Already covered, and more strictly, by Issue / Receive |

## Two changes made deliberately during the port

**Deactivate, never delete.** Their forms ran `DELETE FROM suppliers WHERE supplier_id=…`.
Ours sets `status='inactive'`. A supplier named on a two-year-old purchase order has to keep
resolving, and our foreign keys are `ON DELETE RESTRICT`, so a hard delete would either fail
or tear a hole in the audit trail.

**Stock is not editable on the material form.** Their form let you type a new
`quantity_in_stock` directly. Ours shows it read-only, because a quantity may only move
through a delivery, an issuance, a return or a counted adjustment — otherwise the ledger and
the stock column drift apart and `v_stock_check` stops being a guarantee.

## One shared editor

The four maintenance screens all use `RecordEditor`, a dialog built from a list of field
specifications, rather than four near-identical hand-built forms. Adding a field to any
record is one line.
