using CPMMS.Core.Models;

namespace CPMMS.Core.Services;

/// <summary>One material request, for the project report.</summary>
public class RequestReportRow
{
    public string RequestNo { get; set; } = "";
    public DateTime RequestDate { get; set; }
    public DateTime? NeededDate { get; set; }
    public string Status { get; set; } = "";
    public string RequestedByName { get; set; } = "";
    public int ItemCount { get; set; }
    public decimal QtyRequested { get; set; }
    public decimal QtyIssued { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>One stock movement charged to a project, for the project report.</summary>
public class LedgerReportRow
{
    public DateTime CreatedAt { get; set; }
    public string TxnType { get; set; } = "";
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceAfter { get; set; }
    public string PerformedByName { get; set; } = "";
    public string? Remarks { get; set; }
}

/// <summary>One entry of the activity log.</summary>
public class ActivityRow
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = "";
    public string EntityType { get; set; } = "";
    public string? Description { get; set; }
}

/// <summary>
/// The reports the Administrator and Project Engineers read, plus the activity
/// log. Every report is for one selected project: the Administrator may pick
/// any project, a Project Engineer only the ones assigned to them (checked here,
/// not only on the screen).
/// </summary>
public sealed class ReportService
{
    private const string NoDemo = "Reports need the database. They are not available in demo mode.";

    public IReadOnlyList<Project> GetProjectsFor(User user) =>
        new CatalogService().GetProjects(null, user.IsAdmin ? null : user.Id);

    private static void RequireAccess(User user, int projectId)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        var allowed = DatabaseHelper.QueryOne<int>(
            @"SELECT COUNT(*) FROM projects
              WHERE id = @projectId AND (@isAdmin = 1 OR project_engineer_id = @userId)",
            new { projectId, isAdmin = user.IsAdmin ? 1 : 0, userId = user.Id });
        if (allowed == 0)
            throw new InvalidOperationException("You can only view reports for the projects assigned to you.");
    }

    /// <summary>Schedule and weighted progress of the project's tasks.</summary>
    public IReadOnlyList<ProjectTask> GetProgress(User user, int projectId)
    {
        RequireAccess(user, projectId);
        return new ProjectMonitoringService().GetTasks(projectId);
    }

    /// <summary>The project's material requests with how much of each has been issued.</summary>
    public IReadOnlyList<RequestReportRow> GetRequests(User user, int projectId)
    {
        RequireAccess(user, projectId);
        return DatabaseHelper.Query<RequestReportRow>(
            @"SELECT r.request_no, r.request_date, r.needed_date, r.status, u.full_name AS requested_by_name,
                     COUNT(i.id) AS item_count,
                     COALESCE(SUM(i.qty_requested), 0) AS qty_requested,
                     COALESCE(SUM(i.qty_issued), 0)    AS qty_issued,
                     r.remarks
              FROM   material_requests r
              JOIN   users u ON u.id = r.requested_by
              LEFT   JOIN material_request_items i ON i.request_id = r.id
              WHERE  r.project_id = @projectId
              GROUP  BY r.id, r.request_no, r.request_date, r.needed_date, r.status, u.full_name, r.remarks
              ORDER  BY r.request_date DESC, r.id DESC", new { projectId });
    }

    /// <summary>Every stock movement charged to the project (issues and returns).</summary>
    public IReadOnlyList<LedgerReportRow> GetInventoryTransactions(User user, int projectId)
    {
        RequireAccess(user, projectId);
        return DatabaseHelper.Query<LedgerReportRow>(
            @"SELECT t.created_at, t.txn_type, m.code AS material_code, m.name AS material_name, m.unit,
                     t.quantity, t.unit_cost, t.balance_after, u.full_name AS performed_by_name, t.remarks
              FROM   inventory_transactions t
              JOIN   materials m ON m.id = t.material_id
              JOIN   users u ON u.id = t.performed_by
              WHERE  t.project_id = @projectId
              ORDER  BY t.created_at DESC, t.id DESC", new { projectId });
    }

    /// <summary>Planned against actual quantity and cost per material, with the variance.</summary>
    public IReadOnlyList<VarianceRow> GetUsageAndVariance(User user, int projectId)
    {
        RequireAccess(user, projectId);
        return new CatalogService().GetVariance(projectId);
    }

    // ------------------------------------------------------------ activity log

    public IReadOnlyList<string> GetActivityEntityTypes(User user)
    {
        RequireAdmin(user);
        return DatabaseHelper.Query<string>("SELECT DISTINCT entity_type FROM activity_logs ORDER BY entity_type");
    }

    /// <summary>The newest activity first. Administrator only.</summary>
    public IReadOnlyList<ActivityRow> GetActivity(User user, string? entityType, string? search, int limit = 500)
    {
        RequireAdmin(user);
        search = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%";
        return DatabaseHelper.Query<ActivityRow>(
            @"SELECT l.id, l.created_at, u.full_name AS user_name, l.action, l.entity_type, l.description
              FROM   activity_logs l
              LEFT   JOIN users u ON u.id = l.user_id
              WHERE  (@entityType IS NULL OR l.entity_type = @entityType)
                AND  (@search IS NULL OR l.description LIKE @search OR l.action LIKE @search OR u.full_name LIKE @search)
              ORDER  BY l.created_at DESC, l.id DESC
              LIMIT  @limit", new { entityType, search, limit });
    }

    private static void RequireAdmin(User user)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        if (!user.IsAdmin) throw new InvalidOperationException("Only an administrator can read the activity log.");
    }
}
