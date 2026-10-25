namespace CPMMS.Core.Services;

/// <summary>
/// The rule behind every role-restricted action, enforced in the service layer
/// so it holds no matter which screen (or future code) calls the service. The
/// screens also hide what a role may not do, but that is only courtesy.
/// </summary>
public static class AccessGuard
{
    /// <summary>Throws unless the signed-in user is an Administrator.</summary>
    public static void RequireAdmin(string action)
    {
        var user = AppSession.CurrentUser
                   ?? throw new InvalidOperationException("Sign in first.");
        if (!user.IsAdmin)
            throw new InvalidOperationException($"Only an administrator can {action}.");
    }
}
