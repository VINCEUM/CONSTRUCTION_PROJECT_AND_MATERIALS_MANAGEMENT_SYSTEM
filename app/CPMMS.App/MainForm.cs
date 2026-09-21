using CPMMS.App.Views;
using CPMMS.Core;
using CPMMS.Core.Services;

namespace CPMMS.App;

public partial class MainForm : Form
{
    private readonly List<Button> _navButtons = new();

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        var user = AppSession.Require;
        lblUser.Text = $"{user.FullName}  ·  {user.RoleName}";

        if (DemoMode.Enabled)
        {
            panelHeader.Height = 90;
            var banner = new Label
            {
                Text = "   DEMO MODE — sample data held in memory. No database is connected, "
                     + "and nothing you change here is saved.",
                Dock = DockStyle.Bottom,
                Height = 26,
                BackColor = Color.FromArgb(251, 241, 222),
                ForeColor = Theme.Warn,
                Font = Theme.Body,
                TextAlign = ContentAlignment.MiddleLeft
            };
            panelHeader.Controls.Add(banner);
        }

        // Nav is built in code so items can be added or hidden per role.
        AddNav("Dashboard", () => new DashboardView());
        AddNav("Projects", () => new ProjectsView());
        AddNav("Materials", () => new MaterialsView());
        AddNav("Material Requests", () => new RequestsView());
        AddNav("Purchase Orders", () => new PurchaseOrdersView());
        AddNav("Variance Report", () => new VarianceView());
        AddNav("Suppliers", () => new SuppliersView());
        if (user.IsAdmin) AddNav("Users", () => new UsersView());

        if (_navButtons.Count > 0) _navButtons[0].PerformClick();
    }

    private void AddNav(string text, Func<UserControl> factory)
    {
        var button = new Button
        {
            Text = "   " + text,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Top,
            Height = 42,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.FromArgb(203, 218, 222),
            BackColor = Theme.Nav,
            Font = Theme.Body,
            Cursor = Cursors.Hand,
            Tag = text
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Theme.NavHover;
        button.Click += (_, _) => Show(button, text, factory);

        // Dock=Top stacks in reverse insertion order, so insert at the front.
        panelNavItems.Controls.Add(button);
        panelNavItems.Controls.SetChildIndex(button, 0);
        _navButtons.Add(button);
    }

    private void Show(Button source, string heading, Func<UserControl> factory)
    {
        foreach (var b in _navButtons)
        {
            b.BackColor = Theme.Nav;
            b.ForeColor = Color.FromArgb(203, 218, 222);
            b.Font = Theme.Body;
        }
        source.BackColor = Theme.NavHover;
        source.ForeColor = Color.White;
        source.Font = new Font("Segoe UI Semibold", 9.75F);

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
            MessageBox.Show(this,
                "Could not open that screen.\n\n" + ex.Message,
                "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void btnSignOut_Click(object? sender, EventArgs e)
    {
        AppSession.SignOut();
        Application.Restart();
    }
}
