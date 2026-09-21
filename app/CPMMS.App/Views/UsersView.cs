using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

public sealed class UsersView : UserControl
{
    private readonly MaintenanceService _maintenance = new();
    private readonly DataGridView _grid = UiKit.Grid();
    private readonly Button _edit, _toggle, _password;

    public UsersView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var bar = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };

        var add = UiKit.Secondary("Add user…");
        add.Location = new Point(0, 8);
        add.Click += (_, _) => Edit(null);

        _edit = UiKit.Secondary("Edit…");
        _edit.Location = new Point(104, 8);
        _edit.Click += (_, _) => Edit(Selected());

        _password = UiKit.Secondary("Reset password…");
        _password.Location = new Point(180, 8);
        _password.Click += (_, _) => ResetPassword();

        _toggle = UiKit.Secondary("Deactivate");
        _toggle.Location = new Point(320, 8);
        _toggle.Click += (_, _) => ToggleStatus();

        bar.Controls.AddRange(new Control[] { _toggle, _password, _edit, add });

        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) Edit(Selected()); };
        _grid.CellFormatting += Format;
        _grid.SelectionChanged += (_, _) => UpdateButtons();
        host.Controls.Add(_grid);

        var note = new Label
        {
            Text = "Passwords are stored as bcrypt hashes and are never shown — an admin can only replace one, "
                 + "never read it. Accounts are deactivated rather than deleted so their approvals and issuances stay attributable.",
            Dock = DockStyle.Bottom,
            Height = 36,
            ForeColor = Theme.Muted,
            Font = Theme.Small,
            AutoSize = false
        };

        Controls.Add(host);
        Controls.Add(note);
        Controls.Add(bar);

        Reload();
    }

    private User? Selected() => _grid.CurrentRow?.DataBoundItem as User;

    private void Reload()
    {
        var rows = _maintenance.GetUsers();
        UiKit.Bind(_grid, rows,
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
        _edit.Enabled = user is not null;
        _password.Enabled = user is not null;
        _toggle.Enabled = user is not null && user.Id != AppSession.Require.Id;
        _toggle.Text = user?.Status == "inactive" ? "Reactivate" : "Deactivate";
    }

    private void Format(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not User row) return;
        var column = _grid.Columns[e.ColumnIndex].Name;

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
