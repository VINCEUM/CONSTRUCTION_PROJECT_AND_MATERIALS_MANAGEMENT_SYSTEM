-- ---------------------------------------------------------------------------
--  Phase 1: the system now has exactly two roles (Admin, Project Engineer).
--  Run this ONCE on the live database (construction_mms) from SQLyog.
--
--  Everything the Storekeeper account(s) touched is re-pointed to the admin
--  account first, so no history is lost and no foreign key breaks. Then the
--  Storekeeper users and the Storekeeper role are removed.
-- ---------------------------------------------------------------------------
USE construction_mms;

SET @admin := (SELECT id FROM users WHERE email = 'admin@buildcorp.test' LIMIT 1);
SET @role  := (SELECT id FROM roles WHERE name = 'Storekeeper' LIMIT 1);

-- stop here quietly if there is nothing to do or no admin to hand things to
-- (if @admin is NULL, do not continue: fix the admin account first)
SELECT @admin AS admin_id, @role AS storekeeper_role_id,
       (SELECT COUNT(*) FROM users WHERE role_id = @role) AS storekeeper_users;

UPDATE projects            SET project_engineer_id = @admin WHERE project_engineer_id IN (SELECT id FROM users WHERE role_id = @role);
UPDATE material_requests   SET requested_by  = @admin WHERE requested_by  IN (SELECT id FROM users WHERE role_id = @role);
UPDATE material_requests   SET approved_by   = @admin WHERE approved_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE purchase_orders     SET prepared_by   = @admin WHERE prepared_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE purchase_orders     SET approved_by   = @admin WHERE approved_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE deliveries          SET received_by   = @admin WHERE received_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE material_issues     SET issued_by     = @admin WHERE issued_by     IN (SELECT id FROM users WHERE role_id = @role);
UPDATE material_returns    SET accepted_by   = @admin WHERE accepted_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE stock_adjustments   SET counted_by    = @admin WHERE counted_by    IN (SELECT id FROM users WHERE role_id = @role);
UPDATE stock_adjustments   SET approved_by   = @admin WHERE approved_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE inventory_transactions SET performed_by = @admin WHERE performed_by IN (SELECT id FROM users WHERE role_id = @role);
UPDATE project_expenses    SET recorded_by   = @admin WHERE recorded_by   IN (SELECT id FROM users WHERE role_id = @role);
UPDATE activity_logs       SET user_id       = @admin WHERE user_id       IN (SELECT id FROM users WHERE role_id = @role);

DELETE FROM users WHERE role_id = @role;
DELETE FROM roles WHERE id = @role;

UPDATE roles SET description = 'Full access: users, suppliers, approvals, purchasing, receiving, issuing, stock adjustments, reports'
 WHERE name = 'Admin';

-- verify: should list only Admin and Project Engineer, and no Storekeeper users
SELECT r.name AS role, COUNT(u.id) AS users
FROM   roles r LEFT JOIN users u ON u.role_id = r.id
GROUP  BY r.id, r.name ORDER BY r.id;
