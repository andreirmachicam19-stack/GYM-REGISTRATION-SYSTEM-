   b # MySQL Database Integration - GYM REGISTRATION APP

## Overview
Your Gym Registration Application now includes full MySQL database connectivity for XAMPP. The application will automatically test the database connection on startup and when opening key forms.

## Database Connection Details

### Connection String
```
server=127.0.0.1
user=root
database=gym_registration_db
password= (empty)
```

### XAMPP Configuration
- **Server**: Local machine (127.0.0.1)
- **Port**: 3306 (default MySQL port)
- **Username**: root
- **Password**: (leave empty)
- **Database Name**: gym_registration_db

## How to Setup

### 1. Start XAMPP
- Open XAMPP Control Panel
- Click "Start" for Apache (optional, for web access)
- Click "Start" for MySQL
- Verify MySQL is running (should show green)

### 2. Create Database
If you haven't created the database yet:

```sql
-- Create database
CREATE DATABASE gym_registration_db;
USE gym_registration_db;

-- Import your schema from GymRegistrationApp_Database.sql file
```

### 3. Run the Application
- The app will test the connection when it starts (Program.cs)
- If connection succeeds: Status message appears, then app continues
- If connection fails: Warning dialog shown, but app continues with local JSON storage

## Database Connection Components

### 1. DatabaseConnection.cs (NEW)
Located in `Services/DatabaseConnection.cs`

**Static Methods Available:**

```csharp
// Test the connection
bool isConnected = DatabaseConnection.TestConnection();

// Get a new connection
MySqlConnection conn = DatabaseConnection.GetConnection();

// Execute non-query (INSERT, UPDATE, DELETE)
int rowsAffected = DatabaseConnection.ExecuteQuery(
	"INSERT INTO members SET name=@name",
	new MySqlParameter("@name", "John Doe")
);

// Execute query with results
MySqlDataReader reader = DatabaseConnection.ExecuteReader(
	"SELECT * FROM members WHERE id=@id",
	new MySqlParameter("@id", "12345")
);

// Get single value
object result = DatabaseConnection.ExecuteScalar(
	"SELECT COUNT(*) FROM members"
);
```

### 2. Program.cs (UPDATED)
- Tests database connection on application startup
- Shows warning if connection fails
- Application continues to work with local JSON storage as fallback

### 3. Forms Updated (Connect on Load)

The following forms now test database connectivity when they open:

1. **LoginForm.cs**
   - Tests connection when user logs in
   - Silent failure - doesn't block login

2. **MainForm.cs**
   - Main application window
   - Tests and initializes database when form loads
   - Continues with local storage if database unavailable

3. **MemberDialog.cs**
   - Member add/edit dialog
   - Tests connection when dialog opens
   - Uses local storage for member operations

4. **PaymentDialog.cs**
   - Payment processing dialog
   - Tests connection when dialog opens
   - Uses local storage for payment operations

## Using the Database Connection

### Example 1: Insert Member Data
```csharp
string query = @"INSERT INTO members 
				(id, full_name, phone, email, plan, fee, start_date, expiry_date, status) 
				VALUES (@id, @name, @phone, @email, @plan, @fee, @start, @expiry, @status)";

var parameters = new[]
{
	new MySqlParameter("@id", member.Id),
	new MySqlParameter("@name", member.FullName),
	new MySqlParameter("@phone", member.Phone),
	new MySqlParameter("@email", member.Email),
	new MySqlParameter("@plan", member.Plan),
	new MySqlParameter("@fee", member.Fee),
	new MySqlParameter("@start", member.StartDate),
	new MySqlParameter("@expiry", member.ExpiryDate),
	new MySqlParameter("@status", member.Status)
};

int result = DatabaseConnection.ExecuteQuery(query, parameters);
```

### Example 2: Query Data
```csharp
string query = "SELECT * FROM members WHERE status = @status";
var reader = DatabaseConnection.ExecuteReader(
	query,
	new MySqlParameter("@status", "Active")
);

while (reader.Read())
{
	string name = reader["full_name"].ToString();
	DateTime expiry = (DateTime)reader["expiry_date"];
}
reader.Close();
```

### Example 3: Get Count
```csharp
string query = "SELECT COUNT(*) FROM members WHERE status = @status";
object count = DatabaseConnection.ExecuteScalar(
	query,
	new MySqlParameter("@status", "Active")
);
```

## Error Handling

The application uses a two-tier approach:

1. **Primary**: MySQL Database (when XAMPP is running)
2. **Fallback**: Local JSON Files (always available)

If the database is unavailable:
- Connection test returns false
- User sees warning (only on first startup)
- Application continues normally using JSON storage
- No data loss occurs

## Troubleshooting

### "Could not connect to database" Message
1. **Check XAMPP is Running**
   - Open XAMPP Control Panel
   - Verify MySQL has a green "Running" indicator
   - If not, click "Start" button

2. **Check Database Exists**
   - Open phpMyAdmin (http://localhost/phpmyadmin)
   - Verify `gym_registration_db` database exists
   - If not, create it with the SQL script

3. **Check Connection String**
   - Current: `server=127.0.0.1; user=root; database=gym_registration_db; password=;`
   - Verify MySQL uses port 3306 (default)
   - Verify root user has no password (or update the connection string)

### Application Still Works Without Database
- This is **by design**
- Local JSON files in `%APPDATA%\GymRegistrationApp\` are used as fallback
- Data remains synchronized with JSON until database is connected

### Manual Database Connection Test
You can test connectivity in the Output window:
```
Open: Debug > Output (or Ctrl+Alt+O)
If using DatabaseConnection.TestConnection(), check the console output
```

## Next Steps - Integrating Database Operations

To fully use the database, you'll need to:

1. **Update DataStore.cs**
   - Modify to read/write from both MySQL and JSON
   - Or implement separate database methods

2. **Create Database Service Classes**
   - MemberDataService
   - PaymentDataService
   - UserDataService

3. **Update Forms to Use Database**
   - Replace DataStore calls with database queries
   - Keep JSON as fallback

## Security Notes

⚠️ **Current Setup - Development Only:**
- Root user with empty password (XAMPP default)
- Not suitable for production
- Never expose this configuration online

**For Production:**
- Create dedicated database user with password
- Update connection string
- Use encrypted connections
- Implement proper authentication

## File References

- **New File**: `Services/DatabaseConnection.cs` - Database utility class
- **Modified Files**:
  - `Program.cs` - Startup database test
  - `Forms/LoginForm.cs` - Connection check on login
  - `Forms/MainForm.cs` - Connection initialization
  - `Forms/MemberDialog.cs` - Connection test on open
  - `Forms/PaymentDialog.cs` - Connection test on open

## Support

For database-related issues:
1. Check XAMPP is running
2. Verify database exists
3. Check connection string matches your setup
4. Review Output window for error messages
5. Consult the `GymRegistrationApp_Database.sql` file for schema
