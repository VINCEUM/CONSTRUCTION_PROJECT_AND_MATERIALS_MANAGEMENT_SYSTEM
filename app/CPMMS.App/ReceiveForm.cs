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

public sealed class ReceiveForm : Form
{
    private readonly PurchaseOrderRow _po;
    private readonly BindingList<ReceiveEditRow> _rows;
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly TextBox _drNo = new();
    private readonly DateTimePicker _date = new();
    private readonly Label _total = new();
    private readonly Label _error = new();

    public ReceiveForm(PurchaseOrderRow po)
    {
        _po = po;

        // this form is built entirely in code (no Designer baseline), so let
        // Windows' own per-monitor DPI scaling handle it — WinForms' separate
        // font-ratio auto-scale would otherwise double up and misalign things
        AutoScaleMode = AutoScaleMode.None;

        Text = $"Receive delivery — {po.PoNo}";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(1020, 540);
        BackColor = Theme.Surface;
        Font = Theme.Body;

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

        // ---- header ---------------------------------------------------------
        var header = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Color.White, Padding = new Padding(18, 12, 18, 8) };
        var title = new Label
        {
            Text = $"{po.PoNo}  ·  {po.SupplierName}",
            Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top, Height = 30, AutoSize = false
        };
        var sub = new Label
        {
            Text = $"Ordered {po.OrderDate:d}  ·  status {po.Status}  ·  {po.OutstandingLines} line(s) still outstanding",
            ForeColor = Theme.Muted, Dock = DockStyle.Top, Height = 20, AutoSize = false
        };

        var fields = new Panel { Dock = DockStyle.Top, Height = 34 };
        // positions are computed from each label's own measured width (not a
        // guessed pixel number), so a label can never overlap the control after it
        var lblDr = new Label { Text = "DR number", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _drNo.Location = new Point(lblDr.Right + 12, 4);
        _drNo.Width = 220;
        _drNo.BorderStyle = BorderStyle.FixedSingle;
        _drNo.PlaceholderText = "Supplier's receipt no.";

        var lblDate = new Label { Text = "Delivery date", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(_drNo.Right + 24, 8), AutoSize = true };
        _date.Format = DateTimePickerFormat.Short;
        _date.Location = new Point(lblDate.Right + 12, 4);
        _date.Width = 160;
        _date.Value = DateTime.Today;

        fields.Controls.AddRange(new Control[] { lblDr, _drNo, lblDate, _date });

        header.Controls.Add(fields);
        header.Controls.Add(sub);
        header.Controls.Add(title);

        // ---- grid -----------------------------------------------------------
        UiKit.Bind(_grid, _rows,
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
        foreach (var (name, width) in minWidths) _grid.Columns[name]!.MinimumWidth = width;

        _grid.ReadOnly = false;
        foreach (DataGridViewColumn c in _grid.Columns)
            c.ReadOnly = c.Name is not ("QtyReceivedNow" or "UnitPrice");
        foreach (var name in new[] { "QtyReceivedNow", "UnitPrice" })
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
        _grid.DataError += (_, e) => { e.Cancel = true; _error.Text = "Enter a number."; };

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_grid);

        // ---- footer ---------------------------------------------------------
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 92, BackColor = Color.White, Padding = new Padding(18, 10, 18, 10) };

        _error.ForeColor = Theme.Danger; _error.Dock = DockStyle.Top; _error.Height = 20; _error.AutoSize = false;
        _total.Font = Theme.H2; _total.ForeColor = Theme.Ink; _total.Dock = DockStyle.Top; _total.Height = 26; _total.AutoSize = false;

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 36, FlowDirection = FlowDirection.RightToLeft };
        var post = new Button
        {
            Text = "Post delivery",
            BackColor = Theme.Accent, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 9.75F),
            Height = 32, Width = 130,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly
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

    private void Warn(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
        var row = _rows[e.RowIndex];
        if (_grid.Columns[e.ColumnIndex].Name != "QtyReceivedNow") return;

        // over-delivery is allowed but worth flagging
        if (row.QtyReceivedNow > row.QtyOutstanding) e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void Recalculate()
    {
        var total = _rows.Sum(r => r.LineTotal);
        var lines = _rows.Count(r => r.QtyReceivedNow > 0);
        var over = _rows.Count(r => r.QtyReceivedNow > r.QtyOutstanding);
        _total.Text = $"{lines} line(s)  ·  ₱{total:N2}" + (over > 0 ? $"  ·  {over} line(s) over the ordered quantity" : "");
        _grid.Refresh();
    }

    private void Post(object? sender, EventArgs e)
    {
        _error.Text = "";

        var lines = _rows.Where(r => r.QtyReceivedNow > 0)
                         .Select(r => new ReceiveLine
                         {
                             PoItemId = r.PoItemId,
                             MaterialId = r.MaterialId,
                             Qty = r.QtyReceivedNow,
                             UnitPrice = r.UnitPrice
                         })
                         .ToList();

        if (lines.Count == 0) { _error.Text = "Set a quantity on at least one line."; return; }
        if (string.IsNullOrWhiteSpace(_drNo.Text)) { _error.Text = "Enter the supplier's DR number."; _drNo.Focus(); return; }

        try
        {
            Cursor = Cursors.WaitCursor;
            new InventoryService().ReceiveDelivery(
                _po.Id, _drNo.Text.Trim(), _date.Value.Date, AppSession.Require.Id, null, lines);

            MessageBox.Show(this,
                $"Received {lines.Count} line(s) against {_po.PoNo}.\n\n" +
                "Stock and the ledger have been updated. The purchase order stays open if anything is still outstanding.",
                "Delivery posted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _error.Text = "Could not post: " + ex.Message;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
