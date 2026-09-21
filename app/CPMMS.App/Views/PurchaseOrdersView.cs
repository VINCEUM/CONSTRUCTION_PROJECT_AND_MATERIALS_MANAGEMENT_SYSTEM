using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class PurchaseOrdersView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly DataGridView _orders = UiKit.Grid();
    private readonly DataGridView _items = UiKit.Grid();
    private readonly CheckBox _openOnly = new();
    private readonly Button _receive;

    public PurchaseOrdersView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Surface };

        _openOnly.Text = "Open orders only";
        _openOnly.Font = Theme.Body;
        _openOnly.ForeColor = Theme.InkSoft;
        _openOnly.Checked = true;
        _openOnly.Location = new Point(0, 11);
        _openOnly.AutoSize = true;
        _openOnly.CheckedChanged += (_, _) => Reload();

        _receive = UiKit.Secondary("Receive delivery…");
        _receive.Location = new Point(160, 7);
        _receive.Click += (_, _) => Receive();

        bar.Controls.Add(_receive);
        bar.Controls.Add(_openOnly);

        var detail = new Panel { Dock = DockStyle.Bottom, Height = 230, BackColor = Theme.Surface, Padding = new Padding(0, 8, 0, 0) };
        var detailHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        detailHost.Controls.Add(_items);
        detail.Controls.Add(detailHost);
        detail.Controls.Add(UiKit.SectionTitle("Ordered items"));

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _orders.SelectionChanged += (_, _) => LoadItems();
        _orders.CellFormatting += FormatOrder;
        _orders.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Receive(); };
        _items.CellFormatting += FormatItem;
        host.Controls.Add(_orders);

        Controls.Add(host);
        Controls.Add(detail);
        Controls.Add(bar);

        Reload();
    }

    private void Reload()
    {
        var rows = _catalog.GetPurchaseOrders(_openOnly.Checked);
        UiKit.Bind(_orders, rows,
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
        if (_orders.CurrentRow?.DataBoundItem is not PurchaseOrderRow po)
        {
            _items.DataSource = null;
            _receive.Enabled = false;
            return;
        }

        _receive.Enabled = po.Status is "draft" or "sent" or "partially_received";

        var rows = _catalog.GetPurchaseOrderItems(po.Id);
        UiKit.Bind(_items, rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyOrdered", "ORDERED", "N2"),
            ("QtyReceived", "RECEIVED", "N2"),
            ("QtyOutstanding", "OUTSTANDING", "N2"),
            ("UnitPrice", "UNIT PRICE", "N2"));
    }

    private void FormatOrder(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _orders.Rows[e.RowIndex].DataBoundItem is not PurchaseOrderRow row) return;
        if (_orders.Columns[e.ColumnIndex].Name != "Status") return;

        e.CellStyle!.ForeColor = row.Status switch
        {
            "received" => Theme.Good,
            "partially_received" => Theme.Warn,
            "sent" => Theme.Accent,
            "cancelled" => Theme.Danger,
            _ => Theme.InkSoft
        };
    }

    private void FormatItem(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _items.Rows[e.RowIndex].DataBoundItem is not PurchaseOrderItemRow item) return;
        if (_items.Columns[e.ColumnIndex].Name == "QtyOutstanding" && item.QtyOutstanding > 0)
            e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void Receive()
    {
        if (_orders.CurrentRow?.DataBoundItem is not PurchaseOrderRow po) return;
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
