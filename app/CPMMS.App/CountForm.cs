using System.ComponentModel;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>One material in a physical count.</summary>
public sealed class CountEditRow
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal SystemQty { get; set; }
    public decimal? CountedQty { get; set; }
    public decimal? Variance => CountedQty - SystemQty;
    public string? Remarks { get; set; }
}

/// <summary>
/// A physical stock count. Type what is actually on the shelf for the materials
/// you counted and leave the rest blank. The difference is written to the ledger
/// as an adjustment with the reason you give; stock is never edited directly.
/// </summary>
public sealed class CountForm : Form
{
    private readonly StockControlService _stock = new();
    private readonly BindingList<CountEditRow> _rows;
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly DateTimePicker _date = new();
    private readonly TextBox _reason = new();
    private readonly Label _total = new();
    private readonly Label _error = new();

    public CountForm()
    {
        // code-only form: let Windows' per-monitor DPI scaling handle it
        AutoScaleMode = AutoScaleMode.None;

        Text = "Physical count";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1020, 620);
        BackColor = Theme.Surface;
        Font = Theme.Body;

        _rows = new BindingList<CountEditRow>(new CatalogService().GetMaterials()
            .Where(m => m.Status == "active")
            .Select(m => new CountEditRow
            {
                MaterialId = m.Id, MaterialCode = m.Code, MaterialName = m.Name,
                Unit = m.Unit, SystemQty = m.CurrentStock
            }).ToList());

        // ---- header ---------------------------------------------------------
        var header = new Panel { Dock = DockStyle.Top, Height = 134, BackColor = Color.White, Padding = new Padding(18, 12, 18, 8) };
        var title = new Label { Text = "Physical count", Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top, Height = 30, AutoSize = false };
        var sub = new Label
        {
            Text = "Enter the counted quantity for each material you counted and leave the others blank.",
            ForeColor = Theme.Muted, Dock = DockStyle.Top, Height = 22, AutoSize = false
        };

        var row = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblDate = new Label { Text = "Count date", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _date.Format = DateTimePickerFormat.Short;
        _date.Location = new Point(lblDate.Right + 12, 4);
        _date.Width = 160;
        _date.Value = DateTime.Today;
        var lblReason = new Label { Text = "Reason", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(_date.Right + 24, 8), AutoSize = true };
        _reason.Location = new Point(lblReason.Right + 12, 4);
        _reason.Width = 460;
        _reason.BorderStyle = BorderStyle.FixedSingle;
        _reason.PlaceholderText = "e.g. Month-end physical count";
        row.Controls.AddRange(new Control[] { lblDate, _date, lblReason, _reason });

        header.Controls.Add(row);
        header.Controls.Add(sub);
        header.Controls.Add(title);

        // ---- grid -------------------------------------------------------------
        UiKit.Bind(_grid, _rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("SystemQty", "SYSTEM QTY", "N2"),
            ("CountedQty", "COUNTED QTY", "N2"),
            ("Variance", "VARIANCE", "N2"),
            ("Remarks", "REMARKS", null));

        var minWidths = new Dictionary<string, int>
        {
            ["MaterialCode"] = 85, ["MaterialName"] = 230, ["Unit"] = 65, ["SystemQty"] = 115,
            ["CountedQty"] = 125, ["Variance"] = 105, ["Remarks"] = 180
        };
        foreach (var (name, width) in minWidths) _grid.Columns[name]!.MinimumWidth = width;

        _grid.ReadOnly = false;
        foreach (DataGridViewColumn c in _grid.Columns)
            c.ReadOnly = c.Name is not ("CountedQty" or "Remarks");
        foreach (var name in new[] { "CountedQty", "Remarks" })
            _grid.Columns[name]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
        _grid.Columns["CountedQty"]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
        _grid.EditMode = DataGridViewEditMode.EditOnEnter;
        _grid.CellValueChanged += (_, _) => Recalculate();
        _grid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (_grid.IsCurrentCellDirty) _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        _grid.CellFormatting += PaintVariance;
        _grid.DataError += (_, e) => { e.Cancel = true; ShowError("Enter a number."); };

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_grid);

        // ---- footer -----------------------------------------------------------
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 92, BackColor = Color.White, Padding = new Padding(18, 10, 18, 10) };
        _error.ForeColor = Theme.Danger;
        _error.Dock = DockStyle.Top;
        _error.Height = 22;
        _error.AutoSize = false;
        _total.Font = Theme.H2;
        _total.ForeColor = Theme.Ink;
        _total.Dock = DockStyle.Top;
        _total.Height = 26;
        _total.AutoSize = false;

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 36, FlowDirection = FlowDirection.RightToLeft };
        var post = new Button
        {
            Text = "Post count", BackColor = Theme.Accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9.75F), Height = 32, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly, Width = 140
        };
        post.FlatAppearance.BorderSize = 0;
        post.Click += Post;
        var cancel = UiKit.Secondary("Cancel");
        cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        buttons.Controls.Add(post);
        buttons.Controls.Add(cancel);

        footer.Controls.Add(buttons);
        footer.Controls.Add(_error);
        footer.Controls.Add(_total);

        Controls.Add(host);
        Controls.Add(footer);
        Controls.Add(header);

        Recalculate();
    }

    private void PaintVariance(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
        if (_grid.Columns[e.ColumnIndex].Name != "Variance") return;
        if (_rows[e.RowIndex].Variance is not { } v) return;
        if (v < 0) e.CellStyle!.ForeColor = Theme.Danger;
        else if (v > 0) e.CellStyle!.ForeColor = Theme.Accent;
    }

    private void Recalculate()
    {
        var counted = _rows.Count(r => r.CountedQty is not null);
        var differ = _rows.Count(r => r.Variance is { } v && v != 0);
        _total.Text = $"{counted} counted  ·  {differ} with a difference";
        _grid.Refresh();
    }

    private void ShowError(string message) => _error.Text = message;

    private void Post(object? sender, EventArgs e)
    {
        ShowError("");

        if (string.IsNullOrWhiteSpace(_reason.Text)) { ShowError("Give a reason for the count."); _reason.Focus(); return; }

        var counted = _rows.Where(r => r.CountedQty is not null).ToList();
        if (counted.Count == 0) { ShowError("Enter a counted quantity for at least one material."); return; }
        var negative = counted.FirstOrDefault(r => r.CountedQty < 0);
        if (negative is not null) { ShowError($"{negative.MaterialName}: a count cannot be negative."); return; }

        var differ = counted.Count(r => r.Variance != 0);
        var question = differ == 0
            ? "Post this count? Every counted quantity matches the system."
            : $"Post this count?\n\nThe stock of {differ} material(s) will be set to the quantity you counted, " +
              "and each difference is written to the ledger as an adjustment.";
        if (MessageBox.Show(this, question, "Post count", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        var lines = counted.Select(r => new CountLine
        {
            MaterialId = r.MaterialId, CountedQty = r.CountedQty!.Value, Remarks = r.Remarks
        }).ToList();

        try
        {
            Cursor = Cursors.WaitCursor;
            _stock.PostCount(AppSession.Require, _date.Value.Date, _reason.Text, lines);

            MessageBox.Show(this, "Count posted. Stock and the ledger are updated.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex) { ShowError("Could not post: " + ex.Message); }
        finally { Cursor = Cursors.Default; }
    }
}
