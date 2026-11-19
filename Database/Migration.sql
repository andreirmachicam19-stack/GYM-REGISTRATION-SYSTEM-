USE gym_registration_db;

-- Approved member ID migration: remap existing IDs to GM plus six digits.
-- Run only after creating and verifying a current database backup.
START TRANSACTION;
SET FOREIGN_KEY_CHECKS = 0;
DROP TEMPORARY TABLE IF EXISTS member_id_migration;
CREATE TEMPORARY TABLE member_id_migration (
  old_id VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL PRIMARY KEY,
  new_id VARCHAR(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

SET @member_sequence = 100000;
INSERT INTO member_id_migration (old_id, new_id)
SELECT id, CONCAT('GM', LPAD((@member_sequence := @member_sequence + 1), 6, '0'))
FROM (SELECT id FROM members ORDER BY id) ordered_members;

UPDATE payments p
JOIN member_id_migration map ON map.old_id = p.member_id
SET p.member_id = map.new_id;
UPDATE attendance_records a
JOIN member_id_migration map ON map.old_id = a.member_id
SET a.member_id = map.new_id;
UPDATE daily_gym_visits v
JOIN member_id_migration map ON map.old_id = v.member_id
SET v.member_id = map.new_id;
UPDATE membership_renewals r
JOIN member_id_migration map ON map.old_id = r.member_id
SET r.member_id = map.new_id;
UPDATE staff_activity s
JOIN member_id_migration map ON map.old_id = s.member_id
SET s.member_id = map.new_id;
UPDATE members m
JOIN member_id_migration map ON map.old_id = m.id
SET m.id = map.new_id;

SET FOREIGN_KEY_CHECKS = 1;
DROP TEMPORARY TABLE member_id_migration;
COMMIT;

ALTER TABLE members
  ADD COLUMN IF NOT EXISTS is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  ADD COLUMN IF NOT EXISTS deleted_at DATETIME NULL,
  ADD COLUMN IF NOT EXISTS deleted_by VARCHAR(100) NOT NULL DEFAULT '';

ALTER TABLE daily_gym_visits
  ADD COLUMN IF NOT EXISTS middle_name VARCHAR(100) NOT NULL DEFAULT '';

CREATE TABLE IF NOT EXISTS archive_records (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  record_type VARCHAR(40) NOT NULL,
  record_id VARCHAR(100) NOT NULL,
  display_name VARCHAR(300) NOT NULL DEFAULT '',
  archived_at DATETIME NOT NULL,
  archived_by VARCHAR(100) NOT NULL DEFAULT '',
  details TEXT NULL,
  UNIQUE KEY uq_archive_record (record_type, record_id),
  KEY idx_archive_date (archived_at),
  KEY idx_archive_type (record_type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

ALTER TABLE payments MODIFY COLUMN member_id VARCHAR(50) NULL;

ALTER TABLE payments
  ADD COLUMN IF NOT EXISTS amount_paid DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  ADD COLUMN IF NOT EXISTS change_amount DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  ADD COLUMN IF NOT EXISTS payment_type VARCHAR(60) NOT NULL DEFAULT 'Membership Payment',
  ADD COLUMN IF NOT EXISTS collected_by VARCHAR(100) NOT NULL DEFAULT '',
  ADD COLUMN IF NOT EXISTS is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  ADD COLUMN IF NOT EXISTS deleted_at DATETIME NULL,
  ADD COLUMN IF NOT EXISTS deleted_by VARCHAR(100) NOT NULL DEFAULT '';

CREATE TABLE IF NOT EXISTS attendance_records (
  id VARCHAR(80) PRIMARY KEY,
  member_id VARCHAR(50) NOT NULL,
  member_name VARCHAR(300) NOT NULL,
  attendance_date DATE NOT NULL,
  check_in_time DATETIME NOT NULL,
  recorded_by VARCHAR(100) NOT NULL DEFAULT '',
  is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  deleted_at DATETIME NULL,
  deleted_by VARCHAR(100) NOT NULL DEFAULT '',
  UNIQUE KEY uq_attendance_member_day (member_id, attendance_date),
  KEY idx_attendance_date (attendance_date),
  CONSTRAINT fk_attendance_member FOREIGN KEY (member_id) REFERENCES members(id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS daily_gym_visits (
  id VARCHAR(80) PRIMARY KEY,
  first_name VARCHAR(100) NOT NULL,
  last_name VARCHAR(100) NOT NULL DEFAULT '',
  member_id VARCHAR(50) NULL,
  is_gym_member TINYINT(1) NOT NULL DEFAULT 0,
  visit_date DATE NOT NULL,
  fee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  payment_method VARCHAR(50) NOT NULL DEFAULT 'Cash',
  payment_reference VARCHAR(100) NOT NULL DEFAULT '',
  registered_by VARCHAR(100) NOT NULL DEFAULT '',
  is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  deleted_at DATETIME NULL,
  deleted_by VARCHAR(100) NOT NULL DEFAULT '',
  KEY idx_visits_date (visit_date),
  KEY idx_visits_member (member_id),
  CONSTRAINT fk_visit_member FOREIGN KEY (member_id) REFERENCES members(id) ON UPDATE CASCADE ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS membership_renewals (
  reference VARCHAR(100) PRIMARY KEY,
  member_id VARCHAR(50) NOT NULL,
  member_name VARCHAR(300) NOT NULL,
  previous_plan VARCHAR(30) NOT NULL DEFAULT '',
  new_plan VARCHAR(30) NOT NULL DEFAULT '',
  previous_status VARCHAR(30) NOT NULL DEFAULT '',
  previous_start_date DATE NOT NULL,
  previous_expiry_date DATE NOT NULL,
  previous_fee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  new_start_date DATE NOT NULL,
  new_expiry_date DATE NOT NULL,
  new_fee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  amount_paid DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  outstanding_amount DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  payment_method VARCHAR(50) NOT NULL DEFAULT 'Cash',
  payment_reference VARCHAR(100) NOT NULL DEFAULT '',
  renewal_date DATETIME NOT NULL,
  processed_by VARCHAR(100) NOT NULL DEFAULT '',
  status VARCHAR(30) NOT NULL DEFAULT 'Completed',
  KEY idx_renewals_member (member_id),
  KEY idx_renewals_date (renewal_date),
  CONSTRAINT fk_renewal_member FOREIGN KEY (member_id) REFERENCES members(id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS staff_activity (
  id VARCHAR(100) PRIMARY KEY,
  username VARCHAR(50) NOT NULL,
  staff_name VARCHAR(300) NOT NULL DEFAULT '',
  account_type VARCHAR(30) NOT NULL DEFAULT 'Staff',
  action VARCHAR(100) NOT NULL,
  details TEXT NOT NULL,
  member_id VARCHAR(50) NULL,
  occurred_at DATETIME NOT NULL,
  KEY idx_activity_user_date (username, occurred_at),
  KEY idx_activity_member (member_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

ALTER TABLE users
  ADD COLUMN IF NOT EXISTS full_name VARCHAR(300) NOT NULL DEFAULT '',
  ADD COLUMN IF NOT EXISTS email VARCHAR(150) NOT NULL DEFAULT '',
  ADD COLUMN IF NOT EXISTS phone_number VARCHAR(30) NOT NULL DEFAULT '',
  ADD COLUMN IF NOT EXISTS birthday DATE NULL,
  ADD COLUMN IF NOT EXISTS home_address VARCHAR(300) NOT NULL DEFAULT '',
  ADD COLUMN IF NOT EXISTS joined_date DATE NULL,
  ADD COLUMN IF NOT EXISTS status VARCHAR(30) NOT NULL DEFAULT 'Active',
  ADD COLUMN IF NOT EXISTS permissions_json TEXT NULL,
	ADD COLUMN IF NOT EXISTS requires_password_change TINYINT(1) NOT NULL DEFAULT 0,
  ADD COLUMN IF NOT EXISTS is_active TINYINT(1) NOT NULL DEFAULT 1,
  ADD COLUMN IF NOT EXISTS is_deleted TINYINT(1) NOT NULL DEFAULT 0,
  ADD COLUMN IF NOT EXISTS deleted_at DATETIME NULL,
  ADD COLUMN IF NOT EXISTS deleted_by VARCHAR(100) NOT NULL DEFAULT '';

UPDATE payments SET amount_paid = amount WHERE amount_paid = 0 AND amount <> 0;
UPDATE users SET is_active = 1 WHERE is_active IS NULL;
INSERT IGNORE INTO staff_positions (name) VALUES ('Front Desk Staff'), ('Admin');

CREATE TABLE IF NOT EXISTS gym_plans (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(100) NOT NULL,
  months INT NOT NULL DEFAULT 1,
  price DECIMAL(12,2) NOT NULL DEFAULT 0,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (id),
  UNIQUE KEY uq_gym_plans_name (name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO gym_plans (name, months, price) VALUES
  ('Monthly', 1, 1500.00),
  ('2-Month Plan', 2, 2800.00),
  ('Annual Plan', 12, 15000.00);
