using System.ComponentModel;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>Editable line: what actually arrived on the truck.</summary>
public sealed class ReceiveEditRow
{
    public int PoItemId { get; set; }
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal QtyOrdered { get; set; }
    public decimal QtyAlreadyReceived { get; set; }
    public decimal QtyOutstanding { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal QtyReceivedNow { get; set; }
    public decimal LineTotal => QtyReceivedNow * UnitPrice;
}

/// <summary>
/// Receive a delivery against a purchase order. The layout lives in
/// ReceiveForm.Designer.cs; the editable line grid and posting are here.
/// </summary>
public partial class ReceiveForm : Form
{
    private PurchaseOrderRow _po = null!;
    private BindingList<ReceiveEditRow> _rows = new();

    /// <summary>Parameterless constructor so the form opens in the designer.</summary>
    public ReceiveForm()
    {
        InitializeComponent();
        Theme.Style(gridLines);
    }

    public ReceiveForm(PurchaseOrderRow po) : this()
    {
        _po = po;
        Text = $"Receive delivery — {po.PoNo}";
        lblTitle.Text = $"{po.PoNo}  ·  {po.SupplierName}";
        lblSub.Text = $"Ordered {po.OrderDate:d}  ·  status {po.Status}  ·  {po.OutstandingLines} line(s) still outstanding";
        dtDate.Value = DateTime.Today;

        var items = new CatalogService().GetPurchaseOrderItems(po.Id);
        _rows = new BindingList<ReceiveEditRow>(items.Select(i => new ReceiveEditRow
        {
            PoItemId = i.Id,
            MaterialId = i.MaterialId,
            MaterialCode = i.MaterialCode,
            MaterialName = i.MaterialName,
            Unit = i.Unit,
            QtyOrdered = i.QtyOrdered,
            QtyAlreadyReceived = i.QtyReceived,
            QtyOutstanding = i.QtyOutstanding,
            UnitPrice = i.UnitPrice,
            QtyReceivedNow = i.QtyOutstanding      // assume the full balance arrived; edit down if not
        }).ToList());

        UiKit.Bind(gridLines, _rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyOrdered", "ORDERED", "N2"),
            ("QtyAlreadyReceived", "RECEIVED", "N2"),
            ("QtyOutstanding", "OUTSTANDING", "N2"),
            ("UnitPrice", "UNIT PRICE", "N2"),
            ("QtyReceivedNow", "RECEIVING NOW", "N2"),
            ("LineTotal", "LINE TOTAL", "N2"));

        var minWidths = new Dictionary<string, int>
        {
            ["MaterialCode"] = 80, ["MaterialName"] = 180, ["Unit"] = 65,
            ["QtyOrdered"] = 95, ["QtyAlreadyReceived"] = 95, ["QtyOutstanding"] = 125,
            ["UnitPrice"] = 105, ["QtyReceivedNow"] = 125, ["LineTotal"] = 115
        };
        foreach (var (name, width) in minWidths) gridLines.Columns[name]!.MinimumWidth = width;

        gridLines.ReadOnly = false;
        foreach (DataGridViewColumn c in gridLines.Columns)
            c.ReadOnly = c.Name is not ("QtyReceivedNow" or "UnitPrice");
        foreach (var name in new[] { "QtyReceivedNow", "UnitPrice" })
        {
            gridLines.Columns[name]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
            gridLines.Columns[name]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
        }
        gridLines.EditMode = DataGridViewEditMode.EditOnEnter;
        gridLines.CellValueChanged += (_, _) => Recalculate();
        gridLines.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (gridLines.IsCurrentCellDirty) gridLines.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };
        gridLines.CellFormatting += Warn;
        gridLines.DataError += (_, e) => { e.Cancel = true; lblError.Text = "Enter a number."; };

        Recalculate();
    }

    private void btnCancel_Click(object? sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }

    private void Warn(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
        var row = _rows[e.RowIndex];
        if (gridLines.Columns[e.ColumnIndex].Name != "QtyReceivedNow") return;

        // over-delivery is allowed but worth flagging
        if (row.QtyReceivedNow > row.QtyOutstanding) e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void Recalculate()
    {
        var total = _rows.Sum(r => r.LineTotal);
        var lines = _rows.Count(r => r.QtyReceivedNow > 0);
        var over = _rows.Count(r => r.QtyReceivedNow > r.QtyOutstanding);
        lblTotal.Text = $"{lines} line(s)  ·  ₱{total:N2}" + (over > 0 ? $"  ·  {over} line(s) over the ordered quantity" : "");
        gridLines.Refresh();
    }

    private void btnPost_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";

        var lines = _rows.Where(r => r.QtyReceivedNow > 0)
                         .Select(r => new ReceiveLine
                         {
                             PoItemId = r.PoItemId,
                             MaterialId = r.MaterialId,
                             Qty = r.QtyReceivedNow,
                             UnitPrice = r.UnitPrice
                         })
                         .ToList();

        if (lines.Count == 0) { lblError.Text = "Set a quantity on at least one line."; return; }
        if (string.IsNullOrWhiteSpace(txtDrNo.Text)) { lblError.Text = "Enter the supplier's DR number."; txtDrNo.Focus(); return; }

        try
        {
            Cursor = Cursors.WaitCursor;
            new InventoryService().ReceiveDelivery(
                _po.Id, txtDrNo.Text.Trim(), dtDate.Value.Date, AppSession.Require.Id, null, lines);

            MessageBox.Show(this,
                $"Received {lines.Count} line(s) against {_po.PoNo}.\n\n" +
                "Stock and the ledger have been updated. The purchase order stays open if anything is still outstanding.",
                "Delivery posted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Could not post: " + ex.Message;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
