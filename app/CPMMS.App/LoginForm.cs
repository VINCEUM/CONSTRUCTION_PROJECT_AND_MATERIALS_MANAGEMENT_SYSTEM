using CPMMS.Core;
using CPMMS.Core.Services;

namespace CPMMS.App;

public partial class LoginForm : Form
{
    private readonly AuthService _auth = new();

    public LoginForm()
    {
        InitializeComponent();
    }

    private void LoginForm_Load(object? sender, EventArgs e)
    {
        txtEmail.Text = "admin@buildcorp.test";
        txtPassword.Text = "";
        txtEmail.Focus();

        // Tell the user plainly when MySQL isn't reachable, instead of throwing
        // a connector stack trace at them on the first sign-in attempt.
        var (ok, message) = DatabaseHelper.TestConnection();
        if (!ok)
        {
            lblError.Text = "Cannot reach the database. Is MySQL running, and is the " +
                            "connection string in appsettings.json correct?";
            lblHint.Text = message.Length > 160 ? message[..160] : message;
            btnLogin.Enabled = false;
            btnLogin.BackColor = Color.FromArgb(206, 215, 217);
            btnLogin.ForeColor = Color.FromArgb(120, 136, 141);
            btnLogin.Text = "Sign in — database unavailable";
            btnDemo.Visible = true;
            AcceptButton = btnDemo;
        }
    }

    /// <summary>
    /// Runs the whole interface on the sample data built into the app, so the
    /// system can be shown without MySQL. Clearly labelled on every screen.
    /// </summary>
    private void btnDemo_Click(object? sender, EventArgs e)
    {
        DemoMode.Enable();

        var user = _auth.Login(txtEmail.Text.Trim(), "");
        if (user is null) { lblError.Text = "No demo account matches that email."; return; }

        AppSession.SignIn(user);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnLogin_Click(object? sender, EventArgs e)
    {
        lblError.Text = "";

        var email = txtEmail.Text.Trim();
        var password = txtPassword.Text;

        if (email.Length == 0 || password.Length == 0)
        {
            lblError.Text = "Enter your email and password.";
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;
            var user = _auth.Login(email, password);

            if (user is null)
            {
                lblError.Text = "That email and password do not match an active account.";
                txtPassword.SelectAll();
                txtPassword.Focus();
                return;
            }

            AppSession.SignIn(user);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Could not sign in: " + ex.Message;
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }
}
