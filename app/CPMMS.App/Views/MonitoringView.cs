using CPMMS.Core;
using CPMMS.Core.Models;
using CPMMS.Core.Services;

namespace CPMMS.App.Views;

/// <summary>
/// Project monitoring: pick a project, then follow its schedule of tasks with
/// weighted progress, and keep its Bill of Quantities (the materials it is
/// planned to use). An engineer sees and changes only the projects assigned to
/// them; the Administrator sees all. The service enforces that on every write.
/// </summary>
public sealed class MonitoringView : UserControl
{
    /// <summary>A combo-box entry that remembers its id.</summary>
    private sealed class Item
    {
        public int? Id { get; }
        public string Text { get; }
        public Item(int? id, string text) { Id = id; Text = text; }
        public override string ToString() => Text;
    }

    private readonly ProjectMonitoringService _service = new();
    private readonly CatalogService _catalog = new();
    private readonly ComboBox _project = new();
    private readonly Label _summary = new();
    private readonly Button _tabTasks;
    private readonly Button _tabBoq;
    private readonly Panel _tasksPanel = new();
    private readonly Panel _boqPanel = new();
    private readonly DataGridView _tasksGrid = UiKit.Grid();
    private readonly DataGridView _boqGrid = UiKit.Grid();
    private readonly Label _tasksFoot = new();
    private readonly Label _boqFoot = new();
    private readonly IReadOnlyList<Project> _projects;

    private IReadOnlyList<ProjectTask> _tasks = new List<ProjectTask>();
    private IReadOnlyList<ProjectEstimate> _estimates = new List<ProjectEstimate>();

    public MonitoringView()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Surface;

        var user = AppSession.Require;
        _projects = _service.GetProjectsFor(user);

        // ---- project picker + overall progress ---------------------------
        var top = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Theme.Surface };
        var lblProject = new Label
        {
            Text = "Project", Font = Theme.Body, ForeColor = Theme.InkSoft,
            AutoSize = true, Location = new Point(0, 12)
        };
        _project.DropDownStyle = ComboBoxStyle.DropDownList;
        _project.Location = new Point(lblProject.Right + 12, 8);
        _project.Width = 420;
        foreach (var p in _projects) _project.Items.Add(new Item(p.Id, $"{p.Code} — {p.Name}"));

        _summary.Font = Theme.H2;
        _summary.ForeColor = Theme.Ink;
        _summary.AutoSize = true;
        _summary.Location = new Point(_project.Right + 24, 11);

        top.Controls.Add(_summary);
        top.Controls.Add(_project);
        top.Controls.Add(lblProject);

        // ---- the two sections ---------------------------------------------
        var tabs = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = Theme.Surface };
        _tabTasks = UiKit.Secondary("Tasks & progress");
        _tabTasks.Location = new Point(0, 6);
        _tabTasks.Click += (_, _) => ShowTab(tasks: true);
        _tabBoq = UiKit.Secondary("Material estimates (BOQ)");
        _tabBoq.Location = new Point(_tabTasks.Left + _tabTasks.GetPreferredSize(Size.Empty).Width + 8, 6);
        _tabBoq.Click += (_, _) => ShowTab(tasks: false);
        tabs.Controls.Add(_tabBoq);
        tabs.Controls.Add(_tabTasks);

        BuildTasksPanel();
        BuildBoqPanel();

        Controls.Add(_tasksPanel);
        Controls.Add(_boqPanel);
        Controls.Add(tabs);
        Controls.Add(top);

        ShowTab(tasks: true);

        if (_projects.Count > 0)
        {
            _project.SelectedIndex = 0;
            _project.SelectedIndexChanged += (_, _) => ReloadAll();
            ReloadAll();
        }
        else
        {
            _summary.Text = user.IsAdmin ? "There are no projects yet." : "No project is assigned to you yet.";
        }
    }

    // ============================================================ layout

    private void BuildTasksPanel()
    {
        _tasksPanel.Dock = DockStyle.Fill;

        var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Surface };
        var add = UiKit.Secondary("Add task…");
        add.Location = new Point(0, 7);
        add.Click += (_, _) => EditTask(null);
        var progress = UiKit.Secondary("Update progress…");
        progress.Location = new Point(add.Left + add.GetPreferredSize(Size.Empty).Width + 8, 7);
        progress.Click += (_, _) => UpdateProgress();
        var edit = UiKit.Secondary("Edit task…");
        edit.Location = new Point(progress.Left + progress.GetPreferredSize(Size.Empty).Width + 8, 7);
        edit.Click += (_, _) => EditTask(SelectedTask());
        bar.Controls.Add(edit);
        bar.Controls.Add(progress);
        bar.Controls.Add(add);

        _tasksFoot.Dock = DockStyle.Bottom;
        _tasksFoot.Height = 30;
        _tasksFoot.Font = Theme.Body;
        _tasksFoot.ForeColor = Theme.InkSoft;
        _tasksFoot.TextAlign = ContentAlignment.MiddleLeft;

        _tasksGrid.CellFormatting += FormatTaskCell;
        _tasksGrid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) UpdateProgress(); };
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_tasksGrid);

        _tasksPanel.Controls.Add(host);
        _tasksPanel.Controls.Add(_tasksFoot);
        _tasksPanel.Controls.Add(bar);
    }

    private void BuildBoqPanel()
    {
        _boqPanel.Dock = DockStyle.Fill;

        var bar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Theme.Surface };
        var add = UiKit.Secondary("Add material…");
        add.Location = new Point(0, 7);
        add.Click += (_, _) => EditEstimate(null);
        var edit = UiKit.Secondary("Edit line…");
        edit.Location = new Point(add.Left + add.GetPreferredSize(Size.Empty).Width + 8, 7);
        edit.Click += (_, _) => EditEstimate(SelectedEstimate());
        var remove = UiKit.Secondary("Remove line");
        remove.ForeColor = Theme.Danger;
        remove.FlatAppearance.BorderColor = Theme.Danger;
        remove.Location = new Point(edit.Left + edit.GetPreferredSize(Size.Empty).Width + 8, 7);
        remove.Click += (_, _) => RemoveEstimate();
        bar.Controls.Add(remove);
        bar.Controls.Add(edit);
        bar.Controls.Add(add);

        _boqFoot.Dock = DockStyle.Bottom;
        _boqFoot.Height = 30;
        _boqFoot.Font = Theme.Body;
        _boqFoot.ForeColor = Theme.InkSoft;
        _boqFoot.TextAlign = ContentAlignment.MiddleLeft;

        _boqGrid.CellFormatting += FormatBoqCell;
        _boqGrid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditEstimate(SelectedEstimate()); };
        var host = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
        host.Controls.Add(_boqGrid);

        _boqPanel.Controls.Add(host);
        _boqPanel.Controls.Add(_boqFoot);
        _boqPanel.Controls.Add(bar);
    }

    private void ShowTab(bool tasks)
    {
        _tasksPanel.Visible = tasks;
        _boqPanel.Visible = !tasks;
        Highlight(_tabTasks, tasks);
        Highlight(_tabBoq, !tasks);
    }

    private static void Highlight(Button button, bool active)
    {
        button.BackColor = active ? Theme.AccentSoft : Color.White;
        button.ForeColor = active ? Theme.Accent : Theme.InkSoft;
        button.FlatAppearance.BorderColor = active ? Theme.Accent : Theme.Line;
    }

    // ============================================================ data

    private Project? CurrentProject =>
        _project.SelectedIndex >= 0 && _project.SelectedIndex < _projects.Count
            ? _projects[_project.SelectedIndex] : null;

    private ProjectTask? SelectedTask() => _tasksGrid.CurrentRow?.DataBoundItem as ProjectTask;
    private ProjectEstimate? SelectedEstimate() => _boqGrid.CurrentRow?.DataBoundItem as ProjectEstimate;

    private void ReloadAll()
    {
        var project = CurrentProject;
        if (project is null) return;

        try
        {
            _tasks = _service.GetTasks(project.Id);
            UiKit.Bind(_tasksGrid, _tasks,
                ("Name", "TASK", null),
                ("WeightPercent", "WEIGHT %", "N2"),
                ("StartDate", "START", "d"),
                ("DueDate", "DUE", "d"),
                ("ProgressPercent", "PROGRESS %", "N1"),
                ("Status", "STATUS", null),
                ("Remarks", "REMARKS", null));

            var overall = ProjectMonitoringService.OverallProgress(_tasks);
            var weights = _tasks.Sum(t => t.WeightPercent);
            var done = _tasks.Count(t => t.Status == "completed");
            _summary.Text = $"Overall progress  {overall:N1}%";
            _tasksFoot.Text = _tasks.Count == 0
                ? "  No tasks yet. Add the first one to start tracking this project."
                : $"  {_tasks.Count} task(s)  ·  {done} completed  ·  weights total {weights:N2}% " +
                  (weights < 100 ? $"({100 - weights:N2}% still unassigned)" : "");

            _estimates = _service.GetEstimates(project.Id);
            UiKit.Bind(_boqGrid, _estimates,
                ("TaskName", "TASK", null),
                ("MaterialCode", "CODE", null),
                ("MaterialName", "MATERIAL", null),
                ("Unit", "UNIT", null),
                ("PlannedQty", "PLANNED QTY", "N2"),
                ("PlannedUnitCost", "UNIT COST", "N2"),
                ("PlannedCost", "PLANNED COST", "N2"));

            var planned = _estimates.Sum(e => e.PlannedCost);
            var budget = project.MaterialBudget;
            var over = budget > 0 && planned > budget;
            _boqFoot.ForeColor = over ? Theme.Danger : Theme.InkSoft;
            _boqFoot.Text = _estimates.Count == 0
                ? "  No materials estimated yet. Add what this project is planned to use."
                : $"  {_estimates.Count} line(s)  ·  planned cost ₱{planned:N2}  ·  material budget ₱{budget:N2}" +
                  (budget > 0 ? $"  ({planned / budget:P0} of budget)" : "") +
                  (over ? "  — over budget" : "");
        }
        catch (Exception ex)
        {
            _summary.Text = ex.Message;
        }
    }

    private void FormatTaskCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _tasksGrid.Rows[e.RowIndex].DataBoundItem is not ProjectTask task) return;
        var column = _tasksGrid.Columns[e.ColumnIndex].Name;

        if (column == "Status")
        {
            e.CellStyle!.ForeColor = task.Status switch
            {
                "completed" => Theme.Good,
                "in_progress" => Theme.Accent,
                "on_hold" => Theme.Warn,
                _ => Theme.InkSoft
            };
        }
        else if (column == "DueDate" && task.Status != "completed"
                 && task.DueDate is { } due && due.Date < DateTime.Today)
        {
            e.CellStyle!.ForeColor = Theme.Danger;   // overdue
        }
    }

    private void FormatBoqCell(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (_boqGrid.Columns[e.ColumnIndex].Name == "TaskName" && string.IsNullOrEmpty(e.Value as string))
        {
            e.Value = "(whole project)";
            e.FormattingApplied = true;
        }
    }

    // ============================================================ tasks

    private void EditTask(ProjectTask? existing)
    {
        var project = CurrentProject;
        if (project is null) return;

        var isNew = existing is null;
        var task = existing ?? new ProjectTask { ProjectId = project.Id };

        var fields = new List<FieldSpec>
        {
            new() { Key="name", Label="Task", Value=task.Name, Hint="e.g. Excavation, Foundation, Roofing" },
            new() { Key="weight", Label="Weight %", Kind=FieldKind.Number, Value=isNew ? "" : task.WeightPercent.ToString("0.##"),
                    Hint="Share of the whole project. All tasks together can't exceed 100." },
            new() { Key="start", Label="Start date", Kind=FieldKind.Date, Value=task.StartDate ?? DateTime.Today },
            new() { Key="due", Label="Due date", Kind=FieldKind.Date, Value=task.DueDate ?? DateTime.Today.AddDays(14) },
            new() { Key="progress", Label="Progress %", Kind=FieldKind.Number, Value=task.ProgressPercent.ToString("0.##") },
            new() { Key="state", Label="Task state", Kind=FieldKind.Combo,
                    Options=new object[] { "active", "on hold" },
                    Value=task.Status == "on_hold" ? "on hold" : "active",
                    Hint="Status follows progress by itself. Choose 'on hold' to pause a task." },
            new() { Key="remarks", Label="Remarks", Kind=FieldKind.Multiline, Value=task.Remarks }
        };

        using var editor = new RecordEditor(isNew ? "New task" : "Edit task", fields);
        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        if (editor.IsNotANumber("weight") || editor.IsNotANumber("progress"))
        {
            Warn("Weight and progress must be numbers.");
            return;
        }

        task.Name = editor.Text_("name");
        task.WeightPercent = editor.Decimal_("weight");
        task.StartDate = editor.Date_("start");
        task.DueDate = editor.Date_("due");
        task.ProgressPercent = editor.Decimal_("progress");
        task.Status = editor.Text_("state") == "on hold" ? "on_hold" : "active";
        task.Remarks = editor.Text_("remarks");

        Run(() => _service.SaveTask(AppSession.Require, task));
    }

    private void UpdateProgress()
    {
        var task = SelectedTask();
        if (task is null) { Warn("Select a task first."); return; }

        var fields = new List<FieldSpec>
        {
            new() { Key="task", Label="Task", Value=task.Name, ReadOnly=true },
            new() { Key="progress", Label="Progress %", Kind=FieldKind.Number, Value=task.ProgressPercent.ToString("0.##"),
                    Hint="0 to 100. At 100 the task is marked completed." },
            new() { Key="remarks", Label="Remarks", Kind=FieldKind.Multiline, Value=task.Remarks,
                    Hint="Optional. What was done, or why it is delayed." }
        };

        using var editor = new RecordEditor("Update progress", fields, saveText: "Update");
        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        if (editor.IsNotANumber("progress"))
        {
            Warn("Progress must be a number.");
            return;
        }

        Run(() => _service.UpdateProgress(AppSession.Require, task.Id, editor.Decimal_("progress"), editor.Text_("remarks")));
    }

    // ============================================================ BOQ

    private void EditEstimate(ProjectEstimate? existing)
    {
        var project = CurrentProject;
        if (project is null) return;

        var isNew = existing is null;
        var line = existing ?? new ProjectEstimate { ProjectId = project.Id };

        var materials = _catalog.GetMaterials();
        var materialItems = materials.Select(m => new Item(m.Id, $"{m.Code} — {m.Name} ({m.Unit})")).ToList();
        var taskItems = new List<Item> { new(null, "(whole project)") };
        taskItems.AddRange(_tasks.Select(t => new Item(t.Id, t.Name)));

        var currentMaterial = materialItems.FirstOrDefault(i => i.Id == line.MaterialId);
        var currentTask = taskItems.FirstOrDefault(i => i.Id == line.TaskId);

        var fields = new List<FieldSpec>
        {
            new() { Key="task", Label="For task", Kind=FieldKind.Combo, Options=taskItems.Cast<object>(),
                    Value=currentTask?.Text },
            new() { Key="material", Label="Material", Kind=FieldKind.Combo, Options=materialItems.Cast<object>(),
                    Value=currentMaterial?.Text },
            new() { Key="qty", Label="Planned quantity", Kind=FieldKind.Number,
                    Value=isNew ? "" : line.PlannedQty.ToString("0.###") },
            new() { Key="cost", Label="Unit cost", Kind=FieldKind.Money,
                    Value=isNew ? 0m : line.PlannedUnitCost,
                    Hint="Leave 0 to use the material's reference price." }
        };

        using var editor = new RecordEditor(isNew ? "Add material to estimate" : "Edit estimate line", fields);
        if (editor.ShowDialog(FindForm()) != DialogResult.OK) return;

        if (editor.IsNotANumber("qty") || editor.IsNotANumber("cost"))
        {
            Warn("Quantity and unit cost must be numbers.");
            return;
        }

        var material = editor.Pick<Item>("material");
        if (material?.Id is not int materialId) { Warn("Choose a material."); return; }

        var unitCost = editor.Decimal_("cost");
        if (unitCost == 0) unitCost = materials.First(m => m.Id == materialId).LastUnitCost;

        line.MaterialId = materialId;
        line.TaskId = editor.Pick<Item>("task")?.Id;
        line.PlannedQty = editor.Decimal_("qty");
        line.PlannedUnitCost = unitCost;

        Run(() => _service.SaveEstimate(AppSession.Require, line));
    }

    private void RemoveEstimate()
    {
        var line = SelectedEstimate();
        if (line is null) { Warn("Select an estimate line first."); return; }

        if (MessageBox.Show(this, $"Remove {line.MaterialName} from this project's estimate?",
                "Remove line", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        Run(() => _service.DeleteEstimate(AppSession.Require, line.Id));
    }

    // ============================================================ helpers

    private void Run(Action action)
    {
        try
        {
            action();
            ReloadAll();
        }
        catch (Exception ex)
        {
            Warn(ex.Message);
        }
    }

    private void Warn(string message) =>
        MessageBox.Show(this, message, "CPMMS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
