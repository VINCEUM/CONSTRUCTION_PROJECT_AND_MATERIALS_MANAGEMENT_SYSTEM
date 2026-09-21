using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

public sealed class AuthService
{
    /// <summary>Returns the user on success, null when the email or password is wrong.</summary>
    public User? Login(string email, string password)
    {
        // No database: match a demo account by email so each role can be shown.
        if (DemoMode.Enabled)
            return DemoData.Users.FirstOrDefault(
                       u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase))
                   ?? DemoData.Users[0];

        using var cn = DatabaseHelper.Open();

        var user = cn.QuerySingleOrDefault<User>(
            @"SELECT u.id, u.role_id, u.full_name, u.email, u.password, u.contact_no, u.status,
                     r.name AS role_name
              FROM   users u
              JOIN   roles r ON r.id = u.role_id
              WHERE  u.email = @email AND u.status = 'active'",
            new { email });

        if (user is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

        cn.Execute("UPDATE users SET last_login_at = NOW() WHERE id = @id", new { id = user.Id });
        Log(user.Id, "logged in", "user", user.Id, null);
        return user;
    }

    public static string HashPassword(string plain) => BCrypt.Net.BCrypt.HashPassword(plain, 11);

    /// <summary>Audit trail. Every approval, posting and void goes through here.</summary>
    public static void Log(int? userId, string action, string entityType, long? entityId, string? description)
    {
        DatabaseHelper.Execute(
            @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
              VALUES (@userId, @action, @entityType, @entityId, @description)",
            new { userId, action, entityType, entityId, description });
    }
}

/// <summary>Who is signed in, for the lifetime of the process.</summary>
public static class AppSession
{
    public static User? CurrentUser { get; private set; }

    public static User Require =>
        CurrentUser ?? throw new InvalidOperationException("No user is signed in.");

    public static void SignIn(User user) => CurrentUser = user;
    public static void SignOut() => CurrentUser = null;
}
