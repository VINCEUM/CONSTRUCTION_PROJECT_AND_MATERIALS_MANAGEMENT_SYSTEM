using CPMMS.Core;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Material requests: a master list, the requested items below, and the actions
/// (new, issue, approve, reject, submit, cancel). Layout is in the designer;
/// the data and the actions are here.
/// </summary>
public partial class RequestsView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly ApprovalService _approvals = new();
    private readonly RequestService _requestSvc = new();

    public RequestsView()
    {
        InitializeComponent();
        Theme.Style(gridRequests);
        Theme.Style(gridItems);

        var user = AppSession.CurrentUser;
        btnApprove.Visible = user?.IsAdmin == true;      // approving is an admin job
        btnReject.Visible = user?.IsAdmin == true;
        // engineers request materials — creating one needs a real database
        btnNew.Visible = user?.IsEngineer == true && !DemoMode.Enabled;

        cmbStatus.SelectedIndex = 0;   // triggers the first LoadRequests
    }

    // ---- toolbar handlers ---------------------------------------------------
    private void cmbStatus_SelectedIndexChanged(object? sender, EventArgs e) => LoadRequests();
    private void btnNew_Click(object? sender, EventArgs e) => NewRequest();
    private void btnIssue_Click(object? sender, EventArgs e) => IssueMaterials();
    private void btnApprove_Click(object? sender, EventArgs e) => ApproveSelected();
    private void btnReject_Click(object? sender, EventArgs e) => RejectSelected();
    private void btnSubmit_Click(object? sender, EventArgs e) => SubmitSelected();
    private void btnCancel_Click(object? sender, EventArgs e) => CancelSelected();

    private void gridRequests_SelectionChanged(object? sender, EventArgs e) { LoadItems(); UpdateDecisionButtons(); }
    private void gridRequests_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) IssueMaterials(); }

    private void LoadRequests()
    {
        var status = cmbStatus.SelectedIndex <= 0 ? null : cmbStatus.SelectedItem?.ToString();
        var rows = _catalog.GetRequests(status);
        btnIssue.Enabled = rows.Count > 0;

        UiKit.Bind(gridRequests, rows,
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
        var row = gridRequests.CurrentRow?.DataBoundItem as MaterialRequest;
        var user = AppSession.CurrentUser;

        var submitted = row is { Status: "submitted" };
        btnApprove.Enabled = submitted;
        btnReject.Enabled = submitted;

        btnSubmit.Enabled = row is { Status: "draft" } && row.RequestedBy == user?.Id;
        btnCancel.Enabled = row is { Status: "draft" or "submitted" }
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
        if (gridRequests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

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
        if (gridRequests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

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
        if (gridRequests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

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
        if (gridRequests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

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
        foreach (DataGridViewRow row in gridRequests.Rows)
        {
            if (row.DataBoundItem is MaterialRequest r && r.Id == requestId)
            {
                gridRequests.CurrentCell = row.Cells[0];
                break;
            }
        }
    }

    /// <summary>Only an approved request can be released, and only by the right role.</summary>
    private void IssueMaterials()
    {
        if (gridRequests.CurrentRow?.DataBoundItem is not MaterialRequest request) return;

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

    private void gridRequests_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || gridRequests.Rows[e.RowIndex].DataBoundItem is not MaterialRequest row) return;
        if (gridRequests.Columns[e.ColumnIndex].Name != "Status") return;

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

    private void gridItems_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || gridItems.Rows[e.RowIndex].DataBoundItem is not MaterialRequestItem item) return;
        var column = gridItems.Columns[e.ColumnIndex].Name;

        // flag lines that cannot be filled from stock right now
        if (column == "CurrentStock" && item.QtyOutstanding > item.CurrentStock)
            e.CellStyle!.ForeColor = Theme.Danger;
        if (column == "QtyOutstanding" && item.QtyOutstanding > 0)
            e.CellStyle!.ForeColor = Theme.Warn;
    }

    private void LoadItems()
    {
        if (gridRequests.CurrentRow?.DataBoundItem is not MaterialRequest request)
        {
            gridItems.DataSource = null;
            return;
        }

        var rows = _catalog.GetRequestItems(request.Id);
        UiKit.Bind(gridItems, rows,
            ("MaterialCode", "CODE", null),
            ("MaterialName", "MATERIAL", null),
            ("Unit", "UNIT", null),
            ("QtyRequested", "REQUESTED", "N2"),
            ("QtyIssued", "ISSUED", "N2"),
            ("QtyOutstanding", "OUTSTANDING", "N2"),
            ("CurrentStock", "ON HAND", "N2"));
    }
}
