using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class SuppliersView : UserControl
{
    private readonly MaintenanceService _maintenance = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly CheckBox _showInactive = new();
    private readonly Button _edit, _toggle;

    public SuppliersView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };

        var add = UiKit.Secondary("Add supplier…");
        add.Location = new Point(0, 8);
        add.Click += (_, _) => Edit(null);

        _edit = UiKit.Secondary("Edit…");
        _edit.Location = new Point(120, 8);
        _edit.Click += (_, _) => Edit(Selected());

        _toggle = UiKit.Secondary("Deactivate");
        _toggle.Location = new Point(196, 8);
        _toggle.Click += (_, _) => ToggleStatus();

        _showInactive.Text = "Show inactive";
        _showInactive.Font = Theme.Body;
        _showInactive.ForeColor = Theme.InkSoft;
        _showInactive.Location = new Point(306, 12);
        _showInactive.AutoSize = true;
        _showInactive.CheckedChanged += (_, _) => Reload();

        bar.Controls.AddRange(new Control[] { _showInactive, _toggle, _edit, add });

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Edit(Selected()); };
        _grid.CellFormatting += Format;
        _grid.SelectionChanged += (_, _) => UpdateButtons();
        host.Controls.Add(_grid);

        Controls.Add(host);
        Controls.Add(bar);

        Reload();
    }

    private Supplier? Selected() => _grid.CurrentRow?.DataBoundItem as Supplier;

    private void Reload()
    {
        var rows = _maintenance.GetSuppliers(_showInactive.Checked);
        UiKit.Bind(_grid, rows,
            ("CompanyName", "SUPPLIER", null),
            ("ContactPerson", "CONTACT PERSON", null),
            ("ContactNo", "PHONE", null),
            ("Email", "EMAIL", null),
            ("Address", "ADDRESS", null),
            ("OrderCount", "POs", null),
            ("Status", "STATUS", null));
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        var supplier = Selected();
        _edit.Enabled = supplier is not null;
        _toggle.Enabled = supplier is not null;
        _toggle.Text = supplier?.Status == "inactive" ? "Reactivate" : "Deactivate";
    }

    private void Format(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not Supplier row) return;
        if (_grid.Columns[e.ColumnIndex].Name != "Status") return;
        e.CellStyle!.ForeColor = row.Status == "active" ? Theme.Good : Theme.Muted;
    }

    private void Edit(Supplier? existing)
    {
        var isNew = existing is null;
        var supplier = existing ?? new Supplier();

        using var editor = new RecordEditor(
            isNew ? "New supplier" : "Edit supplier",
            new List<FieldSpec>
            {
                new() { Key="name",    Label="Company name",  Value=supplier.CompanyName },
                new() { Key="person",  Label="Contact person", Value=supplier.ContactPerson },
                new() { Key="phone",   Label="Contact number", Value=supplier.ContactNo },
                new() { Key="email",   Label="Email",          Value=supplier.Email },
                new() { Key="address", Label="Address",        Value=supplier.Address, Kind=FieldKind.Multiline },
                new() { Key="remarks", Label="Remarks",        Value=supplier.Remarks }
            });

        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        supplier.CompanyName = editor.Text_("name");
        supplier.ContactPerson = editor.Text_("person");
        supplier.ContactNo = editor.Text_("phone");
        supplier.Email = editor.Text_("email");
        supplier.Address = editor.Text_("address");
        supplier.Remarks = editor.Text_("remarks");

        try
        {
            _maintenance.SaveSupplier(supplier);
            Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ToggleStatus()
    {
        if (Selected() is not { } supplier) return;
        var activate = supplier.Status != "active";

        // Suppliers are never deleted — an old purchase order has to keep
        // resolving to the company that fulfilled it.
        if (!activate && MessageBox.Show(this,
                $"Deactivate {supplier.CompanyName}?\n\n" +
                "It stays on every past purchase order, but will no longer be offered for new ones.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        _maintenance.SetSupplierStatus(supplier.Id, activate);
        Reload();
    }
}
