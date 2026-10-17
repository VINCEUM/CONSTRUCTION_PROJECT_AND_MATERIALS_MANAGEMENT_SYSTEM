using System.ComponentModel;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>One line being built up before the purchase order is saved.</summary>
public sealed class PoEditRow
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal QtyRequested { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Qty * UnitPrice;
}

/// <summary>
/// Lets an admin turn an approved material request into a purchase order:
/// pick the request, pick a supplier, adjust quantities and prices, send it.
/// All the rules (approved-only, can't exceed what was requested, one PO per
/// request) live in PurchaseOrderService — this form just collects the input.
/// </summary>
public sealed class NewPurchaseOrderForm : Form
{
    private readonly PurchaseOrderService _orders = new();
    private readonly BindingList<PoEditRow> _rows = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly ComboBox _request = new();
    private readonly ComboBox _supplier = new();
    private readonly DateTimePicker _orderDate = new();
    private readonly DateTimePicker _expectedDate = new();
    private readonly TextBox _remarks = new();
    private readonly Label _total = new();
    private readonly Label _error = new();

    private readonly IReadOnlyList<MaterialRequest> _orderable;

    public NewPurchaseOrderForm()
    {
        // this form is built entirely in code (no Designer baseline), so let
        // Windows' own per-monitor DPI scaling handle it — WinForms' separate
        // font-ratio auto-scale would otherwise double up and misalign things
        AutoScaleMode = AutoScaleMode.None;

        Text = "New purchase order";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(980, 580);
        BackColor = Theme.Surface;
        Font = Theme.Body;

        _orderable = _orders.GetOrderableRequests();
        var suppliers = _orders.GetSuppliers();

        // ---- header -----------------------------------------------------
        var header = new Panel { Dock = DockStyle.Top, Height = 162, BackColor = Color.White, Padding = new Padding(18, 12, 18, 8) };
        var title = new Label { Text = "New purchase order", Font = Theme.H1, ForeColor = Theme.Ink, Dock = DockStyle.Top, Height = 30, AutoSize = false };

        // positions are computed from each label's own measured width (not a
        // guessed pixel number), so a label can never overlap the control after it
        var row1 = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblRequest = new Label { Text = "Request", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _request.DropDownStyle = ComboBoxStyle.DropDownList;
        _request.Location = new Point(lblRequest.Right + 12, 4);
        _request.Width = 420;
        _request.DisplayMember = "Name";
        _request.ValueMember = "Id";
        _request.DataSource = _orderable.Select(r => new { r.Id, Name = $"{r.RequestNo} — {r.ProjectName}" }).ToList();
        _request.SelectedIndexChanged += (_, _) => LoadRequestItems();

        var lblSupplier = new Label { Text = "Supplier", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(_request.Right + 24, 8), AutoSize = true };
        _supplier.DropDownStyle = ComboBoxStyle.DropDownList;
        _supplier.Location = new Point(lblSupplier.Right + 12, 4);
        _supplier.Width = 260;
        _supplier.DisplayMember = "CompanyName";
        _supplier.ValueMember = "Id";
        _supplier.DataSource = suppliers;

        row1.Controls.AddRange(new Control[] { lblRequest, _request, lblSupplier, _supplier });

        var row2 = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblOrder = new Label { Text = "Order date", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _orderDate.Format = DateTimePickerFormat.Short;
        _orderDate.Location = new Point(lblOrder.Right + 12, 4);
        _orderDate.Width = 160;
        _orderDate.Value = DateTime.Today;

        var lblExpected = new Label { Text = "Expected delivery", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(_orderDate.Right + 24, 8), AutoSize = true };
        _expectedDate.Format = DateTimePickerFormat.Short;
        _expectedDate.Location = new Point(lblExpected.Right + 12, 4);
        _expectedDate.Width = 160;
        _expectedDate.Value = DateTime.Today.AddDays(7);

        row2.Controls.AddRange(new Control[] { lblOrder, _orderDate, lblExpected, _expectedDate });

        var remarksRow = new Panel { Dock = DockStyle.Top, Height = 34 };
        var lblRemarks = new Label { Text = "Remarks", Font = Theme.Body, ForeColor = Theme.InkSoft, Location = new Point(0, 8), AutoSize = true };
        _remarks.Location = new Point(lblRemarks.Right + 12, 4);
        _remarks.Width = 538;
        _remarks.BorderStyle = BorderStyle.FixedSingle;
        _remarks.PlaceholderText = "optional";
        remarksRow.Controls.AddRange(new Control[] { lblRemarks, _remarks });

        header.Controls.Add(remarksRow);
        header.Controls.Add(row2);
        header.Controls.Add(row1);
        header.Controls.Add(title);

        // ---- grid -------------------------------------------------------
        UiKit.Bind(_grid, _rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyRequested", "REQUESTED", "N2"),
            ("Qty", "QTY TO ORDER", "N2"),
            ("UnitPrice", "UNIT PRICE", "N2"),
            ("LineTotal", "LINE TOTAL", "N2"));

        // each column gets enough room for its own header — Fill mode then
        // shares out whatever space is left over
        var minWidths = new Dictionary<string, int>
        {
            ["MaterialCode"] = 80, ["MaterialName"] = 190, ["Unit"] = 65,
            ["QtyRequested"] = 100, ["Qty"] = 110, ["UnitPrice"] = 105, ["LineTotal"] = 115
        };
        foreach (var (name, width) in minWidths) _grid.Columns[name]!.MinimumWidth = width;

        _grid.ReadOnly = false;
        foreach (DataGridViewColumn c in _grid.Columns)
            c.ReadOnly = c.Name is not ("Qty" or "UnitPrice");
        _grid.Columns["Qty"]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
        _grid.Columns["UnitPrice"]!.DefaultCellStyle.BackColor = Theme.AccentSoft;
        _grid.Columns["Qty"]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
        _grid.Columns["UnitPrice"]!.DefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.75F);
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

        // ---- footer -------------------------------------------------------
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
        var create = new Button
        {
            Text = "Create purchase order",
            BackColor = Theme.Accent,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI Semibold", 9.75F),
            Height = 32,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowOnly,
            Width = 180
        };
        create.FlatAppearance.BorderSize = 0;
        create.Click += Create;

        var cancel = UiKit.Secondary("Cancel");
        cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        buttons.Controls.Add(create);
        buttons.Controls.Add(cancel);

        footer.Controls.Add(buttons);
        footer.Controls.Add(_error);
        footer.Controls.Add(_total);

        Controls.Add(host);
        Controls.Add(footer);
        Controls.Add(header);

        // wait for the form to actually be live before pulling the first
        // selection's items — a data-bound combo's SelectedValue isn't
        // reliable yet while the form is still being built in the constructor
        Load += (_, _) =>
        {
            if (_orderable.Count == 0)
                ShowError("No approved request is waiting for a purchase order right now.");
            else
                LoadRequestItems();
        };
    }

    private void LoadRequestItems()
    {
        ShowError("");
        _rows.Clear();
        if (_request.SelectedValue is not int requestId) { Recalculate(); return; }

        var items = new CatalogService().GetRequestItems(requestId);
        foreach (var i in items)
        {
            _rows.Add(new PoEditRow
            {
                MaterialId = i.MaterialId,
                MaterialCode = i.MaterialCode,
                MaterialName = i.MaterialName,
                Unit = i.Unit,
                QtyRequested = i.QtyRequested,
                Qty = i.QtyRequested,
                UnitPrice = i.LastUnitCost
            });
        }
        Recalculate();
    }

    private void Warn(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _rows.Count) return;
        var row = _rows[e.RowIndex];
        if (_grid.Columns[e.ColumnIndex].Name == "Qty" && row.Qty > row.QtyRequested)
            e.CellStyle!.ForeColor = Theme.Danger;
    }

    private void Recalculate()
    {
        var total = _rows.Sum(r => r.LineTotal);
        var lines = _rows.Count(r => r.Qty > 0);
        _total.Text = $"{lines} line(s) to order  ·  ₱{total:N2}";
        _grid.Refresh();
    }

    private void ShowError(string message) => _error.Text = message;

    private void Create(object? sender, EventArgs e)
    {
        ShowError("");

        if (_request.SelectedValue is not int requestId) { ShowError("Choose a request."); return; }
        if (_supplier.SelectedValue is not int supplierId) { ShowError("Choose a supplier."); return; }

        var lines = _rows.Where(r => r.Qty > 0)
                         .Select(r => new PoLine(r.MaterialId, r.Qty, r.UnitPrice))
                         .ToList();
        if (lines.Count == 0) { ShowError("Set a quantity on at least one line."); return; }

        var over = _rows.FirstOrDefault(r => r.Qty > r.QtyRequested);
        if (over is not null)
        {
            ShowError($"{over.MaterialName}: cannot order more than the {over.QtyRequested:N2} requested.");
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;
            _orders.Create(
                AppSession.Require, requestId, supplierId,
                _orderDate.Value.Date, _expectedDate.Value.Date, _remarks.Text, lines);

            MessageBox.Show(this, "Purchase order created and sent to the supplier.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ShowError("Could not create: " + ex.Message);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
