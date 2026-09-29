using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>Reads for the list screens and the dashboard.</summary>
public sealed class CatalogService
{
    public IReadOnlyList<Material> GetMaterials(string? search = null, bool lowStockOnly = false)
    {
        if (DemoMode.Enabled)
            return DemoData.Materials
                .Where(m => string.IsNullOrWhiteSpace(search)
                            || m.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || m.Code.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || m.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Where(m => !lowStockOnly || m.IsLowStock)
                .OrderBy(m => m.CategoryName).ThenBy(m => m.Name)
                .ToList();

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT m.id, m.category_id, m.code, m.name, m.unit, m.last_unit_cost,
                           m.current_stock, m.minimum_stock, m.status,
                           c.name AS category_name
                    FROM   materials m
                    JOIN   material_categories c ON c.id = m.category_id
                    WHERE  1 = 1";

        if (!string.IsNullOrWhiteSpace(search))
            sql += " AND (m.name LIKE @like OR m.code LIKE @like OR c.name LIKE @like)";
        if (lowStockOnly)
            sql += " AND m.current_stock <= m.minimum_stock";

        sql += " ORDER BY c.name, m.name";

        return cn.Query<Material>(sql, new { like = $"%{search}%" }).ToList();
    }

    public IReadOnlyList<Project> GetProjects(string? status = null)
    {
        if (DemoMode.Enabled)
            return DemoData.Projects.Where(p => status is null || p.Status == status).ToList();

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT p.*, u.full_name AS engineer_name
                    FROM   projects p
                    LEFT   JOIN users u ON u.id = p.project_engineer_id
                    WHERE  1 = 1";
        if (!string.IsNullOrWhiteSpace(status)) sql += " AND p.status = @status";
        sql += " ORDER BY p.start_date DESC";
        return cn.Query<Project>(sql, new { status }).ToList();
    }

    public IReadOnlyList<ProjectCostRow> GetProjectCosts()
    {
        if (DemoMode.Enabled) return DemoData.ProjectCosts;

        using var cn = DatabaseHelper.Open();
        return cn.Query<ProjectCostRow>(
            @"SELECT id, code, name, client_name, status, contract_amount, material_budget,
                     material_cost_to_date, other_expenses, material_budget_remaining, progress_percent
              FROM   v_project_cost_summary
              ORDER  BY FIELD(status,'ongoing','planning','on_hold','completed','cancelled'), code").ToList();
    }

    public IReadOnlyList<VarianceRow> GetVariance(int? projectId = null, decimal? minPercent = null)
    {
        if (DemoMode.Enabled)
            return DemoData.Variance
                .Where(v => projectId is null || v.ProjectId == projectId)
                .Where(v => minPercent is null || v.VariancePercent >= minPercent)
                .OrderByDescending(v => v.VariancePercent ?? decimal.MinValue)
                .ToList();

        using var cn = DatabaseHelper.Open();
        var sql = "SELECT * FROM v_project_material_variance WHERE 1 = 1";
        if (projectId is not null) sql += " AND project_id = @projectId";
        if (minPercent is not null) sql += " AND variance_percent >= @minPercent";
        sql += " ORDER BY variance_percent IS NULL, variance_percent DESC";
        return cn.Query<VarianceRow>(sql, new { projectId, minPercent }).ToList();
    }

    public IReadOnlyList<MaterialRequest> GetRequests(string? status = null)
    {
        if (DemoMode.Enabled)
            return DemoData.Requests.Where(r => status is null || r.Status == status).ToList();

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT r.*, p.name AS project_name, u.full_name AS requester_name
                    FROM   material_requests r
                    JOIN   projects p ON p.id = r.project_id
                    JOIN   users u ON u.id = r.requested_by
                    WHERE  1 = 1";
        if (!string.IsNullOrWhiteSpace(status)) sql += " AND r.status = @status";
        sql += " ORDER BY r.request_date DESC, r.id DESC";
        return cn.Query<MaterialRequest>(sql, new { status }).ToList();
    }

    public IReadOnlyList<MaterialRequestItem> GetRequestItems(int requestId)
    {
        if (DemoMode.Enabled)
            return DemoData.RequestItems.TryGetValue(requestId, out var demo) ? demo : new List<MaterialRequestItem>();

        using var cn = DatabaseHelper.Open();
        return cn.Query<MaterialRequestItem>(
            @"SELECT i.*, m.code AS material_code, m.name AS material_name, m.unit,
                     m.current_stock, m.last_unit_cost
              FROM   material_request_items i
              JOIN   materials m ON m.id = i.material_id
              WHERE  i.request_id = @requestId
              ORDER  BY m.name",
            new { requestId }).ToList();
    }


    public IReadOnlyList<PurchaseOrderRow> GetPurchaseOrders(bool openOnly = false)
    {
        if (DemoMode.Enabled)
            return DemoData.PurchaseOrders
                .Where(p => !openOnly || p.Status is "draft" or "sent" or "partially_received").ToList();

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT po.id, po.po_no, po.supplier_id, po.order_date, po.expected_date,
                           po.status, po.total_amount,
                           s.company_name AS supplier_name,
                           (SELECT COUNT(*) FROM purchase_order_items i WHERE i.po_id = po.id) AS line_count,
                           (SELECT COUNT(*) FROM purchase_order_items i
                             WHERE i.po_id = po.id AND i.qty_received < i.qty_ordered) AS outstanding_lines
                    FROM   purchase_orders po
                    JOIN   suppliers s ON s.id = po.supplier_id
                    WHERE  1 = 1";
        if (openOnly) sql += " AND po.status IN ('draft','sent','partially_received')";
        sql += " ORDER BY po.order_date DESC, po.id DESC";
        return cn.Query<PurchaseOrderRow>(sql).ToList();
    }

    public IReadOnlyList<PurchaseOrderItemRow> GetPurchaseOrderItems(int poId)
    {
        if (DemoMode.Enabled)
            return DemoData.PurchaseOrderItems.TryGetValue(poId, out var demo) ? demo : new List<PurchaseOrderItemRow>();

        using var cn = DatabaseHelper.Open();
        return cn.Query<PurchaseOrderItemRow>(
            @"SELECT i.id, i.po_id, i.material_id, i.qty_ordered, i.qty_received, i.unit_price,
                     m.code AS material_code, m.name AS material_name, m.unit
              FROM   purchase_order_items i
              JOIN   materials m ON m.id = i.material_id
              WHERE  i.po_id = @poId
              ORDER  BY m.name",
            new { poId }).ToList();
    }

    public DashboardTotals GetDashboardTotals()
    {
        if (DemoMode.Enabled) return DemoData.Totals();

        using var cn = DatabaseHelper.Open();
        return cn.QuerySingle<DashboardTotals>(
            @"SELECT
                (SELECT COUNT(*) FROM projects)                                  AS project_count,
                (SELECT COUNT(*) FROM projects WHERE status = 'ongoing')         AS ongoing_projects,
                (SELECT COUNT(*) FROM materials WHERE status = 'active')         AS material_count,
                (SELECT COUNT(*) FROM suppliers WHERE status = 'active')         AS supplier_count,
                (SELECT COUNT(*) FROM materials WHERE current_stock <= minimum_stock) AS low_stock_count,
                (SELECT COUNT(*) FROM material_requests WHERE status = 'submitted')   AS pending_requests,
                (SELECT COALESCE(SUM(contract_amount),0) FROM projects WHERE status IN ('ongoing','planning')) AS total_contract_value,
                (SELECT COALESCE(SUM(current_stock * last_unit_cost),0) FROM materials) AS stock_value");
    }
}
