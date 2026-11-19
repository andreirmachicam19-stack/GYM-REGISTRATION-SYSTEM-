# UniversalGym — Gym Registration System (C# Windows Forms)

A Windows desktop gym registration and transaction tracker inspired by the uploaded `GYM REGISTRATION.pdf` reference: dark navy sidebar, orange accents, staff/owner sign-in, dashboard cards, member lists, registration dialog, payment records, and summary reports.

## Requirements
- Windows 10/11
- .NET 8 SDK (Windows desktop workload / Windows Forms support)
- Visual Studio 2022 or VS Code with C# Dev Kit

## Open and run
1. Extract/open the `GymRegistrationApp` folder in VS Code.
2. Open a terminal in that folder.
3. Run:
   ```powershell
   dotnet restore
   dotnet build
   dotnet run
   ```
   Alternatively, open `GymRegistrationApp.csproj` in Visual Studio and press **F5**.

## Demo logins
- Owner: `admin` / `admin123`
- Staff: `staff` / `staff123`

## Included features
- Login with Owner and Staff roles (Reports page is Owner-only in the UI)
- Dashboard totals and recent members
- Register, search, edit and delete members
- Membership plans: Daily, Weekly, Monthly, Quarterly, Yearly
- Payment transaction records with generated references and payment methods
- Optional membership renewal during payment recording
- Members and payment CSV export
- Local JSON persistence; data is saved under `%LOCALAPPDATA%\GymRegistrationApp`

## Notes
- This is a student/demo application, not a production payment gateway. Payments are recorded as entered; it does not contact GCash, Maya, banks, or card processors to verify real payments.
- Demo passwords are stored locally as plain text for simplicity. Replace this authentication/storage approach before any real-world deployment.
- The reference PDF provides visual Staff View and Owner View screenshots; it does not specify a complete database schema or all business rules. Membership prices are editable defaults and should be confirmed with the gym client.

## QR attendance setup
- The QR form uses `GYM_ATTENDANCE_URL` when it is set to an absolute HTTP/HTTPS URL. For production, set this to the public HTTPS URL of the deployed `AttendanceApi`, for example `https://gym.example.com/attendance`.
- Without that variable, the form uses the computer's first active non-loopback IPv4 address at `http://<LAN-IP>:5080/attendance`. `AttendanceApi` must be running, the phone must be on the same network, and Windows Firewall must allow inbound TCP port 5080. A phone cannot reach `localhost` on the gym computer.
- `AttendanceApi` serves `/attendance` by redirecting to its HTML form at `/`; the form submits to `/api/attendance`. The API still requires `GYM_DB_CONNECTION_STRING` (or its configured database connection string).
