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
        // this form is built entirely in code (no Designer baseline), so let
        // Windows' own per-monitor DPI scaling handle it — WinForms' separate
        // font-ratio auto-scale would otherwise double up and misalign things
        AutoScaleMode = AutoScaleMode.None;

        Text = "New material request";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = UiKit.FitSize(1000, 680);
        BackColor = Theme.Surface;
        Font = Theme.Body;

        var projects = _requests.GetRequestableProjects(AppSession.Require);
        _catalog = _requests.GetRequestableMaterials();

        // ---- header -----------------------------------------------------
        _project.DropDownStyle = ComboBoxStyle.DropDownList;
        _project.Width = 400;
        _project.DisplayMember = "Name";
        _project.ValueMember = "Id";
        _project.DataSource = projects;

        _needed.Format = DateTimePickerFormat.Short;
        _needed.Width = 180;
        _needed.Value = DateTime.Today.AddDays(3);

        _remarks.Width = 660;
        _remarks.PlaceholderText = "optional — what this request is for";

        var header = UiKit.HeaderPanel(
            UiKit.FormTitle("New material request"),
            UiKit.FieldRow(("Project", _project), ("Needed by", _needed)),
            UiKit.FieldRow(("Remarks", _remarks)));

        // ---- add-line bar -------------------------------------------------
        _material.DropDownStyle = ComboBoxStyle.DropDownList;
        _material.Width = 480;
        _material.DisplayMember = "Name";
        _material.ValueMember = "Id";
        _material.DataSource = _catalog.Select(m => new { m.Id, Name = $"{m.Code} — {m.Name} ({m.Unit})", m.CurrentStock }).ToList();

        _qty.Width = 110;
        _qty.DecimalPlaces = 2;
        _qty.Maximum = 999999;
        _qty.Minimum = 0.01m;
        _qty.Value = 1;

        var add = UiKit.Secondary("Add line");
        add.Click += (_, _) => AddLine();

        var remove = UiKit.Secondary("Remove selected");
        remove.ForeColor = Theme.Danger;
        remove.FlatAppearance.BorderColor = Theme.Danger;
        remove.Click += (_, _) => RemoveLine();

        var addBar = UiKit.FieldRow(("Material", _material), ("Qty", _qty), ("", add), ("", remove));
        addBar.BackColor = Theme.Surface;
        addBar.Padding = new Padding(28, 8, 0, 6);

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
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 116, BackColor = Color.White, Padding = new Padding(28, 12, 28, 14) };

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
            Width = 200,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly
        };
        submit.FlatAppearance.BorderSize = 0;
        submit.Click += (_, _) => Save(submitNow: true);

        var saveDraft = UiKit.Secondary("Save as draft");
        saveDraft.Click += (_, _) => Save(submitNow: false);

        var cancel = UiKit.Secondary("Cancel");
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
