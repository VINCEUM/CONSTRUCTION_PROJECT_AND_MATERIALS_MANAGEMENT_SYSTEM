using CPMMS.Core.Models;

namespace CPMMS.Core;


public static class DemoMode
{
    public static bool Enabled { get; private set; }
    public static void Enable() => Enabled = true;
    public static void Disable() => Enabled = false;
}

public static class DemoData
{
    public static readonly List<Material> Materials = new()
    {
        new() { Id=1,  CategoryId=1, Code="CEM-001", Name="Portland Cement Type 1",    Unit="bag",   LastUnitCost=285,  CurrentStock=177,  MinimumStock=50,  CategoryName="Cement & Aggregates" },
        new() { Id=2,  CategoryId=1, Code="AGG-001", Name="Washed Sand",               Unit="cu.m",  LastUnitCost=1200, CurrentStock=32,   MinimumStock=10,  CategoryName="Cement & Aggregates" },
        new() { Id=3,  CategoryId=1, Code="AGG-002", Name="Gravel 3/4\"",              Unit="cu.m",  LastUnitCost=1400, CurrentStock=30,   MinimumStock=10,  CategoryName="Cement & Aggregates" },
        new() { Id=4,  CategoryId=1, Code="CHB-004", Name="Concrete Hollow Block 4\"", Unit="pc",    LastUnitCost=18,   CurrentStock=1200, MinimumStock=500, CategoryName="Cement & Aggregates" },
        new() { Id=5,  CategoryId=1, Code="CHB-006", Name="Concrete Hollow Block 6\"", Unit="pc",    LastUnitCost=24,   CurrentStock=450,  MinimumStock=300, CategoryName="Cement & Aggregates" },
        new() { Id=6,  CategoryId=2, Code="STL-010", Name="Deformed Bar 10mm x 6m",    Unit="pc",    LastUnitCost=180,  CurrentStock=80,   MinimumStock=100, CategoryName="Steel & Rebar" },
        new() { Id=7,  CategoryId=2, Code="STL-012", Name="Deformed Bar 12mm x 6m",    Unit="pc",    LastUnitCost=262,  CurrentStock=180,  MinimumStock=100, CategoryName="Steel & Rebar" },
        new() { Id=8,  CategoryId=2, Code="STL-016", Name="Deformed Bar 16mm x 6m",    Unit="pc",    LastUnitCost=465,  CurrentStock=80,   MinimumStock=60,  CategoryName="Steel & Rebar" },
        new() { Id=9,  CategoryId=2, Code="STL-TW1", Name="G.I. Tie Wire #16",         Unit="kg",    LastUnitCost=95,   CurrentStock=23,   MinimumStock=20,  CategoryName="Steel & Rebar" },
        new() { Id=10, CategoryId=3, Code="LUM-001", Name="Coco Lumber 2x3x10",        Unit="pc",    LastUnitCost=150,  CurrentStock=40,   MinimumStock=50,  CategoryName="Lumber & Plywood" },
        new() { Id=11, CategoryId=3, Code="PLY-004", Name="Plywood 1/4\" 4x8",         Unit="sheet", LastUnitCost=480,  CurrentStock=46,   MinimumStock=20,  CategoryName="Lumber & Plywood" },
        new() { Id=12, CategoryId=3, Code="PLY-012", Name="Marine Plywood 1/2\" 4x8",  Unit="sheet", LastUnitCost=980,  CurrentStock=54,   MinimumStock=15,  CategoryName="Lumber & Plywood" },
        new() { Id=13, CategoryId=4, Code="ELE-012", Name="THHN Wire #12 (150m box)",  Unit="box",   LastUnitCost=2800, CurrentStock=4,    MinimumStock=5,   CategoryName="Electrical" },
        new() { Id=14, CategoryId=4, Code="ELE-PVC", Name="PVC Conduit 1/2\" x 3m",    Unit="pc",    LastUnitCost=85,   CurrentStock=24,   MinimumStock=30,  CategoryName="Electrical" },
        new() { Id=15, CategoryId=4, Code="ELE-OUT", Name="Duplex Outlet, flush type", Unit="set",   LastUnitCost=165,  CurrentStock=12,   MinimumStock=20,  CategoryName="Electrical" },
        new() { Id=16, CategoryId=5, Code="PLB-004", Name="PVC Pipe 4\" x 3m (S1000)", Unit="pc",    LastUnitCost=420,  CurrentStock=24,   MinimumStock=20,  CategoryName="Plumbing" },
        new() { Id=17, CategoryId=5, Code="PLB-ELB", Name="PVC Elbow 4\" 90deg",       Unit="pc",    LastUnitCost=78,   CurrentStock=40,   MinimumStock=30,  CategoryName="Plumbing" },
        new() { Id=18, CategoryId=6, Code="FIN-PNT", Name="Latex Paint, white",        Unit="gal",   LastUnitCost=1250, CurrentStock=16,   MinimumStock=10,  CategoryName="Finishing" },
        new() { Id=19, CategoryId=6, Code="FIN-TIL", Name="Ceramic Floor Tile 60x60",  Unit="pc",    LastUnitCost=215,  CurrentStock=140,  MinimumStock=100, CategoryName="Finishing" },
        new() { Id=20, CategoryId=7, Code="HDW-CWN", Name="Common Wire Nail 3\"",      Unit="kg",    LastUnitCost=78,   CurrentStock=34,   MinimumStock=25,  CategoryName="Hardware" },
    };

    public static readonly List<Project> Projects = new()
    {
        new() { Id=1, Code="PRJ-2026-001", Name="Two-Storey Residential House", ClientName="Juan Dela Cruz",
                Location="Brgy. Magugpo East, Tagum City", ContractAmount=2500000, MaterialBudget=1450000,
                StartDate=new(2026,9,1), TargetEndDate=new(2027,3,30), Status="ongoing", EngineerName="Engr. Karla M. Dizon" },
        new() { Id=2, Code="PRJ-2026-002", Name="Commercial Building Renovation", ClientName="Sunrise Enterprises",
                Location="Apokon Road, Tagum City", ContractAmount=1800000, MaterialBudget=980000,
                StartDate=new(2026,8,15), TargetEndDate=new(2026,12,20), Status="ongoing", EngineerName="Engr. Nico P. Alcantara" },
        new() { Id=3, Code="PRJ-2026-003", Name="Perimeter Fence and Drainage", ClientName="Sto. Nino Parish",
                Location="Brgy. Visayan Village, Tagum City", ContractAmount=650000, MaterialBudget=420000,
                StartDate=new(2026,6,1), TargetEndDate=new(2026,8,15), ActualEndDate=new(2026,8,10),
                Status="completed", EngineerName="Engr. Karla M. Dizon" },
    };

    public static readonly List<ProjectCostRow> ProjectCosts = new()
    {
        new() { Id=1, Code="PRJ-2026-001", Name="Two-Storey Residential House", ClientName="Juan Dela Cruz",
                Status="ongoing", ContractAmount=2500000, MaterialBudget=1450000,
                MaterialCostToDate=120585, OtherExpenses=122500, MaterialBudgetRemaining=1329415, ProgressPercent=49 },
        new() { Id=2, Code="PRJ-2026-002", Name="Commercial Building Renovation", ClientName="Sunrise Enterprises",
                Status="ongoing", ContractAmount=1800000, MaterialBudget=980000,
                MaterialCostToDate=17570, OtherExpenses=64000, MaterialBudgetRemaining=962430, ProgressPercent=59.5m },
        new() { Id=3, Code="PRJ-2026-003", Name="Perimeter Fence and Drainage", ClientName="Sto. Nino Parish",
                Status="completed", ContractAmount=650000, MaterialBudget=420000,
                MaterialCostToDate=389420, OtherExpenses=142000, MaterialBudgetRemaining=30580, ProgressPercent=100 },
    };

    public static readonly List<VarianceRow> Variance = new()
    {
        V(1,"PRJ-2026-001",1,"CEM-001","Portland Cement Type 1","bag",  320,285, 120),
        V(1,"PRJ-2026-001",2,"AGG-001","Washed Sand","cu.m",              8,1200, 10),
        V(1,"PRJ-2026-001",3,"AGG-002","Gravel 3/4\"","cu.m",            34,1400, 12),
        V(1,"PRJ-2026-001",6,"STL-010","Deformed Bar 10mm x 6m","pc",   180,180,  60),
        V(1,"PRJ-2026-001",9,"STL-TW1","G.I. Tie Wire #16","kg",         12, 95,  15),
        V(1,"PRJ-2026-001",7,"STL-012","Deformed Bar 12mm x 6m","pc",   240,262,  60),
        V(1,"PRJ-2026-001",10,"LUM-001","Coco Lumber 2x3x10","pc",      180,150,  80),
        V(1,"PRJ-2026-001",12,"PLY-012","Marine Plywood 1/2\" 4x8","sheet",60,980, 18, 4),
        V(2,"PRJ-2026-002",13,"ELE-012","THHN Wire #12 (150m box)","box", 12,2800, 4),
        V(2,"PRJ-2026-002",14,"ELE-PVC","PVC Conduit 1/2\" x 3m","pc",  120, 85,  40),
        V(2,"PRJ-2026-002",15,"ELE-OUT","Duplex Outlet, flush type","set",48,165, 18),
    };

    private static VarianceRow V(int projectId, string projectCode, int materialId, string code, string name,
                                 string unit, decimal planned, decimal unitCost, decimal issued, decimal returned = 0)
    {
        var actualQty = issued - returned;
        var actualCost = actualQty * unitCost;
        var plannedCost = planned * unitCost;
        return new VarianceRow
        {
            ProjectId = projectId,
            ProjectCode = projectCode,
            ProjectName = Projects.First(p => p.Id == projectId).Name,
            MaterialId = materialId,
            MaterialCode = code,
            MaterialName = name,
            Unit = unit,
            PlannedQty = planned,
            PlannedCost = plannedCost,
            IssuedQty = issued,
            ReturnedQty = returned,
            ActualQty = actualQty,
            ActualCost = actualCost,
            QtyVariance = actualQty - planned,
            CostVariance = actualCost - plannedCost,
            VariancePercent = planned == 0 ? null : Math.Round((actualQty - planned) / planned * 100, 2)
        };
    }

    public static readonly List<MaterialRequest> Requests = new()
    {
        new() { Id=1, RequestNo="MR-2026-0001", ProjectId=1, ProjectName="Two-Storey Residential House",
                RequesterName="Engr. Karla M. Dizon", RequestDate=new(2026,9,16), NeededDate=new(2026,9,19),
                Status="issued", Remarks="Footing pour, phase 1" },
        new() { Id=2, RequestNo="MR-2026-0002", ProjectId=1, ProjectName="Two-Storey Residential House",
                RequesterName="Engr. Karla M. Dizon", RequestDate=new(2026,10,2), NeededDate=new(2026,10,6),
                Status="partially_issued", Remarks="Column rebar and forms" },
        new() { Id=3, RequestNo="MR-2026-0003", ProjectId=2, ProjectName="Commercial Building Renovation",
                RequesterName="Engr. Nico P. Alcantara", RequestDate=new(2026,9,28), NeededDate=new(2026,10,2),
                Status="issued", Remarks="Second floor rewiring" },
        new() { Id=4, RequestNo="MR-2026-0004", ProjectId=1, ProjectName="Two-Storey Residential House",
                RequesterName="Engr. Karla M. Dizon", RequestDate=new(2026,10,14), NeededDate=new(2026,10,18),
                Status="approved", Remarks="Beam forms — ready to issue" },
        new() { Id=5, RequestNo="MR-2026-0005", ProjectId=2, ProjectName="Commercial Building Renovation",
                RequesterName="Engr. Nico P. Alcantara", RequestDate=new(2026,10,20), NeededDate=new(2026,10,24),
                Status="submitted", Remarks="Panel board upgrade" },
    };

    public static readonly Dictionary<int, List<MaterialRequestItem>> RequestItems = new()
    {
        [1] = new() { RI(1,1,1,120,120), RI(2,1,2,10,10), RI(3,1,3,12,12), RI(4,1,6,60,60), RI(5,1,9,15,15) },
        [2] = new() { RI(6,2,7,90,60), RI(7,2,10,80,80), RI(8,2,12,24,18) },
        [3] = new() { RI(9,3,13,4,4), RI(10,3,14,40,40), RI(11,3,15,18,18) },
        [4] = new() { RI(12,4,11,30,0), RI(13,4,20,12,0) },
    };

    private static MaterialRequestItem RI(int id, int requestId, int materialId, decimal requested, decimal issued)
    {
        var m = Materials.First(x => x.Id == materialId);
        return new MaterialRequestItem
        {
            Id = id, RequestId = requestId, MaterialId = materialId,
            QtyRequested = requested, QtyIssued = issued,
            MaterialCode = m.Code, MaterialName = m.Name, Unit = m.Unit,
            CurrentStock = m.CurrentStock, LastUnitCost = m.LastUnitCost
        };
    }

    public static readonly List<PurchaseOrderRow> PurchaseOrders = new()
    {
        new() { Id=1, PoNo="PO-2026-0001", SupplierId=1, SupplierName="Davao Builders Supply Inc.",
                OrderDate=new(2026,10,3), ExpectedDate=new(2026,10,8), Status="received",
                TotalAmount=70640, LineCount=2, OutstandingLines=0 },
        new() { Id=2, PoNo="PO-2026-0002", SupplierId=3, SupplierName="Southern Aggregates Trading",
                OrderDate=new(2026,10,5), ExpectedDate=new(2026,10,7), Status="received",
                TotalAmount=46400, LineCount=2, OutstandingLines=0 },
        new() { Id=3, PoNo="PO-2026-0003", SupplierId=2, SupplierName="Tagum Hardware & Construction Supply",
                OrderDate=new(2026,10,12), ExpectedDate=new(2026,10,18), Status="partially_received",
                TotalAmount=67800, LineCount=2, OutstandingLines=2 },
        new() { Id=4, PoNo="PO-2026-0004", SupplierId=2, SupplierName="Tagum Hardware & Construction Supply",
                OrderDate=new(2026,10,15), ExpectedDate=new(2026,10,22), Status="sent",
                TotalAmount=15336, LineCount=2, OutstandingLines=2 },
    };

    public static readonly Dictionary<int, List<PurchaseOrderItemRow>> PurchaseOrderItems = new()
    {
        [1] = new() { PI(1,1,7,120,120,262), PI(2,1,12,40,40,980) },
        [2] = new() { PI(3,2,2,20,20,1200), PI(4,2,3,16,16,1400) },
        [3] = new() { PI(5,3,1,200,120,285), PI(6,3,4,600,0,18) },
        [4] = new() { PI(7,4,11,30,0,480), PI(8,4,20,12,0,78) },
    };

    private static PurchaseOrderItemRow PI(int id, int poId, int materialId, decimal ordered, decimal received, decimal price)
    {
        var m = Materials.First(x => x.Id == materialId);
        return new PurchaseOrderItemRow
        {
            Id = id, PoId = poId, MaterialId = materialId,
            QtyOrdered = ordered, QtyReceived = received, UnitPrice = price,
            MaterialCode = m.Code, MaterialName = m.Name, Unit = m.Unit
        };
    }

    /// <summary>Ledger rows, kept per material so the stock card has something real to show.</summary>
    public static readonly List<LedgerRow> Ledger = new()
    {
        L(1, 1,"OPENING",  180, 285, 180, null, "opening", "Opening count 01 Sep 2026", new(2026,9,1)),
        L(2, 1,"ISSUED",  -120, 285,  60, 1,    "issue",   "IS-2026-0001",             new(2026,9,19)),
        L(3, 1,"RECEIVED", 120, 285, 180, null, "delivery","DR-77510 partial",          new(2026,10,18)),
        L(4, 1,"ADJUSTMENT", -3, 285,177, null, "adjustment","ADJ-2026-0001 month-end count", new(2026,10,31)),
        L(5, 12,"OPENING",  28, 980,  28, null, "opening", "Opening count 01 Sep 2026", new(2026,9,1)),
        L(6, 12,"RECEIVED", 40, 980,  68, null, "delivery","DR-88412",                  new(2026,10,8)),
        L(7, 12,"ISSUED",  -18, 980,  50, 1,    "issue",   "IS-2026-0002",              new(2026,10,9)),
        L(8, 12,"RETURNED",  4, 980,  54, 1,    "return",  "RT-2026-0001 reusable",     new(2026,10,21)),
    };

    private static LedgerRow L(long id, int materialId, string type, decimal qty, decimal cost, decimal balance,
                               int? projectId, string refType, string remarks, DateTime when)
    {
        var m = Materials.First(x => x.Id == materialId);
        return new LedgerRow
        {
            Id = id, MaterialId = materialId, TxnType = type, Quantity = qty, UnitCost = cost,
            BalanceAfter = balance, ProjectId = projectId, ReferenceType = refType, Remarks = remarks,
            CreatedAt = when, MaterialName = m.Name,
            ProjectName = projectId is null ? null : Projects.First(p => p.Id == projectId).Name,
            PerformedByName = "Mario B. Sagun"
        };
    }

    /// <summary>
    /// Materials with no seeded movements still need a stock card that adds up,
    /// so their opening balance is generated on demand.
    /// </summary>
    public static List<LedgerRow> LedgerFor(int materialId)
    {
        var rows = Ledger.Where(l => l.MaterialId == materialId).ToList();
        if (rows.Count > 0) return rows;

        var m = Materials.First(x => x.Id == materialId);
        return new List<LedgerRow>
        {
            L(0, materialId, "OPENING", m.CurrentStock, m.LastUnitCost, m.CurrentStock,
              null, "opening", "Opening count 01 Sep 2026", new DateTime(2026, 9, 1))
        };
    }

    // ------------------------------------------------------------------ derived

    public static List<StockCheckRow> StockCheck(bool lowStockOnly) =>
        Materials
            .Where(m => !lowStockOnly || m.CurrentStock <= m.MinimumStock)
            .Select(m => new StockCheckRow
            {
                MaterialId = m.Id, Code = m.Code, Name = m.Name, Unit = m.Unit,
                CachedStock = m.CurrentStock,
                LedgerStock = m.CurrentStock,    // they agree, which is the point
                Difference = 0,
                MinimumStock = m.MinimumStock,
                IsLowStock = m.CurrentStock <= m.MinimumStock ? 1 : 0
            })
            .OrderBy(r => r.Name)
            .ToList();

    public static DashboardTotals Totals() => new()
    {
        ProjectCount = Projects.Count,
        OngoingProjects = Projects.Count(p => p.Status == "ongoing"),
        MaterialCount = Materials.Count,
        SupplierCount = 3,
        LowStockCount = Materials.Count(m => m.CurrentStock <= m.MinimumStock),
        PendingRequests = Requests.Count(r => r.Status == "submitted"),
        TotalContractValue = Projects.Where(p => p.Status is "ongoing" or "planning").Sum(p => p.ContractAmount),
        StockValue = Materials.Sum(m => m.CurrentStock * m.LastUnitCost)
    };

    /// <summary>
    /// Applies an issuance to the in-memory data so the demo behaves like the
    /// real thing: stock drops, the ledger grows, the request advances.
    /// </summary>
    public static int ApplyIssue(int requestId, IList<IssueLine> lines, DateTime when)
    {
        var request = Requests.First(r => r.Id == requestId);

        foreach (var line in lines)
        {
            var material = Materials.First(m => m.Id == line.MaterialId);
            if (line.Qty > material.CurrentStock)
                throw new InvalidOperationException(
                    $"Not enough {material.Name} in stock. Available {material.CurrentStock:N2}, requested {line.Qty:N2}.");
        }

        foreach (var line in lines)
        {
            var material = Materials.First(m => m.Id == line.MaterialId);
            material.CurrentStock -= line.Qty;

            Ledger.Insert(0, L(Ledger.Count + 1, material.Id, "ISSUED", -line.Qty, line.UnitCost,
                               material.CurrentStock, request.ProjectId, "issue", "demo issuance", when));

            if (RequestItems.TryGetValue(requestId, out var items))
            {
                var item = items.FirstOrDefault(i => i.MaterialId == line.MaterialId);
                if (item is not null)
                {
                    item.QtyIssued += line.Qty;
                    item.CurrentStock = material.CurrentStock;
                }
            }
        }

        if (RequestItems.TryGetValue(requestId, out var all))
            request.Status = all.All(i => i.QtyIssued >= i.QtyRequested) ? "issued" : "partially_issued";

        return 9000 + Ledger.Count;
    }

    /// <summary>Same idea for receiving against a purchase order.</summary>
    public static int ApplyReceive(int poId, IList<ReceiveLine> lines, DateTime when)
    {
        foreach (var line in lines)
        {
            var material = Materials.First(m => m.Id == line.MaterialId);
            material.CurrentStock += line.Qty;
            material.LastUnitCost = line.UnitPrice;

            Ledger.Insert(0, L(Ledger.Count + 1, material.Id, "RECEIVED", line.Qty, line.UnitPrice,
                               material.CurrentStock, null, "delivery", "demo delivery", when));

            if (PurchaseOrderItems.TryGetValue(poId, out var items))
            {
                var item = items.FirstOrDefault(i => i.Id == line.PoItemId);
                if (item is not null) item.QtyReceived += line.Qty;
            }
        }

        var po = PurchaseOrders.First(p => p.Id == poId);
        if (PurchaseOrderItems.TryGetValue(poId, out var all))
        {
            po.OutstandingLines = all.Count(i => i.QtyReceived < i.QtyOrdered);
            po.Status = po.OutstandingLines == 0 ? "received" : "partially_received";
        }

        return 8000 + Ledger.Count;
    }

    public static readonly List<Supplier> Suppliers = new()
    {
        new() { Id=1, CompanyName="Davao Builders Supply Inc.", ContactPerson="Lourdes Ancheta",
                ContactNo="082-225-4410", Email="sales@davaobuilders.test",
                Address="Quimpo Blvd., Davao City", Status="active", OrderCount=1 },
        new() { Id=2, CompanyName="Tagum Hardware & Construction Supply", ContactPerson="Ricardo Mendez",
                ContactNo="084-216-7788", Email="rmendez@tagumhardware.test",
                Address="National Highway, Tagum City", Status="active", OrderCount=2 },
        new() { Id=3, CompanyName="Southern Aggregates Trading", ContactPerson="Precious Dalisay",
                ContactNo="082-333-9021", Email="orders@southernaggregates.test",
                Address="Panabo City, Davao del Norte", Status="active", OrderCount=1 },
    };

    public static int SaveSupplier(Supplier s)
    {
        if (s.Id == 0)
        {
            s.Id = Suppliers.Max(x => x.Id) + 1;
            Suppliers.Add(s);
        }
        else
        {
            var existing = Suppliers.First(x => x.Id == s.Id);
            existing.CompanyName = s.CompanyName; existing.ContactPerson = s.ContactPerson;
            existing.ContactNo = s.ContactNo; existing.Email = s.Email;
            existing.Address = s.Address; existing.Status = s.Status; existing.Remarks = s.Remarks;
        }
        return s.Id;
    }

    public static int SaveMaterial(Material m)
    {
        if (m.Id == 0)
        {
            m.Id = Materials.Max(x => x.Id) + 1;
            m.CurrentStock = 0;                 // new materials start empty, same as the real rule
            Materials.Add(m);
        }
        else
        {
            var existing = Materials.First(x => x.Id == m.Id);
            existing.CategoryId = m.CategoryId; existing.CategoryName = m.CategoryName;
            existing.Code = m.Code; existing.Name = m.Name; existing.Unit = m.Unit;
            existing.LastUnitCost = m.LastUnitCost; existing.MinimumStock = m.MinimumStock;
            existing.Status = m.Status;
        }
        return m.Id;
    }

    public static int SaveProject(Project p)
    {
        if (p.Id == 0)
        {
            p.Id = Projects.Max(x => x.Id) + 1;
            Projects.Add(p);
            ProjectCosts.Add(new ProjectCostRow
            {
                Id = p.Id, Code = p.Code, Name = p.Name, ClientName = p.ClientName, Status = p.Status,
                ContractAmount = p.ContractAmount, MaterialBudget = p.MaterialBudget,
                MaterialBudgetRemaining = p.MaterialBudget
            });
        }
        else
        {
            var existing = Projects.First(x => x.Id == p.Id);
            existing.Code = p.Code; existing.Name = p.Name; existing.ClientName = p.ClientName;
            existing.Location = p.Location; existing.ContractAmount = p.ContractAmount;
            existing.MaterialBudget = p.MaterialBudget; existing.StartDate = p.StartDate;
            existing.TargetEndDate = p.TargetEndDate; existing.ActualEndDate = p.ActualEndDate;
            existing.Status = p.Status; existing.Description = p.Description;

            var cost = ProjectCosts.FirstOrDefault(c => c.Id == p.Id);
            if (cost is not null)
            {
                cost.Code = p.Code; cost.Name = p.Name; cost.ClientName = p.ClientName;
                cost.Status = p.Status; cost.ContractAmount = p.ContractAmount;
                cost.MaterialBudget = p.MaterialBudget;
                cost.MaterialBudgetRemaining = p.MaterialBudget - cost.MaterialCostToDate;
            }
        }
        return p.Id;
    }

    public static int SaveUser(User u)
    {
        if (u.Id == 0)
        {
            u.Id = Users.Max(x => x.Id) + 1;
            Users.Add(u);
        }
        else
        {
            var existing = Users.First(x => x.Id == u.Id);
            existing.RoleId = u.RoleId; existing.RoleName = u.RoleName;
            existing.FullName = u.FullName; existing.Email = u.Email;
            existing.ContactNo = u.ContactNo; existing.Status = u.Status;
        }
        return u.Id;
    }

    public static readonly List<User> Users = new()
    {
        new() { Id=1, RoleId=1, FullName="Engr. Ramon T. Villareal", Email="admin@buildcorp.test",       RoleName="Admin" },
        new() { Id=2, RoleId=2, FullName="Engr. Karla M. Dizon",     Email="karla.dizon@buildcorp.test", RoleName="Project Engineer" },
        new() { Id=3, RoleId=2, FullName="Engr. Nico P. Alcantara",  Email="nico.alcantara@buildcorp.test", RoleName="Project Engineer" },
        new() { Id=4, RoleId=3, FullName="Mario B. Sagun",           Email="mario.sagun@buildcorp.test", RoleName="Storekeeper" },
    };
}
