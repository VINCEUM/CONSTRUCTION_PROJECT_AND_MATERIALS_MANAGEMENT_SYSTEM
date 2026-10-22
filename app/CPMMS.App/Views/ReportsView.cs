using System.Text;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Project reports: pick a project and a report, read it, and export it to a
/// CSV file (opens in Excel). The Administrator can pick any project; an
/// engineer only the projects assigned to them, and ReportService enforces that.
/// </summary>
public sealed class ReportsView : UserControl
{
    private static readonly string[] ReportNames =
    {
        "Project progress",
        "Material requests",
        "Inventory transactions",
        "Material usage",
        "Cost and quantity variance"
    };

    private readonly ReportService _service = new();
    private readonly ComboBox _project = new();
    private readonly ComboBox _report = new();
    private readonly Label _summary = new();
    private readonly Label _message = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly IReadOnlyList<Project> _projects;

    public ReportsView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var user = AppSession.Require;
        _projects = _service.GetProjectsFor(user);

        // ---- pickers ------------------------------------------------------
        var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };
        var lblProject = new Label { Text = "Project", Font = Theme.Body, ForeColor = Theme.InkSoft, AutoSize = true, Location = new Point(0, 12) };
        _project.DropDownStyle = ComboBoxStyle.DropDownList;
        _project.Location = new Point(lblProject.Right + 12, 8);
        _project.Width = 360;
        foreach (var p in _projects) _project.Items.Add($"{p.Code} — {p.Name}");

        var lblReport = new Label { Text = "Report", Font = Theme.Body, ForeColor = Theme.InkSoft, AutoSize = true, Location = new Point(_project.Right + 24, 12) };
        _report.DropDownStyle = ComboBoxStyle.DropDownList;
        _report.Location = new Point(lblReport.Right + 12, 8);
        _report.Width = 240;
        _report.Items.AddRange(ReportNames);

        var export = UiKit.Secondary("Export CSV…");
        export.Location = new Point(_report.Right + 16, 7);
        export.Click += (_, _) => ExportCsv();

        bar.Controls.Add(export);
        bar.Controls.Add(_report);
        bar.Controls.Add(lblReport);
        bar.Controls.Add(_project);
        bar.Controls.Add(lblProject);

        _summary.Dock = DockStyle.Top;
        _summary.Height = 34;
        _summary.Font = Theme.H2;
        _summary.ForeColor = Theme.Ink;
        _summary.TextAlign = ContentAlignment.MiddleLeft;

        _message.Dock = DockStyle.Bottom;
        _message.Height = 28;
        _message.Font = Theme.Body;
        _message.ForeColor = Theme.InkSoft;
        _message.TextAlign = ContentAlignment.MiddleLeft;

        _grid.CellFormatting += FormatCell;
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_grid);

        Controls.Add(host);
        Controls.Add(_message);
        Controls.Add(_summary);
        Controls.Add(bar);

        if (_projects.Count == 0)
        {
            _summary.Text = user.IsAdmin ? "There are no projects yet." : "No project is assigned to you yet.";
            return;
        }

        _project.SelectedIndex = 0;
        _report.SelectedIndex = 0;
        _project.SelectedIndexChanged += (_, _) => Generate();
        _report.SelectedIndexChanged += (_, _) => Generate();
        Generate();
    }

    private Project? CurrentProject =>
        _project.SelectedIndex >= 0 && _project.SelectedIndex < _projects.Count ? _projects[_project.SelectedIndex] : null;

    private void Generate()
    {
        var project = CurrentProject;
        if (project is null || _report.SelectedIndex < 0) return;

        var user = AppSession.Require;
        _message.Text = "";

        try
        {
            switch (ReportNames[_report.SelectedIndex])
            {
                case "Project progress":
                {
                    var tasks = _service.GetProgress(user, project.Id);
                    UiKit.Bind(_grid, tasks,
                        ("Name", "TASK", null), ("WeightPercent", "WEIGHT %", "N2"),
                        ("StartDate", "START", "d"), ("DueDate", "DUE", "d"),
                        ("ProgressPercent", "PROGRESS %", "N1"), ("Status", "STATUS", null),
                        ("Remarks", "REMARKS", null));
                    _summary.Text = $"Overall progress {ProjectMonitoringService.OverallProgress(tasks):N1}%  ·  {tasks.Count} task(s)";
                    break;
                }
                case "Material requests":
                {
                    var rows = _service.GetRequests(user, project.Id);
                    UiKit.Bind(_grid, rows,
                        ("RequestNo", "REQUEST NO.", null), ("RequestDate", "DATE", "d"), ("NeededDate", "NEEDED", "d"),
                        ("Status", "STATUS", null), ("RequestedByName", "REQUESTED BY", null),
                        ("ItemCount", "ITEMS", "N0"), ("QtyRequested", "QTY REQUESTED", "N2"),
                        ("QtyIssued", "QTY ISSUED", "N2"), ("Remarks", "REMARKS", null));
                    var byStatus = string.Join("  ·  ", rows.GroupBy(r => r.Status).Select(g => $"{g.Count()} {g.Key}"));
                    _summary.Text = rows.Count == 0 ? "No material requests for this project yet." : $"{rows.Count} request(s)  ·  {byStatus}";
                    break;
                }
                case "Inventory transactions":
                {
                    var rows = _service.GetInventoryTransactions(user, project.Id);
                    UiKit.Bind(_grid, rows,
                        ("CreatedAt", "WHEN", "g"), ("TxnType", "TYPE", null),
                        ("MaterialCode", "CODE", null), ("MaterialName", "MATERIAL", null), ("Unit", "UNIT", null),
                        ("Quantity", "QTY", "N2"), ("UnitCost", "UNIT COST", "N2"), ("BalanceAfter", "BALANCE AFTER", "N2"),
                        ("PerformedByName", "BY", null), ("Remarks", "REFERENCE", null));
                    _summary.Text = rows.Count == 0 ? "No stock movements are charged to this project yet." : $"{rows.Count} stock movement(s)";
                    break;
                }
                case "Material usage":
                {
                    var rows = _service.GetUsageAndVariance(user, project.Id);
                    UiKit.Bind(_grid, rows,
                        ("MaterialCode", "CODE", null), ("MaterialName", "MATERIAL", null), ("Unit", "UNIT", null),
                        ("IssuedQty", "ISSUED", "N2"), ("ReturnedQty", "RETURNED", "N2"),
                        ("ActualQty", "NET USED", "N2"), ("ActualCost", "ACTUAL COST ₱", "N2"));
                    _summary.Text = $"{rows.Count} material(s)  ·  actual cost ₱{rows.Sum(r => r.ActualCost):N2}";
                    break;
                }
                default:   // Cost and quantity variance
                {
                    var rows = _service.GetUsageAndVariance(user, project.Id);
                    UiKit.Bind(_grid, rows,
                        ("MaterialCode", "CODE", null), ("MaterialName", "MATERIAL", null), ("Unit", "UNIT", null),
                        ("PlannedQty", "PLANNED QTY", "N2"), ("ActualQty", "ACTUAL QTY", "N2"), ("QtyVariance", "QTY VARIANCE", "N2"),
                        ("PlannedCost", "PLANNED ₱", "N2"), ("ActualCost", "ACTUAL ₱", "N2"), ("CostVariance", "COST VARIANCE ₱", "N2"),
                        ("VariancePercent", "VARIANCE %", "N1"));
                    var planned = rows.Sum(r => r.PlannedCost);
                    var actual = rows.Sum(r => r.ActualCost);
                    _summary.Text = planned == 0
                        ? $"Actual cost ₱{actual:N2}  ·  nothing planned yet (add the project's material estimates)"
                        : $"Planned ₱{planned:N2}  ·  actual ₱{actual:N2}  ·  {(actual - planned) / planned:+0.0%;-0.0%;0.0%} against plan";
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _summary.Text = "";
            _message.Text = "  " + ex.Message;
        }
    }

    /// <summary>Over plan reads red, under plan reads green.</summary>
    private void FormatCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _grid.Rows.Count) return;
        var column = _grid.Columns[e.ColumnIndex].Name;
        if (column is not ("QtyVariance" or "CostVariance" or "VariancePercent")) return;
        if (_grid.Rows[e.RowIndex].DataBoundItem is not VarianceRow row) return;

        var value = column switch { "QtyVariance" => row.QtyVariance, "CostVariance" => row.CostVariance, _ => row.VariancePercent ?? 0m };
        if (value > 0) e.CellStyle!.ForeColor = Theme.Danger;
        else if (value < 0) e.CellStyle!.ForeColor = Theme.Good;
    }

    // ============================================================ export

    private void ExportCsv()
    {
        var project = CurrentProject;
        if (project is null || _grid.Rows.Count == 0) { _message.Text = "  There is nothing to export yet."; return; }

        var report = ReportNames[_report.SelectedIndex];
        using var dialog = new SaveFileDialog
        {
            Title = "Export report",
            Filter = "CSV file (*.csv)|*.csv",
            FileName = $"{project.Code}_{report.Replace(' ', '-')}_{DateTime.Today:yyyyMMdd}.csv"
        };
        if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

        try
        {
            var text = new StringBuilder();
            text.AppendLine($"Project,{Csv($"{project.Code} {project.Name}")}");
            text.AppendLine($"Report,{Csv(report)}");
            text.AppendLine($"Generated,{DateTime.Now:yyyy-MM-dd HH:mm}");
            text.AppendLine();

            var columns = _grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible).ToList();
            text.AppendLine(string.Join(",", columns.Select(c => Csv(c.HeaderText))));
            foreach (DataGridViewRow row in _grid.Rows)
                text.AppendLine(string.Join(",", columns.Select(c => Csv(row.Cells[c.Index].FormattedValue?.ToString()))));

            File.WriteAllText(dialog.FileName, text.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            _message.Text = $"  Saved {Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception ex)
        {
            _message.Text = "  Could not save the file: " + ex.Message;
        }
    }

    private static string Csv(string? value)
    {
        value ??= "";
        return value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
    }
}
