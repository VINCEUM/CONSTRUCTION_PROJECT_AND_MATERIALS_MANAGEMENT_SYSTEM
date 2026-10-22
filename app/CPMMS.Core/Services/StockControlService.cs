using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>One posted return, for the history list.</summary>
public class ReturnRow
{
    public int Id { get; set; }
    public string ReturnNo { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public DateTime ReturnDate { get; set; }
    public string ReturnedBy { get; set; } = "";
    public string AcceptedByName { get; set; } = "";
    public int LineCount { get; set; }
    public decimal ReusableValue { get; set; }
    public decimal DamagedValue { get; set; }
    public string Status { get; set; } = "";
}

/// <summary>One physical count, for the history list.</summary>
public class AdjustmentRow
{
    public int Id { get; set; }
    public string AdjustmentNo { get; set; } = "";
    public DateTime CountDate { get; set; }
    public string CountedByName { get; set; } = "";
    public string Reason { get; set; } = "";
    public int LineCount { get; set; }
    public int ChangedLines { get; set; }
    public decimal NetVariance { get; set; }
    public string Status { get; set; } = "";
}

/// <summary>An issuance a return can come from.</summary>
public class IssueOption
{
    public int Id { get; set; }
    public string IssueNo { get; set; } = "";
    public DateTime IssueDate { get; set; }
    public override string ToString() => $"{IssueNo}  ·  {IssueDate:d}";
}

/// <summary>What one issuance handed out of a material, and how much has already come back.</summary>
public class IssueLineRow
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal QtyIssued { get; set; }
    public decimal QtyReturned { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Returnable => QtyIssued - QtyReturned;
}

/// <summary>
/// Returns of unused material and physical counts. Both are Administrator
/// actions, and both change stock, so they go through InventoryService, which
/// writes the ledger. This class adds the permission rule and the checks that
/// a return can only bring back what the issuance actually handed out.
/// </summary>
public sealed class StockControlService
{
    private const string NoDemo = "Returns and physical counts need the database. They are not available in demo mode.";

    // ------------------------------------------------------------ history

    public IReadOnlyList<ReturnRow> GetReturns()
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<ReturnRow>(
            @"SELECT r.id, r.return_no, p.name AS project_name, r.return_date, r.returned_by,
                     u.full_name AS accepted_by_name, r.status,
                     COUNT(i.id) AS line_count,
                     COALESCE(SUM(CASE WHEN i.disposition = 'reusable' THEN i.qty_returned * i.unit_cost END), 0) AS reusable_value,
                     COALESCE(SUM(CASE WHEN i.disposition = 'damaged'  THEN i.qty_returned * i.unit_cost END), 0) AS damaged_value
              FROM   material_returns r
              JOIN   projects p ON p.id = r.project_id
              JOIN   users u ON u.id = r.accepted_by
              LEFT   JOIN material_return_items i ON i.return_id = r.id
              GROUP  BY r.id, r.return_no, p.name, r.return_date, r.returned_by, u.full_name, r.status
              ORDER  BY r.return_date DESC, r.id DESC");
    }

    public IReadOnlyList<AdjustmentRow> GetAdjustments()
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<AdjustmentRow>(
            @"SELECT a.id, a.adjustment_no, a.count_date, u.full_name AS counted_by_name, a.reason, a.status,
                     COUNT(i.id) AS line_count,
                     CAST(COALESCE(SUM(i.variance_qty <> 0), 0) AS SIGNED) AS changed_lines,
                     COALESCE(SUM(i.variance_qty), 0) AS net_variance
              FROM   stock_adjustments a
              JOIN   users u ON u.id = a.counted_by
              LEFT   JOIN stock_adjustment_items i ON i.adjustment_id = a.id
              GROUP  BY a.id, a.adjustment_no, a.count_date, u.full_name, a.reason, a.status
              ORDER  BY a.count_date DESC, a.id DESC");
    }

    // ------------------------------------------------------------ returns

    public IReadOnlyList<IssueOption> GetIssuesForProject(int projectId)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<IssueOption>(
            @"SELECT id, issue_no, issue_date FROM material_issues
              WHERE  project_id = @projectId AND status = 'posted'
              ORDER  BY issue_date DESC, id DESC", new { projectId });
    }

    public IReadOnlyList<IssueLineRow> GetIssueLines(int issueId)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<IssueLineRow>(
            @"SELECT ii.material_id, m.code AS material_code, m.name AS material_name, m.unit,
                     SUM(ii.qty_issued) AS qty_issued,
                     COALESCE((SELECT SUM(ri.qty_returned)
                               FROM   material_return_items ri
                               JOIN   material_returns r ON r.id = ri.return_id
                               WHERE  r.issue_id = ii.issue_id AND r.status = 'posted'
                                 AND  ri.material_id = ii.material_id), 0) AS qty_returned,
                     MAX(ii.unit_cost) AS unit_cost
              FROM   material_issue_items ii
              JOIN   materials m ON m.id = ii.material_id
              WHERE  ii.issue_id = @issueId
              GROUP  BY ii.issue_id, ii.material_id, m.code, m.name, m.unit
              ORDER  BY m.name", new { issueId });
    }

    /// <summary>
    /// Posts a return against an issuance. Reusable stock goes back on the
    /// shelf; damaged stock does not, and stays charged to the project as wastage.
    /// </summary>
    public int PostReturn(User user, int projectId, int issueId, DateTime returnDate,
                          string returnedBy, string? note, IList<ReturnLine> lines)
    {
        if (!user.IsAdmin) throw new InvalidOperationException("Only an administrator can accept a return.");
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        if (lines.Count == 0) throw new InvalidOperationException("Nothing to return.");

        returnedBy = (returnedBy ?? "").Trim();
        if (returnedBy.Length == 0) throw new InvalidOperationException("Record who brought the materials back.");
        if (returnedBy.Length > 120) throw new InvalidOperationException("Keep the name under 120 characters.");
        note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (note is { Length: > 255 }) throw new InvalidOperationException("Keep the note under 255 characters.");

        var belongs = DatabaseHelper.QueryOne<int>(
            "SELECT COUNT(*) FROM material_issues WHERE id = @issueId AND project_id = @projectId AND status = 'posted'",
            new { issueId, projectId });
        if (belongs == 0) throw new InvalidOperationException("That issuance doesn't belong to the chosen project.");

        // a return can only bring back what the issuance handed out and hasn't already been returned
        var issued = GetIssueLines(issueId).ToDictionary(l => l.MaterialId);
        foreach (var group in lines.GroupBy(l => l.MaterialId))
        {
            if (!issued.TryGetValue(group.Key, out var line))
                throw new InvalidOperationException("One of the materials was not part of that issuance.");

            var qty = group.Sum(l => l.Qty);
            if (qty > line.Returnable)
                throw new InvalidOperationException(
                    $"{line.MaterialName}: only {line.Returnable:N2} {line.Unit} can still be returned from this issuance " +
                    $"({line.QtyIssued:N2} issued, {line.QtyReturned:N2} already returned).");

            foreach (var l in group) if (l.UnitCost == 0) l.UnitCost = line.UnitCost;
        }

        return new InventoryService().ReturnMaterials(projectId, issueId, returnDate.Date, returnedBy,
                                                      user.Id, note, lines);
    }

    // ------------------------------------------------------------ physical count

    public int PostCount(User user, DateTime countDate, string reason, IList<CountLine> lines)
    {
        if (!user.IsAdmin) throw new InvalidOperationException("Only an administrator can post a physical count.");
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);
        if (lines.Count == 0) throw new InvalidOperationException("Enter at least one counted quantity.");

        reason = (reason ?? "").Trim();
        if (reason.Length == 0) throw new InvalidOperationException("Give a reason for the count.");
        if (reason.Length > 255) throw new InvalidOperationException("Keep the reason under 255 characters.");
        if (lines.GroupBy(l => l.MaterialId).Any(g => g.Count() > 1))
            throw new InvalidOperationException("A material can be counted only once per count.");

        return new InventoryService().PostStockAdjustment(countDate.Date, user.Id, reason, lines);
    }
}
