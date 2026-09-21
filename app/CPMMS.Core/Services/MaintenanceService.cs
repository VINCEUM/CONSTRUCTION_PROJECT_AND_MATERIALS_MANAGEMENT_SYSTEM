using CPMMS.Core.Models;
using Dapper;

namespace CPMMS.Core.Services;

/// <summary>
/// Add / edit / deactivate for the reference records: suppliers, materials,
/// projects and users.
///
/// One rule throughout: nothing is deleted. Records are deactivated, because a
/// supplier on an old purchase order or a material on an old issuance must stay
/// resolvable forever. The foreign keys are ON DELETE RESTRICT for the same
/// reason — a hard delete would either fail or tear a hole in the audit trail.
/// </summary>
public sealed class MaintenanceService
{
    // ------------------------------------------------------------- suppliers

    public IReadOnlyList<Supplier> GetSuppliers(bool includeInactive = false)
    {
        if (DemoMode.Enabled)
            return DemoData.Suppliers.Where(s => includeInactive || s.Status == "active").ToList();

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT s.*, (SELECT COUNT(*) FROM purchase_orders p WHERE p.supplier_id = s.id) AS order_count
                    FROM suppliers s WHERE 1 = 1";
        if (!includeInactive) sql += " AND s.status = 'active'";
        sql += " ORDER BY s.company_name";
        return cn.Query<Supplier>(sql).ToList();
    }

    public int SaveSupplier(Supplier supplier)
    {
        if (string.IsNullOrWhiteSpace(supplier.CompanyName))
            throw new ArgumentException("A supplier needs a company name.");

        if (DemoMode.Enabled) return DemoData.SaveSupplier(supplier);

        using var cn = DatabaseHelper.Open();
        if (supplier.Id == 0)
        {
            cn.Execute(
                @"INSERT INTO suppliers (company_name, contact_person, contact_no, email, address, status, remarks)
                  VALUES (@CompanyName, @ContactPerson, @ContactNo, @Email, @Address, @Status, @Remarks)",
                supplier);
            return (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()");
        }

        cn.Execute(
            @"UPDATE suppliers SET company_name=@CompanyName, contact_person=@ContactPerson,
                     contact_no=@ContactNo, email=@Email, address=@Address, status=@Status, remarks=@Remarks
              WHERE id=@Id", supplier);
        return supplier.Id;
    }

    public void SetSupplierStatus(int id, bool active)
    {
        if (DemoMode.Enabled)
        {
            var s = DemoData.Suppliers.First(x => x.Id == id);
            s.Status = active ? "active" : "inactive";
            return;
        }
        DatabaseHelper.Execute("UPDATE suppliers SET status=@status WHERE id=@id",
            new { status = active ? "active" : "inactive", id });
    }

    // ------------------------------------------------------------- materials

    public IReadOnlyList<MaterialCategory> GetCategories()
    {
        if (DemoMode.Enabled)
            return DemoData.Materials
                .Select(m => new MaterialCategory { Id = m.CategoryId, Name = m.CategoryName })
                .DistinctBy(c => c.Id).OrderBy(c => c.Name).ToList();

        using var cn = DatabaseHelper.Open();
        return cn.Query<MaterialCategory>(
            "SELECT id, name, description, status FROM material_categories WHERE status='active' ORDER BY name").ToList();
    }

    public int SaveMaterial(Material material)
    {
        if (string.IsNullOrWhiteSpace(material.Name)) throw new ArgumentException("A material needs a name.");
        if (string.IsNullOrWhiteSpace(material.Code)) throw new ArgumentException("A material needs a code.");
        if (string.IsNullOrWhiteSpace(material.Unit)) throw new ArgumentException("A material needs a unit.");
        if (material.MinimumStock < 0) throw new ArgumentException("The reorder level cannot be negative.");

        if (DemoMode.Enabled) return DemoData.SaveMaterial(material);

        using var cn = DatabaseHelper.Open();
        if (material.Id == 0)
        {
            // Opening stock is NOT settable here. A new material starts at zero
            // and only a delivery or a counted adjustment can move it, so the
            // ledger stays the single source of truth.
            cn.Execute(
                @"INSERT INTO materials (category_id, code, name, unit, last_unit_cost,
                                         current_stock, minimum_stock, status)
                  VALUES (@CategoryId, @Code, @Name, @Unit, @LastUnitCost, 0, @MinimumStock, @Status)",
                material);
            return (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()");
        }

        cn.Execute(
            @"UPDATE materials SET category_id=@CategoryId, code=@Code, name=@Name, unit=@Unit,
                     last_unit_cost=@LastUnitCost, minimum_stock=@MinimumStock, status=@Status
              WHERE id=@Id", material);
        return material.Id;
    }

    public void SetMaterialStatus(int id, bool active)
    {
        if (DemoMode.Enabled)
        {
            DemoData.Materials.First(m => m.Id == id).Status = active ? "active" : "inactive";
            return;
        }
        DatabaseHelper.Execute("UPDATE materials SET status=@status WHERE id=@id",
            new { status = active ? "active" : "inactive", id });
    }

    // -------------------------------------------------------------- projects

    public int SaveProject(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Name)) throw new ArgumentException("A project needs a name.");
        if (string.IsNullOrWhiteSpace(project.Code)) throw new ArgumentException("A project needs a code.");
        if (project.TargetEndDate < project.StartDate)
            throw new ArgumentException("The target end date cannot be before the start date.");

        if (DemoMode.Enabled) return DemoData.SaveProject(project);

        using var cn = DatabaseHelper.Open();
        if (project.Id == 0)
        {
            cn.Execute(
                @"INSERT INTO projects (code, name, client_name, location, contract_amount, material_budget,
                                        start_date, target_end_date, status, project_engineer_id, description)
                  VALUES (@Code, @Name, @ClientName, @Location, @ContractAmount, @MaterialBudget,
                          @StartDate, @TargetEndDate, @Status, @ProjectEngineerId, @Description)",
                project);
            return (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()");
        }

        cn.Execute(
            @"UPDATE projects SET code=@Code, name=@Name, client_name=@ClientName, location=@Location,
                     contract_amount=@ContractAmount, material_budget=@MaterialBudget,
                     start_date=@StartDate, target_end_date=@TargetEndDate, actual_end_date=@ActualEndDate,
                     status=@Status, project_engineer_id=@ProjectEngineerId, description=@Description
              WHERE id=@Id", project);
        return project.Id;
    }

    // ----------------------------------------------------------------- users

    public IReadOnlyList<User> GetUsers(bool includeInactive = true)
    {
        if (DemoMode.Enabled)
            return DemoData.Users.Where(u => includeInactive || u.Status == "active").ToList();

        using var cn = DatabaseHelper.Open();
        var sql = @"SELECT u.id, u.role_id, u.full_name, u.email, '' AS password, u.contact_no, u.status,
                           r.name AS role_name
                    FROM users u JOIN roles r ON r.id = u.role_id WHERE 1 = 1";
        if (!includeInactive) sql += " AND u.status = 'active'";
        sql += " ORDER BY r.name, u.full_name";
        return cn.Query<User>(sql).ToList();
    }

    public IReadOnlyList<Role> GetRoles()
    {
        if (DemoMode.Enabled)
            return DemoData.Users.Select(u => new Role { Id = u.RoleId, Name = u.RoleName })
                                 .DistinctBy(r => r.Id).OrderBy(r => r.Id).ToList();

        using var cn = DatabaseHelper.Open();
        return cn.Query<Role>("SELECT id, name, description FROM roles ORDER BY id").ToList();
    }

    /// <summary>Pass a plain password to set or reset it; leave it empty to keep the existing one.</summary>
    public int SaveUser(User user, string? newPassword)
    {
        if (string.IsNullOrWhiteSpace(user.FullName)) throw new ArgumentException("A user needs a full name.");
        if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("A user needs an email address.");
        if (user.Id == 0 && string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Set a password for the new account.");
        if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < 8)
            throw new ArgumentException("The password must be at least 8 characters.");

        if (DemoMode.Enabled) return DemoData.SaveUser(user);

        using var cn = DatabaseHelper.Open();

        if (user.Id == 0)
        {
            cn.Execute(
                @"INSERT INTO users (role_id, full_name, email, password, contact_no, status)
                  VALUES (@roleId, @fullName, @email, @password, @contactNo, @status)",
                new
                {
                    roleId = user.RoleId, fullName = user.FullName, email = user.Email,
                    password = AuthService.HashPassword(newPassword!),
                    contactNo = user.ContactNo, status = user.Status
                });
            return (int)cn.ExecuteScalar<ulong>("SELECT LAST_INSERT_ID()");
        }

        cn.Execute(
            @"UPDATE users SET role_id=@roleId, full_name=@fullName, email=@email,
                     contact_no=@contactNo, status=@status WHERE id=@id",
            new
            {
                roleId = user.RoleId, fullName = user.FullName, email = user.Email,
                contactNo = user.ContactNo, status = user.Status, id = user.Id
            });

        if (!string.IsNullOrWhiteSpace(newPassword))
            cn.Execute("UPDATE users SET password=@password WHERE id=@id",
                new { password = AuthService.HashPassword(newPassword), id = user.Id });

        return user.Id;
    }

    public void SetUserStatus(int id, bool active)
    {
        if (DemoMode.Enabled)
        {
            DemoData.Users.First(u => u.Id == id).Status = active ? "active" : "inactive";
            return;
        }
        DatabaseHelper.Execute("UPDATE users SET status=@status WHERE id=@id",
            new { status = active ? "active" : "inactive", id });
    }
}
