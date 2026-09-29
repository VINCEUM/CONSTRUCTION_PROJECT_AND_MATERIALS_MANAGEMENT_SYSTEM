using CPMMS.Core;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class RequestsView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly DataGridView _requests = UiKit.Grid();
    private readonly DataGridView _items = UiKit.Grid();
    private readonly ComboBox _status = new();
    private readonly ApprovalService _approvals = new();
    private readonly RequestService _requestSvc = new();
    private readonly Button _issue;
    private readonly Button _new;
    private readonly Button _approve;
    private readonly Button _reject;
    private readonly Button _submit;
    private readonly Button _cancel;

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
            "All statuses", "draft", "submitted", "approved", "partially_issued", "issued",
            "rejected", "cancelled", "voided"
        });
        _status.SelectedIndex = 0;
        _status.SelectedIndexChanged += (_, _) => LoadRequests();

        var user = AppSession.CurrentUser;
        var canDecide = user?.IsAdmin == true;      // approving is an admin job
        // engineers request materials — creating one needs a real database
        var canCreate = user?.IsEngineer == true && !DemoMode.Enabled;

        _new = UiKit.Secondary("New request…");
        _new.Location = new Point(212, 7);
        _new.Visible = canCreate;
        _new.Click += (_, _) => NewRequest();

        var afterNew = canCreate ? _new.Left + _new.GetPreferredSize(Size.Empty).Width + 8 : 212;

        _issue = UiKit.Secondary("Issue materials…");
        _issue.Location = new Point(afterNew, 7);
        _issue.Click += (_, _) => IssueMaterials();

        var next = _issue.Left + _issue.GetPreferredSize(Size.Empty).Width + 8;

        _approve = UiKit.Secondary("Approve");
        _approve.ForeColor = Theme.Accent;
        _approve.FlatAppearance.BorderColor = Theme.Accent;
        _approve.Location = new Point(next, 7);
        _approve.Visible = canDecide;
        _approve.Enabled = false;
        _approve.Click += (_, _) => ApproveSelected();

        _reject = UiKit.Secondary("Reject…");
        _reject.ForeColor = Theme.Danger;
        _reject.FlatAppearance.BorderColor = Theme.Danger;
        _reject.Location = new Point(next + _approve.GetPreferredSize(Size.Empty).Width + 8, 7);
        _reject.Visible = canDecide;
        _reject.Enabled = false;
        _reject.Click += (_, _) => RejectSelected();

        var afterReject = _reject.Left + _reject.GetPreferredSize(Size.Empty).Width + 8;

        _submit = UiKit.Secondary("Submit…");
        _submit.Location = new Point(afterReject, 7);
        _submit.Enabled = false;
        _submit.Click += (_, _) => SubmitSelected();

        _cancel = UiKit.Secondary("Cancel…");
        _cancel.ForeColor = Theme.Danger;
        _cancel.FlatAppearance.BorderColor = Theme.Danger;
        _cancel.Location = new Point(_submit.Left + _submit.GetPreferredSize(Size.Empty).Width + 8, 7);
        _cancel.Enabled = false;
        _cancel.Click += (_, _) => CancelSelected();

        bar.Controls.Add(_cancel);
        bar.Controls.Add(_submit);
        bar.Controls.Add(_reject);
        bar.Controls.Add(_approve);
        bar.Controls.Add(_issue);
        bar.Controls.Add(_new);
        bar.Controls.Add(_status);

        // --- detail (bottom) --------------------------------------------------
        var detail = new Panel { Dock = DockStyle.Bottom, Height = 240, BackColor = Theme.Surface, Padding = new Padding(0, 8, 0, 0) };
        var detailHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        detailHost.Controls.Add(_items);
        detail.Controls.Add(detailHost);
        detail.Controls.Add(UiKit.SectionTitle("Requested items"));

        // --- master (fills) ---------------------------------------------------
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _requests.SelectionChanged += (_, _) => { LoadItems(); UpdateDecisionButtons(); };
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
            ("Remarks", "REMARKS", null),
            ("RejectReason", "REJECT REASON", null));

        LoadItems();
    }

    /// <summary>Each action only makes sense for a request in the right status.</summary>
    private void UpdateDecisionButtons()
    {
        var row = _requests.CurrentRow?.DataBoundItem as MaterialRequest;
        var user = AppSession.CurrentUser;

        var submitted = row is { Status: "submitted" };
        _approve.Enabled = submitted;
        _reject.Enabled = submitted;

        _submit.Enabled = row is { Status: "draft" } && row.RequestedBy == user?.Id;
        _cancel.Enabled = row is { Status: "draft" or "submitted" }
                          && (row.RequestedBy == user?.Id || user?.IsAdmin == true);
    }

    /// <summary>Opens the request builder. Draft and submit both happen through RequestService.</summary>
    private void NewRequest()
    {
        using var form = new NewRequestForm();
        if (form.ShowDialog(FindForm()) == DialogResult.OK) LoadRequests();
    }

    private void SubmitSelected()
    {
        if (_requests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

        if (MessageBox.Show(this,
                $"Submit {request.RequestNo} for approval?",
                "Submit request", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            _requestSvc.Submit(request.Id, AppSession.Require);
            AfterDecision(request.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not submit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void CancelSelected()
    {
        if (_requests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

        using var editor = new RecordEditor($"Cancel {request.RequestNo}",
            new List<FieldSpec>
            {
                new() { Key="reason", Label="Reason for cancelling", Kind=FieldKind.Multiline,
                        Hint="Required. Kept on the record — nothing is deleted." }
            },
            saveText: "Cancel request");

        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        var reason = editor.Text_("reason");
        try
        {
            _requestSvc.Cancel(request.Id, AppSession.Require, reason);
            AfterDecision(request.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not cancel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ApproveSelected()
    {
        if (_requests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

        if (MessageBox.Show(this,
                $"Approve {request.RequestNo} for {request.ProjectName}?\n\n"
                + "The storekeeper will then be able to issue the materials.",
                "Approve request", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            _approvals.ApproveRequest(request.Id, AppSession.Require);
            AfterDecision(request.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not approve", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void RejectSelected()
    {
        if (_requests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

        using var editor = new RecordEditor($"Reject {request.RequestNo}",
            new List<FieldSpec>
            {
                new() { Key="reason", Label="Reason for rejecting", Kind=FieldKind.Multiline,
                        Hint="Required. It is shown next to the request so the engineer knows what to fix." }
            },
            saveText: "Reject");

        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        var reason = editor.Text_("reason");
        if (string.IsNullOrWhiteSpace(reason))
        {
            MessageBox.Show(this, "Please write a reason before rejecting.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            _approvals.RejectRequest(request.Id, AppSession.Require, reason);
            AfterDecision(request.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not reject", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>Reload, then keep the same request selected (unless the filter now hides it).</summary>
    private void AfterDecision(int requestId)
    {
        LoadRequests();
        foreach (DataGridViewRow row in _requests.Rows)
        {
            if (row.DataBoundItem is MaterialRequest r && r.Id == requestId)
            {
                _requests.CurrentCell = row.Cells[0];
                break;
            }
        }
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
            "draft" => Theme.InkSoft,
            "submitted" => Theme.Warn,
            "approved" => Theme.Accent,
            "issued" => Theme.Good,
            "partially_issued" => Theme.Warn,
            "rejected" or "cancelled" or "voided" => Theme.Danger,
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
