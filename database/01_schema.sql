-- ============================================================================
--  Construction Project and Materials Management System (CPMMS)
--  CCE 106 · Database schema · MySQL 8.0
--
--  Design principles
--   1. inventory_transactions is the SINGLE SOURCE OF TRUTH for stock.
--      materials.current_stock is a cached figure kept in step with it inside
--      one database transaction, never edited by hand.
--   2. Nothing is hard-deleted. Documents are voided/cancelled with a reason.
--   3. Unit cost is snapshotted onto every issuance, so a later price change
--      never rewrites what a finished project cost.
--   4. Stock may never go negative.
-- ============================================================================

DROP DATABASE IF EXISTS construction_mms;
CREATE DATABASE construction_mms
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_0900_ai_ci;
USE construction_mms;

-- ---------------------------------------------------------------------------
-- 1. ACCESS CONTROL
-- ---------------------------------------------------------------------------

CREATE TABLE roles (
  id              TINYINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  name            VARCHAR(40)  NOT NULL UNIQUE,   -- Admin, Project Engineer, Storekeeper
  description     VARCHAR(160) NULL,
  created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE users (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  role_id         TINYINT UNSIGNED NOT NULL,
  full_name       VARCHAR(120) NOT NULL,
  email           VARCHAR(160) NOT NULL UNIQUE,
  password        VARCHAR(255) NOT NULL,           -- bcrypt hash
  contact_no      VARCHAR(30)  NULL,
  status          ENUM('active','inactive') NOT NULL DEFAULT 'active',
  last_login_at   DATETIME     NULL,
  created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_users_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE RESTRICT,
  INDEX idx_users_role (role_id),
  INDEX idx_users_status (status)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 2. REFERENCE DATA
-- ---------------------------------------------------------------------------

CREATE TABLE material_categories (
  id              SMALLINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  name            VARCHAR(80)  NOT NULL UNIQUE,    -- Cement & Aggregates, Steel, Electrical...
  description     VARCHAR(200) NULL,
  status          ENUM('active','inactive') NOT NULL DEFAULT 'active'
) ENGINE=InnoDB;

CREATE TABLE suppliers (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  company_name    VARCHAR(160) NOT NULL,
  contact_person  VARCHAR(120) NULL,
  contact_no      VARCHAR(30)  NULL,
  email           VARCHAR(160) NULL,
  address         VARCHAR(255) NULL,
  status          ENUM('active','inactive') NOT NULL DEFAULT 'active',
  remarks         VARCHAR(255) NULL,
  created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_supplier_name (company_name),
  INDEX idx_suppliers_status (status)
) ENGINE=InnoDB;

CREATE TABLE materials (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  category_id     SMALLINT UNSIGNED NOT NULL,
  code            VARCHAR(24)  NOT NULL UNIQUE,     -- CEM-001, STL-010
  name            VARCHAR(160) NOT NULL,
  unit            VARCHAR(20)  NOT NULL,            -- bag, pc, cu.m, kg, m
  -- last_unit_cost is the most recent purchase price; issuance snapshots its
  -- own cost so history is never rewritten (see material_issue_items).
  last_unit_cost  DECIMAL(12,2) NOT NULL DEFAULT 0.00,
  current_stock   DECIMAL(14,3) NOT NULL DEFAULT 0.000,   -- cached; ledger is authoritative
  minimum_stock   DECIMAL(14,3) NOT NULL DEFAULT 0.000,   -- reorder point
  status          ENUM('active','inactive') NOT NULL DEFAULT 'active',
  created_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_materials_category FOREIGN KEY (category_id) REFERENCES material_categories(id) ON DELETE RESTRICT,
  CONSTRAINT chk_materials_stock_nonneg CHECK (current_stock >= 0),
  CONSTRAINT chk_materials_min_nonneg   CHECK (minimum_stock >= 0),
  INDEX idx_materials_category (category_id),
  INDEX idx_materials_name (name)
) ENGINE=InnoDB;

-- one material may come from several suppliers, each at its own price
CREATE TABLE supplier_materials (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  supplier_id     INT UNSIGNED NOT NULL,
  material_id     INT UNSIGNED NOT NULL,
  quoted_price    DECIMAL(12,2) NOT NULL,
  lead_time_days  SMALLINT UNSIGNED NULL,
  quoted_at       DATE NOT NULL,
  CONSTRAINT fk_sm_supplier FOREIGN KEY (supplier_id) REFERENCES suppliers(id) ON DELETE CASCADE,
  CONSTRAINT fk_sm_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE CASCADE,
  UNIQUE KEY uq_supplier_material_date (supplier_id, material_id, quoted_at),
  INDEX idx_sm_material (material_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 3. PROJECTS
-- ---------------------------------------------------------------------------

CREATE TABLE projects (
  id                INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  code              VARCHAR(24)  NOT NULL UNIQUE,   -- PRJ-2026-001
  name              VARCHAR(180) NOT NULL,
  client_name       VARCHAR(160) NOT NULL,
  location          VARCHAR(200) NOT NULL,
  contract_amount   DECIMAL(14,2) NOT NULL DEFAULT 0.00,
  material_budget   DECIMAL(14,2) NOT NULL DEFAULT 0.00,
  start_date        DATE NOT NULL,
  target_end_date   DATE NOT NULL,
  actual_end_date   DATE NULL,
  status            ENUM('planning','ongoing','on_hold','completed','cancelled')
                    NOT NULL DEFAULT 'planning',
  project_engineer_id INT UNSIGNED NULL,
  description       TEXT NULL,
  created_at        TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at        TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_projects_engineer FOREIGN KEY (project_engineer_id) REFERENCES users(id) ON DELETE SET NULL,
  CONSTRAINT chk_projects_dates CHECK (target_end_date >= start_date),
  INDEX idx_projects_status (status),
  INDEX idx_projects_engineer (project_engineer_id)
) ENGINE=InnoDB;

-- phases/tasks. weight drives the rolled-up project progress.
CREATE TABLE project_tasks (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  project_id      INT UNSIGNED NOT NULL,
  name            VARCHAR(160) NOT NULL,            -- Excavation, Foundation, Roofing
  sequence_no     SMALLINT UNSIGNED NOT NULL DEFAULT 1,
  weight_percent  DECIMAL(5,2) NOT NULL DEFAULT 0.00,  -- share of the whole project
  start_date      DATE NULL,
  due_date        DATE NULL,
  progress_percent DECIMAL(5,2) NOT NULL DEFAULT 0.00, -- entered by the engineer
  status          ENUM('not_started','in_progress','completed','on_hold')
                  NOT NULL DEFAULT 'not_started',
  remarks         VARCHAR(255) NULL,
  updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_tasks_project FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE CASCADE,
  CONSTRAINT chk_task_progress CHECK (progress_percent BETWEEN 0 AND 100),
  CONSTRAINT chk_task_weight   CHECK (weight_percent  BETWEEN 0 AND 100),
  INDEX idx_tasks_project (project_id, sequence_no)
) ENGINE=InnoDB;

-- the planned side: what the project SHOULD consume (Bill of Quantities)
CREATE TABLE project_material_estimates (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  project_id      INT UNSIGNED NOT NULL,
  task_id         INT UNSIGNED NULL,
  material_id     INT UNSIGNED NOT NULL,
  planned_qty     DECIMAL(14,3) NOT NULL,
  planned_unit_cost DECIMAL(12,2) NOT NULL,
  planned_cost    DECIMAL(14,2) AS (planned_qty * planned_unit_cost) STORED,
  CONSTRAINT fk_est_project  FOREIGN KEY (project_id)  REFERENCES projects(id) ON DELETE CASCADE,
  CONSTRAINT fk_est_task     FOREIGN KEY (task_id)     REFERENCES project_tasks(id) ON DELETE SET NULL,
  CONSTRAINT fk_est_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT chk_est_qty CHECK (planned_qty > 0),
  UNIQUE KEY uq_estimate (project_id, task_id, material_id),
  INDEX idx_est_material (material_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 4. MATERIAL REQUEST  (site asks for materials)
-- ---------------------------------------------------------------------------

CREATE TABLE material_requests (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  request_no      VARCHAR(24) NOT NULL UNIQUE,      -- MR-2026-0001
  project_id      INT UNSIGNED NOT NULL,
  task_id         INT UNSIGNED NULL,
  requested_by    INT UNSIGNED NOT NULL,
  request_date    DATE NOT NULL,
  needed_date     DATE NULL,
  status          ENUM('pending','approved','rejected','partially_issued','issued','closed','cancelled')
                  NOT NULL DEFAULT 'pending',
  approved_by     INT UNSIGNED NULL,
  approved_at     DATETIME NULL,
  reject_reason   VARCHAR(255) NULL,
  remarks         VARCHAR(255) NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_mr_project  FOREIGN KEY (project_id)   REFERENCES projects(id)  ON DELETE RESTRICT,
  CONSTRAINT fk_mr_task     FOREIGN KEY (task_id)      REFERENCES project_tasks(id) ON DELETE SET NULL,
  CONSTRAINT fk_mr_by       FOREIGN KEY (requested_by) REFERENCES users(id)     ON DELETE RESTRICT,
  CONSTRAINT fk_mr_approver FOREIGN KEY (approved_by)  REFERENCES users(id)     ON DELETE SET NULL,
  INDEX idx_mr_project (project_id),
  INDEX idx_mr_status (status)
) ENGINE=InnoDB;

CREATE TABLE material_request_items (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  request_id      INT UNSIGNED NOT NULL,
  material_id     INT UNSIGNED NOT NULL,
  qty_requested   DECIMAL(14,3) NOT NULL,
  qty_issued      DECIMAL(14,3) NOT NULL DEFAULT 0.000,
  remarks         VARCHAR(200) NULL,
  CONSTRAINT fk_mri_request  FOREIGN KEY (request_id)  REFERENCES material_requests(id) ON DELETE CASCADE,
  CONSTRAINT fk_mri_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT chk_mri_qty CHECK (qty_requested > 0 AND qty_issued >= 0),
  UNIQUE KEY uq_request_material (request_id, material_id),
  INDEX idx_mri_material (material_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 5. PURCHASING  (request -> PO -> delivery)
-- ---------------------------------------------------------------------------

CREATE TABLE purchase_orders (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  po_no           VARCHAR(24) NOT NULL UNIQUE,      -- PO-2026-0001
  supplier_id     INT UNSIGNED NOT NULL,
  request_id      INT UNSIGNED NULL,                -- the shortage that triggered it
  order_date      DATE NOT NULL,
  expected_date   DATE NULL,
  status          ENUM('draft','sent','partially_received','received','closed','cancelled')
                  NOT NULL DEFAULT 'draft',
  total_amount    DECIMAL(14,2) NOT NULL DEFAULT 0.00,
  prepared_by     INT UNSIGNED NOT NULL,
  approved_by     INT UNSIGNED NULL,
  approved_at     DATETIME NULL,
  remarks         VARCHAR(255) NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_po_supplier FOREIGN KEY (supplier_id) REFERENCES suppliers(id) ON DELETE RESTRICT,
  CONSTRAINT fk_po_request  FOREIGN KEY (request_id)  REFERENCES material_requests(id) ON DELETE SET NULL,
  CONSTRAINT fk_po_prepared FOREIGN KEY (prepared_by) REFERENCES users(id) ON DELETE RESTRICT,
  CONSTRAINT fk_po_approved FOREIGN KEY (approved_by) REFERENCES users(id) ON DELETE SET NULL,
  INDEX idx_po_supplier (supplier_id),
  INDEX idx_po_status (status)
) ENGINE=InnoDB;

CREATE TABLE purchase_order_items (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  po_id           INT UNSIGNED NOT NULL,
  material_id     INT UNSIGNED NOT NULL,
  qty_ordered     DECIMAL(14,3) NOT NULL,
  qty_received    DECIMAL(14,3) NOT NULL DEFAULT 0.000,   -- partial deliveries are normal
  unit_price      DECIMAL(12,2) NOT NULL,
  line_total      DECIMAL(14,2) AS (qty_ordered * unit_price) STORED,
  CONSTRAINT fk_poi_po       FOREIGN KEY (po_id)       REFERENCES purchase_orders(id) ON DELETE CASCADE,
  CONSTRAINT fk_poi_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT chk_poi_qty CHECK (qty_ordered > 0 AND qty_received >= 0),
  UNIQUE KEY uq_po_material (po_id, material_id),
  INDEX idx_poi_material (material_id)
) ENGINE=InnoDB;

-- what actually arrived on site
CREATE TABLE deliveries (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  dr_no           VARCHAR(40) NOT NULL,             -- supplier's delivery receipt number
  po_id           INT UNSIGNED NOT NULL,
  delivery_date   DATE NOT NULL,
  received_by     INT UNSIGNED NOT NULL,
  photo_path      VARCHAR(255) NULL,                -- proof of delivery
  remarks         VARCHAR(255) NULL,
  status          ENUM('posted','voided') NOT NULL DEFAULT 'posted',
  void_reason     VARCHAR(255) NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_dlv_po       FOREIGN KEY (po_id)       REFERENCES purchase_orders(id) ON DELETE RESTRICT,
  CONSTRAINT fk_dlv_received FOREIGN KEY (received_by) REFERENCES users(id) ON DELETE RESTRICT,
  UNIQUE KEY uq_delivery_po_dr (po_id, dr_no),
  INDEX idx_dlv_date (delivery_date)
) ENGINE=InnoDB;

CREATE TABLE delivery_items (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  delivery_id     INT UNSIGNED NOT NULL,
  po_item_id      INT UNSIGNED NOT NULL,
  material_id     INT UNSIGNED NOT NULL,
  qty_received    DECIMAL(14,3) NOT NULL,
  unit_price      DECIMAL(12,2) NOT NULL,
  remarks         VARCHAR(200) NULL,
  CONSTRAINT fk_di_delivery FOREIGN KEY (delivery_id) REFERENCES deliveries(id) ON DELETE CASCADE,
  CONSTRAINT fk_di_po_item  FOREIGN KEY (po_item_id)  REFERENCES purchase_order_items(id) ON DELETE RESTRICT,
  CONSTRAINT fk_di_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT chk_di_qty CHECK (qty_received > 0),
  INDEX idx_di_material (material_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 6. ISSUANCE AND RETURN  (stock leaving for, and coming back from, a project)
-- ---------------------------------------------------------------------------

CREATE TABLE material_issues (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  issue_no        VARCHAR(24) NOT NULL UNIQUE,      -- IS-2026-0001
  request_id      INT UNSIGNED NOT NULL,            -- issuance always answers an approved request
  project_id      INT UNSIGNED NOT NULL,
  issue_date      DATE NOT NULL,
  issued_by       INT UNSIGNED NOT NULL,            -- storekeeper
  received_by     VARCHAR(120) NOT NULL,            -- person on site who signed for it
  total_cost      DECIMAL(14,2) NOT NULL DEFAULT 0.00,
  status          ENUM('posted','voided') NOT NULL DEFAULT 'posted',
  void_reason     VARCHAR(255) NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_mi_request FOREIGN KEY (request_id) REFERENCES material_requests(id) ON DELETE RESTRICT,
  CONSTRAINT fk_mi_project FOREIGN KEY (project_id) REFERENCES projects(id) ON DELETE RESTRICT,
  CONSTRAINT fk_mi_issued  FOREIGN KEY (issued_by)  REFERENCES users(id) ON DELETE RESTRICT,
  INDEX idx_mi_project (project_id),
  INDEX idx_mi_date (issue_date)
) ENGINE=InnoDB;

CREATE TABLE material_issue_items (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  issue_id        INT UNSIGNED NOT NULL,
  request_item_id INT UNSIGNED NULL,
  material_id     INT UNSIGNED NOT NULL,
  qty_issued      DECIMAL(14,3) NOT NULL,
  unit_cost       DECIMAL(12,2) NOT NULL,           -- SNAPSHOT at time of issue
  line_cost       DECIMAL(14,2) AS (qty_issued * unit_cost) STORED,
  CONSTRAINT fk_mii_issue    FOREIGN KEY (issue_id)        REFERENCES material_issues(id) ON DELETE CASCADE,
  CONSTRAINT fk_mii_req_item FOREIGN KEY (request_item_id) REFERENCES material_request_items(id) ON DELETE SET NULL,
  CONSTRAINT fk_mii_material FOREIGN KEY (material_id)     REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT chk_mii_qty CHECK (qty_issued > 0),
  INDEX idx_mii_material (material_id)
) ENGINE=InnoDB;

CREATE TABLE material_returns (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  return_no       VARCHAR(24) NOT NULL UNIQUE,      -- RT-2026-0001
  project_id      INT UNSIGNED NOT NULL,
  issue_id        INT UNSIGNED NULL,                -- what it came from, when known
  return_date     DATE NOT NULL,
  returned_by     VARCHAR(120) NOT NULL,            -- person from site
  accepted_by     INT UNSIGNED NOT NULL,            -- storekeeper
  condition_note  VARCHAR(255) NULL,
  status          ENUM('posted','voided') NOT NULL DEFAULT 'posted',
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_mrt_project  FOREIGN KEY (project_id)  REFERENCES projects(id) ON DELETE RESTRICT,
  CONSTRAINT fk_mrt_issue    FOREIGN KEY (issue_id)    REFERENCES material_issues(id) ON DELETE SET NULL,
  CONSTRAINT fk_mrt_accepted FOREIGN KEY (accepted_by) REFERENCES users(id) ON DELETE RESTRICT,
  INDEX idx_mrt_project (project_id)
) ENGINE=InnoDB;

CREATE TABLE material_return_items (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  return_id       INT UNSIGNED NOT NULL,
  material_id     INT UNSIGNED NOT NULL,
  qty_returned    DECIMAL(14,3) NOT NULL,
  unit_cost       DECIMAL(12,2) NOT NULL,
  -- reusable goes back to stock; damaged leaves stock but stays charged to the project
  disposition     ENUM('reusable','damaged') NOT NULL DEFAULT 'reusable',
  CONSTRAINT fk_mrti_return   FOREIGN KEY (return_id)   REFERENCES material_returns(id) ON DELETE CASCADE,
  CONSTRAINT fk_mrti_material FOREIGN KEY (material_id) REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT chk_mrti_qty CHECK (qty_returned > 0),
  INDEX idx_mrti_material (material_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 7. PHYSICAL COUNT  (system stock vs what is really in the bodega)
-- ---------------------------------------------------------------------------

CREATE TABLE stock_adjustments (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  adjustment_no   VARCHAR(24) NOT NULL UNIQUE,      -- ADJ-2026-0001
  count_date      DATE NOT NULL,
  counted_by      INT UNSIGNED NOT NULL,
  approved_by     INT UNSIGNED NULL,
  reason          VARCHAR(255) NOT NULL,
  status          ENUM('draft','posted','voided') NOT NULL DEFAULT 'draft',
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_adj_counted  FOREIGN KEY (counted_by)  REFERENCES users(id) ON DELETE RESTRICT,
  CONSTRAINT fk_adj_approved FOREIGN KEY (approved_by) REFERENCES users(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE stock_adjustment_items (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  adjustment_id   INT UNSIGNED NOT NULL,
  material_id     INT UNSIGNED NOT NULL,
  system_qty      DECIMAL(14,3) NOT NULL,           -- what the system said
  counted_qty     DECIMAL(14,3) NOT NULL,           -- what was on the shelf
  variance_qty    DECIMAL(14,3) AS (counted_qty - system_qty) STORED,
  remarks         VARCHAR(200) NULL,
  CONSTRAINT fk_adji_adjustment FOREIGN KEY (adjustment_id) REFERENCES stock_adjustments(id) ON DELETE CASCADE,
  CONSTRAINT fk_adji_material   FOREIGN KEY (material_id)   REFERENCES materials(id) ON DELETE RESTRICT,
  UNIQUE KEY uq_adjustment_material (adjustment_id, material_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 8. THE LEDGER  (every movement of stock, ever)
-- ---------------------------------------------------------------------------

CREATE TABLE inventory_transactions (
  id              BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  material_id     INT UNSIGNED NOT NULL,
  txn_type        ENUM('RECEIVED','ISSUED','RETURNED','DAMAGED','ADJUSTMENT','OPENING')
                  NOT NULL,
  -- signed: positive adds to stock, negative removes. RECEIVED/RETURNED/OPENING > 0,
  -- ISSUED/DAMAGED < 0, ADJUSTMENT either way.
  quantity        DECIMAL(14,3) NOT NULL,
  unit_cost       DECIMAL(12,2) NOT NULL DEFAULT 0.00,
  balance_after   DECIMAL(14,3) NOT NULL,           -- running balance, for audit
  project_id      INT UNSIGNED NULL,                -- which project consumed it
  reference_type  ENUM('delivery','issue','return','adjustment','opening') NOT NULL,
  reference_id    BIGINT UNSIGNED NULL,             -- id within that document table
  performed_by    INT UNSIGNED NOT NULL,
  remarks         VARCHAR(255) NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_txn_material  FOREIGN KEY (material_id)  REFERENCES materials(id) ON DELETE RESTRICT,
  CONSTRAINT fk_txn_project   FOREIGN KEY (project_id)   REFERENCES projects(id) ON DELETE SET NULL,
  CONSTRAINT fk_txn_user      FOREIGN KEY (performed_by) REFERENCES users(id) ON DELETE RESTRICT,
  CONSTRAINT chk_txn_nonzero  CHECK (quantity <> 0),
  CONSTRAINT chk_txn_balance  CHECK (balance_after >= 0),
  INDEX idx_txn_material_date (material_id, created_at),
  INDEX idx_txn_project (project_id),
  INDEX idx_txn_reference (reference_type, reference_id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 9. PROJECT EXPENSES  (materials are posted automatically; the rest by hand)
-- ---------------------------------------------------------------------------

CREATE TABLE project_expenses (
  id              INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  project_id      INT UNSIGNED NOT NULL,
  category        ENUM('materials','labor','equipment','transportation','permits','other')
                  NOT NULL,
  description     VARCHAR(255) NOT NULL,
  amount          DECIMAL(14,2) NOT NULL,
  expense_date    DATE NOT NULL,
  source          ENUM('manual','material_issue') NOT NULL DEFAULT 'manual',
  reference_id    INT UNSIGNED NULL,                -- material_issues.id when auto-posted
  recorded_by     INT UNSIGNED NOT NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_exp_project FOREIGN KEY (project_id)  REFERENCES projects(id) ON DELETE CASCADE,
  CONSTRAINT fk_exp_user    FOREIGN KEY (recorded_by) REFERENCES users(id) ON DELETE RESTRICT,
  CONSTRAINT chk_exp_amount CHECK (amount > 0),
  INDEX idx_exp_project (project_id, expense_date),
  INDEX idx_exp_category (category)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 10. AUDIT AND SETTINGS
-- ---------------------------------------------------------------------------

CREATE TABLE activity_logs (
  id              BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  user_id         INT UNSIGNED NULL,
  action          VARCHAR(60)  NOT NULL,            -- created, approved, voided, posted
  entity_type     VARCHAR(60)  NOT NULL,            -- material_request, purchase_order
  entity_id       BIGINT UNSIGNED NULL,
  description     VARCHAR(255) NULL,
  ip_address      VARCHAR(45)  NULL,
  created_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_log_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL,
  INDEX idx_log_entity (entity_type, entity_id),
  INDEX idx_log_created (created_at)
) ENGINE=InnoDB;

CREATE TABLE settings (
  id              SMALLINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  setting_key     VARCHAR(60)  NOT NULL UNIQUE,
  setting_value   VARCHAR(160) NOT NULL,
  description     VARCHAR(200) NULL,
  updated_at      TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------------
-- 11. VIEWS  (reports read from here, and they prove the ledger adds up)
-- ---------------------------------------------------------------------------

-- Stock rebuilt from the ledger, next to the cached figure. If the two ever
-- disagree, something wrote to current_stock outside a transaction.
CREATE OR REPLACE VIEW v_stock_check AS
SELECT
  m.id                        AS material_id,
  m.code,
  m.name,
  m.unit,
  m.current_stock             AS cached_stock,
  COALESCE(SUM(t.quantity),0) AS ledger_stock,
  m.current_stock - COALESCE(SUM(t.quantity),0) AS difference,
  m.minimum_stock,
  CASE WHEN m.current_stock <= m.minimum_stock THEN 1 ELSE 0 END AS is_low_stock
FROM materials m
LEFT JOIN inventory_transactions t ON t.material_id = m.id
GROUP BY m.id, m.code, m.name, m.unit, m.current_stock, m.minimum_stock;

-- Planned vs actual per material, per project: the heart of the system.
CREATE OR REPLACE VIEW v_project_material_variance AS
WITH pairs AS (
  -- every project/material combination that was either planned or actually used
  SELECT project_id, material_id FROM project_material_estimates
  UNION
  SELECT i.project_id, ii.material_id
  FROM material_issues i
  JOIN material_issue_items ii ON ii.issue_id = i.id
  WHERE i.status = 'posted'
)
SELECT
  p.id   AS project_id,
  p.code AS project_code,
  p.name AS project_name,
  m.id   AS material_id,
  m.code AS material_code,
  m.name AS material_name,
  m.unit,
  COALESCE(e.planned_qty, 0)  AS planned_qty,
  COALESCE(e.planned_cost, 0) AS planned_cost,
  COALESCE(iss.issued_qty, 0)   AS issued_qty,
  COALESCE(ret.returned_qty, 0) AS returned_qty,
  COALESCE(iss.issued_qty, 0)  - COALESCE(ret.returned_qty, 0)  AS actual_qty,
  COALESCE(iss.issued_cost, 0) - COALESCE(ret.returned_cost, 0) AS actual_cost,
  (COALESCE(iss.issued_qty, 0)  - COALESCE(ret.returned_qty, 0))  - COALESCE(e.planned_qty, 0)  AS qty_variance,
  (COALESCE(iss.issued_cost, 0) - COALESCE(ret.returned_cost, 0)) - COALESCE(e.planned_cost, 0) AS cost_variance,
  CASE WHEN COALESCE(e.planned_qty, 0) = 0 THEN NULL
       ELSE ROUND((((COALESCE(iss.issued_qty,0) - COALESCE(ret.returned_qty,0)) - e.planned_qty)
                   / e.planned_qty) * 100, 2)
  END AS variance_percent
FROM pairs x
JOIN projects  p ON p.id = x.project_id
JOIN materials m ON m.id = x.material_id
LEFT JOIN (
  SELECT project_id, material_id,
         SUM(planned_qty)  AS planned_qty,
         SUM(planned_cost) AS planned_cost
  FROM project_material_estimates
  GROUP BY project_id, material_id
) e ON e.project_id = x.project_id AND e.material_id = x.material_id
LEFT JOIN (
  -- actual consumption = issued, less anything handed back in reusable condition
  SELECT i.project_id, ii.material_id,
         SUM(ii.qty_issued) AS issued_qty,
         SUM(ii.line_cost)  AS issued_cost
  FROM material_issues i
  JOIN material_issue_items ii ON ii.issue_id = i.id
  WHERE i.status = 'posted'
  GROUP BY i.project_id, ii.material_id
) iss ON iss.project_id = x.project_id AND iss.material_id = x.material_id
LEFT JOIN (
  SELECT r.project_id, ri.material_id,
         SUM(ri.qty_returned)                  AS returned_qty,
         SUM(ri.qty_returned * ri.unit_cost)   AS returned_cost
  FROM material_returns r
  JOIN material_return_items ri ON ri.return_id = r.id
  WHERE r.status = 'posted' AND ri.disposition = 'reusable'
  GROUP BY r.project_id, ri.material_id
) ret ON ret.project_id = x.project_id AND ret.material_id = x.material_id;

-- Budget vs spend per project.
CREATE OR REPLACE VIEW v_project_cost_summary AS
SELECT
  p.id, p.code, p.name, p.client_name, p.status,
  p.contract_amount,
  p.material_budget,
  COALESCE(mc.material_cost, 0) AS material_cost_to_date,
  COALESCE(ex.other_cost, 0)    AS other_expenses,
  p.material_budget - COALESCE(mc.material_cost, 0) AS material_budget_remaining,
  COALESCE(pr.progress, 0)      AS progress_percent
FROM projects p
LEFT JOIN (
  SELECT i.project_id, SUM(ii.line_cost) AS material_cost
  FROM material_issues i
  JOIN material_issue_items ii ON ii.issue_id = i.id
  WHERE i.status = 'posted'
  GROUP BY i.project_id
) mc ON mc.project_id = p.id
LEFT JOIN (
  SELECT project_id, SUM(amount) AS other_cost
  FROM project_expenses
  WHERE category <> 'materials'
  GROUP BY project_id
) ex ON ex.project_id = p.id
LEFT JOIN (
  -- project progress is the weighted roll-up of its tasks
  SELECT project_id,
         ROUND(SUM(progress_percent * weight_percent) / NULLIF(SUM(weight_percent),0), 2) AS progress
  FROM project_tasks
  GROUP BY project_id
) pr ON pr.project_id = p.id;
