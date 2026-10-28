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
    private readonly TextBox _photo = new();
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
        ClientSize = UiKit.FitSize(1200, 720);
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
        _drNo.Width = 280;
        _drNo.PlaceholderText = "Supplier's receipt no.";
        _date.Format = DateTimePickerFormat.Short;
        _date.Width = 180;
        _date.Value = DateTime.Today;

        // proof of delivery (a photo of the signed receipt or the load) and who received it
        _photo.Width = 420;
        _photo.ReadOnly = true;
        _photo.PlaceholderText = "optional — attach a photo (JPG or PNG)";
        var browse = UiKit.Secondary("Browse…");
        browse.Click += (_, _) => BrowsePhoto();
        var recipient = new Label { Text = AppSession.Require.FullName };

        var header = UiKit.HeaderPanel(
            UiKit.FormTitle($"{po.PoNo}  ·  {po.SupplierName}"),
            UiKit.FormSubtitle($"Ordered {po.OrderDate:d}  ·  status {po.Status}  ·  {po.OutstandingLines} line(s) still outstanding"),
            UiKit.FieldRow(("DR number", _drNo), ("Delivery date", _date)),
            UiKit.FieldRow(("Delivery photo", _photo), ("", browse), ("Received by", recipient)));

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
            ["MaterialCode"] = 105, ["MaterialName"] = 200, ["Unit"] = 65,
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
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 116, BackColor = Color.White, Padding = new Padding(28, 12, 28, 14) };

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

        string? photoPath = null;
        try
        {
            Cursor = Cursors.WaitCursor;
            photoPath = SavePhoto();
            new InventoryService().ReceiveDelivery(
                _po.Id, _drNo.Text.Trim(), _date.Value.Date, AppSession.Require.Id, photoPath, lines);

            MessageBox.Show(this,
                $"Received {lines.Count} line(s) against {_po.PoNo}.\n\n" +
                "Stock and the ledger have been updated. The purchase order stays open if anything is still outstanding.",
                "Delivery posted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            // the delivery was not posted, so don't leave its photo lying around
            if (photoPath is not null)
            {
                try { File.Delete(Path.Combine(AppContext.BaseDirectory, photoPath)); } catch { /* best effort */ }
            }
            _error.Text = "Could not post: " + ex.Message;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void BrowsePhoto()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Choose a photo of the delivery",
            Filter = "Photos (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
        };
        if (dialog.ShowDialog(this) == DialogResult.OK) _photo.Text = dialog.FileName;
    }

    /// <summary>Copies the chosen photo into the app's uploads folder and returns its relative path (or null if none).</summary>
    private string? SavePhoto()
    {
        if (string.IsNullOrWhiteSpace(_photo.Text)) return null;

        var source = _photo.Text;
        if (!File.Exists(source)) throw new InvalidOperationException("The chosen photo can't be found any more.");

        var extension = Path.GetExtension(source).ToLowerInvariant();
        if (extension is not (".jpg" or ".jpeg" or ".png"))
            throw new InvalidOperationException("The photo must be a JPG or PNG file.");
        if (new FileInfo(source).Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("The photo is larger than 10 MB. Choose a smaller one.");

        var safeDr = string.Concat(_drNo.Text.Trim().Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        var fileName = $"{_po.PoNo}_{safeDr}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
        var relative = Path.Combine("uploads", "deliveries", fileName);

        var folder = Path.Combine(AppContext.BaseDirectory, "uploads", "deliveries");
        Directory.CreateDirectory(folder);
        File.Copy(source, Path.Combine(AppContext.BaseDirectory, relative), overwrite: false);
        return relative.Replace('\\', '/');
    }
}
