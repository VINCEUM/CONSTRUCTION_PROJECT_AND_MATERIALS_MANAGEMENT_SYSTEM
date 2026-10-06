using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>
/// The stock card: every movement that produced this material's balance.
/// The layout lives in LedgerForm.Designer.cs (editable in the designer);
/// the data is loaded in LoadMaterial below.
/// </summary>
public partial class LedgerForm : Form
{
    /// <summary>Parameterless constructor so the form opens in the WinForms designer.</summary>
    public LedgerForm()
    {
        InitializeComponent();
        Theme.Style(gridLedger);
    }

    /// <summary>The constructor the application uses, for one material.</summary>
    public LedgerForm(Material material) : this()
    {
        LoadMaterial(material);
    }

    private void LoadMaterial(Material material)
    {
        Text = $"Stock card — {material.Name}";
        lblTitle.Text = $"{material.Code}  ·  {material.Name}";
        lblSub.Text = $"On hand {material.CurrentStock:N2} {material.Unit}  ·  reorder at {material.MinimumStock:N2}  ·  " +
                      $"last cost ₱{material.LastUnitCost:N2}";

        var rows = new InventoryService().GetLedger(material.Id);
        var ledgerTotal = rows.Count == 0 ? 0 : rows[0].BalanceAfter;

        UiKit.Bind(gridLedger, rows,
            ("CreatedAt", "WHEN", null),
            ("TxnType", "TYPE", null),
            ("Quantity", "QTY", "N2"),
            ("BalanceAfter", "BALANCE", "N2"),
            ("UnitCost", "UNIT COST", "N2"),
            ("ProjectName", "PROJECT", null),
            ("Remarks", "REFERENCE", null),
            ("PerformedByName", "BY", null));

        gridLedger.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0 || gridLedger.Rows[e.RowIndex].DataBoundItem is not LedgerRow row) return;
            if (gridLedger.Columns[e.ColumnIndex].Name == "Quantity")
                e.CellStyle!.ForeColor = row.Quantity < 0 ? Theme.Danger : Theme.Good;
        };

        var agrees = Math.Abs(ledgerTotal - material.CurrentStock) < 0.0005m;
        lblFooter.Text = agrees
            ? $"  Ledger total {ledgerTotal:N2} matches the stock figure {material.CurrentStock:N2}."
            : $"  Ledger total {ledgerTotal:N2} does NOT match the stock figure {material.CurrentStock:N2}.";
        lblFooter.ForeColor = agrees ? Theme.Good : Theme.Danger;
        panelFooter.BackColor = agrees ? Theme.AccentSoft : Color.FromArgb(250, 235, 234);
    }
}
