using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Materials list with a search/filter toolbar. Layout is in the designer;
/// loading, filtering and the add/edit/stock-card actions are here.
/// </summary>
public partial class MaterialsView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly MaintenanceService _maintenance = new();

    public MaterialsView()
    {
        InitializeComponent();
        Theme.Style(grid);
        Reload();
    }

    private void txtSearch_TextChanged(object? sender, EventArgs e) => Reload();
    private void chkLowOnly_CheckedChanged(object? sender, EventArgs e) => Reload();
    private void btnStockCard_Click(object? sender, EventArgs e) => OpenLedger();
    private void btnAdd_Click(object? sender, EventArgs e) => EditMaterial(null);
    private void btnEdit_Click(object? sender, EventArgs e) => EditMaterial(grid.CurrentRow?.DataBoundItem as Material);
    private void grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) OpenLedger(); }

    private void Reload()
    {
        var rows = _catalog.GetMaterials(txtSearch.Text, chkLowOnly.Checked);
        UiKit.Bind(grid, rows,
            ("Code", "CODE", null),
            ("Name", "MATERIAL", null),
            ("CategoryName", "CATEGORY", null),
            ("Unit", "UNIT", null),
            ("CurrentStock", "ON HAND", "N2"),
            ("MinimumStock", "REORDER AT", "N2"),
            ("LastUnitCost", "UNIT COST", "N2"),
            ("StockValue", "VALUE", "N2"));

        btnEdit.Enabled = rows.Count > 0;
        lblCount.Text = $"{rows.Count} material(s)  ·  double-click a row for its stock card";
    }

    /// <summary>
    /// Stock is deliberately NOT editable here. A quantity can only move through
    /// a delivery, an issuance, a return or a counted adjustment, so the ledger
    /// stays the single source of truth.
    /// </summary>
    private void EditMaterial(Material? existing)
    {
        var isNew = existing is null;
        var material = existing ?? new Material { Status = "active" };
        var categories = _maintenance.GetCategories().ToList();

        var fields = new List<FieldSpec>
        {
            new() { Key="code", Label="Code", Value=material.Code },
            new() { Key="name", Label="Material name", Value=material.Name },
            new() { Key="cat",  Label="Category", Kind=FieldKind.Combo, Options=categories.Cast<object>(),
                    Value=isNew ? null : material.CategoryName },
            new() { Key="unit", Label="Unit", Value=material.Unit, Hint="bag, pc, cu.m, kg, sheet, set" },
            new() { Key="cost", Label="Unit cost", Kind=FieldKind.Money, Value=material.LastUnitCost,
                    Hint="Reference price. Deliveries overwrite it with what was actually paid." },
            new() { Key="min",  Label="Reorder level", Kind=FieldKind.Number, Value=material.MinimumStock },
        };

        if (!isNew)
            fields.Add(new FieldSpec
            {
                Key = "stock", Label = "On hand", Value = material.CurrentStock, ReadOnly = true,
                Hint = "Changed only by deliveries, issuances, returns and physical counts."
            });

        using var editor = new RecordEditor(isNew ? "New material" : "Edit material", fields);
        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        if (editor.IsNotANumber("cost") || editor.IsNotANumber("min"))
        {
            MessageBox.Show(this, "Unit cost and reorder level must be numbers.",
                "Check the values", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        material.Code = editor.Text_("code");
        material.Name = editor.Text_("name");
        material.Unit = editor.Text_("unit");
        material.LastUnitCost = editor.Decimal_("cost");
        material.MinimumStock = editor.Decimal_("min");

        if (editor.Pick<MaterialCategory>("cat") is { } category)
        {
            material.CategoryId = category.Id;
            material.CategoryName = category.Name;
        }

        try
        {
            _maintenance.SaveMaterial(material);
            Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not Material material) return;
        if (!material.IsLowStock) return;

        e.CellStyle!.ForeColor = Theme.Warn;
        if (grid.Columns[e.ColumnIndex].Name == "CurrentStock")
            e.CellStyle.Font = new Font(Theme.Body, FontStyle.Bold);
    }

    private void OpenLedger()
    {
        if (grid.CurrentRow?.DataBoundItem is not Material material) return;
        using var form = new LedgerForm(material);
        form.ShowDialog(FindForm());
    }
}
