using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// The audit trail (Administrator): who did what and when. Approvals, rejections,
/// cancellations, voids, stock adjustments, deliveries, issues, reorder alerts and
/// the other key actions all land here.
/// </summary>
public sealed class ActivityLogView : UserControl
{
    private const string AllTypes = "All records";

    private readonly ReportService _service = new();
    private readonly ComboBox _type = new();
    private readonly TextBox _search = new();
    private readonly Label _message = new();
    private readonly DataGridView _grid = UiKit.Grid();

    public ActivityLogView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };
        var lblType = new Label { Text = "Record", Font = Theme.Body, ForeColor = Theme.InkSoft, AutoSize = true, Location = new Point(0, 12) };
        _type.DropDownStyle = ComboBoxStyle.DropDownList;
        _type.Location = new Point(lblType.Right + 12, 8);
        _type.Width = 200;

        var lblSearch = new Label { Text = "Search", Font = Theme.Body, ForeColor = Theme.InkSoft, AutoSize = true, Location = new Point(_type.Right + 24, 12) };
        _search.Location = new Point(lblSearch.Right + 12, 9);
        _search.Width = 260;
        _search.BorderStyle = BorderStyle.FixedSingle;
        _search.PlaceholderText = "user, action or details";
        _search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Reload(); } };

        var refresh = UiKit.Secondary("Refresh");
        refresh.Location = new Point(_search.Right + 12, 7);
        refresh.Click += (_, _) => Reload();

        bar.Controls.Add(refresh);
        bar.Controls.Add(_search);
        bar.Controls.Add(lblSearch);
        bar.Controls.Add(_type);
        bar.Controls.Add(lblType);

        _message.Dock = DockStyle.Bottom;
        _message.Height = 28;
        _message.Font = Theme.Body;
        _message.ForeColor = Theme.InkSoft;
        _message.TextAlign = ContentAlignment.MiddleLeft;

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_grid);

        Controls.Add(host);
        Controls.Add(_message);
        Controls.Add(bar);

        try
        {
            _type.Items.Add(AllTypes);
            foreach (var t in _service.GetActivityEntityTypes(AppSession.Require)) _type.Items.Add(t);
            _type.SelectedIndex = 0;
            _type.SelectedIndexChanged += (_, _) => Reload();
            Reload();
        }
        catch (Exception ex)
        {
            _message.Text = "  " + ex.Message;
        }
    }

    private void Reload()
    {
        try
        {
            var type = _type.SelectedIndex <= 0 ? null : _type.SelectedItem?.ToString();
            var rows = _service.GetActivity(AppSession.Require, type, _search.Text);

            UiKit.Bind(_grid, rows,
                ("CreatedAt", "WHEN", "g"),
                ("UserName", "USER", null),
                ("Action", "ACTION", null),
                ("EntityType", "RECORD", null),
                ("Description", "DETAILS", null));

            _message.Text = rows.Count >= 500
                ? "  Showing the newest 500 entries. Narrow it down with the record type or the search box."
                : $"  {rows.Count} entr{(rows.Count == 1 ? "y" : "ies")}";
        }
        catch (Exception ex)
        {
            _message.Text = "  " + ex.Message;
        }
    }
}
