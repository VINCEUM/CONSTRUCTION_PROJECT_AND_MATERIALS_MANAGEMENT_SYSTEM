using CPMMS.Core.Models;
using Dapper;
using MySqlConnector;

namespace CPMMS.Core.Services;

public class InsufficientStockException : Exception
{
    public string MaterialName { get; }
    public decimal Available { get; }
    public decimal Requested { get; }

    public InsufficientStockException(string materialName, decimal available, decimal requested)
        : base($"Not enough {materialName} in stock. Available {available:N2}, requested {requested:N2}.")
    {
        MaterialName = materialName;
        Available = available;
        Requested = requested;
    }
}

/// <summary>
/// Everything that moves stock goes through here, so the four rules in the
/// schema header hold in one place:
///   • stock never goes negative
///   • every movement writes a row to inventory_transactions
///   • materials.current_stock is updated in the SAME transaction
///   • unit cost is snapshotted onto the issuance line
/// </summary>
public sealed class InventoryService
{
    /// <summary>
    /// Releases materials against an approved request. Returns the new issue id.
    /// Throws <see cref="InsufficientStockException"/> and writes nothing if any
    /// line exceeds available stock.
    /// </summary>
    public int IssueMaterials(int requestId, DateTime issueDate, int issuedByUserId,
                              string receivedBy, IList<IssueLine> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ArgumentException("Nothing to issue.", nameof(lines));
        if (lines.Any(l => l.Qty <= 0))
            throw new ArgumentException("Every line must have a quantity greater than zero.", nameof(lines));
        if (string.IsNullOrWhiteSpace(receivedBy))
            throw new ArgumentException("Record who received the materials on site.", nameof(receivedBy));

        if (DemoMode.Enabled) return DemoData.ApplyIssue(requestId, lines, issueDate);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();

        try
        {
            // --- the request must exist and be approved -------------------------
            var request = cn.QuerySingleOrDefault<(int Id, int ProjectId, string Status)>(
                @"SELECT id AS Id, project_id AS ProjectId, status AS Status
                  FROM   material_requests
                  WHERE  id = @requestId FOR UPDATE",
                new { requestId }, tx);

            if (request.Id == 0)
                throw new InvalidOperationException("That material request no longer exists.");
            if (request.Status != "approved" && request.Status != "partially_issued")
                throw new InvalidOperationException(
                    $"Only approved requests can be issued. This one is '{request.Status}'.");

            var projectId = request.ProjectId;

            // --- lock every material row first, then check every line ------------
            // Locking before writing is what makes two storekeepers issuing the
            // last stock at the same time safe.
            var stockNow = new Dictionary<int, decimal>();
            var names = new Dictionary<int, string>();

            foreach (var materialId in lines.Select(l => l.MaterialId).Distinct().OrderBy(id => id))
            {
                var row = cn.QuerySingle<(decimal Stock, string Name)>(
                    @"SELECT current_stock AS Stock, name AS Name
                      FROM   materials WHERE id = @materialId FOR UPDATE",
                    new { materialId }, tx);

                stockNow[materialId] = row.Stock;
                names[materialId] = row.Name;
            }

            foreach (var group in lines.GroupBy(l => l.MaterialId))
            {
                var wanted = group.Sum(l => l.Qty);
                if (wanted > stockNow[group.Key])
                    throw new InsufficientStockException(names[group.Key], stockNow[group.Key], wanted);
            }

            // --- header ----------------------------------------------------------
            var issueNo = NextDocumentNo(cn, tx, "material_issues", "issue_no", "IS", issueDate.Year);
            var totalCost = lines.Sum(l => l.Qty * l.UnitCost);

            cn.Execute(
                @"INSERT INTO material_issues
                    (issue_no, request_id, project_id, issue_date, issued_by, received_by, total_cost, status)
                  VALUES
                    (@issueNo, @requestId, @projectId, @issueDate, @issuedByUserId, @receivedBy, @totalCost, 'posted')",
                new { issueNo, requestId, projectId, issueDate, issuedByUserId, receivedBy, totalCost }, tx);

            var issueId = (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()", transaction: tx);

            // --- lines, ledger, cached stock, request fulfilment -----------------
            foreach (var line in lines)
            {
                cn.Execute(
                    @"INSERT INTO material_issue_items
                        (issue_id, request_item_id, material_id, qty_issued, unit_cost)
                      VALUES (@issueId, @requestItemId, @materialId, @qty, @unitCost)",
                    new { issueId, requestItemId = line.RequestItemId, line.MaterialId, qty = line.Qty, line.UnitCost }, tx);

                var newBalance = stockNow[line.MaterialId] - line.Qty;
                stockNow[line.MaterialId] = newBalance;

                cn.Execute(
                    @"UPDATE materials SET current_stock = @newBalance WHERE id = @materialId",
                    new { newBalance, line.MaterialId }, tx);

                cn.Execute(
                    @"INSERT INTO inventory_transactions
                        (material_id, txn_type, quantity, unit_cost, balance_after, project_id,
                         reference_type, reference_id, performed_by, remarks)
                      VALUES
                        (@materialId, 'ISSUED', @quantity, @unitCost, @newBalance, @projectId,
                         'issue', @issueId, @issuedByUserId, @remarks)",
                    new
                    {
                        line.MaterialId,
                        quantity = -line.Qty,          // signed: issuing removes stock
                        line.UnitCost,
                        newBalance,
                        projectId,
                        issueId,
                        issuedByUserId,
                        remarks = issueNo
                    }, tx);

                if (line.RequestItemId is int requestItemId)
                {
                    cn.Execute(
                        @"UPDATE material_request_items
                          SET    qty_issued = qty_issued + @qty
                          WHERE  id = @requestItemId",
                        new { qty = line.Qty, requestItemId }, tx);
                }
            }

            // --- is the request now fully served? --------------------------------
            var outstanding = cn.ExecuteScalar<int>(
                @"SELECT COUNT(*) FROM material_request_items
                  WHERE request_id = @requestId AND qty_issued < qty_requested",
                new { requestId }, tx);

            cn.Execute(
                "UPDATE material_requests SET status = @status WHERE id = @requestId",
                new { status = outstanding == 0 ? "issued" : "partially_issued", requestId }, tx);

            // --- materials become a project expense ------------------------------
            cn.Execute(
                @"INSERT INTO project_expenses
                    (project_id, category, description, amount, expense_date, source, reference_id, recorded_by)
                  VALUES
                    (@projectId, 'materials', @description, @totalCost, @issueDate, 'material_issue', @issueId, @issuedByUserId)",
                new
                {
                    projectId,
                    description = $"Material issuance {issueNo}",
                    totalCost,
                    issueDate,
                    issueId,
                    issuedByUserId
                }, tx);

            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@issuedByUserId, 'posted', 'material_issue', @issueId, @description)",
                new { issuedByUserId, issueId, description = $"{issueNo} to project #{projectId}, {totalCost:N2}" }, tx);

            tx.Commit();
            return issueId;
        }
        catch
        {
            tx.Rollback();   // nothing partial is ever left behind
            throw;
        }
    }

    /// <summary>
    /// Receives goods against a purchase order. Partial deliveries are normal:
    /// the PO stays open until every line is fully received.
    /// </summary>
    public int ReceiveDelivery(int poId, string drNo, DateTime deliveryDate, int receivedByUserId,
                               string? photoPath, IList<ReceiveLine> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ArgumentException("Nothing to receive.", nameof(lines));
        if (lines.Any(l => l.Qty <= 0))
            throw new ArgumentException("Every line must have a quantity greater than zero.", nameof(lines));
        if (string.IsNullOrWhiteSpace(drNo))
            throw new ArgumentException("Record the supplier's delivery receipt number.", nameof(drNo));

        if (DemoMode.Enabled) return DemoData.ApplyReceive(poId, lines, deliveryDate);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();

        try
        {
            var po = cn.QuerySingleOrDefault<(int Id, string Status)>(
                @"SELECT id AS Id, status AS Status FROM purchase_orders WHERE id = @poId FOR UPDATE",
                new { poId }, tx);

            if (po.Id == 0)
                throw new InvalidOperationException("That purchase order no longer exists.");
            if (po.Status is "cancelled" or "closed")
                throw new InvalidOperationException($"This purchase order is '{po.Status}' and cannot receive goods.");

            cn.Execute(
                @"INSERT INTO deliveries (dr_no, po_id, delivery_date, received_by, photo_path, remarks, status)
                  VALUES (@drNo, @poId, @deliveryDate, @receivedByUserId, @photoPath, NULL, 'posted')",
                new { drNo, poId, deliveryDate, receivedByUserId, photoPath }, tx);

            var deliveryId = (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()", transaction: tx);

            foreach (var line in lines)
            {
                cn.Execute(
                    @"INSERT INTO delivery_items (delivery_id, po_item_id, material_id, qty_received, unit_price)
                      VALUES (@deliveryId, @poItemId, @materialId, @qty, @unitPrice)",
                    new { deliveryId, line.PoItemId, line.MaterialId, qty = line.Qty, line.UnitPrice }, tx);

                cn.Execute(
                    "UPDATE purchase_order_items SET qty_received = qty_received + @qty WHERE id = @poItemId",
                    new { qty = line.Qty, line.PoItemId }, tx);

                var stock = cn.QuerySingle<decimal>(
                    "SELECT current_stock FROM materials WHERE id = @materialId FOR UPDATE",
                    new { line.MaterialId }, tx);

                var newBalance = stock + line.Qty;

                cn.Execute(
                    @"UPDATE materials SET current_stock = @newBalance, last_unit_cost = @unitPrice
                      WHERE id = @materialId",
                    new { newBalance, line.UnitPrice, line.MaterialId }, tx);

                cn.Execute(
                    @"INSERT INTO inventory_transactions
                        (material_id, txn_type, quantity, unit_cost, balance_after, project_id,
                         reference_type, reference_id, performed_by, remarks)
                      VALUES
                        (@materialId, 'RECEIVED', @qty, @unitPrice, @newBalance, NULL,
                         'delivery', @deliveryId, @receivedByUserId, @remarks)",
                    new { line.MaterialId, qty = line.Qty, line.UnitPrice, newBalance,
                          deliveryId, receivedByUserId, remarks = drNo }, tx);
            }

            var outstanding = cn.ExecuteScalar<int>(
                @"SELECT COUNT(*) FROM purchase_order_items
                  WHERE po_id = @poId AND qty_received < qty_ordered",
                new { poId }, tx);

            cn.Execute(
                "UPDATE purchase_orders SET status = @status WHERE id = @poId",
                new { status = outstanding == 0 ? "received" : "partially_received", poId }, tx);

            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@receivedByUserId, 'posted', 'delivery', @deliveryId, @description)",
                new { receivedByUserId, deliveryId,
                      description = $"DR {drNo} against PO #{poId}, {lines.Count} line(s)" }, tx);

            tx.Commit();
            return deliveryId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Unused materials coming back from a site. Reusable stock returns to the
    /// bodega; damaged stock does not — it already left at issuance and stays
    /// charged to the project, which is how wastage becomes visible.
    /// </summary>
    public int ReturnMaterials(int projectId, int? issueId, DateTime returnDate, string returnedBy,
                               int acceptedByUserId, string? conditionNote, IList<ReturnLine> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ArgumentException("Nothing to return.", nameof(lines));
        if (lines.Any(l => l.Qty <= 0))
            throw new ArgumentException("Every line must have a quantity greater than zero.", nameof(lines));
        if (string.IsNullOrWhiteSpace(returnedBy))
            throw new ArgumentException("Record who brought the materials back.", nameof(returnedBy));

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();

        try
        {
            var returnNo = NextDocumentNo(cn, tx, "material_returns", "return_no", "RT", returnDate.Year);

            cn.Execute(
                @"INSERT INTO material_returns
                    (return_no, project_id, issue_id, return_date, returned_by, accepted_by, condition_note, status)
                  VALUES
                    (@returnNo, @projectId, @issueId, @returnDate, @returnedBy, @acceptedByUserId, @conditionNote, 'posted')",
                new { returnNo, projectId, issueId, returnDate, returnedBy, acceptedByUserId, conditionNote }, tx);

            var returnId = (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()", transaction: tx);

            foreach (var line in lines)
            {
                cn.Execute(
                    @"INSERT INTO material_return_items (return_id, material_id, qty_returned, unit_cost, disposition)
                      VALUES (@returnId, @materialId, @qty, @unitCost, @disposition)",
                    new { returnId, line.MaterialId, qty = line.Qty, line.UnitCost, line.Disposition }, tx);

                if (line.Disposition != "reusable") continue;   // damaged never re-enters stock

                var stock = cn.QuerySingle<decimal>(
                    "SELECT current_stock FROM materials WHERE id = @materialId FOR UPDATE",
                    new { line.MaterialId }, tx);

                var newBalance = stock + line.Qty;

                cn.Execute("UPDATE materials SET current_stock = @newBalance WHERE id = @materialId",
                    new { newBalance, line.MaterialId }, tx);

                cn.Execute(
                    @"INSERT INTO inventory_transactions
                        (material_id, txn_type, quantity, unit_cost, balance_after, project_id,
                         reference_type, reference_id, performed_by, remarks)
                      VALUES
                        (@materialId, 'RETURNED', @qty, @unitCost, @newBalance, @projectId,
                         'return', @returnId, @acceptedByUserId, @remarks)",
                    new { line.MaterialId, qty = line.Qty, line.UnitCost, newBalance, projectId,
                          returnId, acceptedByUserId, remarks = returnNo }, tx);
            }

            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@acceptedByUserId, 'posted', 'material_return', @returnId, @description)",
                new { acceptedByUserId, returnId, description = $"{returnNo} from project #{projectId}" }, tx);

            tx.Commit();
            return returnId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Physical count. The counted figure becomes the new stock, and the
    /// difference is written to the ledger as an ADJUSTMENT with a reason —
    /// stock is never edited directly.
    /// </summary>
    public int PostStockAdjustment(DateTime countDate, int countedByUserId, string reason,
                                   IList<CountLine> lines)
    {
        if (lines is null || lines.Count == 0)
            throw new ArgumentException("Nothing counted.", nameof(lines));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Give a reason for the adjustment.", nameof(reason));
        if (lines.Any(l => l.CountedQty < 0))
            throw new ArgumentException("A counted quantity cannot be negative.", nameof(lines));

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();

        try
        {
            var adjNo = NextDocumentNo(cn, tx, "stock_adjustments", "adjustment_no", "ADJ", countDate.Year);

            cn.Execute(
                @"INSERT INTO stock_adjustments
                    (adjustment_no, count_date, counted_by, approved_by, reason, status)
                  VALUES (@adjNo, @countDate, @countedByUserId, @countedByUserId, @reason, 'posted')",
                new { adjNo, countDate, countedByUserId, reason }, tx);

            var adjustmentId = (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()", transaction: tx);
            var changed = 0;

            foreach (var line in lines)
            {
                var stock = cn.QuerySingle<decimal>(
                    "SELECT current_stock FROM materials WHERE id = @materialId FOR UPDATE",
                    new { line.MaterialId }, tx);

                cn.Execute(
                    @"INSERT INTO stock_adjustment_items
                        (adjustment_id, material_id, system_qty, counted_qty, remarks)
                      VALUES (@adjustmentId, @materialId, @systemQty, @countedQty, @remarks)",
                    new { adjustmentId, line.MaterialId, systemQty = stock,
                          countedQty = line.CountedQty, line.Remarks }, tx);

                var variance = line.CountedQty - stock;
                if (variance == 0) continue;   // counted exactly: nothing to write

                cn.Execute("UPDATE materials SET current_stock = @counted WHERE id = @materialId",
                    new { counted = line.CountedQty, line.MaterialId }, tx);

                cn.Execute(
                    @"INSERT INTO inventory_transactions
                        (material_id, txn_type, quantity, unit_cost, balance_after, project_id,
                         reference_type, reference_id, performed_by, remarks)
                      VALUES
                        (@materialId, 'ADJUSTMENT', @variance, 0, @counted, NULL,
                         'adjustment', @adjustmentId, @countedByUserId, @remarks)",
                    new { line.MaterialId, variance, counted = line.CountedQty,
                          adjustmentId, countedByUserId, remarks = $"{adjNo} — {reason}" }, tx);

                changed++;
            }

            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@countedByUserId, 'posted', 'stock_adjustment', @adjustmentId, @description)",
                new { countedByUserId, adjustmentId,
                      description = $"{adjNo}: {lines.Count} counted, {changed} corrected" }, tx);

            tx.Commit();
            return adjustmentId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>IS-2026-0001, PO-2026-0007 … sequential within the year.</summary>
    private static string NextDocumentNo(MySqlConnection cn, MySqlTransaction tx,
                                         string table, string column, string prefix, int year)
    {
        var last = cn.ExecuteScalar<string?>(
            $@"SELECT MAX({column}) FROM {table} WHERE {column} LIKE @pattern",
            new { pattern = $"{prefix}-{year}-%" }, tx);

        var next = 1;
        if (!string.IsNullOrEmpty(last))
        {
            var tail = last.Split('-').Last();
            if (int.TryParse(tail, out var n)) next = n + 1;
        }
        return $"{prefix}-{year}-{next:0000}";
    }

    // ---------------------------------------------------------------- reads

    public IReadOnlyList<StockCheckRow> GetStockCheck(bool lowStockOnly = false)
    {
        if (DemoMode.Enabled) return DemoData.StockCheck(lowStockOnly);

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT material_id, code, name, unit, cached_stock, ledger_stock,
                           difference, minimum_stock, is_low_stock
                    FROM   v_stock_check";
        if (lowStockOnly) sql += " WHERE is_low_stock = 1";
        sql += " ORDER BY name";
        return cn.Query<StockCheckRow>(sql).ToList();
    }

    /// <summary>The audit trail for one material — what produced its current balance.</summary>
    public IReadOnlyList<LedgerRow> GetLedger(int materialId, int limit = 200)
    {
        if (DemoMode.Enabled) return DemoData.LedgerFor(materialId);

        using var cn = DatabaseHelper.Open();
        return cn.Query<LedgerRow>(
            @"SELECT t.id, t.material_id, t.txn_type, t.quantity, t.unit_cost, t.balance_after,
                     t.project_id, t.reference_type, t.reference_id, t.remarks, t.created_at,
                     m.name AS material_name, p.name AS project_name, u.full_name AS performed_by_name
              FROM   inventory_transactions t
              JOIN   materials m ON m.id = t.material_id
              LEFT   JOIN projects p ON p.id = t.project_id
              JOIN   users u ON u.id = t.performed_by
              WHERE  t.material_id = @materialId
              ORDER  BY t.id DESC
              LIMIT  @limit",
            new { materialId, limit }).ToList();
    }

    /// <summary>Must always be empty. Shown on the dashboard as a health indicator.</summary>
    public IReadOnlyList<StockCheckRow> GetLedgerMismatches()
    {
        if (DemoMode.Enabled) return new List<StockCheckRow>();

        using var cn = DatabaseHelper.Open();
        return cn.Query<StockCheckRow>(
            @"SELECT material_id, code, name, unit, cached_stock, ledger_stock,
                     difference, minimum_stock, is_low_stock
              FROM   v_stock_check WHERE difference <> 0").ToList();
    }
}
