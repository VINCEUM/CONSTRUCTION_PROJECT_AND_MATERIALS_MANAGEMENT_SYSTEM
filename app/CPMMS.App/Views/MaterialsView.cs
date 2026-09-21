using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class MaterialsView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly TextBox _search = new();
    private readonly CheckBox _lowOnly = new();
    private readonly Label _count = new();
    private readonly MaintenanceService _maintenance = new();
    private readonly Button _edit;

    public MaterialsView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        // --- toolbar ---------------------------------------------------------
        var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };

        _search.PlaceholderText = "Search by name, code or category…";
        _search.BorderStyle = BorderStyle.FixedSingle;
        _search.Font = Theme.Body;
        _search.Location = new Point(0, 8);
        _search.Size = new Size(320, 26);
        _search.TextChanged += (_, _) => Reload();

        _lowOnly.Text = "Low stock only";
        _lowOnly.Font = Theme.Body;
        _lowOnly.ForeColor = Theme.InkSoft;
        _lowOnly.Location = new Point(336, 11);
        _lowOnly.AutoSize = true;
        _lowOnly.CheckedChanged += (_, _) => Reload();

        var stockCard = UiKit.Secondary("Stock card");
        stockCard.Location = new Point(470, 8);
        stockCard.Click += (_, _) => OpenLedger();

        var add = UiKit.Secondary("Add...");
        add.Location = new Point(556, 8);
        add.Click += (_, _) => EditMaterial(null);

        _edit = UiKit.Secondary("Edit...");
        _edit.Location = new Point(616, 8);
        _edit.Click += (_, _) => EditMaterial(_grid.CurrentRow?.DataBoundItem as Material);

        _count.ForeColor = Theme.Muted;
        _count.Font = Theme.Small;
        _count.AutoSize = false;
        _count.Location = new Point(688, 14);
        _count.Size = new Size(300, 20);

        bar.Controls.Add(_edit);
        bar.Controls.Add(add);
        bar.Controls.Add(_count);
        bar.Controls.Add(stockCard);
        bar.Controls.Add(_lowOnly);
        bar.Controls.Add(_search);

        // --- grid ------------------------------------------------------------
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) OpenLedger(); };
        _grid.CellFormatting += HighlightLowStock;
        host.Controls.Add(_grid);

        Controls.Add(host);
        Controls.Add(bar);

        Reload();
    }

    private void Reload()
    {
        var rows = _catalog.GetMaterials(_search.Text, _lowOnly.Checked);
        UiKit.Bind(_grid, rows,
            ("Code", "CODE", null),
            ("Name", "MATERIAL", null),
            ("CategoryName", "CATEGORY", null),
            ("Unit", "UNIT", null),
            ("CurrentStock", "ON HAND", "N2"),
            ("MinimumStock", "REORDER AT", "N2"),
            ("LastUnitCost", "UNIT COST", "N2"),
            ("StockValue", "VALUE", "N2"));

        _edit.Enabled = rows.Count > 0;
        _count.Text = $"{rows.Count} material(s)  ·  double-click a row for its stock card";
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

    private void HighlightLowStock(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not Material material) return;
        if (!material.IsLowStock) return;

        e.CellStyle!.ForeColor = Theme.Warn;
        if (_grid.Columns[e.ColumnIndex].Name == "CurrentStock")
            e.CellStyle.Font = new Font(Theme.Body, FontStyle.Bold);
    }

    private void OpenLedger()
    {
        if (_grid.CurrentRow?.DataBoundItem is not Material material) return;
        using var form = new LedgerForm(material);
        form.ShowDialog(FindForm());
    }
}
