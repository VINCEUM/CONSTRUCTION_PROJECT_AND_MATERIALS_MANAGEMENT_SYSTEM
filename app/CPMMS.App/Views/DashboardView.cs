    using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class DashboardView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly InventoryService _inventory = new();

    private void InitializeComponent()
    {

    }

    public DashboardView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var totals = _catalog.GetDashboardTotals();
        var mismatches = _inventory.GetLedgerMismatches();
        var lowStock = _inventory.GetStockCheck(lowStockOnly: true);

        // --- low stock table (fills the rest of the screen) ------------------
        var grid = UiKit.Grid();
        UiKit.Bind(grid, lowStock,
            ("Code", "CODE", null),
            ("Name", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("CachedStock", "ON HAND", "N2"),
            ("MinimumStock", "REORDER AT", "N2"));

        var gridHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        gridHost.Controls.Add(grid);

        var lowTitle = UiKit.SectionTitle(
            lowStock.Count == 0
                ? "Low stock — nothing needs reordering"
                : $"Low stock — {lowStock.Count} material(s) at or below the reorder point");

        // --- KPI cards -------------------------------------------------------
        var cards = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 108,
            BackColor = Theme.Surface,
            WrapContents = false,
            AutoScroll = false
        };
        cards.Controls.Add(UiKit.Card("Ongoing projects", totals.OngoingProjects.ToString(),
            $"{totals.ProjectCount} in total"));
        cards.Controls.Add(UiKit.Card("Contract value", "₱" + totals.TotalContractValue.ToString("N0"),
            "ongoing and planned"));
        cards.Controls.Add(UiKit.Card("Stock value", "₱" + totals.StockValue.ToString("N0"),
            $"{totals.MaterialCount} materials"));
        cards.Controls.Add(UiKit.Card("Low stock", totals.LowStockCount.ToString(),
            "at or below reorder point",
            totals.LowStockCount > 0 ? Theme.Warn : Theme.Good));
        cards.Controls.Add(UiKit.Card("Pending requests", totals.PendingRequests.ToString(),
            "waiting for approval",
            totals.PendingRequests > 0 ? Theme.Warn : Theme.Good));

        // --- ledger health ---------------------------------------------------
        // The whole design rests on cached stock agreeing with the ledger, so
        // the dashboard says so out loud.
        var banner = mismatches.Count == 0
            ? UiKit.Banner("  Ledger check passed — every material's stock matches the sum of its transactions.",
                           Theme.Good, Theme.AccentSoft)
            : UiKit.Banner($"  Ledger check FAILED for {mismatches.Count} material(s). Stock was changed outside a transaction.",
                           Theme.Danger, Color.FromArgb(250, 235, 234));

        Controls.Add(gridHost);
        Controls.Add(lowTitle);
        Controls.Add(banner);
        Controls.Add(cards);
    }
}
