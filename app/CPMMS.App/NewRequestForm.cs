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
/// save it as a draft or submit it. The layout lives in the designer; the data
/// (projects, materials, the line grid) and the save go through RequestService.
/// </summary>
public partial class NewRequestForm : Form
{
    private readonly RequestService _requests = new();
    private readonly BindingList<NewRequestLine> _lines = new();
    private IReadOnlyList<Material> _catalog = new List<Material>();

    public NewRequestForm()
    {
        InitializeComponent();
        Theme.Style(grid);

        var projects = _requests.GetRequestableProjects(AppSession.Require);
        _catalog = _requests.GetRequestableMaterials();

        cmbProject.DisplayMember = "Name";
        cmbProject.ValueMember = "Id";
        cmbProject.DataSource = projects;

        dtNeeded.Value = DateTime.Today.AddDays(3);

        cmbMaterial.DisplayMember = "Name";
        cmbMaterial.ValueMember = "Id";
        cmbMaterial.DataSource = _catalog
            .Select(m => new { m.Id, Name = $"{m.Code} — {m.Name} ({m.Unit})", m.CurrentStock })
            .ToList();

        UiKit.Bind(grid, _lines,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("OnHand", "ON HAND", "N2"),
            ("Qty", "QTY REQUESTED", "N2"));
        grid.ReadOnly = true;
    }

    private void btnAdd_Click(object? sender, EventArgs e) => AddLine();
    private void btnRemove_Click(object? sender, EventArgs e) => RemoveLine();
    private void btnSubmit_Click(object? sender, EventArgs e) => Save(submitNow: true);
    private void btnSaveDraft_Click(object? sender, EventArgs e) => Save(submitNow: false);
    private void btnCancel_Click(object? sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }

    private void AddLine()
    {
        ShowError("");
        if (cmbMaterial.SelectedValue is not int materialId) return;

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
            Qty = numQty.Value
        });
    }

    private void RemoveLine()
    {
        if (grid.CurrentRow?.DataBoundItem is NewRequestLine line) _lines.Remove(line);
    }

    private void ShowError(string message) => lblError.Text = message;

    private void Save(bool submitNow)
    {
        ShowError("");

        if (cmbProject.SelectedValue is not int projectId)
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
                AppSession.Require, projectId, dtNeeded.Value.Date,
                txtRemarks.Text, lines);

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
