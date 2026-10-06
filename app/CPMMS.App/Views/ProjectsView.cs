using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Projects list: budget vs actual material spend vs weighted progress.
/// Layout is in the designer; data and the add/edit actions are here.
/// </summary>
public partial class ProjectsView : UserControl
{
    private readonly CatalogService _catalog = new();
    private readonly MaintenanceService _maintenance = new();

    public ProjectsView()
    {
        InitializeComponent();
        Theme.Style(grid);
        Reload();
    }

    private void btnAdd_Click(object? sender, EventArgs e) => EditProject(null);
    private void btnEdit_Click(object? sender, EventArgs e) => EditProject((grid.CurrentRow?.DataBoundItem as ProjectCostRow)?.Id);
    private void grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) EditProject((grid.CurrentRow?.DataBoundItem as ProjectCostRow)?.Id);
    }

    private void Reload()
    {
        UiKit.Bind(grid, _catalog.GetProjectCosts(),
            ("Code", "CODE", null),
            ("Name", "PROJECT", null),
            ("ClientName", "CLIENT", null),
            ("Status", "STATUS", null),
            ("ProgressPercent", "PROGRESS %", "N1"),
            ("ContractAmount", "CONTRACT", "N0"),
            ("MaterialBudget", "MAT. BUDGET", "N0"),
            ("MaterialCostToDate", "MAT. SPENT", "N0"),
            ("MaterialBudgetRemaining", "REMAINING", "N0"));
        btnEdit.Enabled = grid.Rows.Count > 0;
    }

    private void grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || grid.Rows[e.RowIndex].DataBoundItem is not ProjectCostRow row) return;
        var column = grid.Columns[e.ColumnIndex].Name;

        if (column == "MaterialBudgetRemaining")
            e.CellStyle!.ForeColor = row.MaterialBudgetRemaining < 0 ? Theme.Danger : Theme.Ink;

        if (column == "Status")
            e.CellStyle!.ForeColor = row.Status switch
            {
                "ongoing" => Theme.Good,
                "completed" => Theme.Muted,
                "on_hold" => Theme.Warn,
                "cancelled" => Theme.Danger,
                _ => Theme.InkSoft
            };
    }

    private void EditProject(int? projectId)
    {
        var isNew = projectId is null;
        var project = isNew
            ? new Project { Status = "planning", StartDate = DateTime.Today,
                            TargetEndDate = DateTime.Today.AddMonths(6) }
            : _catalog.GetProjects().FirstOrDefault(p => p.Id == projectId);

        if (project is null) return;

        var statuses = new object[] { "planning", "ongoing", "on_hold", "completed", "cancelled" };

        using var editor = new RecordEditor(isNew ? "New project" : "Edit project",
            new List<FieldSpec>
            {
                new() { Key="code",     Label="Project code", Value=project.Code, Hint="e.g. PRJ-2026-004" },
                new() { Key="name",     Label="Project name", Value=project.Name },
                new() { Key="client",   Label="Client", Value=project.ClientName },
                new() { Key="location", Label="Location", Value=project.Location },
                new() { Key="contract", Label="Contract amount", Kind=FieldKind.Money, Value=project.ContractAmount },
                new() { Key="budget",   Label="Material budget", Kind=FieldKind.Money, Value=project.MaterialBudget,
                        Hint="What the variance report measures material spend against." },
                new() { Key="start",    Label="Start date", Kind=FieldKind.Date, Value=project.StartDate },
                new() { Key="target",   Label="Target end date", Kind=FieldKind.Date, Value=project.TargetEndDate },
                new() { Key="status",   Label="Status", Kind=FieldKind.Combo, Options=statuses, Value=project.Status },
                new() { Key="desc",     Label="Description", Kind=FieldKind.Multiline, Value=project.Description }
            });

        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        if (editor.IsNotANumber("contract") || editor.IsNotANumber("budget"))
        {
            MessageBox.Show(this, "Contract amount and material budget must be numbers.",
                "Check the values", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        project.Code = editor.Text_("code");
        project.Name = editor.Text_("name");
        project.ClientName = editor.Text_("client");
        project.Location = editor.Text_("location");
        project.ContractAmount = editor.Decimal_("contract");
        project.MaterialBudget = editor.Decimal_("budget");
        project.StartDate = editor.Date_("start");
        project.TargetEndDate = editor.Date_("target");
        project.Status = editor.Text_("status");
        project.Description = editor.Text_("desc");

        try
        {
            _maintenance.SaveProject(project);
            Reload();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
