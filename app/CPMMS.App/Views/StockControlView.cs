using CPMMS.Core;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Stock control (Administrator): the history of returns and physical counts,
/// with the buttons that start a new one. Every return and count is posted
/// through the ledger, so the Materials stock card and the dashboard's ledger
/// check stay in step.
/// </summary>
public sealed class StockControlView : UserControl
{
    private readonly StockControlService _service = new();
    private readonly Button _tabReturns;
    private readonly Button _tabCounts;
    private readonly Panel _returnsPanel = new();
    private readonly Panel _countsPanel = new();
    private readonly DataGridView _returnsGrid = UiKit.Grid();
    private readonly DataGridView _countsGrid = UiKit.Grid();
    private readonly Label _message = new();

    public StockControlView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var tabs = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = Theme.Surface };
        _tabReturns = UiKit.Secondary("Returns");
        _tabReturns.Location = new Point(0, 6);
        _tabReturns.Click += (_, _) => ShowTab(returns: true);
        _tabCounts = UiKit.Secondary("Physical counts");
        _tabCounts.Location = new Point(_tabReturns.Left + _tabReturns.GetPreferredSize(Size.Empty).Width + 8, 6);
        _tabCounts.Click += (_, _) => ShowTab(returns: false);
        tabs.Controls.Add(_tabCounts);
        tabs.Controls.Add(_tabReturns);

        BuildPanel(_returnsPanel, _returnsGrid, "New return…", () => NewReturn());
        BuildPanel(_countsPanel, _countsGrid, "New physical count…", () => NewCount());

        _message.Dock = DockStyle.Bottom;
        _message.Height = 28;
        _message.Font = Theme.Body;
        _message.ForeColor = Theme.InkSoft;
        _message.TextAlign = ContentAlignment.MiddleLeft;

        Controls.Add(_returnsPanel);
        Controls.Add(_countsPanel);
        Controls.Add(_message);
        Controls.Add(tabs);

        ShowTab(returns: true);
        Reload();
    }

    private static void BuildPanel(Panel panel, DataGridView grid, string buttonText, Action onClick)
    {
        panel.Dock = DockStyle.Fill;

        var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Surface };
        var button = UiKit.Secondary(buttonText);
        button.Location = new Point(0, 7);
        button.Click += (_, _) => onClick();
        bar.Controls.Add(button);

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(grid);

        panel.Controls.Add(host);
        panel.Controls.Add(bar);
    }

    private void ShowTab(bool returns)
    {
        _returnsPanel.Visible = returns;
        _countsPanel.Visible = !returns;
        Highlight(_tabReturns, returns);
        Highlight(_tabCounts, !returns);
    }

    private static void Highlight(Button button, bool active)
    {
        button.BackColor = active ? Theme.AccentSoft : Color.White;
        button.ForeColor = active ? Theme.Accent : Theme.InkSoft;
        button.FlatAppearance.BorderColor = active ? Theme.Accent : Theme.Line;
    }

    private void Reload()
    {
        try
        {
            _message.Text = "";

            UiKit.Bind(_returnsGrid, _service.GetReturns(),
                ("ReturnNo", "RETURN NO.", null),
                ("ReturnDate", "DATE", "d"),
                ("ProjectName", "PROJECT", null),
                ("ReturnedBy", "RETURNED BY", null),
                ("AcceptedByName", "ACCEPTED BY", null),
                ("LineCount", "LINES", "N0"),
                ("ReusableValue", "BACK TO STOCK ₱", "N2"),
                ("DamagedValue", "WASTAGE ₱", "N2"),
                ("Status", "STATUS", null));

            UiKit.Bind(_countsGrid, _service.GetAdjustments(),
                ("AdjustmentNo", "COUNT NO.", null),
                ("CountDate", "DATE", "d"),
                ("CountedByName", "COUNTED BY", null),
                ("Reason", "REASON", null),
                ("LineCount", "ITEMS COUNTED", "N0"),
                ("ChangedLines", "CORRECTED", "N0"),
                ("NetVariance", "NET VARIANCE", "N2"),
                ("Status", "STATUS", null));
        }
        catch (Exception ex)
        {
            _message.Text = "  " + ex.Message;
        }
    }

    private void NewReturn()
    {
        if (DemoMode.Enabled) { _message.Text = "  Returns need the database. They are not available in demo mode."; return; }
        using var form = new ReturnForm();
        if (form.ShowDialog(FindForm()) == DialogResult.OK) Reload();
    }

    private void NewCount()
    {
        if (DemoMode.Enabled) { _message.Text = "  Physical counts need the database. They are not available in demo mode."; return; }
        using var form = new CountForm();
        if (form.ShowDialog(FindForm()) == DialogResult.OK) Reload();
    }
}
