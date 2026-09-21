using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class RequestsView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly DataGridView _requests = UiKit.Grid();
    private readonly DataGridView _items = UiKit.Grid();
    private readonly ComboBox _status = new();
    private readonly Button _issue;

    public RequestsView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        // --- filter ----------------------------------------------------------
        var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Surface };
        _status.DropDownStyle = ComboBoxStyle.DropDownList;
        _status.Font = Theme.Body;
        _status.Location = new Point(0, 8);
        _status.Width = 200;
        _status.Items.AddRange(new object[]
        {
            "All statuses", "pending", "approved", "partially_issued", "issued", "rejected", "cancelled"
        });
        _status.SelectedIndex = 0;
        _status.SelectedIndexChanged += (_, _) => LoadRequests();

        _issue = UiKit.Secondary("Issue materials…");
        _issue.Location = new Point(212, 7);
        _issue.Click += (_, _) => IssueMaterials();

        bar.Controls.Add(_issue);
        bar.Controls.Add(_status);

        // --- detail (bottom) --------------------------------------------------
        var detail = new Panel { Dock = DockStyle.Bottom, Height = 240, BackColor = Theme.Surface, Padding = new Padding(0, 8, 0, 0) };
        var detailHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        detailHost.Controls.Add(_items);
        detail.Controls.Add(detailHost);
        detail.Controls.Add(UiKit.SectionTitle("Requested items"));

        // --- master (fills) ---------------------------------------------------
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _requests.SelectionChanged += (_, _) => LoadItems();
        _requests.CellFormatting += FormatRequest;
        _requests.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) IssueMaterials(); };
        _items.CellFormatting += FormatItem;
        host.Controls.Add(_requests);

        Controls.Add(host);
        Controls.Add(detail);
        Controls.Add(bar);

        LoadRequests();
    }

    private void LoadRequests()
    {
        var status = _status.SelectedIndex <= 0 ? null : _status.SelectedItem?.ToString();
        var rows = _catalog.GetRequests(status);
        _issue.Enabled = rows.Count > 0;

        UiKit.Bind(_requests, rows,
            ("RequestNo", "REQUEST NO.", null),
            ("ProjectName", "PROJECT", null),
            ("RequesterName", "REQUESTED BY", null),
            ("RequestDate", "DATE", "d"),
            ("NeededDate", "NEEDED", "d"),
            ("Status", "STATUS", null),
            ("Remarks", "REMARKS", null));

        LoadItems();
    }

    /// <summary>Only an approved request can be released, and only by the right role.</summary>
    private void IssueMaterials()
    {
        if (_requests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

        var user = AppSession.Require;
        if (!(user.IsStorekeeper || user.IsAdmin))
        {
            MessageBox.Show(this, "Only a storekeeper or an admin can release materials.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (request.Status is not ("approved" or "partially_issued"))
        {
            MessageBox.Show(this,
                $"This request is '{request.Status}'. Only an approved request can be issued.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var form = new IssueForm(request);
        if (form.ShowDialog(FindForm()) == DialogResult.OK) LoadRequests();
    }

    private void FormatRequest(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _requests.Rows[e.RowIndex].DataBoundItem is not MaterialRequest row) return;
        if (_requests.Columns[e.ColumnIndex].Name != "Status") return;

        e.CellStyle!.ForeColor = row.Status switch
        {
            "pending" => Theme.Warn,
            "approved" => Theme.Accent,
            "issued" => Theme.Good,
            "partially_issued" => Theme.Warn,
            "rejected" or "cancelled" => Theme.Danger,
            _ => Theme.InkSoft
        };
    }

    private void FormatItem(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _items.Rows[e.RowIndex].DataBoundItem is not MaterialRequestItem item) return;
        var column = _items.Columns[e.ColumnIndex].Name;

        // flag lines that cannot be filled from stock right now
        if (column == "CurrentStock" && item.QtyOutstanding > item.CurrentStock)
            e.CellStyle!.ForeColor = Theme.Danger;
        if (column == "QtyOutstanding" && item.QtyOutstanding > 0)
            e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void LoadItems()
    {
        if (_requests.CurrentRow?.DataBoundItem is not MaterialRequest request)
        {
            _items.DataSource = null;
            return;
        }

        var rows = _catalog.GetRequestItems(request.Id);
        UiKit.Bind(_items, rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyRequested", "REQUESTED", "N2"),
            ("QtyIssued", "ISSUED", "N2"),
            ("QtyOutstanding", "OUTSTANDING", "N2"),
            ("CurrentStock", "ON HAND", "N2"));


    }
}
