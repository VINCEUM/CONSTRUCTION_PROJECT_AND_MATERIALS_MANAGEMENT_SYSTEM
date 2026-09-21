-- ============================================================================
--  CPMMS · demo data
--  Enough to make every report non-empty: two live projects, one finished,
--  a full purchase cycle, issuances, a return and a physical count.
--
--  Balances are not typed by hand. Transactions are inserted with a zero
--  balance, then the running balance and the cached stock are computed from
--  the ledger itself (bottom of this file) — the same rule the application
--  follows.
-- ============================================================================

USE construction_mms;

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE activity_logs;
TRUNCATE TABLE inventory_transactions;
TRUNCATE TABLE stock_adjustment_items;
TRUNCATE TABLE stock_adjustments;
TRUNCATE TABLE material_return_items;
TRUNCATE TABLE material_returns;
TRUNCATE TABLE material_issue_items;
TRUNCATE TABLE material_issues;
TRUNCATE TABLE delivery_items;
TRUNCATE TABLE deliveries;
TRUNCATE TABLE purchase_order_items;
TRUNCATE TABLE purchase_orders;
TRUNCATE TABLE material_request_items;
TRUNCATE TABLE material_requests;
TRUNCATE TABLE project_material_estimates;
TRUNCATE TABLE project_expenses;
TRUNCATE TABLE project_tasks;
TRUNCATE TABLE projects;
TRUNCATE TABLE supplier_materials;
TRUNCATE TABLE materials;
TRUNCATE TABLE material_categories;
TRUNCATE TABLE suppliers;
TRUNCATE TABLE users;
TRUNCATE TABLE roles;
TRUNCATE TABLE settings;
SET FOREIGN_KEY_CHECKS = 1;

-- ---------------------------------------------------------------------------
-- roles and users   (all demo passwords are "password")
-- ---------------------------------------------------------------------------
INSERT INTO roles (id, name, description) VALUES
 (1,'Admin','Full access: users, suppliers, approvals, reports, settings'),
 (2,'Project Engineer','Projects, tasks, estimates, material requests, progress'),
 (3,'Storekeeper','Receiving, issuance, returns, physical count');

INSERT INTO users (id, role_id, full_name, email, password, contact_no, status) VALUES
 (1,1,'Engr. Ramon T. Villareal','admin@buildcorp.test','$2a$11$ydikIdVlcBs.8SCoHLNT3OPedddOgf2pNmwp8tJCsmEUAwrldP7Dy','0917-555-0101','active'),
 (2,2,'Engr. Karla M. Dizon','karla.dizon@buildcorp.test','$2a$11$ydikIdVlcBs.8SCoHLNT3OPedddOgf2pNmwp8tJCsmEUAwrldP7Dy','0917-555-0102','active'),
 (3,2,'Engr. Nico P. Alcantara','nico.alcantara@buildcorp.test','$2a$11$ydikIdVlcBs.8SCoHLNT3OPedddOgf2pNmwp8tJCsmEUAwrldP7Dy','0917-555-0103','active'),
 (4,3,'Mario B. Sagun','mario.sagun@buildcorp.test','$2a$11$ydikIdVlcBs.8SCoHLNT3OPedddOgf2pNmwp8tJCsmEUAwrldP7Dy','0917-555-0104','active');

-- ---------------------------------------------------------------------------
-- reference data
-- ---------------------------------------------------------------------------
INSERT INTO material_categories (id, name, description) VALUES
 (1,'Cement & Aggregates','Cement, sand, gravel, hollow blocks'),
 (2,'Steel & Rebar','Deformed bars, tie wire, angle bars'),
 (3,'Lumber & Plywood','Form lumber, plywood, coco lumber'),
 (4,'Electrical','Wires, conduits, outlets, breakers'),
 (5,'Plumbing','PVC pipes, fittings, fixtures'),
 (6,'Finishing','Paint, tiles, adhesives'),
 (7,'Hardware','Nails, screws, consumables');

INSERT INTO suppliers (id, company_name, contact_person, contact_no, email, address, status) VALUES
 (1,'Davao Builders Supply Inc.','Lourdes Ancheta','082-225-4410','sales@davaobuilders.test','Quimpo Blvd., Davao City','active'),
 (2,'Tagum Hardware & Construction Supply','Ricardo Mendez','084-216-7788','rmendez@tagumhardware.test','National Highway, Tagum City','active'),
 (3,'Southern Aggregates Trading','Precious Dalisay','082-333-9021','orders@southernaggregates.test','Panabo City, Davao del Norte','active');

INSERT INTO materials (id, category_id, code, name, unit, last_unit_cost, current_stock, minimum_stock) VALUES
 (1 ,1,'CEM-001','Portland Cement Type 1','bag'    , 285.00, 0, 50),
 (2 ,1,'AGG-001','Washed Sand','cu.m'              ,1200.00, 0, 10),
 (3 ,1,'AGG-002','Gravel 3/4"','cu.m'              ,1400.00, 0, 10),
 (4 ,1,'CHB-004','Concrete Hollow Block 4"','pc'   ,  18.00, 0,500),
 (5 ,1,'CHB-006','Concrete Hollow Block 6"','pc'   ,  24.00, 0,300),
 (6 ,2,'STL-010','Deformed Bar 10mm x 6m','pc'     , 180.00, 0,100),
 (7 ,2,'STL-012','Deformed Bar 12mm x 6m','pc'     , 262.00, 0,100),
 (8 ,2,'STL-016','Deformed Bar 16mm x 6m','pc'     , 465.00, 0, 60),
 (9 ,2,'STL-TW1','G.I. Tie Wire #16','kg'          ,  95.00, 0, 20),
 (10,3,'LUM-001','Coco Lumber 2x3x10','pc'         , 150.00, 0, 50),
 (11,3,'PLY-004','Plywood 1/4" 4x8','sheet'        , 480.00, 0, 20),
 (12,3,'PLY-012','Marine Plywood 1/2" 4x8','sheet' , 980.00, 0, 15),
 (13,4,'ELE-012','THHN Wire #12 (150m box)','box'  ,2800.00, 0,  5),
 (14,4,'ELE-PVC','PVC Conduit 1/2" x 3m','pc'      ,  85.00, 0, 30),
 (15,4,'ELE-OUT','Duplex Outlet, flush type','set' , 165.00, 0, 20),
 (16,5,'PLB-004','PVC Pipe 4" x 3m (S1000)','pc'   , 420.00, 0, 20),
 (17,5,'PLB-ELB','PVC Elbow 4" 90deg','pc'         ,  78.00, 0, 30),
 (18,6,'FIN-PNT','Latex Paint, white','gal'        ,1250.00, 0, 10),
 (19,6,'FIN-TIL','Ceramic Floor Tile 60x60','pc'   , 215.00, 0,100),
 (20,7,'HDW-CWN','Common Wire Nail 3"','kg'        ,  78.00, 0, 25);

INSERT INTO supplier_materials (supplier_id, material_id, quoted_price, lead_time_days, quoted_at) VALUES
 (1, 1, 285.00, 2,'2026-08-15'), (2, 1, 292.00, 1,'2026-08-15'),
 (1, 6, 180.00, 3,'2026-08-15'), (2, 6, 176.00, 4,'2026-08-15'),
 (1, 7, 262.00, 3,'2026-08-15'), (2, 7, 265.00, 4,'2026-08-15'),
 (3, 2,1200.00, 1,'2026-08-20'), (3, 3,1400.00, 1,'2026-08-20'),
 (2, 4,  18.00, 2,'2026-08-20'), (2, 5,  24.00, 2,'2026-08-20'),
 (1,11, 480.00, 2,'2026-08-22'), (1,12, 980.00, 2,'2026-08-22'),
 (2,13,2800.00, 5,'2026-08-22'), (2,16, 420.00, 3,'2026-08-22');

INSERT INTO settings (setting_key, setting_value, description) VALUES
 ('company_name','BuildCorp Construction Services','Shown on printed documents'),
 ('costing_method','last_purchase_price','How unit cost is snapshotted at issuance'),
 ('allow_negative_stock','0','Issuing more than available is rejected'),
 ('low_stock_alert','1','Warn when stock reaches the reorder point'),
 ('wastage_alert_percent','10','Flag a material when actual exceeds planned by this %');

-- ---------------------------------------------------------------------------
-- projects
-- ---------------------------------------------------------------------------
INSERT INTO projects (id, code, name, client_name, location, contract_amount, material_budget,
                      start_date, target_end_date, actual_end_date, status, project_engineer_id, description) VALUES
 (1,'PRJ-2026-001','Two-Storey Residential House','Juan Dela Cruz','Brgy. Magugpo East, Tagum City',
     2500000.00, 1450000.00,'2026-09-01','2027-03-30',NULL,'ongoing',2,
     '180 sqm two-storey residence, reinforced concrete frame with CHB walls.'),
 (2,'PRJ-2026-002','Commercial Building Renovation','Sunrise Enterprises','Apokon Road, Tagum City',
     1800000.00,  980000.00,'2026-08-15','2026-12-20',NULL,'ongoing',3,
     'Interior renovation and electrical upgrade of a two-storey commercial building.'),
 (3,'PRJ-2026-003','Perimeter Fence and Drainage','Sto. Nino Parish','Brgy. Visayan Village, Tagum City',
      650000.00,  420000.00,'2026-06-01','2026-08-15','2026-08-10','completed',2,
     '120-metre perimeter fence with box-culvert drainage.');

INSERT INTO project_tasks (id, project_id, name, sequence_no, weight_percent, start_date, due_date, progress_percent, status) VALUES
 (1,1,'Site Works and Excavation',1,10.00,'2026-09-01','2026-09-20',100.00,'completed'),
 (2,1,'Foundation and Footing'   ,2,20.00,'2026-09-15','2026-10-25', 85.00,'in_progress'),
 (3,1,'Structural Framing'       ,3,25.00,'2026-10-20','2026-12-15', 40.00,'in_progress'),
 (4,1,'Masonry and Walls'        ,4,15.00,'2026-11-15','2027-01-20',  0.00,'not_started'),
 (5,1,'Roofing'                  ,5,10.00,'2027-01-05','2027-02-10',  0.00,'not_started'),
 (6,1,'Electrical and Plumbing'  ,6,10.00,'2027-01-15','2027-02-28',  0.00,'not_started'),
 (7,1,'Finishing and Painting'   ,7,10.00,'2027-02-01','2027-03-25',  0.00,'not_started'),
 (8,2,'Demolition and Hauling'   ,1,15.00,'2026-08-15','2026-09-05',100.00,'completed'),
 (9,2,'Partition and Drywall'    ,2,30.00,'2026-09-01','2026-10-15', 70.00,'in_progress'),
 (10,2,'Electrical Rewiring'     ,3,30.00,'2026-09-20','2026-11-10', 45.00,'in_progress'),
 (11,2,'Painting and Finishing'  ,4,25.00,'2026-11-01','2026-12-15',  0.00,'not_started'),
 (12,3,'Excavation'              ,1,20.00,'2026-06-01','2026-06-20',100.00,'completed'),
 (13,3,'Fence Construction'      ,2,50.00,'2026-06-15','2026-07-30',100.00,'completed'),
 (14,3,'Drainage Box Culvert'    ,3,30.00,'2026-07-01','2026-08-10',100.00,'completed');

-- the planned side (Bill of Quantities) for the residential house
INSERT INTO project_material_estimates (project_id, task_id, material_id, planned_qty, planned_unit_cost) VALUES
 (1, 2, 1, 320.000, 285.00),   -- cement, foundation
 (1, 2, 2,   8.000,1200.00),
 (1, 2, 3,  34.000,1400.00),
 (1, 2, 6, 180.000, 180.00),
 (1, 2, 8, 120.000, 465.00),
 (1, 2, 9,  12.000,  95.00),
 (1, 3, 1, 260.000, 285.00),   -- cement, framing
 (1, 3, 7, 240.000, 262.00),
 (1, 3,10, 180.000, 150.00),
 (1, 3,12,  60.000, 980.00),
 (1, 4, 1, 140.000, 285.00),
 (1, 4, 4,2800.000,  18.00),
 (1, 4, 5, 900.000,  24.00),
 (2, 9,11,  85.000, 480.00),
 (2, 9,20,  40.000,  78.00),
 (2,10,13,  12.000,2800.00),
 (2,10,14, 120.000,  85.00),
 (2,10,15,  48.000, 165.00);

-- ---------------------------------------------------------------------------
-- opening stock  (what the bodega held before the system went live)
-- ---------------------------------------------------------------------------
INSERT INTO inventory_transactions
 (material_id, txn_type, quantity, unit_cost, balance_after, project_id, reference_type, reference_id, performed_by, remarks)
VALUES
 (1 ,'OPENING', 180.000, 285.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (2 ,'OPENING',  22.000,1200.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (3 ,'OPENING',  26.000,1400.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (4 ,'OPENING',1200.000,  18.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (5 ,'OPENING', 450.000,  24.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (6 ,'OPENING', 140.000, 180.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (7 ,'OPENING', 120.000, 262.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (8 ,'OPENING',  80.000, 465.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (9 ,'OPENING',  38.000,  95.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (10,'OPENING', 120.000, 150.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (11,'OPENING',  46.000, 480.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (12,'OPENING',  28.000, 980.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (13,'OPENING',   8.000,2800.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (14,'OPENING',  64.000,  85.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (15,'OPENING',  30.000, 165.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (16,'OPENING',  24.000, 420.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (17,'OPENING',  40.000,  78.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (18,'OPENING',  16.000,1250.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (19,'OPENING', 140.000, 215.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026'),
 (20,'OPENING',  34.000,  78.00, 0, NULL,'opening',NULL,1,'Opening count 01 Sep 2026');

-- ---------------------------------------------------------------------------
-- material requests
-- ---------------------------------------------------------------------------
INSERT INTO material_requests (id, request_no, project_id, task_id, requested_by, request_date, needed_date,
                               status, approved_by, approved_at, remarks) VALUES
 (1,'MR-2026-0001',1,2,2,'2026-09-16','2026-09-19','issued'          ,1,'2026-09-16 14:05:00','Footing pour, phase 1'),
 (2,'MR-2026-0002',1,2,2,'2026-10-02','2026-10-06','partially_issued',1,'2026-10-02 09:40:00','Column rebar and forms'),
 (3,'MR-2026-0003',2,10,3,'2026-09-28','2026-10-02','issued'         ,1,'2026-09-28 11:15:00','Second floor rewiring'),
 (4,'MR-2026-0004',1,3,2,'2026-10-14','2026-10-18','pending'         ,NULL,NULL,'Beam forms, awaiting approval');

INSERT INTO material_request_items (request_id, material_id, qty_requested, qty_issued, remarks) VALUES
 (1, 1,120.000,120.000,NULL),
 (1, 2, 10.000, 10.000,NULL),
 (1, 3, 12.000, 12.000,NULL),
 (1, 6, 60.000, 60.000,NULL),
 (1, 9, 15.000, 15.000,NULL),
 (2, 7, 90.000, 60.000,'Balance to follow after delivery'),
 (2,10, 80.000, 80.000,NULL),
 (2,12, 24.000, 18.000,'Short by 6 sheets'),
 (3,13,  4.000,  4.000,NULL),
 (3,14, 40.000, 40.000,NULL),
 (3,15, 18.000, 18.000,NULL),
 (4,11, 30.000,  0.000,NULL),
 (4,20, 12.000,  0.000,NULL);

-- ---------------------------------------------------------------------------
-- purchasing: PO -> delivery -> stock in
-- ---------------------------------------------------------------------------
INSERT INTO purchase_orders (id, po_no, supplier_id, request_id, order_date, expected_date, status,
                             total_amount, prepared_by, approved_by, approved_at, remarks) VALUES
 (1,'PO-2026-0001',1,2,'2026-10-03','2026-10-08','received'          , 70640.00,1,1,'2026-10-03 10:00:00','Rebar and marine plywood'),
 (2,'PO-2026-0002',3,NULL,'2026-10-05','2026-10-07','received'       , 46400.00,1,1,'2026-10-05 08:30:00','Sand and gravel restock'),
 (3,'PO-2026-0003',2,NULL,'2026-10-12','2026-10-18','partially_received', 67800.00,1,1,'2026-10-12 15:20:00','Cement and CHB restock'),
 (4,'PO-2026-0004',2,4,'2026-10-15','2026-10-22','sent'              , 15336.00,1,NULL,NULL,'Awaiting approval of MR-2026-0004');

INSERT INTO purchase_order_items (id, po_id, material_id, qty_ordered, qty_received, unit_price) VALUES
 (1,1, 7,120.000,120.000, 262.00),
 (2,1,12, 40.000, 40.000, 980.00),
 (3,2, 2, 20.000, 20.000,1200.00),
 (4,2, 3, 16.000, 16.000,1400.00),
 (5,3, 1,200.000,120.000, 285.00),
 (6,3, 4,600.000,  0.000,  18.00),
 (7,4,11, 30.000,  0.000, 480.00),
 (8,4,20, 12.000,  0.000,  78.00);

INSERT INTO deliveries (id, dr_no, po_id, delivery_date, received_by, photo_path, remarks, status) VALUES
 (1,'DR-88412',1,'2026-10-08',4,'/storage/deliveries/dr-88412.jpg','Complete per PO',            'posted'),
 (2,'DR-10233',2,'2026-10-07',4,'/storage/deliveries/dr-10233.jpg','Complete per PO',            'posted'),
 (3,'DR-77510',3,'2026-10-18',4,'/storage/deliveries/dr-77510.jpg','Partial: 120 of 200 bags cement; CHB to follow','posted');

INSERT INTO delivery_items (delivery_id, po_item_id, material_id, qty_received, unit_price, remarks) VALUES
 (1,1, 7,120.000, 262.00,NULL),
 (1,2,12, 40.000, 980.00,NULL),
 (2,3, 2, 20.000,1200.00,NULL),
 (2,4, 3, 16.000,1400.00,NULL),
 (3,5, 1,120.000, 285.00,'Balance 80 bags on backorder');

INSERT INTO inventory_transactions
 (material_id, txn_type, quantity, unit_cost, balance_after, project_id, reference_type, reference_id, performed_by, remarks)
VALUES
 ( 7,'RECEIVED',120.000, 262.00,0,NULL,'delivery',1,4,'DR-88412'),
 (12,'RECEIVED', 40.000, 980.00,0,NULL,'delivery',1,4,'DR-88412'),
 ( 2,'RECEIVED', 20.000,1200.00,0,NULL,'delivery',2,4,'DR-10233'),
 ( 3,'RECEIVED', 16.000,1400.00,0,NULL,'delivery',2,4,'DR-10233'),
 ( 1,'RECEIVED',120.000, 285.00,0,NULL,'delivery',3,4,'DR-77510 partial');

-- ---------------------------------------------------------------------------
-- issuance: stock leaving for a project
-- ---------------------------------------------------------------------------
INSERT INTO material_issues (id, issue_no, request_id, project_id, issue_date, issued_by, received_by, total_cost, status) VALUES
 (1,'IS-2026-0001',1,1,'2026-09-19',4,'Foreman Delfin Rosales', 75225.00,'posted'),
 (2,'IS-2026-0002',2,1,'2026-10-09',4,'Foreman Delfin Rosales', 45360.00,'posted'),
 (3,'IS-2026-0003',3,2,'2026-10-02',4,'Electrician Boyet Lim' , 17570.00,'posted');

INSERT INTO material_issue_items (issue_id, request_item_id, material_id, qty_issued, unit_cost) VALUES
 (1,1, 1,120.000, 285.00),
 (1,2, 2, 10.000,1200.00),
 (1,3, 3, 12.000,1400.00),
 (1,4, 6, 60.000, 180.00),
 (1,5, 9, 15.000,  95.00),
 (2,6, 7, 60.000, 262.00),
 (2,7,10, 80.000, 150.00),
 (2,8,12, 18.000, 980.00),
 (3,9,13,  4.000,2800.00),
 (3,10,14,40.000,  85.00),
 (3,11,15,18.000, 165.00);

INSERT INTO inventory_transactions
 (material_id, txn_type, quantity, unit_cost, balance_after, project_id, reference_type, reference_id, performed_by, remarks)
VALUES
 ( 1,'ISSUED',-120.000, 285.00,0,1,'issue',1,4,'IS-2026-0001'),
 ( 2,'ISSUED', -10.000,1200.00,0,1,'issue',1,4,'IS-2026-0001'),
 ( 3,'ISSUED', -12.000,1400.00,0,1,'issue',1,4,'IS-2026-0001'),
 ( 6,'ISSUED', -60.000, 180.00,0,1,'issue',1,4,'IS-2026-0001'),
 ( 9,'ISSUED', -15.000,  95.00,0,1,'issue',1,4,'IS-2026-0001'),
 ( 7,'ISSUED', -60.000, 262.00,0,1,'issue',2,4,'IS-2026-0002'),
 (10,'ISSUED', -80.000, 150.00,0,1,'issue',2,4,'IS-2026-0002'),
 (12,'ISSUED', -18.000, 980.00,0,1,'issue',2,4,'IS-2026-0002'),
 (13,'ISSUED',  -4.000,2800.00,0,2,'issue',3,4,'IS-2026-0003'),
 (14,'ISSUED', -40.000,  85.00,0,2,'issue',3,4,'IS-2026-0003'),
 (15,'ISSUED', -18.000, 165.00,0,2,'issue',3,4,'IS-2026-0003');

-- ---------------------------------------------------------------------------
-- return: 6 sheets of plywood come back, 2 of them damaged
-- ---------------------------------------------------------------------------
INSERT INTO material_returns (id, return_no, project_id, issue_id, return_date, returned_by, accepted_by, condition_note, status) VALUES
 (1,'RT-2026-0001',1,2,'2026-10-21','Foreman Delfin Rosales',4,'Forms stripped early; 2 sheets warped by rain','posted');

INSERT INTO material_return_items (return_id, material_id, qty_returned, unit_cost, disposition) VALUES
 (1,12,4.000,980.00,'reusable'),
 (1,12,2.000,980.00,'damaged');

INSERT INTO inventory_transactions
 (material_id, txn_type, quantity, unit_cost, balance_after, project_id, reference_type, reference_id, performed_by, remarks)
VALUES
 (12,'RETURNED', 4.000, 980.00,0,1,'return',1,4,'RT-2026-0001 reusable');
-- No ledger row for the 2 warped sheets: they left stock when they were issued
-- and never came back. They stay charged to the project, which is how wastage
-- shows up in v_project_material_variance. The DAMAGED type is for goods that
-- spoil while still IN the bodega.

-- ---------------------------------------------------------------------------
-- physical count: the bodega is 3 bags short of what the system says
-- ---------------------------------------------------------------------------
INSERT INTO stock_adjustments (id, adjustment_no, count_date, counted_by, approved_by, reason, status) VALUES
 (1,'ADJ-2026-0001','2026-10-31',4,1,'Month-end physical count','posted');

INSERT INTO stock_adjustment_items (adjustment_id, material_id, system_qty, counted_qty, remarks) VALUES
 (1, 1,180.000,177.000,'3 bags hardened in storage'),
 (1, 4,1200.000,1200.000,'Tallies'),
 (1, 9, 23.000, 23.000,'Tallies');

INSERT INTO inventory_transactions
 (material_id, txn_type, quantity, unit_cost, balance_after, project_id, reference_type, reference_id, performed_by, remarks)
VALUES
 (1,'ADJUSTMENT',-3.000,285.00,0,NULL,'adjustment',1,4,'ADJ-2026-0001 month-end count');

-- ---------------------------------------------------------------------------
-- project expenses  (materials posted from issuances, the rest entered by hand)
-- ---------------------------------------------------------------------------
INSERT INTO project_expenses (project_id, category, description, amount, expense_date, source, reference_id, recorded_by) VALUES
 (1,'materials','Material issuance IS-2026-0001', 75225.00,'2026-09-19','material_issue',1,4),
 (1,'materials','Material issuance IS-2026-0002', 45360.00,'2026-10-09','material_issue',2,4),
 (2,'materials','Material issuance IS-2026-0003', 17570.00,'2026-10-02','material_issue',3,4),
 (1,'labor','Masonry crew, September payroll',    96000.00,'2026-09-30','manual',NULL,1),
 (1,'equipment','Concrete mixer rental, 12 days', 18000.00,'2026-10-05','manual',NULL,1),
 (1,'transportation','Hauling of aggregates',      8500.00,'2026-10-07','manual',NULL,1),
 (2,'labor','Electrical subcontractor, phase 1',   64000.00,'2026-10-15','manual',NULL,1),
 (3,'materials','Closed project, materials total',389420.00,'2026-08-10','manual',NULL,1),
 (3,'labor','Closed project, labor total',        142000.00,'2026-08-10','manual',NULL,1);

-- ---------------------------------------------------------------------------
-- derive the running balances and the cached stock FROM the ledger
-- (this is exactly what the application does inside one transaction)
-- ---------------------------------------------------------------------------
UPDATE inventory_transactions t
JOIN (
  SELECT id,
         SUM(quantity) OVER (PARTITION BY material_id ORDER BY id
                             ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS running
  FROM inventory_transactions
) r ON r.id = t.id
SET t.balance_after = r.running;

UPDATE materials m
JOIN (
  SELECT material_id, SUM(quantity) AS qty
  FROM inventory_transactions
  GROUP BY material_id
) l ON l.material_id = m.id
SET m.current_stock = l.qty;

-- document totals are derived from their line items, never typed
UPDATE purchase_orders po
JOIN (SELECT po_id, SUM(qty_ordered * unit_price) AS amt FROM purchase_order_items GROUP BY po_id) x
  ON x.po_id = po.id
SET po.total_amount = x.amt;

UPDATE material_issues i
JOIN (SELECT issue_id, SUM(qty_issued * unit_cost) AS amt FROM material_issue_items GROUP BY issue_id) x
  ON x.issue_id = i.id
SET i.total_cost = x.amt;

-- last purchase price becomes the material's reference cost
UPDATE materials m
JOIN (
  SELECT di.material_id, di.unit_price
  FROM delivery_items di
  JOIN deliveries d ON d.id = di.delivery_id AND d.status = 'posted'
  WHERE di.id IN (SELECT MAX(di2.id) FROM delivery_items di2 GROUP BY di2.material_id)
) lp ON lp.material_id = m.id
SET m.last_unit_cost = lp.unit_price;

-- ---------------------------------------------------------------------------
-- self-checks: both of these must return zero rows
-- ---------------------------------------------------------------------------
-- SELECT * FROM v_stock_check WHERE difference <> 0;
-- SELECT * FROM inventory_transactions WHERE balance_after < 0;
