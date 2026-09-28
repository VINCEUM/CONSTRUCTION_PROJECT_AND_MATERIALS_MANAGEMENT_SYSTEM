namespace CPMMS.Core.Models;

public class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";      // bcrypt hash
    public string? ContactNo { get; set; }
    public string Status { get; set; } = "active";
    public string RoleName { get; set; } = "";

    public bool IsAdmin => RoleName == "Admin";
    public bool IsEngineer => RoleName == "Project Engineer";
    public bool IsStorekeeper => RoleName == "Storekeeper";
}

public class Material
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal LastUnitCost { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public string Status { get; set; } = "active";
    public string CategoryName { get; set; } = "";

    public bool IsLowStock => CurrentStock <= MinimumStock;
    public decimal StockValue => CurrentStock * LastUnitCost;
}

/// <summary>Row of v_stock_check — cached stock next to the ledger's own total.</summary>
public class StockCheckRow
{
    public int MaterialId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal CachedStock { get; set; }
    public decimal LedgerStock { get; set; }
    public decimal Difference { get; set; }
    public decimal MinimumStock { get; set; }
    public int IsLowStock { get; set; }
}

public class Project
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string ClientName { get; set; } = "";
    public string Location { get; set; } = "";
    public decimal ContractAmount { get; set; }
    public decimal MaterialBudget { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = "planning";
    public int? ProjectEngineerId { get; set; }
    public string? Description { get; set; }
    public string? EngineerName { get; set; }
}

/// <summary>Row of v_project_cost_summary.</summary>
public class ProjectCostRow
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string ClientName { get; set; } = "";
    public string Status { get; set; } = "";
    public decimal ContractAmount { get; set; }
    public decimal MaterialBudget { get; set; }
    public decimal MaterialCostToDate { get; set; }
    public decimal OtherExpenses { get; set; }
    public decimal MaterialBudgetRemaining { get; set; }
    public decimal ProgressPercent { get; set; }
}

/// <summary>Row of v_project_material_variance — planned against actual.</summary>
public class VarianceRow
{
    public int ProjectId { get; set; }
    public string ProjectCode { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal PlannedQty { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal IssuedQty { get; set; }
    public decimal ReturnedQty { get; set; }
    public decimal ActualQty { get; set; }
    public decimal ActualCost { get; set; }
    public decimal QtyVariance { get; set; }
    public decimal CostVariance { get; set; }
    public decimal? VariancePercent { get; set; }
}

public class MaterialRequest
{
    public int Id { get; set; }
    public string RequestNo { get; set; } = "";
    public int ProjectId { get; set; }
    public int? TaskId { get; set; }
    public int RequestedBy { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? NeededDate { get; set; }
    public string Status { get; set; } = "pending";
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectReason { get; set; }
    public string? Remarks { get; set; }
    public string ProjectName { get; set; } = "";
    public string RequesterName { get; set; } = "";
}

public class MaterialRequestItem
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public int MaterialId { get; set; }
    public decimal QtyRequested { get; set; }
    public decimal QtyIssued { get; set; }
    public string? Remarks { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";
    public decimal CurrentStock { get; set; }
    public decimal LastUnitCost { get; set; }

    public decimal QtyOutstanding => QtyRequested - QtyIssued;
}

/// <summary>One line the storekeeper is about to release.</summary>
public class IssueLine
{
    public int MaterialId { get; set; }
    public int? RequestItemId { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitCost { get; set; }
}

public class LedgerRow
{
    public long Id { get; set; }
    public int MaterialId { get; set; }
    public string TxnType { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceAfter { get; set; }
    public int? ProjectId { get; set; }
    public string ReferenceType { get; set; } = "";
    public long? ReferenceId { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public string MaterialName { get; set; } = "";
    public string? ProjectName { get; set; }
    public string PerformedByName { get; set; } = "";
}

public class DashboardTotals
{
    public int ProjectCount { get; set; }
    public int OngoingProjects { get; set; }
    public int MaterialCount { get; set; }
    public int SupplierCount { get; set; }
    public int LowStockCount { get; set; }
    public int PendingRequests { get; set; }
    public decimal TotalContractValue { get; set; }
    public decimal StockValue { get; set; }
}

/// <summary>One line being received against a purchase order.</summary>
public class ReceiveLine
{
    public int PoItemId { get; set; }
    public int MaterialId { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>One line coming back from a site.</summary>
public class ReturnLine
{
    public int MaterialId { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitCost { get; set; }
    public string Disposition { get; set; } = "reusable";   // reusable | damaged
}

/// <summary>One line of a physical count.</summary>
public class CountLine
{
    public int MaterialId { get; set; }
    public decimal CountedQty { get; set; }
    public string? Remarks { get; set; }
}

public class PurchaseOrderRow
{
    public int Id { get; set; }
    public string PoNo { get; set; } = "";
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = "";
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public int LineCount { get; set; }
    public int OutstandingLines { get; set; }
}

public class PurchaseOrderItemRow
{
    public int Id { get; set; }
    public int PoId { get; set; }
    public int MaterialId { get; set; }
    public decimal QtyOrdered { get; set; }
    public decimal QtyReceived { get; set; }
    public decimal UnitPrice { get; set; }
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public string Unit { get; set; } = "";

    public decimal QtyOutstanding => QtyOrdered - QtyReceived;
}

public class Supplier
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = "";
    public string? ContactPerson { get; set; }
    public string? ContactNo { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "active";
    public string? Remarks { get; set; }
    public int OrderCount { get; set; }
}

public class MaterialCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string Status { get; set; } = "active";
    public override string ToString() => Name;
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public override string ToString() => Name;
}
