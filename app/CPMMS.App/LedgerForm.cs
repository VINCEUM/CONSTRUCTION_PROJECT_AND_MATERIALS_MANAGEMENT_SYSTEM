using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>
/// The stock card: every movement that produced this material's balance.
/// This is the screen that makes "why does it say 98 bags?" answerable.
/// </summary>
public sealed class LedgerForm : Form
{
    public LedgerForm(Material material)
    {
        Text = $"Stock card — {material.Name}";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(940, 560);
        BackColor = Theme.Surface;
        Font = Theme.Body;

        var rows = new InventoryService().GetLedger(material.Id);
        var ledgerTotal = rows.Count == 0 ? 0 : rows[0].BalanceAfter;

        var header = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Color.White, Padding = new Padding(18, 14, 18, 8) };
        header.Controls.Add(new Label
        {
            Text = $"{material.Code}  ·  {material.Name}",
            Font = Theme.H1,
            ForeColor = Theme.Ink,
            Dock = DockStyle.Top,
            Height = 32,
            AutoSize = false
        });
        header.Controls.SetChildIndex(header.Controls[0], 0);

        var sub = new Label
        {
            Text = $"On hand {material.CurrentStock:N2} {material.Unit}  ·  reorder at {material.MinimumStock:N2}  ·  " +
                   $"last cost ₱{material.LastUnitCost:N2}",
            ForeColor = Theme.Muted,
            Dock = DockStyle.Bottom,
            Height = 22,
            AutoSize = false
        };
        header.Controls.Add(sub);

        var grid = UiKit.Grid();
        UiKit.Bind(grid, rows,
            ("CreatedAt", "WHEN", null),
            ("TxnType", "TYPE", null),
            ("Quantity", "QTY", "N2"),
            ("BalanceAfter", "BALANCE", "N2"),
            ("UnitCost", "UNIT COST", "N2"),
            ("ProjectName", "PROJECT", null),
            ("Remarks", "REFERENCE", null),
            ("PerformedByName", "BY", null));

        grid.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not LedgerRow row) return;
            if (grid.Columns[e.ColumnIndex].Name == "Quantity")
                e.CellStyle!.ForeColor = row.Quantity < 0 ? Theme.Danger : Theme.Good;
        };

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(grid);

        // the point of the whole design, stated on screen
        var agrees = Math.Abs(ledgerTotal - material.CurrentStock) < 0.0005m;
        var footer = UiKit.Banner(
            agrees
                ? $"  Ledger total {ledgerTotal:N2} matches the stock figure {material.CurrentStock:N2}."
                : $"  Ledger total {ledgerTotal:N2} does NOT match the stock figure {material.CurrentStock:N2}.",
            agrees ? Theme.Good : Theme.Danger,
            agrees ? Theme.AccentSoft : Color.FromArgb(250, 235, 234));
        footer.Dock = DockStyle.Bottom;

        Controls.Add(host);
        Controls.Add(footer);
        Controls.Add(header);
    }

    private void InitializeComponent()
    {

    }
}
