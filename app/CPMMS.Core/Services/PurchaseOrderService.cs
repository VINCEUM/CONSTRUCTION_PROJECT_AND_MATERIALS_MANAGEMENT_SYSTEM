using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>One line of a purchase order being drafted.</summary>
public sealed record PoLine(int MaterialId, decimal Qty, decimal UnitPrice);

/// <summary>
/// Turns an approved material request into a purchase order to a supplier.
/// An admin picks the request, a supplier, and how much of each requested
/// material to order (up to what was requested — this is procurement, not a
/// second request). The order comes out as 'sent', ready for ReceiveForm.
/// </summary>
public sealed class PurchaseOrderService
{
    private const string NoDemo =
        "Creating a purchase order needs the database. It is not available in demo mode.";

    // ------------------------------------------------------------ lookups

    /// <summary>Approved requests that don't already have a live purchase order.</summary>
    public IReadOnlyList<MaterialRequest> GetOrderableRequests()
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<MaterialRequest>(
            @"SELECT r.*, p.name AS project_name, u.full_name AS requester_name
              FROM   material_requests r
              JOIN   projects p ON p.id = r.project_id
              JOIN   users u ON u.id = r.requested_by
              WHERE  r.status = 'approved'
                AND  NOT EXISTS (
                       SELECT 1 FROM purchase_orders po
                       WHERE po.request_id = r.id AND po.status <> 'cancelled'
                     )
              ORDER  BY r.approved_at");
    }

    public IReadOnlyList<Supplier> GetSuppliers()
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<Supplier>(
            "SELECT * FROM suppliers WHERE status = 'active' ORDER BY company_name");
    }

    // ------------------------------------------------------------ cancel

    /// <summary>
    /// Cancels a purchase order that has not received anything yet, freeing its
    /// request to be ordered again (or voided). The record stays, with the reason.
    /// </summary>
    public void Cancel(User user, int poId, string reason)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        if (!user.IsAdmin) throw new InvalidOperationException("Only an administrator can cancel a purchase order.");

        reason = (reason ?? "").Trim();
        if (reason.Length == 0) throw new InvalidOperationException("Give a reason for cancelling the purchase order.");
        if (reason.Length > 200) throw new InvalidOperationException("Keep the reason under 200 characters.");

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var po = cn.QuerySingleOrDefault<(string PoNo, string Status)>(
                "SELECT po_no AS PoNo, status AS Status FROM purchase_orders WHERE id = @poId FOR UPDATE",
                new { poId }, tx);
            if (po.PoNo is null) throw new InvalidOperationException("That purchase order no longer exists.");
            if (po.Status is not ("draft" or "sent"))
                throw new InvalidOperationException(
                    $"{po.PoNo} is '{po.Status}'. Only an order that has not received anything yet can be cancelled.");

            var received = cn.ExecuteScalar<decimal>(
                "SELECT COALESCE(SUM(qty_received), 0) FROM purchase_order_items WHERE po_id = @poId", new { poId }, tx);
            if (received > 0)
                throw new InvalidOperationException($"{po.PoNo} already received materials, so it cannot be cancelled.");

            cn.Execute(
                @"UPDATE purchase_orders
                  SET    status = 'cancelled',
                         remarks = LEFT(CONCAT(COALESCE(NULLIF(remarks, ''), ''), IF(COALESCE(remarks, '') = '', '', ' | '),
                                               'Cancelled: ', @reason), 255)
                  WHERE  id = @poId", new { reason, poId }, tx);

            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@userId, 'cancelled', 'purchase_order', @poId, @description)",
                new { userId = user.Id, poId, description = $"{po.PoNo}: {reason}" }, tx);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ------------------------------------------------------------ create

    /// <summary>Creates the order. Returns the new purchase order id.</summary>
    public int Create(User user, int requestId, int supplierId, DateTime orderDate, DateTime? expectedDate,
                      string? remarks, IReadOnlyList<PoLine> lines)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        if (!user.IsAdmin)
            throw new InvalidOperationException("Only an admin can create a purchase order.");
        if (lines.Count == 0)
            throw new InvalidOperationException("Add at least one material to order.");
        if (lines.Any(l => l.Qty <= 0))
            throw new InvalidOperationException("Every ordered quantity must be greater than zero.");
        if (lines.Any(l => l.UnitPrice < 0))
            throw new InvalidOperationException("Unit price cannot be negative.");
        if (lines.GroupBy(l => l.MaterialId).Any(g => g.Count() > 1))
            throw new InvalidOperationException("A material can appear only once in a purchase order.");
        if (expectedDate is { } exp && exp.Date < orderDate.Date)
            throw new InvalidOperationException("The expected delivery date can't be before the order date.");

        remarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        if (remarks is { Length: > 255 })
            throw new InvalidOperationException("Keep the remarks under 255 characters.");

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var request = cn.QuerySingleOrDefault<(string RequestNo, string Status)>(
                @"SELECT request_no AS RequestNo, status AS Status FROM material_requests
                  WHERE id = @requestId FOR UPDATE", new { requestId }, tx);
            if (request.RequestNo is null)
                throw new InvalidOperationException("That request no longer exists.");
            if (request.Status != "approved")
                throw new InvalidOperationException(
                    $"{request.RequestNo} is '{request.Status}'. Only an approved request can be ordered.");

            var existing = cn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM purchase_orders WHERE request_id = @requestId AND status <> 'cancelled'",
                new { requestId }, tx);
            if (existing > 0)
                throw new InvalidOperationException("A purchase order already exists for this request.");

            // every line must be something that was actually requested, and
            // ordering more than was requested would defeat the point of approval
            var requested = cn.Query<(int MaterialId, decimal QtyRequested)>(
                @"SELECT material_id AS MaterialId, qty_requested AS QtyRequested
                  FROM material_request_items WHERE request_id = @requestId",
                new { requestId }, tx).ToDictionary(r => r.MaterialId, r => r.QtyRequested);

            foreach (var line in lines)
            {
                if (!requested.TryGetValue(line.MaterialId, out var qtyRequested))
                    throw new InvalidOperationException("One of the materials was not part of this request.");
                if (line.Qty > qtyRequested)
                    throw new InvalidOperationException("Cannot order more than what was requested for a material.");
            }

            // PO-2026-0001, PO-2026-0002, ... locked so two admins cannot take the same number
            var prefix = $"PO-{orderDate.Year}-";
            var last = cn.ExecuteScalar<int>(
                @"SELECT COALESCE(MAX(CAST(SUBSTRING(po_no, @start) AS UNSIGNED)), 0)
                  FROM   purchase_orders
                  WHERE  po_no LIKE @like FOR UPDATE",
                new { start = prefix.Length + 1, like = prefix + "%" }, tx);
            var poNo = $"{prefix}{last + 1:0000}";

            var totalAmount = lines.Sum(l => l.Qty * l.UnitPrice);

            cn.Execute(
                @"INSERT INTO purchase_orders
                    (po_no, supplier_id, request_id, order_date, expected_date, status, total_amount, prepared_by, remarks)
                  VALUES
                    (@poNo, @supplierId, @requestId, @orderDate, @expectedDate, 'sent', @totalAmount, @userId, @remarks)",
                new
                {
                    poNo, supplierId, requestId, orderDate = orderDate.Date, expectedDate = expectedDate?.Date,
                    totalAmount, userId = user.Id, remarks
                }, tx);
            var poId = cn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()", transaction: tx);

            foreach (var line in lines)
            {
                cn.Execute(
                    @"INSERT INTO purchase_order_items (po_id, material_id, qty_ordered, qty_received, unit_price)
                      VALUES (@poId, @materialId, @qty, 0, @unitPrice)",
                    new { poId, materialId = line.MaterialId, qty = line.Qty, unitPrice = line.UnitPrice }, tx);
            }

            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@userId, 'created', 'purchase_order', @poId, @description)",
                new { userId = user.Id, poId, description = $"{poNo} for {request.RequestNo}" }, tx);

            tx.Commit();
            return poId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
