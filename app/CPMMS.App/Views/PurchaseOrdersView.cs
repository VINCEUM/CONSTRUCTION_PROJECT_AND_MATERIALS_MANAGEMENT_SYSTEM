using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Purchase orders: a master list with its ordered items below, and a Receive
/// action. Layout is in the designer; the data and actions are here.
/// </summary>
public partial class PurchaseOrdersView : UserControl
{
    private readonly CatalogService _catalog = new();

    public PurchaseOrdersView()
    {
        InitializeComponent();
        Theme.Style(gridOrders);
        Theme.Style(gridItems);
        Reload();
    }

    private void chkOpenOnly_CheckedChanged(object? sender, EventArgs e) => Reload();
    private void btnReceive_Click(object? sender, EventArgs e) => Receive();
    private void gridOrders_SelectionChanged(object? sender, EventArgs e) => LoadItems();
    private void gridOrders_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) Receive(); }

    private void Reload()
    {
        var rows = _catalog.GetPurchaseOrders(chkOpenOnly.Checked);
        UiKit.Bind(gridOrders, rows,
            ("PoNo", "PO NO.", null),
            ("SupplierName", "SUPPLIER", null),
            ("OrderDate", "ORDERED", "d"),
            ("ExpectedDate", "EXPECTED", "d"),
            ("Status", "STATUS", null),
            ("LineCount", "LINES", null),
            ("OutstandingLines", "OUTSTANDING", null),
            ("TotalAmount", "TOTAL", "N2"));
        LoadItems();
    }

    private void LoadItems()
    {
        if (gridOrders.CurrentRow?.DataBoundItem is not PurchaseOrderRow po)
        {
            gridItems.DataSource = null;
            btnReceive.Enabled = false;
            return;
        }

        btnReceive.Enabled = po.Status is "draft" or "sent" or "partially_received";

        var rows = _catalog.GetPurchaseOrderItems(po.Id);
        UiKit.Bind(gridItems, rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyOrdered", "ORDERED", "N2"),
            ("QtyReceived", "RECEIVED", "N2"),
            ("QtyOutstanding", "OUTSTANDING", "N2"),
            ("UnitPrice", "UNIT PRICE", "N2"));
    }

    private void gridOrders_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || gridOrders.Rows[e.RowIndex].DataBoundItem is not PurchaseOrderRow row) return;
        if (gridOrders.Columns[e.ColumnIndex].Name != "Status") return;

        e.CellStyle!.ForeColor = row.Status switch
        {
            "received" => Theme.Good,
            "partially_received" => Theme.Warn,
            "sent" => Theme.Accent,
            "cancelled" => Theme.Danger,
            _ => Theme.InkSoft
        };
    }

    private void gridItems_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || gridItems.Rows[e.RowIndex].DataBoundItem is not PurchaseOrderItemRow item) return;
        if (gridItems.Columns[e.ColumnIndex].Name == "QtyOutstanding" && item.QtyOutstanding > 0)
            e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void Receive()
    {
        if (gridOrders.CurrentRow?.DataBoundItem is not PurchaseOrderRow po) return;
        if (po.Status is "received" or "closed" or "cancelled")
        {
            MessageBox.Show(this, $"This order is '{po.Status}' and cannot receive goods.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var form = new ReceiveForm(po);
        if (form.ShowDialog(FindForm()) == DialogResult.OK) Reload();
    }
}
