using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Suppliers: add, edit, deactivate/reactivate. Layout is in the designer;
/// the data and actions are here.
/// </summary>
public partial class SuppliersView : UserControl
{
    private readonly MaintenanceService _maintenance = new();

    public SuppliersView()
    {
        InitializeComponent();
        Theme.Style(grid);
        Reload();
    }

    private void btnAdd_Click(object? sender, EventArgs e) => Edit(null);
    private void btnEdit_Click(object? sender, EventArgs e) => Edit(Selected());
    private void btnToggle_Click(object? sender, EventArgs e) => ToggleStatus();
    private void chkInactive_CheckedChanged(object? sender, EventArgs e) => Reload();
    private void grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) Edit(Selected()); }
    private void grid_SelectionChanged(object? sender, EventArgs e) => UpdateButtons();

    private Supplier? Selected() => grid.CurrentRow?.DataBoundItem as Supplier;

    private void Reload()
    {
        var rows = _maintenance.GetSuppliers(chkInactive.Checked);
        UiKit.Bind(grid, rows,
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
        btnEdit.Enabled = supplier is not null;
        btnToggle.Enabled = supplier is not null;
        btnToggle.Text = supplier?.Status == "inactive" ? "Reactivate" : "Deactivate";
    }

    private void grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Supplier row) return;
        if (grid.Columns[e.ColumnIndex].Name != "Status") return;
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
