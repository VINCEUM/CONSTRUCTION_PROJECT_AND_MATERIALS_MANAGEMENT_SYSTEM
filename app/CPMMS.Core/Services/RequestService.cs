using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>One line of a request being drafted.</summary>
public sealed record DraftLine(int MaterialId, decimal Qty);

/// <summary>
/// The life of a material request before it reaches the storekeeper:
///
///   draft -> submitted -> approved / rejected   (ApprovalService decides)
///   draft | submitted -> cancelled              (requester or admin, reason required)
///   approved -> voided                          (admin only, nothing issued yet, reason required)
///
/// Nothing here moves stock. Rules are enforced here, not behind a button,
/// so they hold no matter which screen calls them.
/// </summary>
public sealed class RequestService
{
    private const string NoDemo =
        "Creating and changing requests needs the database. It is not available in demo mode.";

    // ------------------------------------------------------------ lookups

    /// <summary>Projects this user may request materials for: their own, or all for an admin.</summary>
    public IReadOnlyList<Project> GetRequestableProjects(User user)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<Project>(
            @"SELECT p.*
              FROM   projects p
              WHERE  p.status IN ('planning', 'ongoing')
                AND (@isAdmin = 1 OR p.project_engineer_id = @userId)
              ORDER  BY p.code",
            new { isAdmin = user.IsAdmin ? 1 : 0, userId = user.Id });
    }

    public IReadOnlyList<Material> GetRequestableMaterials()
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<Material>(
            @"SELECT id, category_id, code, name, unit, last_unit_cost, current_stock, minimum_stock, status
              FROM   materials
              WHERE  status = 'active'
              ORDER  BY name");
    }

    // ------------------------------------------------------------ create

    /// <summary>Saves a new request as a draft. Returns the new request id.</summary>
    public int CreateDraft(User user, int projectId, DateTime? neededDate, string? remarks,
                           IReadOnlyList<DraftLine> lines)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        if (!user.IsEngineer)
            throw new InvalidOperationException("Only a project engineer can create a material request.");
        if (lines.Count == 0)
            throw new InvalidOperationException("Add at least one material to the request.");
        if (lines.Any(l => l.Qty <= 0))
            throw new InvalidOperationException("Every quantity must be greater than zero.");
        if (lines.GroupBy(l => l.MaterialId).Any(g => g.Count() > 1))
            throw new InvalidOperationException("A material can appear only once in a request.");
        if (neededDate is { } needed && needed.Date < DateTime.Today)
            throw new InvalidOperationException("The needed date cannot be in the past.");

        remarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        if (remarks is { Length: > 255 })
            throw new InvalidOperationException("Keep the remarks under 255 characters.");

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var mine = cn.ExecuteScalar<int>(
                @"SELECT COUNT(*) FROM projects
                  WHERE id = @projectId AND project_engineer_id = @userId AND status IN ('planning','ongoing')",
                new { projectId, userId = user.Id }, tx);
            if (mine == 0)
                throw new InvalidOperationException("You can only request materials for your own active projects.");

            // MR-2026-0001, MR-2026-0002, ... locked so two engineers cannot take the same number
            var prefix = $"MR-{DateTime.Today.Year}-";
            var last = cn.ExecuteScalar<int>(
                @"SELECT COALESCE(MAX(CAST(SUBSTRING(request_no, @start) AS UNSIGNED)), 0)
                  FROM   material_requests
                  WHERE  request_no LIKE @like FOR UPDATE",
                new { start = prefix.Length + 1, like = prefix + "%" }, tx);
            var requestNo = $"{prefix}{last + 1:0000}";

            cn.Execute(
                @"INSERT INTO material_requests
                    (request_no, project_id, requested_by, request_date, needed_date, status, remarks)
                  VALUES
                    (@requestNo, @projectId, @userId, CURDATE(), @neededDate, 'draft', @remarks)",
                new { requestNo, projectId, userId = user.Id, neededDate = neededDate?.Date, remarks }, tx);
            var requestId = cn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()", transaction: tx);

            foreach (var line in lines)
            {
                cn.Execute(
                    @"INSERT INTO material_request_items (request_id, material_id, qty_requested, qty_issued)
                      VALUES (@requestId, @materialId, @qty, 0)",
                    new { requestId, materialId = line.MaterialId, qty = line.Qty }, tx);
            }

            Log(cn, tx, user.Id, "created", requestId, requestNo);
            tx.Commit();
            return requestId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ------------------------------------------------------------ submit

    /// <summary>Sends a draft to the approver. Only the requester can do this.</summary>
    public void Submit(int requestId, User user)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var request = Lock(cn, tx, requestId);
            if (request.Status != "draft")
                throw new InvalidOperationException($"{request.RequestNo} is '{request.Status}'. Only a draft can be submitted.");
            if (request.RequestedBy != user.Id)
                throw new InvalidOperationException("Only the person who created this request can submit it.");

            var lineCount = cn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM material_request_items WHERE request_id = @requestId",
                new { requestId }, tx);
            if (lineCount == 0)
                throw new InvalidOperationException("This request has no materials.");

            cn.Execute("UPDATE material_requests SET status = 'submitted' WHERE id = @requestId",
                       new { requestId }, tx);
            Log(cn, tx, user.Id, "submitted", requestId, request.RequestNo);
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ------------------------------------------------------------ cancel / void

    /// <summary>Withdraws a draft or submitted request. The requester or an admin may do it.</summary>
    public void Cancel(int requestId, User user, string reason)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        reason = CleanReason(reason);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var request = Lock(cn, tx, requestId);
            if (request.Status is not ("draft" or "submitted"))
                throw new InvalidOperationException(
                    $"{request.RequestNo} is '{request.Status}'. Only a draft or submitted request can be cancelled.");
            if (request.RequestedBy != user.Id && !user.IsAdmin)
                throw new InvalidOperationException("Only the requester or an admin can cancel this request.");

            cn.Execute("UPDATE material_requests SET status = 'cancelled', void_reason = @reason WHERE id = @requestId",
                       new { reason, requestId }, tx);
            Log(cn, tx, user.Id, "cancelled", requestId, $"{request.RequestNo}: {reason}");
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Voids an approved request that has not been acted on. Admin only.
    /// Blocked once anything was issued or a purchase order exists, because
    /// those documents would be left pointing at a dead request.
    /// </summary>
    public void Void(int requestId, User user, string reason)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        if (!user.IsAdmin)
            throw new InvalidOperationException("Only an admin can void a request.");
        reason = CleanReason(reason);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var request = Lock(cn, tx, requestId);
            if (request.Status != "approved")
                throw new InvalidOperationException(
                    $"{request.RequestNo} is '{request.Status}'. Only an approved request can be voided.");

            var issued = cn.ExecuteScalar<decimal>(
                "SELECT COALESCE(SUM(qty_issued), 0) FROM material_request_items WHERE request_id = @requestId",
                new { requestId }, tx);
            if (issued > 0)
                throw new InvalidOperationException("Materials were already issued against this request, so it cannot be voided.");

            var orders = cn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM purchase_orders WHERE request_id = @requestId AND status <> 'cancelled'",
                new { requestId }, tx);
            if (orders > 0)
                throw new InvalidOperationException("A purchase order exists for this request. Cancel the purchase order first.");

            cn.Execute("UPDATE material_requests SET status = 'voided', void_reason = @reason WHERE id = @requestId",
                       new { reason, requestId }, tx);
            Log(cn, tx, user.Id, "voided", requestId, $"{request.RequestNo}: {reason}");
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ------------------------------------------------------------ helpers

    private static string CleanReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Give a reason.");
        reason = reason.Trim();
        if (reason.Length > 200)
            throw new InvalidOperationException("Keep the reason under 200 characters.");
        return reason;
    }

    private static (string RequestNo, string Status, int RequestedBy) Lock(
        MySqlConnector.MySqlConnection cn, MySqlConnector.MySqlTransaction tx, int requestId)
    {
        var row = cn.QuerySingleOrDefault<(string RequestNo, string Status, int RequestedBy)>(
            @"SELECT request_no AS RequestNo, status AS Status, requested_by AS RequestedBy
              FROM   material_requests
              WHERE  id = @requestId FOR UPDATE",
            new { requestId }, tx);

        if (row.RequestNo is null)
            throw new InvalidOperationException("That request no longer exists.");
        return row;
    }

    private static void Log(MySqlConnector.MySqlConnection cn, MySqlConnector.MySqlTransaction tx,
                            int userId, string action, int requestId, string description)
    {
        cn.Execute(
            @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
              VALUES (@userId, @action, 'material_request', @requestId, @description)",
            new { userId, action, requestId, description }, tx);
    }
}
