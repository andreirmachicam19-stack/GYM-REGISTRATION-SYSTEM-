# Troubleshooting Guide - MySQL Database Connection

## Common Issues & Solutions

### Issue 1: "Warning: Could not connect to the database" on Startup

**Symptoms:**
- Warning dialog appears when starting application
- Message says "Could not connect to the database"
- App continues to work normally

**Causes:**
- XAMPP MySQL is not running
- Database doesn't exist
- Wrong connection string
- MySQL service stopped

**Solutions:**

#### ✅ Solution A: Start XAMPP MySQL (Most Common)
```
1. Open XAMPP Control Panel
2. Look for "MySQL" in the service list
3. If status shows "Stopped" or red light:
   - Click the green "Start" button next to MySQL
   - Wait for light to turn green
   - Try running the application again
```

#### ✅ Solution B: Check Database Exists
```
1. Open web browser
2. Go to: http://localhost/phpmyadmin
3. Look for "gym_registration_db" in left panel
4. If it doesn't exist:
   - Click "New Database"
   - Enter name: gym_registration_db
   - Click "Create"
   - Import schema from GymRegistrationApp_Database.sql
```

#### ✅ Solution C: Verify MySQL is Running
```powershell
# Open Command Prompt and run:
netstat -ano | findstr :3306

# If you see output, MySQL is running
# If no output, MySQL is NOT running - start XAMPP
```

#### ✅ Solution D: Check Connection String
```
Current: server=127.0.0.1; user=root; database=gym_registration_db; password=;

Verify:
- server=127.0.0.1  ← Should be localhost or 127.0.0.1
- user=root         ← Should match your MySQL username
- database=gym_registration_db ← Should match your database name
- password=         ← Empty (XAMPP default) or your password

If different, modify in: Services/DatabaseConnection.cs line 12
```

---

### Issue 2: Application Runs But Can't Access Database

**Symptoms:**
- App starts successfully
- No warning dialog (app thinks DB is connected)
- But data operations fail
- Getting SQL errors

**Causes:**
- Database exists but schema is missing
- Database tables not created
- Wrong table names in queries
- Schema mismatch

**Solutions:**

#### ✅ Solution A: Import Database Schema
```
1. Open phpMyAdmin: http://localhost/phpmyadmin
2. Select gym_registration_db database
3. Click "Import" tab
4. Upload file: GymRegistrationApp_Database.sql
5. Click "Go" to import
6. Check that tables appear in left panel:
   - members
   - payments
   - users (if needed)
```

#### ✅ Solution B: Verify Table Structure
```sql
-- In phpMyAdmin, run these commands:

-- Check if tables exist
SHOW TABLES IN gym_registration_db;

-- Check members table
DESCRIBE members;

-- Check payments table
DESCRIBE payments;
```

#### ✅ Solution C: Check Table Names in Code
```csharp
// If you modified table names, update these:
// Services/DatabaseConnection.cs

string query = "INSERT INTO members SET ...";  // ← Table name must match
```

---

### Issue 3: Connection Test Passes But Operations Fail

**Symptoms:**
- No warning on startup
- Forms open normally  
- But CRUD operations fail

**Causes:**
- File permissions on database
- Table column names wrong
- Data type mismatches
- Network connectivity issues

**Solutions:**

#### ✅ Solution A: Check PHP/MySQL Logs
```
1. Open XAMPP Control Panel
2. Click "Logs" for MySQL
3. Look for errors in the log output
4. Common issues:
   - "Access Denied"
   - "Unknown Column"
   - "Table doesn't exist"
```

#### ✅ Solution B: Test Connection Manually
```csharp
// Add this to LoginForm or MainForm constructor temporarily:

bool connected = DatabaseConnection.TestConnection();
MessageBox.Show($"Database Connected: {connected}");

// If it shows "true" but operations fail, issue is in your SQL queries
```

#### ✅ Solution C: Verify Column Names
```sql
-- In phpMyAdmin, check actual column names:
DESC members;

-- Common issues:
-- ❌ Code uses: member_name
-- ✅ Database has: full_name
-- Result: "Unknown column" error
```

---

### Issue 4: Port 3306 Already in Use

**Symptoms:**
- Can't start XAMPP MySQL
- Error: "port 3306 already in use"
- Another application using the port

**Causes:**
- Another MySQL instance running
- Another application on port 3306

**Solutions:**

#### ✅ Solution A: Find What's Using Port 3306
```powershell
# Find process using port 3306:
netstat -ano | findstr :3306

# Example output:
#   TCP    127.0.0.1:3306     0.0.0.0:0    LISTENING    1234

# Kill the process (if it's MySQL):
taskkill /PID 1234 /F
```

#### ✅ Solution B: Change XAMPP MySQL Port
```
1. Stop MySQL in XAMPP Control Panel
2. Edit: xampp/mysql/bin/my.ini
3. Find line: port=3306
4. Change to: port=3307
5. Save file
6. Start MySQL again
7. Update connection string in ApplicationConfiguration.cs:
   - From: server=127.0.0.1
   - To: server=127.0.0.1:3307
```

#### ✅ Solution C: Use Different Application
If another app needs port 3306:
- Stop that application
- Start XAMPP MySQL
- Run your app
- The warning dialog won't appear if MySQL is running

---

### Issue 5: Connection String Has Wrong Password

**Symptoms:**
- "Access Denied" errors
- Can't connect to database
- MySQL running but connection fails

**Causes:**
- Root password set but connection string empty
- MySQL user doesn't exist

**Solutions:**

#### ✅ Solution A: Check Root Password
```
1. Open phpMyAdmin: http://localhost/phpmyadmin
2. If you see login screen:
   - Username: root
   - Password: (try empty first)
   - Click "Go"
3. If login succeeds with empty password:
   - Connection string in code should have: password=;
   - That's correct - no changes needed
4. If login fails with empty password:
   - Password was set
   - Need to update connection string with password
```

#### ✅ Solution B: Update Connection String
```csharp
// Edit: Services/DatabaseConnection.cs, line 12

// If password is "yourpassword":
string ConnectionString = "server=127.0.0.1; user=root; database=gym_registration_db; password=yourpassword;";

// Then rebuild and rerun
```

#### ✅ Solution C: Reset Root Password (If Forgotten)
```
1. Stop XAMPP MySQL
2. Open Command Prompt as Administrator
3. Navigate to XAMPP directory
4. Run: mysql-u root (no password)
5. Execute: ALTER USER 'root'@'localhost' IDENTIFIED BY '';
6. Execute: FLUSH PRIVILEGES;
7. Restart MySQL in XAMPP
```

---

### Issue 6: Forms Load But Connection Test Hangs

**Symptoms:**
- Application seems to freeze for 5-10 seconds
- Then continues normally
- Happens on every startup or form open

**Causes:**
- Network timeout trying to connect
- Slow XAMPP response
- System resource issues

**Solutions:**

#### ✅ Solution A: Check System Resources
```powershell
# Check CPU/Memory usage:
# Open Windows Task Manager
# Look for high CPU or Memory usage
# Close unnecessary applications
```

#### ✅ Solution B: Increase XAMPP MySQL Timeout
```
1. Edit: xampp/mysql/bin/my.ini
2. Under [mysqld] section, add:
   connect_timeout=10
   read_timeout=30
   write_timeout=30
3. Save and restart MySQL
```

#### ✅ Solution C: Make Connection Test Async (Advanced)
```csharp
// If you want non-blocking connection tests in future:
// Modify DatabaseConnection.cs to use async/await

public static async Task<bool> TestConnectionAsync()
{
	// Runs connection in background thread
}
```

---

### Issue 7: JSON Files Not Syncing with Database

**Symptoms:**
- Add member via form → saved to JSON
- Member doesn't appear in database
- Database operations don't update JSON
- Two separate data stores

**Causes:**
- No sync code implemented yet
- Database operations separate from JSON operations
- Current design: JSON is primary storage

**Solutions:**

#### ✅ Solution A: Enable Dual-Write (Simple)
```csharp
// In MainForm.cs, when adding a member:

public void AddMember(Member member)
{
	// 1. Save to JSON
	DataStore.Members.Add(member);
	DataStore.SaveMembers();

	// 2. Save to Database (if connected)
	if (DatabaseConnection.TestConnection())
	{
		// Insert into database
		string query = "INSERT INTO members ...";
		DatabaseConnection.ExecuteQuery(query, ...);
	}
}
```

#### ✅ Solution B: Switch to Database-First (Recommended)
```csharp
// Create new MemberService class:
// Services/MemberService.cs

public class MemberService
{
	public static void Add(Member member)
	{
		if (DatabaseConnection.TestConnection())
		{
			// Try database first
			InsertIntoDatabase(member);
		}
		// Always fallback to JSON
		DataStore.Members.Add(member);
		DataStore.SaveMembers();
	}
}
```

#### ✅ Solution C: Implement Background Sync
```csharp
// TODO: Advanced feature for later
// Create background task that:
// 1. Monitors JSON file changes
// 2. Syncs new entries to database
// 3. Happens every 5-10 seconds
```

---

### Issue 8: "Unknown Column" Error in Queries

**Symptoms:**
- SQL error: "Unknown column 'xyz' in field list"
- Database operations fail
- Other operations work fine

**Causes:**
- Table structure changed
- Code uses wrong column names
- Schema not imported correctly

**Solutions:**

#### ✅ Solution A: Check Actual Column Names
```sql
-- In phpMyAdmin:
DESC members;

-- Output shows:
-- Field          | Type
-- id             | VARCHAR(50)
-- full_name      | VARCHAR(100)
-- phone          | VARCHAR(20)
-- ...

-- Make sure code uses exact names:
-- ✅ INSERT INTO members (id, full_name, phone) ...
-- ❌ INSERT INTO members (id, member_name, telephone) ...
```

#### ✅ Solution B: Update Schema
```
1. Backup current database (if any data)
2. Delete and recreate database
3. Run import: GymRegistrationApp_Database.sql
4. Verify tables in phpMyAdmin
5. Restart application
```

#### ✅ Solution C: Debug Query
```csharp
// Temporarily log the query:
string query = "INSERT INTO members SET full_name=@name";
Console.WriteLine($"Query: {query}");
Console.WriteLine($"Connection: {DatabaseConnection.TestConnection()}");

// Try query in phpMyAdmin directly
// to see exact error message
```

---

### Issue 9: Application Works Offline, Need to Enable Database

**Symptoms:**
- App runs fine with JSON files
- Want to save data to MySQL now
- Don't know how to switch

**Solutions:**

#### ✅ Solution A: Keep Using JSON (Current Design)
```
• Current app prioritizes JSON
• Database connection is OPTIONAL
• You can use just JSON forever
• This is fine for development!
```

#### ✅ Solution B: Add Database Operations Gradually
```
1. Keep existing JSON storage working
2. Add database save alongside JSON
3. Both save simultaneously (dual-write pattern)
4. Increases redundancy, ensures no data loss

// In each Add/Edit/Delete operation:
DataStore.Add(item);        // Save to JSON
DatabaseInsert(item);        // Save to Database (if available)
```

#### ✅ Solution C: Migrate to Database Primary (Later)
```
1. Create migration script
2. Copy all data from JSON to database
3. Update code to:
   - Load from database
   - Fallback to JSON if DB unavailable
4. Requires comprehensive testing
```

---

## Quick Diagnostic Commands

### Check XAMPP MySQL Status
```powershell
# Test if MySQL is running
Test-NetConnection localhost -Port 3306

# Expected output: TcpTestSucceeded : True
```

### Check Database Connection
```sql
-- In phpMyAdmin console:
SELECT 1;          -- Should return 1 (successful)
SHOW DATABASES;    -- Lists all databases
SHOW TABLES;       -- Lists tables in current database
```

### Test from Command Line
```powershell
# Test MySQL connectivity
mysql -u root -p -h 127.0.0.1

# Press Enter if no password set
# Should show: mysql>
# Then type: SELECT 1;
# You should see: 1
# Type: EXIT to quit
```

---

## When Everything Works

✅ **Signs that DB is working:**
- App starts with no warning
- Forms load quickly
- Operations complete without errors
- No SQL exceptions in output window
- Can add/edit/delete data

✅ **Verify with:**
```csharp
// Add to any form's Load event:
bool dbOk = DatabaseConnection.TestConnection();
Console.WriteLine($"Database Status: {(dbOk ? "Connected ✓" : "Disconnected ✗")}");

// Check Output window (Debug > Output)
// Should show: Database Status: Connected ✓
```

---

## When to Seek Help

📞 **Escalation Path:**
1. Follow diagnostic steps above
2. Check Output window for error messages
3. Review DATABASE_INTEGRATION_GUIDE.md
4. Check DATABASE_ARCHITECTURE.md for flow diagrams
5. If still stuck, post the exact error message from:
   - Output window
   - phpMyAdmin
   - XAMPP logs
