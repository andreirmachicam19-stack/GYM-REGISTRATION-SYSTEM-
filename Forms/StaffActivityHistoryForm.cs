using GymRegistrationApp.Models;
using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public sealed class StaffActivityHistoryForm : Form
{
    private readonly DateTimePicker _datePicker = new();
    private readonly ComboBox _staffFilter = new();
    private readonly ComboBox _accountTypeFilter = new();
    private readonly DataGridView _grid = new();

    public StaffActivityHistoryForm()
    {
        Text = "Staff activity history";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(900, 520);
        Size = new Size(1100, 650);
        BackColor = Color.FromArgb(248, 249, 251);

        var header = new Label
        {
            Text = "Staff activity history",
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            ForeColor = Color.FromArgb(29, 36, 52),
            Location = new Point(24, 18),
            AutoSize = true
        };
        var subtitle = new Label
        {
            Text = "Review logins, timeouts, registrations, payments, and other staff processes.",
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(130, 136, 147),
            Location = new Point(24, 52),
            AutoSize = true
        };

        _datePicker.Format = DateTimePickerFormat.Short;
        _datePicker.Value = DateTime.Today;
        _datePicker.Width = 150;
        _datePicker.ValueChanged += (_, _) => LoadActivities();
        _staffFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _staffFilter.Width = 220;
        _staffFilter.SelectedIndexChanged += (_, _) => LoadActivities();
        _accountTypeFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _accountTypeFilter.Width = 150;
        _accountTypeFilter.Items.AddRange(new object[] { "All account types", "Admin", "Staff" });
        _accountTypeFilter.SelectedIndex = 0;
        _accountTypeFilter.SelectedIndexChanged += (_, _) =>
        {
            PopulateNameFilter();
            LoadActivities();
        };
        PopulateNameFilter();

        var filters = new FlowLayoutPanel
        {
            Location = new Point(24, 90),
            Size = new Size(1030, 42),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            WrapContents = false,
            BackColor = Color.White,
            Padding = new Padding(10, 6, 10, 4)
        };
        filters.Controls.Add(new Label { Text = "Date", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(0, 5, 6, 0) });
        filters.Controls.Add(_datePicker);
        filters.Controls.Add(new Label { Text = "Name", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(22, 5, 6, 0) });
        filters.Controls.Add(_staffFilter);
        filters.Controls.Add(new Label { Text = "Account type", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Margin = new Padding(22, 5, 6, 0) });
        filters.Controls.Add(_accountTypeFilter);

        _grid.Location = new Point(24, 148);
        _grid.Size = new Size(1030, 450);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        _grid.BackgroundColor = Color.White;
        _grid.BorderStyle = BorderStyle.FixedSingle;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.RowHeadersVisible = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        _grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        _grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        _grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        _grid.DefaultCellStyle.Padding = new Padding(6, 5, 6, 5);
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DATE / TIME", DataPropertyName = "When", FillWeight = 22, MinimumWidth = 180 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "STAFF", DataPropertyName = "Staff", FillWeight = 18, MinimumWidth = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ACCOUNT TYPE", DataPropertyName = "AccountType", FillWeight = 14, MinimumWidth = 110 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ACTION", DataPropertyName = "Action", FillWeight = 18, MinimumWidth = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DETAILS", DataPropertyName = "Details", FillWeight = 28, MinimumWidth = 260 });
        Controls.AddRange(new Control[] { header, subtitle, filters, _grid });
        LoadActivities();
    }

    private void LoadActivities()
    {
        var staff = _staffFilter.SelectedItem?.ToString();
        var staffUsername = DataStore.Staff
            .FirstOrDefault(s => string.Equals(s.FullName, staff, StringComparison.OrdinalIgnoreCase))?.Username ?? staff;
        var accountType = _accountTypeFilter.SelectedItem?.ToString();
        var rows = DataStore.StaffActivity
            .Where(a => a.OccurredAt.Date == _datePicker.Value.Date)
            .Where(a => string.Equals(a.Username, staffUsername, StringComparison.OrdinalIgnoreCase))
            .Select(a => new { Activity = a, AccountType = GetAccountType(a) })
            .Where(a => accountType == "All account types" || string.Equals(a.AccountType, accountType, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(a => a.Activity.OccurredAt)
            .Select(a => new
            {
                When = a.Activity.OccurredAt.ToString("MMM d, yyyy h:mm:ss tt"),
                Staff = string.IsNullOrWhiteSpace(a.Activity.StaffName) ? a.Activity.Username : a.Activity.StaffName,
                a.AccountType,
                a.Activity.Action,
                Details = string.IsNullOrWhiteSpace(a.Activity.MemberId) ? a.Activity.Details : $"{a.Activity.Details} (Member: {a.Activity.MemberId})"
            })
            .ToList();
        _grid.DataSource = rows;
    }

    private void PopulateNameFilter()
    {
        var selectedType = _accountTypeFilter.SelectedItem?.ToString();
        var names = DataStore.Users
            .Where(u => !u.IsDeleted && u.IsActive)
            .Where(u => selectedType == "All account types" || GetAccountType(u.Role) == selectedType)
            .Select(u =>
            {
                var profile = DataStore.Staff.FirstOrDefault(s => s.Username.Equals(u.Username, StringComparison.OrdinalIgnoreCase));
                return profile is null || string.IsNullOrWhiteSpace(profile.FullName) ? u.Username : profile.FullName;
            })
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        _staffFilter.Items.Clear();
        _staffFilter.Items.AddRange(names.Cast<object>().ToArray());
        if (_staffFilter.Items.Count > 0) _staffFilter.SelectedIndex = 0;
    }

    private static string GetAccountType(StaffActivityRecord activity)
    {
        var user = DataStore.Users.FirstOrDefault(u => u.Username.Equals(activity.Username, StringComparison.OrdinalIgnoreCase));
        var staff = DataStore.Staff.FirstOrDefault(s => s.Username.Equals(activity.Username, StringComparison.OrdinalIgnoreCase));
        var role = !string.IsNullOrWhiteSpace(user?.Role)
            ? user.Role
            : !string.IsNullOrWhiteSpace(activity.AccountType)
                ? activity.AccountType
                : staff?.Position ?? "Staff";

        return role.Equals("Owner", StringComparison.OrdinalIgnoreCase)
            || role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "Staff";
    }

    private static string GetAccountType(string? role) =>
        string.Equals(role, "Owner", StringComparison.OrdinalIgnoreCase)
        || string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "Staff";

    private static bool IsStaffAccount(string username, string position)
    {
        var user = DataStore.Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        var role = !string.IsNullOrWhiteSpace(user?.Role) ? user.Role : position;
        return !role.Equals("Owner", StringComparison.OrdinalIgnoreCase)
            && !role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }
}
