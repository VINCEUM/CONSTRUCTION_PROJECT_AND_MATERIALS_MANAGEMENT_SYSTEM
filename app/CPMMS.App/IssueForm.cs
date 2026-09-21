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

public sealed class IssueForm : Form
{
    private readonly MaterialRequest _request;
    private readonly BindingList<IssueEditRow> _rows;
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly TextBox _receivedBy = new();
    private readonly DateTimePicker _date = new();
    private readonly Label _total = new();
    private readonly Label _error = new();

    public int? IssueId { get; private set; }

    public IssueForm(MaterialRequest request)
    {
        _request = request;

        Text = $"Issue materials — {request.RequestNo}";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(940, 560);
        BackColor = Theme.Surface;
        Font = Theme.Body;

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

        // ---- header ---------------------------------------------------------
        var header = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Color.White, Padding = new Padding(18, 12, 18, 8) };
        var title = new Label
        {
            Text = $"{request.RequestNo}  ·  {request.ProjectName}",
            Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top, Height = 30, AutoSize = false
        };
        var sub = new Label
        {
            Text = $"Requested by {request.RequesterName} on {request.RequestDate:d}  ·  status {request.Status}",
            ForeColor = Theme.Muted, Dock = DockStyle.Top, Height = 20, AutoSize = false
        };
        var fields = new Panel { Dock = DockStyle.Top, Height = 34 };

        var lblDate = new Label { Text = "Issue date", ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _date.Format = DateTimePickerFormat.Short;
        _date.Location = new Point(70, 4);
        _date.Width = 120;
        _date.Value = DateTime.Today;

        var lblRecv = new Label { Text = "Received by", ForeColor = Theme.InkSoft, Location = new Point(210, 8), AutoSize = true };
        _receivedBy.Location = new Point(288, 4);
        _receivedBy.Width = 260;
        _receivedBy.BorderStyle = BorderStyle.FixedSingle;
        _receivedBy.PlaceholderText = "name of the person on site";

        fields.Controls.AddRange(new Control[] { lblDate, _date, lblRecv, _receivedBy });

        header.Controls.Add(fields);
        header.Controls.Add(sub);
        header.Controls.Add(title);

        // ---- grid -----------------------------------------------------------
        UiKit.Bind(_grid, _rows,
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

        _grid.ReadOnly = false;
        foreach (DataGridViewColumn c in _grid.Columns)
            c.ReadOnly = c.Name != "QtyToIssue";
        _grid.Columns["QtyToIssue"]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
        _grid.Columns["QtyToIssue"]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
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

        // ---- footer ---------------------------------------------------------
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 92, BackColor = Color.White, Padding = new Padding(18, 10, 18, 10) };

        _error.ForeColor = Theme.Danger;
        _error.Dock = DockStyle.Top;
        _error.Height = 20;
        _error.AutoSize = false;

        _total.Font = Theme.H2;
        _total.ForeColor = Theme.Ink;
        _total.Dock = DockStyle.Top;
        _total.Height = 26;
        _total.AutoSize = false;

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 36, FlowDirection = FlowDirection.RightToLeft };
        var post = new Button
        {
            Text = "Post issuance",
            BackColor = Theme.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9.75F),
            Height = 32,
            Width = 130
        };
        post.FlatAppearance.BorderSize = 0;
        post.Click += Post;

        var cancel = UiKit.Secondary("Cancel");
        cancel.Width = 90;
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
        var column = _grid.Columns[e.ColumnIndex].Name;

        if (column == "QtyToIssue" && row.QtyToIssue > row.OnHand)
            e.CellStyle!.ForeColor = Theme.Danger;
        if (column == "OnHand" && row.OnHand < row.QtyOutstanding)
            e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void Recalculate()
    {
        var total = _rows.Sum(r => r.LineCost);
        var lines = _rows.Count(r => r.QtyToIssue > 0);
        _total.Text = $"{lines} line(s) to issue  ·  ₱{total:N2}";
        _grid.Refresh();
    }

    private void ShowError(string message) => _error.Text = message;

    private void Post(object? sender, EventArgs e)
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
        if (string.IsNullOrWhiteSpace(_receivedBy.Text)) { ShowError("Record who received the materials on site."); _receivedBy.Focus(); return; }

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
                _request.Id, _date.Value.Date, AppSession.Require.Id, _receivedBy.Text.Trim(), lines);

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
