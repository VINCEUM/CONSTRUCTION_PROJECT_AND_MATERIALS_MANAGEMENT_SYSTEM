using CPMMS.App.Views;
using CPMMS.Core;
using CPMMS.Core.Services;

namespace CPMMS.App;

/// <summary>
/// The shell: a left sidebar of buttons (placed in the designer) and a content
/// panel. Clicking a sidebar button swaps the matching screen into the panel.
/// The buttons live in MainForm.Designer.cs so they are visible and editable in
/// the Visual Studio designer; only the swapping and highlighting is in code.
/// </summary>
public partial class MainForm : Form
{
    private Button? _active;

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        var user = AppSession.Require;
        lblUser.Text = $"{user.FullName}  ·  {user.RoleName}";

        // The Users screen is admin-only; hide the button for everyone else.
        btnUsers.Visible = user.IsAdmin;

        if (DemoMode.Enabled)
        {
            panelHeader.Height = 90;
            panelHeader.Controls.Add(new Label
            {
                Text = "   DEMO MODE — sample data held in memory. No database is connected, "
                     + "and nothing you change here is saved.",
                Dock = DockStyle.Bottom,
                Height = 26,
                BackColor = Color.FromArgb(251, 241, 222),
                ForeColor = Theme.Warn,
                Font = Theme.Body,
                TextAlign = ContentAlignment.MiddleLeft
            });
        }

        // Open the dashboard first.
        Open(btnDashboard, "Dashboard", () => new DashboardView());
    }

    // ---- one click handler per sidebar button -------------------------------
    private void btnDashboard_Click(object? s, EventArgs e)      => Open(btnDashboard,      "Dashboard",         () => new DashboardView());
    private void btnProjects_Click(object? s, EventArgs e)       => Open(btnProjects,       "Projects",          () => new ProjectsView());
    private void btnMaterials_Click(object? s, EventArgs e)      => Open(btnMaterials,      "Materials",         () => new MaterialsView());
    private void btnRequests_Click(object? s, EventArgs e)       => Open(btnRequests,       "Material Requests", () => new RequestsView());
    private void btnPurchaseOrders_Click(object? s, EventArgs e) => Open(btnPurchaseOrders, "Purchase Orders",   () => new PurchaseOrdersView());
    private void btnVariance_Click(object? s, EventArgs e)       => Open(btnVariance,       "Variance Report",   () => new VarianceView());
    private void btnSuppliers_Click(object? s, EventArgs e)      => Open(btnSuppliers,      "Suppliers",         () => new SuppliersView());
    private void btnUsers_Click(object? s, EventArgs e)          => Open(btnUsers,          "Users",             () => new UsersView());

    private void Open(Button source, string heading, Func<UserControl> factory)
    {
        // reset the previously selected button
        if (_active is not null)
        {
            _active.BackColor = Theme.Nav;
            _active.ForeColor = Color.FromArgb(203, 218, 222);
            _active.Font = Theme.Body;
        }
        source.BackColor = Theme.NavHover;
        source.ForeColor = Color.White;
        source.Font = new Font("Segoe UI Semibold", 9.75F);
        _active = source;

        lblHeading.Text = heading;

        try
        {
            Cursor = Cursors.WaitCursor;
            var view = factory();
            view.Dock = DockStyle.Fill;
            foreach (Control old in panelContent.Controls) old.Dispose();
            panelContent.Controls.Clear();
            panelContent.Controls.Add(view);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Could not open that screen.\n\n" + ex.Message,
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally { Cursor = Cursors.Default; }
    }

    private void btnSignOut_Click(object? sender, EventArgs e)
    {
        AppSession.SignOut();
        Application.Restart();
    }
}
