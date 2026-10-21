using System.Data;
using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>
/// The "monitoring" side of a project: its schedule of tasks with weighted
/// progress, and its Bill of Quantities (the materials it is planned to use).
///
/// Who may change what: the Administrator can change any project; a Project
/// Engineer only the projects assigned to them. That rule is enforced here, on
/// every write, not just by hiding buttons on a screen.
/// </summary>
public sealed class ProjectMonitoringService
{
    private const string NoDemo = "Project monitoring needs the database. It is not available in demo mode.";

    // ------------------------------------------------------------ access

    public IReadOnlyList<Project> GetProjectsFor(User user) =>
        new CatalogService().GetProjects(null, user.IsAdmin ? null : user.Id);

    private static void RequireAccess(IDbConnection cn, IDbTransaction? tx, User user, int projectId)
    {
        var exists = cn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM projects WHERE id = @projectId", new { projectId }, tx);
        if (exists == 0) throw new InvalidOperationException("That project no longer exists.");
        if (user.IsAdmin) return;

        var mine = cn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM projects WHERE id = @projectId AND project_engineer_id = @userId",
            new { projectId, userId = user.Id }, tx);
        if (mine == 0)
            throw new InvalidOperationException("You can only change the projects assigned to you.");
    }

    private static void Log(IDbConnection cn, IDbTransaction tx, User user,
                            string action, string entity, int entityId, string description)
    {
        if (description.Length > 250) description = description[..250];
        cn.Execute(
            @"INSERT INTO activity_logs (user_id, action, entity_type, entity_id, description)
              VALUES (@userId, @action, @entity, @entityId, @description)",
            new { userId = user.Id, action, entity, entityId, description }, tx);
    }

    private static string ProjectCode(IDbConnection cn, IDbTransaction tx, int projectId) =>
        cn.ExecuteScalar<string?>("SELECT code FROM projects WHERE id = @projectId", new { projectId }, tx) ?? "";

    // ------------------------------------------------------------ tasks

    public IReadOnlyList<ProjectTask> GetTasks(int projectId)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<ProjectTask>(
            @"SELECT id, project_id, name, weight_percent, start_date, due_date,
                     progress_percent, status, remarks
              FROM   project_tasks
              WHERE  project_id = @projectId
              ORDER  BY sequence_no, id", new { projectId });
    }

    /// <summary>The weighted roll-up: each task counts in proportion to its weight.</summary>
    public static decimal OverallProgress(IEnumerable<ProjectTask> tasks)
    {
        var list = tasks.ToList();
        var weights = list.Sum(t => t.WeightPercent);
        return weights == 0 ? 0 : Math.Round(list.Sum(t => t.ProgressPercent * t.WeightPercent) / weights, 2);
    }

    /// <summary>Status follows progress by itself; the only manual state is 'on hold'.</summary>
    private static string DeriveStatus(ProjectTask t) =>
        t.Status == "on_hold" ? "on_hold"
        : t.ProgressPercent >= 100 ? "completed"
        : t.ProgressPercent > 0 ? "in_progress"
        : "not_started";

    private static void ValidateTask(ProjectTask t)
    {
        t.Name = (t.Name ?? "").Trim();
        if (t.Name.Length == 0) throw new InvalidOperationException("A task needs a name.");
        if (t.Name.Length > 160) throw new InvalidOperationException("Keep the task name under 160 characters.");
        if (t.WeightPercent <= 0 || t.WeightPercent > 100)
            throw new InvalidOperationException("The weight must be more than 0 and at most 100.");
        if (t.ProgressPercent < 0 || t.ProgressPercent > 100)
            throw new InvalidOperationException("Progress must be between 0 and 100.");
        if (t.StartDate is { } s && t.DueDate is { } d && d.Date < s.Date)
            throw new InvalidOperationException("The due date can't be before the start date.");
        t.Remarks = string.IsNullOrWhiteSpace(t.Remarks) ? null : t.Remarks.Trim();
        if (t.Remarks is { Length: > 255 })
            throw new InvalidOperationException("Keep the remarks under 255 characters.");
    }

    /// <summary>Adds a task (Id = 0) or changes one. Returns the task id.</summary>
    public int SaveTask(User user, ProjectTask task)
    {
        ValidateTask(task);
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            RequireAccess(cn, tx, user, task.ProjectId);

            // the weights of a project's tasks share one 100%
            var others = cn.ExecuteScalar<decimal>(
                @"SELECT COALESCE(SUM(weight_percent), 0) FROM project_tasks
                  WHERE project_id = @ProjectId AND id <> @Id FOR UPDATE", task, tx);
            if (others + task.WeightPercent > 100m)
                throw new InvalidOperationException(
                    $"The task weights would total {others + task.WeightPercent:N2}%, but a project's tasks can't exceed 100%. " +
                    $"{100m - others:N2}% is still available.");

            task.Status = DeriveStatus(task);
            string action;

            if (task.Id == 0)
            {
                var seq = cn.ExecuteScalar<int>(
                    "SELECT COALESCE(MAX(sequence_no), 0) + 1 FROM project_tasks WHERE project_id = @ProjectId", task, tx);
                cn.Execute(
                    @"INSERT INTO project_tasks (project_id, name, sequence_no, weight_percent, start_date, due_date,
                                                 progress_percent, status, remarks)
                      VALUES (@ProjectId, @Name, @seq, @WeightPercent, @StartDate, @DueDate,
                              @ProgressPercent, @Status, @Remarks)",
                    new { task.ProjectId, task.Name, seq, task.WeightPercent, task.StartDate, task.DueDate,
                          task.ProgressPercent, task.Status, task.Remarks }, tx);
                task.Id = (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()", transaction: tx);
                action = "created";
            }
            else
            {
                var changed = cn.Execute(
                    @"UPDATE project_tasks
                      SET    name = @Name, weight_percent = @WeightPercent, start_date = @StartDate,
                             due_date = @DueDate, progress_percent = @ProgressPercent,
                             status = @Status, remarks = @Remarks
                      WHERE  id = @Id AND project_id = @ProjectId", task, tx);
                if (changed == 0) throw new InvalidOperationException("That task no longer exists.");
                action = "updated";
            }

            Log(cn, tx, user, action, "project_task", task.Id,
                $"{ProjectCode(cn, tx, task.ProjectId)}: {task.Name} ({task.ProgressPercent:N0}%)");
            tx.Commit();
            return task.Id;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>The engineer's everyday action: move a task's progress and say why.</summary>
    public void UpdateProgress(User user, int taskId, decimal progress, string? remarks)
    {
        if (progress < 0 || progress > 100)
            throw new InvalidOperationException("Progress must be between 0 and 100.");
        remarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        if (remarks is { Length: > 255 })
            throw new InvalidOperationException("Keep the remarks under 255 characters.");
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var task = cn.QuerySingleOrDefault<ProjectTask>(
                @"SELECT id, project_id, name, weight_percent, start_date, due_date,
                         progress_percent, status, remarks
                  FROM project_tasks WHERE id = @taskId FOR UPDATE", new { taskId }, tx);
            if (task is null) throw new InvalidOperationException("That task no longer exists.");

            RequireAccess(cn, tx, user, task.ProjectId);

            var before = task.ProgressPercent;
            task.ProgressPercent = progress;
            task.Status = DeriveStatus(task);

            cn.Execute(
                @"UPDATE project_tasks SET progress_percent = @ProgressPercent, status = @Status,
                         remarks = COALESCE(@remarks, remarks)
                  WHERE id = @Id", new { task.ProgressPercent, task.Status, remarks, task.Id }, tx);

            Log(cn, tx, user, "progress", "project_task", task.Id,
                $"{ProjectCode(cn, tx, task.ProjectId)}: {task.Name} {before:N0}% -> {progress:N0}%");
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    // ------------------------------------------------------------ BOQ

    public IReadOnlyList<ProjectEstimate> GetEstimates(int projectId)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        return DatabaseHelper.Query<ProjectEstimate>(
            @"SELECT e.id, e.project_id, e.task_id, t.name AS task_name,
                     e.material_id, m.code AS material_code, m.name AS material_name, m.unit,
                     e.planned_qty, e.planned_unit_cost, e.planned_cost
              FROM   project_material_estimates e
              JOIN   materials m ON m.id = e.material_id
              LEFT   JOIN project_tasks t ON t.id = e.task_id
              WHERE  e.project_id = @projectId
              ORDER  BY COALESCE(t.sequence_no, 0), m.name", new { projectId });
    }

    /// <summary>Adds an estimate line (Id = 0) or changes one. Returns the line id.</summary>
    public int SaveEstimate(User user, ProjectEstimate e)
    {
        if (e.PlannedQty <= 0) throw new InvalidOperationException("The planned quantity must be more than zero.");
        if (e.PlannedUnitCost < 0) throw new InvalidOperationException("The unit cost cannot be negative.");
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            RequireAccess(cn, tx, user, e.ProjectId);

            var materialOk = cn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM materials WHERE id = @MaterialId", e, tx);
            if (materialOk == 0) throw new InvalidOperationException("Choose a material.");

            if (e.TaskId is not null)
            {
                var taskOk = cn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM project_tasks WHERE id = @TaskId AND project_id = @ProjectId", e, tx);
                if (taskOk == 0) throw new InvalidOperationException("That task doesn't belong to this project.");
            }

            // one line per material per task (MySQL's unique key ignores NULL tasks, so check by hand)
            var duplicate = cn.ExecuteScalar<int>(
                @"SELECT COUNT(*) FROM project_material_estimates
                  WHERE project_id = @ProjectId AND material_id = @MaterialId AND id <> @Id
                    AND ((task_id IS NULL AND @TaskId IS NULL) OR task_id = @TaskId)", e, tx);
            if (duplicate > 0)
                throw new InvalidOperationException("That material is already estimated for the same task. Edit that line instead.");

            string action;
            if (e.Id == 0)
            {
                cn.Execute(
                    @"INSERT INTO project_material_estimates (project_id, task_id, material_id, planned_qty, planned_unit_cost)
                      VALUES (@ProjectId, @TaskId, @MaterialId, @PlannedQty, @PlannedUnitCost)", e, tx);
                e.Id = (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()", transaction: tx);
                action = "created";
            }
            else
            {
                var changed = cn.Execute(
                    @"UPDATE project_material_estimates
                      SET task_id = @TaskId, material_id = @MaterialId,
                          planned_qty = @PlannedQty, planned_unit_cost = @PlannedUnitCost
                      WHERE id = @Id AND project_id = @ProjectId", e, tx);
                if (changed == 0) throw new InvalidOperationException("That estimate line no longer exists.");
                action = "updated";
            }

            Log(cn, tx, user, action, "project_estimate", e.Id,
                $"{ProjectCode(cn, tx, e.ProjectId)}: {e.PlannedQty:N2} x material #{e.MaterialId} @ {e.PlannedUnitCost:N2}");
            tx.Commit();
            return e.Id;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    /// <summary>Estimates are planning data, not posted records, so a wrong line may be removed (and the removal is logged).</summary>
    public void DeleteEstimate(User user, int estimateId)
    {
        if (DemoMode.Enabled) throw new InvalidOperationException(NoDemo);

        using var cn = DatabaseHelper.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            var projectId = cn.ExecuteScalar<int?>(
                "SELECT project_id FROM project_material_estimates WHERE id = @estimateId FOR UPDATE",
                new { estimateId }, tx);
            if (projectId is null) throw new InvalidOperationException("That estimate line no longer exists.");

            RequireAccess(cn, tx, user, projectId.Value);

            cn.Execute("DELETE FROM project_material_estimates WHERE id = @estimateId", new { estimateId }, tx);
            Log(cn, tx, user, "removed", "project_estimate", estimateId,
                $"{ProjectCode(cn, tx, projectId.Value)}: estimate line #{estimateId} removed");
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
