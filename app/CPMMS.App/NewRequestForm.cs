using System.ComponentModel;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>One line being built up before the request is saved.</summary>
public sealed class NewRequestLine
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal OnHand { get; set; }
    public decimal Qty { get; set; }
}

/// <summary>
/// Lets a Project Engineer build a material request line by line, then either
/// save it as a draft or submit it straight away. Both go through
/// RequestService, so the same rules apply no matter which button is clicked.
/// </summary>
public sealed class NewRequestForm : Form
{
    private readonly RequestService _requests = new();
    private readonly BindingList<NewRequestLine> _lines = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly ComboBox _project = new();
    private readonly ComboBox _material = new();
    private readonly NumericUpDown _qty = new();
    private readonly DateTimePicker _needed = new();
    private readonly TextBox _remarks = new();
    private readonly Label _error = new();

    private readonly IReadOnlyList<Material> _catalog;

    public NewRequestForm()
    {
        Text = "New material request";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(820, 560);
        BackColor = Theme.Surface;
        Font = Theme.Body;

        var projects = _requests.GetRequestableProjects(AppSession.Require);
        _catalog = _requests.GetRequestableMaterials();

        // ---- header -----------------------------------------------------
        var header = new Panel { Dock = DockStyle.Top, Height = 128, BackColor = Color.White, Padding = new Padding(18, 12, 18, 8) };
        var title = new Label { Text = "New material request", Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top, Height = 30, AutoSize = false };

        var fields = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblProject = new Label { Text = "Project", ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _project.DropDownStyle = ComboBoxStyle.DropDownList;
        _project.Location = new Point(60, 4);
        _project.Width = 320;
        _project.DisplayMember = "Name";
        _project.ValueMember = "Id";
        _project.DataSource = projects;

        var lblNeeded = new Label { Text = "Needed by", ForeColor = Theme.InkSoft, Location = new Point(400, 8), AutoSize = true };
        _needed.Format = DateTimePickerFormat.Short;
        _needed.Location = new Point(478, 4);
        _needed.Width = 120;
        _needed.Value = DateTime.Today.AddDays(3);

        fields.Controls.AddRange(new Control[] { lblProject, _project, lblNeeded, _needed });

        var remarksRow = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblRemarks = new Label { Text = "Remarks", ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _remarks.Location = new Point(60, 4);
        _remarks.Width = 538;
        _remarks.BorderStyle = BorderStyle.FixedSingle;
        _remarks.PlaceholderText = "optional — what this request is for";
        remarksRow.Controls.AddRange(new Control[] { lblRemarks, _remarks });

        header.Controls.Add(remarksRow);
        header.Controls.Add(fields);
        header.Controls.Add(title);

        // ---- add-line bar -------------------------------------------------
        var addBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Surface, Padding = new Padding(0, 8, 0, 0) };
        _material.DropDownStyle = ComboBoxStyle.DropDownList;
        _material.Location = new Point(0, 8);
        _material.Width = 420;
        _material.DisplayMember = "Name";
        _material.ValueMember = "Id";
        _material.DataSource = _catalog.Select(m => new { m.Id, Name = $"{m.Code} — {m.Name} ({m.Unit})", m.CurrentStock }).ToList();

        _qty.Location = new Point(430, 8);
        _qty.Width = 90;
        _qty.DecimalPlaces = 2;
        _qty.Maximum = 999999;
        _qty.Minimum = 0.01m;
        _qty.Value = 1;

        var add = UiKit.Secondary("Add line");
        add.Location = new Point(528, 6);
        add.Click += (_, _) => AddLine();

        var remove = UiKit.Secondary("Remove selected");
        remove.ForeColor = Theme.Danger;
        remove.FlatAppearance.BorderColor = Theme.Danger;
        remove.Location = new Point(528 + add.GetPreferredSize(Size.Empty).Width + 8, 6);
        remove.Click += (_, _) => RemoveLine();

        addBar.Controls.Add(remove);
        addBar.Controls.Add(add);
        addBar.Controls.Add(_qty);
        addBar.Controls.Add(_material);

        // ---- grid -----------------------------------------------------------
        UiKit.Bind(_grid, _lines,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("OnHand", "ON HAND", "N2"),
            ("Qty", "QTY REQUESTED", "N2"));
        _grid.ReadOnly = true;

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_grid);

        // ---- footer ---------------------------------------------------------
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 76, BackColor = Color.White, Padding = new Padding(18, 10, 18, 10) };

        _error.ForeColor = Theme.Danger;
        _error.Dock = DockStyle.Top;
        _error.Height = 20;
        _error.AutoSize = false;

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 36, FlowDirection = FlowDirection.RightToLeft };

        var submit = new Button
        {
            Text = "Submit for approval",
            BackColor = Theme.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9.75F),
            Height = 32,
            Width = 160
        };
        submit.FlatAppearance.BorderSize = 0;
        submit.Click += (_, _) => Save(submitNow: true);

        var saveDraft = UiKit.Secondary("Save as draft");
        saveDraft.Width = 120;
        saveDraft.Click += (_, _) => Save(submitNow: false);

        var cancel = UiKit.Secondary("Cancel");
        cancel.Width = 90;
        cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        buttons.Controls.Add(submit);
        buttons.Controls.Add(saveDraft);
        buttons.Controls.Add(cancel);

        footer.Controls.Add(buttons);
        footer.Controls.Add(_error);

        Controls.Add(host);
        Controls.Add(addBar);
        Controls.Add(footer);
        Controls.Add(header);
    }

    private void AddLine()
    {
        ShowError("");
        if (_material.SelectedValue is not int materialId) return;

        var material = _catalog.First(m => m.Id == materialId);
        if (_lines.Any(l => l.MaterialId == materialId))
        {
            ShowError($"{material.Name} is already on this request.");
            return;
        }

        _lines.Add(new NewRequestLine
        {
            MaterialId = material.Id,
            MaterialCode = material.Code,
            MaterialName = material.Name,
            Unit = material.Unit,
            OnHand = material.CurrentStock,
            Qty = _qty.Value
        });
    }

    private void RemoveLine()
    {
        if (_grid.CurrentRow?.DataBoundItem is NewRequestLine line) _lines.Remove(line);
    }

    private void ShowError(string message) => _error.Text = message;

    private void Save(bool submitNow)
    {
        ShowError("");

        if (_project.SelectedValue is not int projectId)
        {
            ShowError("Choose a project.");
            return;
        }
        if (_lines.Count == 0)
        {
            ShowError("Add at least one material.");
            return;
        }

        var lines = _lines.Select(l => new DraftLine(l.MaterialId, l.Qty)).ToList();

        try
        {
            Cursor = Cursors.WaitCursor;
            var requestId = _requests.CreateDraft(
                AppSession.Require, projectId, _needed.Value.Date,
                _remarks.Text, lines);

            if (submitNow)
                _requests.Submit(requestId, AppSession.Require);

            MessageBox.Show(this,
                submitNow ? "Request submitted for approval." : "Request saved as a draft.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ShowError("Could not save: " + ex.Message);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
