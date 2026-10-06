using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Users: add, edit, reset password, deactivate (admin only). Layout is in the
/// designer; the data and actions are here.
/// </summary>
public partial class UsersView : UserControl
{
    private readonly MaintenanceService _maintenance = new();

    public UsersView()
    {
        InitializeComponent();
        Theme.Style(grid);
        Reload();
    }

    private void btnAdd_Click(object? sender, EventArgs e) => Edit(null);
    private void btnEdit_Click(object? sender, EventArgs e) => Edit(Selected());
    private void btnPassword_Click(object? sender, EventArgs e) => ResetPassword();
    private void btnToggle_Click(object? sender, EventArgs e) => ToggleStatus();
    private void grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) Edit(Selected()); }
    private void grid_SelectionChanged(object? sender, EventArgs e) => UpdateButtons();

    private User? Selected() => grid.CurrentRow?.DataBoundItem as User;

    private void Reload()
    {
        var rows = _maintenance.GetUsers();
        UiKit.Bind(grid, rows,
            ("FullName", "NAME", null),
            ("Email", "EMAIL", null),
            ("RoleName", "ROLE", null),
            ("ContactNo", "CONTACT", null),
            ("Status", "STATUS", null));
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        var user = Selected();
        btnEdit.Enabled = user is not null;
        btnPassword.Enabled = user is not null;
        btnToggle.Enabled = user is not null && user.Id != AppSession.Require.Id;
        btnToggle.Text = user?.Status == "inactive" ? "Reactivate" : "Deactivate";
    }

    private void grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not User row) return;
        var column = grid.Columns[e.ColumnIndex].Name;

        if (column == "Status")
            e.CellStyle!.ForeColor = row.Status == "active" ? Theme.Good : Theme.Muted;
        if (column == "RoleName" && row.IsAdmin)
            e.CellStyle!.ForeColor = Theme.Accent;
    }

    private void Edit(User? existing)
    {
        var isNew = existing is null;
        var user = existing ?? new User { Status = "active" };
        var roles = _maintenance.GetRoles().ToList();

        var fields = new List<FieldSpec>
        {
            new() { Key="name",   Label="Full name", Value=user.FullName },
            new() { Key="email",  Label="Email",     Value=user.Email },
            new() { Key="phone",  Label="Contact number", Value=user.ContactNo },
            new() { Key="role",   Label="Role", Kind=FieldKind.Combo, Options=roles.Cast<object>(),
                    Value=isNew ? null : user.RoleName }
        };

        if (isNew)
            fields.Add(new FieldSpec
            {
                Key = "password", Label = "Password", Kind = FieldKind.Password,
                Hint = "At least 8 characters. Stored as a bcrypt hash."
            });

        using var editor = new RecordEditor(isNew ? "New user" : "Edit user", fields);
        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        user.FullName = editor.Text_("name");
        user.Email = editor.Text_("email");
        user.ContactNo = editor.Text_("phone");

        if (editor.Pick<Role>("role") is { } role)
        {
            user.RoleId = role.Id;
            user.RoleName = role.Name;
        }

        try
        {
            _maintenance.SaveUser(user, isNew ? editor.Text_("password") : null);
            Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ResetPassword()
    {
        if (Selected() is not { } user) return;

        using var editor = new RecordEditor($"Reset password — {user.FullName}",
            new List<FieldSpec>
            {
                new() { Key="password", Label="New password", Kind=FieldKind.Password,
                        Hint="At least 8 characters. The old password cannot be recovered, only replaced." }
            },
            saveText: "Set password");

        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        try
        {
            _maintenance.SaveUser(user, editor.Text_("password"));
            MessageBox.Show(this, $"Password updated for {user.FullName}.",
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ToggleStatus()
    {
        if (Selected() is not { } user) return;
        if (user.Id == AppSession.Require.Id) return;      // never lock yourself out

        var activate = user.Status != "active";
        if (!activate && MessageBox.Show(this,
                $"Deactivate {user.FullName}?\n\nThey will not be able to sign in, but everything they "
                + "approved or issued stays on record in their name.",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        _maintenance.SetUserStatus(user.Id, activate);
        Reload();
    }
}
