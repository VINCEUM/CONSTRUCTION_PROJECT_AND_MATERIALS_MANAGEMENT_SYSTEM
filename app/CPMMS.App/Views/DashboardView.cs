using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Dashboard: KPI cards, a ledger-health banner, and the low-stock table.
/// The layout lives in DashboardView.Designer.cs; the numbers are filled here.
/// </summary>
public partial class DashboardView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly InventoryService _inventory = new();

    public DashboardView()
    {
        InitializeComponent();

        var totals = _catalog.GetDashboardTotals();
        var mismatches = _inventory.GetLedgerMismatches();
        var lowStock = _inventory.GetStockCheck(lowStockOnly: true);

        // --- KPI cards (labels are in the designer; values set here) ----------
        lblCard1Val.Text = totals.OngoingProjects.ToString();
        lblCard1Sub.Text = $"{totals.ProjectCount} in total";

        lblCard2Val.Text = "₱" + totals.TotalContractValue.ToString("N0");

        lblCard3Val.Text = "₱" + totals.StockValue.ToString("N0");
        lblCard3Sub.Text = $"{totals.MaterialCount} materials";

        lblCard4Val.Text = totals.LowStockCount.ToString();
        lblCard4Val.ForeColor = totals.LowStockCount > 0 ? Theme.Warn : Theme.Good;

        lblCard5Val.Text = totals.PendingRequests.ToString();
        lblCard5Val.ForeColor = totals.PendingRequests > 0 ? Theme.Warn : Theme.Good;

        // --- ledger health banner --------------------------------------------
        if (mismatches.Count == 0)
        {
            lblBanner.Text = "  Ledger check passed — every material's stock matches the sum of its transactions.";
            lblBanner.ForeColor = Theme.Good;
            panelBanner.BackColor = Theme.AccentSoft;
        }
        else
        {
            lblBanner.Text = $"  Ledger check FAILED for {mismatches.Count} material(s). Stock was changed outside a transaction.";
            lblBanner.ForeColor = Theme.Danger;
            panelBanner.BackColor = Color.FromArgb(250, 235, 234);
        }

        // --- low-stock title + table -----------------------------------------
        lblLowTitle.Text = lowStock.Count == 0
            ? "Low stock — nothing needs reordering"
            : $"Low stock — {lowStock.Count} material(s) at or below the reorder point";

        Theme.Style(gridLow);
        UiKit.Bind(gridLow, lowStock,
            ("Code", "CODE", null),
            ("Name", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("CachedStock", "ON HAND", "N2"),
            ("MinimumStock", "REORDER AT", "N2"));
    }
}
