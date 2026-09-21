using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Planned against actual, per material, per project — the report the whole
/// system exists to produce.
/// </summary>
public sealed class VarianceView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly ComboBox _project = new();
    private readonly CheckBox _overrunsOnly = new();
    private readonly Label _summary = new();

    public VarianceView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };

        _project.DropDownStyle = ComboBoxStyle.DropDownList;
        _project.Font = Theme.Body;
        _project.Location = new Point(0, 8);
        _project.Width = 300;
        _project.DisplayMember = "Name";
        _project.ValueMember = "Id";
        _project.Items.Add(new ProjectOption(0, "All projects"));
        foreach (var p in _catalog.GetProjects())
            _project.Items.Add(new ProjectOption(p.Id, $"{p.Code} — {p.Name}"));
        _project.SelectedIndex = 0;
        _project.SelectedIndexChanged += (_, _) => Reload();

        _overrunsOnly.Text = "Overruns only (over 10%)";
        _overrunsOnly.Font = Theme.Body;
        _overrunsOnly.ForeColor = Theme.InkSoft;
        _overrunsOnly.Location = new Point(316, 11);
        _overrunsOnly.AutoSize = true;
        _overrunsOnly.CheckedChanged += (_, _) => Reload();

        _summary.ForeColor = Theme.Muted;
        _summary.Font = Theme.Small;
        _summary.AutoSize = false;
        _summary.Location = new Point(530, 14);
        _summary.Size = new Size(420, 20);

        bar.Controls.Add(_summary);
        bar.Controls.Add(_overrunsOnly);
        bar.Controls.Add(_project);

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _grid.CellFormatting += Colourise;
        host.Controls.Add(_grid);

        Controls.Add(host);
        Controls.Add(bar);

        Reload();
    }

    private void Reload()
    {
        var option = _project.SelectedItem as ProjectOption;
        int? projectId = option is null || option.Id == 0 ? null : option.Id;
        decimal? minPercent = _overrunsOnly.Checked ? 10m : null;

        var rows = _catalog.GetVariance(projectId, minPercent);

        UiKit.Bind(_grid, rows,
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
        _summary.Text = $"{rows.Count} line(s)  ·  {overruns} over 10%  ·  ₱{overspend:N0} above plan";
    }

    private void Colourise(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not VarianceRow row) return;
        var column = _grid.Columns[e.ColumnIndex].Name;

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
