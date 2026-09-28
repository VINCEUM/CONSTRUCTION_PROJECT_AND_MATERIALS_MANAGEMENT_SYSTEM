using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>
/// Approving or rejecting a material request. Kept apart from InventoryService
/// because nothing here moves stock - it only decides whether a request may
/// be issued later.
///
/// Rules (docs/database-design.md, rule 9 - separation of duties):
///   * only an Admin may approve or reject
///   * nobody approves their own request
///   * only a 'pending' request can be decided, and only once
///   * a rejection must carry a reason
/// </summary>
public sealed class ApprovalService
{
    public void ApproveRequest(int requestId, User approver)
    {
        Decide(requestId, approver, approve: true, reason: null);
    }

    public void RejectRequest(int requestId, User approver, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Give a reason for rejecting this request.");

        if (reason.Trim().Length > 200)
            throw new InvalidOperationException("Keep the reason under 200 characters.");

        Decide(requestId, approver, approve: false, reason: reason.Trim());
    }

    private static void Decide(int requestId, User approver, bool approve, string? reason)
    {
        if (!approver.IsAdmin)
            throw new InvalidOperationException("Only an admin can approve or reject requests.");

        if (DemoMode.Enabled)
        {
            var demo = DemoData.Requests.FirstOrDefault(r => r.Id == requestId)
                       ?? throw new InvalidOperationException("That request no longer exists.");
            if (demo.Status != "pending")
                throw new InvalidOperationException($"This request is already '{demo.Status}'.");

            demo.Status = approve ? "approved" : "rejected";
            demo.RejectReason = approve ? null : reason;
            return;
        }

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();

        try
        {
            // lock the row so two people cannot decide the same request at once
            var request = cn.QuerySingleOrDefault<(string RequestNo, string Status, int RequestedBy)>(
                @"SELECT request_no AS RequestNo, status AS Status, requested_by AS RequestedBy
                  FROM   material_requests
                  WHERE  id = @requestId FOR UPDATE",
                new { requestId }, tx);

            if (request.RequestNo is null)
                throw new InvalidOperationException("That request no longer exists.");
            if (request.Status != "pending")
                throw new InvalidOperationException(
                    $"{request.RequestNo} is already '{request.Status}', so it cannot be decided again.");
            if (request.RequestedBy == approver.Id)
                throw new InvalidOperationException("You cannot approve or reject your own request.");

            if (approve)
            {
                cn.Execute(
                    @"UPDATE material_requests
                      SET    status = 'approved', approved_by = @by, approved_at = NOW(), reject_reason = NULL
                      WHERE  id = @requestId",
                    new { by = approver.Id, requestId }, tx);
            }
            else
            {
                cn.Execute(
                    @"UPDATE material_requests
                      SET    status = 'rejected', approved_by = NULL, approved_at = NULL, reject_reason = @reason
                      WHERE  id = @requestId",
                    new { reason, requestId }, tx);
            }

            // the audit trail records WHO decided, even for a rejection
            cn.Execute(
                @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
                  VALUES (@userId, @action, 'material_request', @requestId, @description)",
                new
                {
                    userId = approver.Id,
                    action = approve ? "approved" : "rejected",
                    requestId,
                    description = approve ? request.RequestNo : $"{request.RequestNo}: {reason}"
                }, tx);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
