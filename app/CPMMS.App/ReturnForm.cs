using System.ComponentModel;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>One material of an issuance, and how much of it is coming back.</summary>
public sealed class ReturnEditRow
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal QtyIssued { get; set; }
    public decimal QtyAlreadyReturned { get; set; }
    public decimal UnitCost { get; set; }
    public decimal CanReturn => QtyIssued - QtyAlreadyReturned;
    public decimal QtyReusable { get; set; }
    public decimal QtyDamaged { get; set; }
}

/// <summary>
/// Accepting unused material back from a site. A return always points at the
/// issuance it came from, so it can't bring back more than was handed out.
/// Reusable quantities go back on the shelf; damaged ones stay charged to the
/// project as wastage. StockControlService holds the rules.
/// </summary>
public sealed class ReturnForm : Form
{
    private readonly StockControlService _stock = new();
    private readonly BindingList<ReturnEditRow> _rows = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly ComboBox _project = new();
    private readonly ComboBox _issue = new();
    private readonly DateTimePicker _date = new();
    private readonly TextBox _returnedBy = new();
    private readonly TextBox _note = new();
    private readonly Label _total = new();
    private readonly Label _error = new();
    private readonly IReadOnlyList<Project> _projects;

    public ReturnForm()
    {
        // code-only form: let Windows' per-monitor DPI scaling handle it
        AutoScaleMode = AutoScaleMode.None;

        Text = "Return materials";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1020, 600);
        BackColor = Theme.Surface;
        Font = Theme.Body;

        _projects = new CatalogService().GetProjects();

        // ---- header ---------------------------------------------------------
        var header = new Panel { Dock = DockStyle.Top, Height = 168, BackColor = Color.White, Padding = new Padding(18, 12, 18, 8) };
        var title = new Label { Text = "Return materials", Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top, Height = 30, AutoSize = false };

        var row1 = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblProject = new Label { Text = "Project", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _project.DropDownStyle = ComboBoxStyle.DropDownList;
        _project.Location = new Point(lblProject.Right + 12, 4);
        _project.Width = 380;
        foreach (var p in _projects) _project.Items.Add($"{p.Code} — {p.Name}");
        var lblIssue = new Label { Text = "From issuance", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(_project.Right + 24, 8), AutoSize = true };
        _issue.DropDownStyle = ComboBoxStyle.DropDownList;
        _issue.Location = new Point(lblIssue.Right + 12, 4);
        _issue.Width = 240;
        row1.Controls.AddRange(new Control[] { lblProject, _project, lblIssue, _issue });

        var row2 = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblDate = new Label { Text = "Return date", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _date.Format = DateTimePickerFormat.Short;
        _date.Location = new Point(lblDate.Right + 12, 4);
        _date.Width = 160;
        _date.Value = DateTime.Today;
        var lblBy = new Label { Text = "Returned by", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(_date.Right + 24, 8), AutoSize = true };
        _returnedBy.Location = new Point(lblBy.Right + 12, 4);
        _returnedBy.Width = 280;
        _returnedBy.BorderStyle = BorderStyle.FixedSingle;
        _returnedBy.PlaceholderText = "Name of the person from site";
        row2.Controls.AddRange(new Control[] { lblDate, _date, lblBy, _returnedBy });

        var row3 = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblNote = new Label { Text = "Condition note", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _note.Location = new Point(lblNote.Right + 12, 4);
        _note.Width = 520;
        _note.BorderStyle = BorderStyle.FixedSingle;
        _note.PlaceholderText = "optional";
        row3.Controls.AddRange(new Control[] { lblNote, _note });

        header.Controls.Add(row3);
        header.Controls.Add(row2);
        header.Controls.Add(row1);
        header.Controls.Add(title);

        // ---- grid -------------------------------------------------------------
        UiKit.Bind(_grid, _rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyIssued", "ISSUED", "N2"),
            ("QtyAlreadyReturned", "ALREADY RETURNED", "N2"),
            ("CanReturn", "CAN RETURN", "N2"),
            ("QtyReusable", "REUSABLE QTY", "N2"),
            ("QtyDamaged", "DAMAGED QTY", "N2"));

        var minWidths = new Dictionary<string, int>
        {
            ["MaterialCode"] = 80, ["MaterialName"] = 190, ["Unit"] = 65, ["QtyIssued"] = 95,
            ["QtyAlreadyReturned"] = 150, ["CanReturn"] = 115, ["QtyReusable"] = 130, ["QtyDamaged"] = 130
        };
        foreach (var (name, width) in minWidths) _grid.Columns[name]!.MinimumWidth = width;

        _grid.ReadOnly = false;
        foreach (DataGridViewColumn c in _grid.Columns)
            c.ReadOnly = c.Name is not ("QtyReusable" or "QtyDamaged");
        foreach (var name in new[] { "QtyReusable", "QtyDamaged" })
        {
            _grid.Columns[name]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
            _grid.Columns[name]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
        }
        _grid.EditMode = DataGridViewEditMode.EditOnEnter;
        _grid.CellValueChanged += (_, _) => Recalculate();
        _grid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (_grid.IsCurrentCellDirty) _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        _grid.CellFormatting += Warn;
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
            Text = "Post return", BackColor = Theme.Accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
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

        // wait until the form is live before reading the combo selections
        Load += (_, _) =>
        {
            if (_projects.Count == 0) { ShowError("There are no projects yet."); return; }
            _project.SelectedIndexChanged += (_, _) => LoadIssues();
            _issue.SelectedIndexChanged += (_, _) => LoadLines();
            _project.SelectedIndex = 0;
        };
    }

    private Project? CurrentProject =>
        _project.SelectedIndex >= 0 && _project.SelectedIndex < _projects.Count ? _projects[_project.SelectedIndex] : null;

    private void LoadIssues()
    {
        ShowError("");
        _rows.Clear();
        _issue.Items.Clear();
        Recalculate();

        var project = CurrentProject;
        if (project is null) return;

        try
        {
            var issues = _stock.GetIssuesForProject(project.Id);
            foreach (var i in issues) _issue.Items.Add(i);
            if (issues.Count == 0) ShowError("No materials have been issued to this project yet, so there is nothing to return.");
            else _issue.SelectedIndex = 0;
        }
        catch (Exception ex) { ShowError(ex.Message); }
    }

    private void LoadLines()
    {
        ShowError("");
        _rows.Clear();
        if (_issue.SelectedItem is IssueOption issue)
        {
            try
            {
                foreach (var l in _stock.GetIssueLines(issue.Id))
                    _rows.Add(new ReturnEditRow
                    {
                        MaterialId = l.MaterialId, MaterialCode = l.MaterialCode, MaterialName = l.MaterialName,
                        Unit = l.Unit, QtyIssued = l.QtyIssued, QtyAlreadyReturned = l.QtyReturned, UnitCost = l.UnitCost
                    });
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }
        Recalculate();
    }

    private void Warn(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
        var row = _rows[e.RowIndex];
        var name = _grid.Columns[e.ColumnIndex].Name;
        if ((name == "QtyReusable" || name == "QtyDamaged") && row.QtyReusable + row.QtyDamaged > row.CanReturn)
            e.CellStyle!.ForeColor = Theme.Danger;
    }

    private void Recalculate()
    {
        var reusable = _rows.Sum(r => r.QtyReusable);
        var damaged = _rows.Sum(r => r.QtyDamaged);
        var wastage = _rows.Sum(r => r.QtyDamaged * r.UnitCost);
        var lines = _rows.Count(r => r.QtyReusable + r.QtyDamaged > 0);
        _total.Text = $"{lines} line(s)  ·  back to stock {reusable:N2}  ·  damaged {damaged:N2}  (wastage ₱{wastage:N2})";
        _grid.Refresh();
    }

    private void ShowError(string message) => _error.Text = message;

    private void Post(object? sender, EventArgs e)
    {
        ShowError("");

        var project = CurrentProject;
        if (project is null || _issue.SelectedItem is not IssueOption issue) { ShowError("Choose a project and the issuance the materials came from."); return; }
        if (string.IsNullOrWhiteSpace(_returnedBy.Text)) { ShowError("Enter who brought the materials back."); _returnedBy.Focus(); return; }

        var bad = _rows.FirstOrDefault(r => r.QtyReusable < 0 || r.QtyDamaged < 0);
        if (bad is not null) { ShowError($"{bad.MaterialName}: quantities cannot be negative."); return; }
        var over = _rows.FirstOrDefault(r => r.QtyReusable + r.QtyDamaged > r.CanReturn);
        if (over is not null) { ShowError($"{over.MaterialName}: only {over.CanReturn:N2} {over.Unit} can still be returned."); return; }

        var lines = new List<ReturnLine>();
        foreach (var r in _rows)
        {
            if (r.QtyReusable > 0) lines.Add(new ReturnLine { MaterialId = r.MaterialId, Qty = r.QtyReusable, UnitCost = r.UnitCost, Disposition = "reusable" });
            if (r.QtyDamaged > 0) lines.Add(new ReturnLine { MaterialId = r.MaterialId, Qty = r.QtyDamaged, UnitCost = r.UnitCost, Disposition = "damaged" });
        }
        if (lines.Count == 0) { ShowError("Enter a reusable or damaged quantity on at least one line."); return; }

        try
        {
            Cursor = Cursors.WaitCursor;
            _stock.PostReturn(AppSession.Require, project.Id, issue.Id, _date.Value.Date, _returnedBy.Text, _note.Text, lines);

            MessageBox.Show(this,
                "Return posted.\n\nReusable materials are back in stock. Damaged materials stay charged to the project as wastage.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex) { ShowError("Could not post: " + ex.Message); }
        finally { Cursor = Cursors.Default; }
    }
}
