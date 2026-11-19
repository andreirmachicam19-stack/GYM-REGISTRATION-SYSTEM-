-- ===================================================================
-- GYM REGISTRATION APP - XAMPP DATABASE SETUP
-- MySQL/MariaDB Script
-- ===================================================================

-- Create Database
CREATE DATABASE IF NOT EXISTS gym_registration_db;
USE gym_registration_db;

-- ===================================================================
-- TABLE 1: Users (Staff and Management)
-- ===================================================================
CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(50) NOT NULL UNIQUE,
  password VARCHAR(255) NOT NULL,
  role VARCHAR(30) NOT NULL DEFAULT 'Staff',
	requires_password_change TINYINT(1) NOT NULL DEFAULT 0,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_username (username),
  INDEX idx_role (role)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===================================================================
-- TABLE 4: Staff Positions / Account Types
-- ===================================================================
CREATE TABLE IF NOT EXISTS staff_positions (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(100) NOT NULL UNIQUE,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO staff_positions (name) VALUES
  ('Front Desk Staff'),
  ('Admin');

-- ===================================================================
-- TABLE 5: Gym Membership Plans
-- ===================================================================
CREATE TABLE IF NOT EXISTS gym_plans (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(100) NOT NULL UNIQUE,
  months INT NOT NULL DEFAULT 1,
  price DECIMAL(12,2) NOT NULL DEFAULT 0.00,
  is_active TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO gym_plans (name, months, price) VALUES
	('Daily', 1, 50.00),
  ('Monthly', 1, 600.00),
  ('2-Month Plan', 2, 1200.00),
  ('Annual Plan', 12, 7200.00);

-- ===================================================================
-- TABLE 2: Members (Gym Members)
-- ===================================================================
CREATE TABLE IF NOT EXISTS members (
  id VARCHAR(50) PRIMARY KEY NOT NULL,
  first_name VARCHAR(100) NOT NULL,
  middle_name VARCHAR(100) DEFAULT '',
  last_name VARCHAR(100) NOT NULL,
  full_name VARCHAR(300) NOT NULL,
  phone VARCHAR(20) NOT NULL,
  email VARCHAR(100) DEFAULT '',
  plan VARCHAR(30) NOT NULL DEFAULT 'Monthly',
  fee DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  start_date DATE NOT NULL,
  expiry_date DATE NOT NULL,
  status VARCHAR(30) NOT NULL DEFAULT 'Active',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX idx_phone (phone),
  INDEX idx_email (email),
  INDEX idx_full_name (full_name),
  INDEX idx_status (status),
  INDEX idx_expiry_date (expiry_date),
  INDEX idx_start_date (start_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===================================================================
-- TABLE 3: Payments (Payment Records)
-- ===================================================================
CREATE TABLE IF NOT EXISTS payments (
  id INT AUTO_INCREMENT PRIMARY KEY,
  reference VARCHAR(100) NOT NULL UNIQUE,
  member_id VARCHAR(50) NOT NULL,
  member_name VARCHAR(300) NOT NULL,
  amount DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  method VARCHAR(50) NOT NULL DEFAULT 'Cash',
  date DATETIME NOT NULL,
  status VARCHAR(30) NOT NULL DEFAULT 'Paid',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (member_id) REFERENCES members(id) ON DELETE CASCADE,
  INDEX idx_reference (reference),
  INDEX idx_member_id (member_id),
  INDEX idx_date (date),
  INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===================================================================
-- INSERT SAMPLE DATA - USERS
-- ===================================================================
INSERT INTO users (username, password, role) VALUES
('owner', 'owner123', 'Owner'),
('manager', 'manager123', 'Manager'),
('staff1', 'staff123', 'Staff'),
('staff2', 'staff456', 'Staff'),
('receptionist', 'reception123', 'Staff');

-- ===================================================================
-- INSERT SAMPLE DATA - MEMBERS
-- ===================================================================
INSERT INTO members (id, first_name, middle_name, last_name, full_name, phone, email, plan, fee, start_date, expiry_date, status) VALUES
('MEM001', 'Juan', 'Carlos', 'Santos', 'Juan Carlos Santos', '09171234567', 'juan.santos@email.com', 'Monthly', 1500.00, '2024-01-15', '2025-01-15', 'Active'),
('MEM002', 'Maria', 'Grace', 'Rodriguez', 'Maria Grace Rodriguez', '09172345678', 'maria.rodriguez@email.com', 'Quarterly', 4000.00, '2024-02-10', '2025-05-10', 'Active'),
('MEM003', 'Jose', '', 'Cruz', 'Jose Cruz', '09173456789', 'jose.cruz@email.com', 'Yearly', 14000.00, '2023-03-20', '2025-03-20', 'Active'),
('MEM004', 'Ana', 'Marie', 'Lopez', 'Ana Marie Lopez', '09174567890', 'ana.lopez@email.com', 'Monthly', 1500.00, '2024-11-01', '2024-12-01', 'Active'),
('MEM005', 'Pedro', 'Manuel', 'Garcia', 'Pedro Manuel Garcia', '09175678901', 'pedro.garcia@email.com', 'Weekly', 400.00, '2024-12-10', '2024-12-17', 'Inactive'),
('MEM006', 'Rosa', 'Lyn', 'Fernandez', 'Rosa Lyn Fernandez', '09176789012', 'rosa.fernandez@email.com', 'Monthly', 1500.00, '2024-10-15', '2024-11-15', 'Inactive'),
('MEM007', 'Miguel', 'Angel', 'Ruiz', 'Miguel Angel Ruiz', '09177890123', 'miguel.ruiz@email.com', 'Quarterly', 4000.00, '2024-09-01', '2024-12-01', 'Active'),
('MEM008', 'Carmen', 'Sofia', 'Torres', 'Carmen Sofia Torres', '09178901234', 'carmen.torres@email.com', 'Monthly', 1500.00, '2024-11-20', '2024-12-20', 'Active'),
('MEM009', 'Felipe', 'Diego', 'Morales', 'Felipe Diego Morales', '09179012345', 'felipe.morales@email.com', 'Monthly', 1500.00, '2024-12-01', '2025-01-01', 'Active'),
('MEM010', 'Isabella', 'Rose', 'Mendoza', 'Isabella Rose Mendoza', '09180123456', 'isabella.mendoza@email.com', 'Daily', 100.00, '2024-12-14', '2024-12-14', 'Active');

-- ===================================================================
-- INSERT SAMPLE DATA - PAYMENTS
-- ===================================================================
INSERT INTO payments (reference, member_id, member_name, amount, method, date, status) VALUES
('PAY20241201001', 'MEM001', 'Juan Carlos Santos', 1500.00, 'Cash', '2024-12-01 09:30:00', 'Paid'),
('PAY20241201002', 'MEM002', 'Maria Grace Rodriguez', 4000.00, 'Debit Card', '2024-12-02 10:15:00', 'Paid'),
('PAY20241203003', 'MEM003', 'Jose Cruz', 14000.00, 'Cash', '2024-03-20 14:45:00', 'Paid'),
('PAY20241205004', 'MEM004', 'Ana Marie Lopez', 1500.00, 'GCash', '2024-12-05 11:20:00', 'Paid'),
('PAY20241206005', 'MEM005', 'Pedro Manuel Garcia', 400.00, 'Cash', '2024-12-06 08:00:00', 'Paid'),
('PAY20241207006', 'MEM006', 'Rosa Lyn Fernandez', 1500.00, 'Debit Card', '2024-10-15 15:30:00', 'Paid'),
('PAY20241208007', 'MEM007', 'Miguel Angel Ruiz', 4000.00, 'Cash', '2024-09-01 09:00:00', 'Paid'),
('PAY20241209008', 'MEM008', 'Carmen Sofia Torres', 1500.00, 'GCash', '2024-11-20 13:45:00', 'Paid'),
('PAY20241210009', 'MEM009', 'Felipe Diego Morales', 1500.00, 'Cash', '2024-12-01 10:30:00', 'Paid'),
('PAY20241211010', 'MEM010', 'Isabella Rose Mendoza', 100.00, 'Cash', '2024-12-14 16:00:00', 'Paid'),
('PAY20241212011', 'MEM001', 'Juan Carlos Santos', 1500.00, 'Debit Card', '2024-11-15 09:30:00', 'Paid'),
('PAY20241213012', 'MEM002', 'Maria Grace Rodriguez', 1000.00, 'GCash', '2024-11-10 14:20:00', 'Paid'),
('PAY20241214013', 'MEM003', 'Jose Cruz', 500.00, 'Cash', '2024-12-14 11:45:00', 'Pending');

-- ===================================================================
-- CREATE VIEWS FOR REPORTING
-- ===================================================================

-- Active Members View
CREATE OR REPLACE VIEW vw_active_members AS
SELECT 
  id, 
  full_name, 
  phone, 
  email, 
  plan, 
  fee, 
  start_date, 
  expiry_date, 
  status,
  DATEDIFF(expiry_date, CURDATE()) as days_remaining
FROM members
WHERE status = 'Active' AND expiry_date >= CURDATE()
ORDER BY full_name;

-- Expired Members View
CREATE OR REPLACE VIEW vw_expired_members AS
SELECT 
  id, 
  full_name, 
  phone, 
  email, 
  plan, 
  fee, 
  start_date, 
  expiry_date, 
  status,
  DATEDIFF(CURDATE(), expiry_date) as days_expired
FROM members
WHERE expiry_date < CURDATE()
ORDER BY expiry_date DESC;

-- Payment Summary View
CREATE OR REPLACE VIEW vw_payment_summary AS
SELECT 
  DATE(date) as payment_date,
  COUNT(*) as transaction_count,
  SUM(amount) as total_amount,
  GROUP_CONCAT(DISTINCT method SEPARATOR ', ') as payment_methods
FROM payments
WHERE status = 'Paid'
GROUP BY DATE(date)
ORDER BY payment_date DESC;

-- Revenue by Plan View
CREATE OR REPLACE VIEW vw_revenue_by_plan AS
SELECT 
  m.plan,
  COUNT(DISTINCT m.id) as member_count,
  SUM(p.amount) as total_revenue,
  AVG(m.fee) as avg_fee
FROM members m
LEFT JOIN payments p ON m.id = p.member_id AND p.status = 'Paid'
GROUP BY m.plan
ORDER BY total_revenue DESC;

-- Outstanding Balances View
CREATE OR REPLACE VIEW vw_outstanding_balances AS
SELECT 
  m.id,
  m.full_name,
  m.phone,
  m.email,
  m.plan,
  m.fee,
  m.start_date,
  m.expiry_date,
  m.status,
  COALESCE(SUM(CASE WHEN p.status = 'Paid' THEN p.amount ELSE 0 END), 0) as total_paid,
  m.fee - COALESCE(SUM(CASE WHEN p.status = 'Paid' THEN p.amount ELSE 0 END), 0) as balance_due
FROM members m
LEFT JOIN payments p ON m.id = p.member_id
GROUP BY m.id, m.full_name, m.phone, m.email, m.plan, m.fee, m.start_date, m.expiry_date, m.status
HAVING balance_due > 0
ORDER BY balance_due DESC;

-- ===================================================================
-- CREATE STORED PROCEDURES (Optional - for advanced operations)
-- ===================================================================

DELIMITER $$

-- Procedure to add a new member
CREATE PROCEDURE sp_add_member(
  IN p_id VARCHAR(50),
  IN p_first_name VARCHAR(100),
  IN p_middle_name VARCHAR(100),
  IN p_last_name VARCHAR(100),
  IN p_full_name VARCHAR(300),
  IN p_phone VARCHAR(20),
  IN p_email VARCHAR(100),
  IN p_plan VARCHAR(30),
  IN p_fee DECIMAL(10, 2),
  IN p_start_date DATE,
  IN p_expiry_date DATE
)
BEGIN
  INSERT INTO members 
  (id, first_name, middle_name, last_name, full_name, phone, email, plan, fee, start_date, expiry_date, status)
  VALUES 
  (p_id, p_first_name, p_middle_name, p_last_name, p_full_name, p_phone, p_email, p_plan, p_fee, p_start_date, p_expiry_date, 'Active');
END$$

-- Procedure to record a payment
CREATE PROCEDURE sp_record_payment(
  IN p_reference VARCHAR(100),
  IN p_member_id VARCHAR(50),
  IN p_member_name VARCHAR(300),
  IN p_amount DECIMAL(10, 2),
  IN p_method VARCHAR(50)
)
BEGIN
  INSERT INTO payments 
  (reference, member_id, member_name, amount, method, date, status)
  VALUES 
  (p_reference, p_member_id, p_member_name, p_amount, p_method, NOW(), 'Paid');
END$$

DELIMITER ;

-- ===================================================================
-- SUMMARY STATISTICS
-- ===================================================================
/*
Total Members: 10
Active Members: 8
Inactive/Expired Members: 2
Total Payments Recorded: 13
Total Revenue: ₱47,000.00

Membership Plans:
- Daily: ₱100 (1 member)
- Weekly: ₱400 (1 member)
- Monthly: ₱1,500 (5 members)
- Quarterly: ₱4,000 (2 members)
- Yearly: ₱14,000 (1 member)
*/

-- ===================================================================
-- EXPORT/BACKUP INFORMATION
-- ===================================================================
/*
HOW TO USE THIS FILE:

1. XAMPP Setup:
   - Ensure XAMPP is running (Apache + MySQL)
   - Open phpMyAdmin: http://localhost/phpmyadmin

2. Import Database:
   - Click "Import" tab in phpMyAdmin
   - Choose this SQL file
   - Click "Go" to execute

3. OR Command Line:
   mysql -u root -p < GymRegistrationApp_Database.sql

4. Connection String for .NET Application:
   Server=localhost;Database=gym_registration_db;User Id=root;Password=;

5. Sample Credentials:
   - Owner: username=owner, password=owner123
   - Manager: username=manager, password=manager123
   - Staff: username=staff1, password=staff123

6. Testing:
   - Login with any of the sample credentials
   - You'll see 10 sample members and payment records
   - Test all features (Add members, Payment recording, Reports)
*/

-- ===================================================================
-- END OF DATABASE SETUP SCRIPT
-- ===================================================================

-- Daily Gym walk-in records use a fixed PHP 50 Cash pass. The WinForms runtime persists these records in daily-gym-visits.json.

