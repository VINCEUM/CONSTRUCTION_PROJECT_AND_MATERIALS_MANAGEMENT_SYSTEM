using System.ComponentModel;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>Editable line: what the storekeeper is actually releasing.</summary>
public sealed class IssueEditRow
{
    public int RequestItemId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal QtyRequested { get; set; }
    public decimal QtyAlreadyIssued { get; set; }
    public decimal QtyOutstanding { get; set; }
    public decimal OnHand { get; set; }
    public decimal UnitCost { get; set; }
    public decimal QtyToIssue { get; set; }
    public decimal LineCost => QtyToIssue * UnitCost;
}

/// <summary>
/// Release materials against an approved request. The layout lives in
/// IssueForm.Designer.cs; the editable line grid and the stock rules are here.
/// </summary>
public partial class IssueForm : Form
{
    private MaterialRequest _request = null!;
    private BindingList<IssueEditRow> _rows = new();

    public int? IssueId { get; private set; }

    /// <summary>Parameterless constructor so the form opens in the designer.</summary>
    public IssueForm()
    {
        InitializeComponent();
        Theme.Style(gridLines);
    }

    public IssueForm(MaterialRequest request) : this()
    {
        _request = request;
        Text = $"Issue materials — {request.RequestNo}";
        lblTitle.Text = $"{request.RequestNo}  ·  {request.ProjectName}";
        lblSub.Text = $"Requested by {request.RequesterName} on {request.RequestDate:d}  ·  status {request.Status}";
        dtDate.Value = DateTime.Today;

        var items = new CatalogService().GetRequestItems(request.Id);
        _rows = new BindingList<IssueEditRow>(items.Select(i => new IssueEditRow
        {
            RequestItemId = i.Id,
            MaterialId = i.MaterialId,
            MaterialCode = i.MaterialCode,
            MaterialName = i.MaterialName,
            Unit = i.Unit,
            QtyRequested = i.QtyRequested,
            QtyAlreadyIssued = i.QtyIssued,
            QtyOutstanding = i.QtyOutstanding,
            OnHand = i.CurrentStock,
            UnitCost = i.LastUnitCost,
            // pre-fill with what can actually be released today
            QtyToIssue = Math.Max(0, Math.Min(i.QtyOutstanding, i.CurrentStock))
        }).ToList());

        UiKit.Bind(gridLines, _rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyRequested", "REQUESTED", "N2"),
            ("QtyAlreadyIssued", "ISSUED", "N2"),
            ("QtyOutstanding", "OUTSTANDING", "N2"),
            ("OnHand", "ON HAND", "N2"),
            ("UnitCost", "UNIT COST", "N2"),
            ("QtyToIssue", "ISSUE NOW", "N2"),
            ("LineCost", "LINE COST", "N2"));

        var minWidths = new Dictionary<string, int>
        {
            ["MaterialCode"] = 80, ["MaterialName"] = 180, ["Unit"] = 65,
            ["QtyRequested"] = 100, ["QtyAlreadyIssued"] = 80, ["QtyOutstanding"] = 125,
            ["OnHand"] = 95, ["UnitCost"] = 105, ["QtyToIssue"] = 105, ["LineCost"] = 105
        };
        foreach (var (name, width) in minWidths) gridLines.Columns[name]!.MinimumWidth = width;

        gridLines.ReadOnly = false;
        foreach (DataGridViewColumn c in gridLines.Columns)
            c.ReadOnly = c.Name != "QtyToIssue";
        gridLines.Columns["QtyToIssue"]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
        gridLines.Columns["QtyToIssue"]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
        gridLines.EditMode = DataGridViewEditMode.EditOnEnter;
        gridLines.CellValueChanged += (_, _) => Recalculate();
        gridLines.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (gridLines.IsCurrentCellDirty) gridLines.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        gridLines.CellFormatting += Warn;
        gridLines.DataError += (_, e) => { e.Cancel = true; ShowError("Enter a number."); };

        Recalculate();
    }

    private void btnCancel_Click(object? sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }

    private void Warn(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
        var row = _rows[e.RowIndex];
        var column = gridLines.Columns[e.ColumnIndex].Name;

        if (column == "QtyToIssue" && row.QtyToIssue > row.OnHand)
            e.CellStyle!.ForeColor = Theme.Danger;
        if (column == "OnHand" && row.OnHand < row.QtyOutstanding)
            e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void Recalculate()
    {
        var total = _rows.Sum(r => r.LineCost);
        var lines = _rows.Count(r => r.QtyToIssue > 0);
        lblTotal.Text = $"{lines} line(s) to issue  ·  ₱{total:N2}";
        gridLines.Refresh();
    }

    private void ShowError(string message) => lblError.Text = message;

    private void btnPost_Click(object? sender, EventArgs e)
    {
        ShowError("");

        var lines = _rows.Where(r => r.QtyToIssue > 0)
                         .Select(r => new IssueLine
                         {
                             MaterialId = r.MaterialId,
                             RequestItemId = r.RequestItemId,
                             Qty = r.QtyToIssue,
                             UnitCost = r.UnitCost
                         })
                         .ToList();

        if (lines.Count == 0) { ShowError("Set a quantity on at least one line."); return; }
        if (string.IsNullOrWhiteSpace(txtReceivedBy.Text)) { ShowError("Record who received the materials on site."); txtReceivedBy.Focus(); return; }

        var over = _rows.FirstOrDefault(r => r.QtyToIssue > r.QtyOutstanding);
        if (over is not null)
        {
            ShowError($"{over.MaterialName}: cannot issue more than the {over.QtyOutstanding:N2} still outstanding.");
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;
            IssueId = new InventoryService().IssueMaterials(
                _request.Id, dtDate.Value.Date, AppSession.Require.Id, txtReceivedBy.Text.Trim(), lines);

            MessageBox.Show(this,
                $"Issued {lines.Count} line(s) to {_request.ProjectName}.\n\n" +
                "Stock, the ledger and the project's material cost have all been updated.",
                "Issuance posted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (InsufficientStockException ex)
        {
            // the service refused the whole issuance — nothing was written
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            ShowError("Could not post: " + ex.Message);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
