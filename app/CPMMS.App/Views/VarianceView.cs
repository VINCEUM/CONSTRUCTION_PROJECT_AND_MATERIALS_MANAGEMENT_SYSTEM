using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Planned against actual, per material, per project — the report the whole
/// system exists to produce. Layout is in the designer; filtering/colouring here.
/// </summary>
public partial class VarianceView : UserControl
{
    private readonly CatalogService _catalog = new();

    public VarianceView()
    {
        InitializeComponent();
        Theme.Style(grid);

        cmbProject.DisplayMember = "Name";
        cmbProject.ValueMember = "Id";
        cmbProject.Items.Add(new ProjectOption(0, "All projects"));
        foreach (var p in _catalog.GetProjects())
            cmbProject.Items.Add(new ProjectOption(p.Id, $"{p.Code} — {p.Name}"));
        cmbProject.SelectedIndex = 0;   // triggers the first Reload
    }

    private void cmbProject_SelectedIndexChanged(object? sender, EventArgs e) => Reload();
    private void chkOverruns_CheckedChanged(object? sender, EventArgs e) => Reload();

    private void Reload()
    {
        var option = cmbProject.SelectedItem as ProjectOption;
        int? projectId = option is null || option.Id == 0 ? null : option.Id;
        decimal? minPercent = chkOverruns.Checked ? 10m : null;

        var rows = _catalog.GetVariance(projectId, minPercent);

        UiKit.Bind(grid, rows,
            ("ProjectCode", "PROJECT", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("PlannedQty", "PLANNED", "N2"),
            ("ActualQty", "ACTUAL", "N2"),
            ("QtyVariance", "VARIANCE", "N2"),
            ("VariancePercent", "VARIANCE %", "N1"),
            ("PlannedCost", "PLANNED ₱", "N0"),
            ("ActualCost", "ACTUAL ₱", "N0"),
            ("CostVariance", "₱ VARIANCE", "N0"));

        var overruns = rows.Count(r => r.VariancePercent > 10);
        var overspend = rows.Where(r => r.CostVariance > 0).Sum(r => r.CostVariance);
        lblSummary.Text = $"{rows.Count} line(s)  ·  {overruns} over 10%  ·  ₱{overspend:N0} above plan";
    }

    private void grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not VarianceRow row) return;
        var column = grid.Columns[e.ColumnIndex].Name;

        if (column is "QtyVariance" or "VariancePercent" or "CostVariance")
        {
            var over = row.VariancePercent is > 10;
            var under = row.QtyVariance < 0;
            e.CellStyle!.ForeColor = over ? Theme.Danger : under ? Theme.Good : Theme.InkSoft;
            if (over) e.CellStyle.Font = new Font(Theme.Body, FontStyle.Bold);
        }
    }

    private sealed record ProjectOption(int Id, string Name)
    {
        public override string ToString() => Name;
    }
}
