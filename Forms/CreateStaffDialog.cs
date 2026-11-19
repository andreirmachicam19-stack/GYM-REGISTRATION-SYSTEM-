#nullable enable

using GymRegistrationApp.Models;
using GymRegistrationApp.Services;
using System.Security.Cryptography;

namespace GymRegistrationApp.Forms;

public partial class CreateStaffDialog : Form
{
    private Staff? _staff;
    private readonly Staff? _editingStaff;
    private Panel? _contentPanel;
    private readonly Dictionary<string, CheckBox> _permissionChecks = new();

    public CreateStaffDialog(Staff? staff = null)
    {
        _editingStaff = staff;
        InitializeUI();
    }

    public Staff? GetStaff() => _staff;

    private void InitializeUI()
    {
        this.Text = _editingStaff is null ? "Create new account" : "Edit staff account";
        this.ClientSize = new Size(760, 820);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MinimizeBox = false;
        this.MaximizeBox = false;
        this.BackColor = Color.White;

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(24),
            BackColor = Color.White
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 4,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize,
            BackColor = Color.White,
            Padding = new Padding(0)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var infoBox = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 18),
            BackColor = Color.FromArgb(255, 247, 237),
            BorderStyle = BorderStyle.FixedSingle
        };
        var infoLabel = new Label
        {
            Text = "Staff account access: New users can manage members, renewals, payments, and balances. Income reports and user management remain owner-only.",
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(191, 76, 20),
            AutoSize = false,
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };
        infoBox.Controls.Add(infoLabel);
        layout.Controls.Add(infoBox, 0, 0);

        var personalLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 6,
            RowCount = 4,
            Margin = new Padding(0, 0, 0, 18),
            Padding = new Padding(0)
        };
        for (var column = 0; column < 6; column++)
            personalLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6667f));
        for (var row = 0; row < 4; row++)
            personalLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));

        var labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
        var inputPadding = new Padding(8, 5, 8, 5);

        Panel AddField(string labelText, Control input, int columnSpan = 2)
        {
            var field = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 12, 10) };
            var label = new Label
            {
                Text = labelText,
                Font = labelFont,
                Dock = DockStyle.Top,
                Height = 24,
                AutoSize = false
            };
            input.Dock = DockStyle.Top;
            input.Height = 32;
            field.Controls.Add(input);
            field.Controls.Add(label);
            personalLayout.Controls.Add(field);
            personalLayout.SetColumnSpan(field, columnSpan);
            return field;
        }

        var txtFirstName = new TextBox { Name = "txtFirstName", PlaceholderText = "e.g. Maria", Padding = inputPadding };
        var txtMiddleName = new TextBox { Name = "txtMiddleName", PlaceholderText = "e.g. Dela Cruz", Padding = inputPadding };
        var txtLastName = new TextBox { Name = "txtLastName", PlaceholderText = "e.g. Santos", Padding = inputPadding };
        AddField("First name", txtFirstName);
        AddField("Middle name", txtMiddleName);
        AddField("Last name", txtLastName);

        var txtEmail = new TextBox { Name = "txtEmail", PlaceholderText = "staff@universalgym.ph", Padding = inputPadding };
        var txtPhone = new TextBox { Name = "txtPhone", PlaceholderText = "09XXXXXXXXX", MaxLength = 11, Padding = inputPadding };
        txtPhone.KeyPress += (_, args) =>
        {
            if (!char.IsControl(args.KeyChar) && !char.IsDigit(args.KeyChar))
                args.Handled = true;
        };
        AddField("Username", txtEmail, 3);
        AddField("Phone number", txtPhone, 3);

        var txtBirthday = new DateTimePicker
        {
            Name = "txtBirthday",
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "MM/dd/yyyy",
            MaxDate = DateTime.Today,
            Value = _editingStaff?.Birthday > DateTime.Today ? DateTime.Today : (_editingStaff?.Birthday ?? DateTime.Today)
        };
        var cmbAccountType = new ComboBox
        {
            Name = "cmbAccountType",
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbAccountType.Items.AddRange(DatabaseConnection.GetStaffPositions().Cast<object>().ToArray());
        cmbAccountType.SelectedItem = _editingStaff?.Position ?? "Front Desk Staff";
        if (cmbAccountType.SelectedIndex < 0 && cmbAccountType.Items.Count > 0) cmbAccountType.SelectedIndex = 0;
        AddField("Birthday", txtBirthday, 3);
        AddField("Position", cmbAccountType, 3);

        var txtAddress = new TextBox { Name = "txtAddress", PlaceholderText = "Street, City, Province", Padding = inputPadding };
        AddField("Home address", txtAddress, 6);
        layout.Controls.Add(personalLayout, 0, 1);

        var permissionsBox = new GroupBox
        {
            Text = "Account permissions",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 258,
            Margin = new Padding(0, 0, 0, 18),
            Padding = new Padding(14, 24, 14, 12),
            ForeColor = Color.FromArgb(29, 36, 52)
        };
        var permissionsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 7,
            Padding = new Padding(0)
        };
        permissionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        permissionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (var row = 0; row < 7; row++)
            permissionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857f));
        var permissions = new (string Key, string Text)[]
        {
            ("CanViewMembers", "View members"),
            ("CanAddMembers", "Add members"),
            ("CanEditMembers", "Edit members"),
            ("CanDeleteMembers", "Delete members"),
            ("CanManagePayments", "Manage payments"),
            ("CanManageDailyGym", "Manage Daily Gym"),
            ("CanViewReports", "View reports"),
            ("CanManageExpenses", "Manage expenses"),
            ("CanManageStaff", "Manage staff accounts"),
            ("CanViewDeletedRecords", "View Archive"),
            ("CanViewOverviewMemberships", "Overview: memberships"),
            ("CanViewOverviewDailyGym", "Overview: Daily Gym"),
            ("CanViewOverviewPayments", "Overview: payment records"),
            ("CanViewOverviewRevenue", "Overview: revenue and trend")
        };
        for (var index = 0; index < permissions.Length; index++)
        {
            var permission = permissions[index];
            var checkBox = new CheckBox
            {
                Name = permission.Key,
                Text = permission.Text,
                Checked = GetPermissionValue(_editingStaff?.Permissions, permission.Key),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 12, 4),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(70, 80, 90)
            };
            _permissionChecks[permission.Key] = checkBox;
            permissionsLayout.Controls.Add(checkBox, index % 2, index / 2);
        }
        permissionsBox.Controls.Add(permissionsLayout);
        layout.Controls.Add(permissionsBox, 0, 2);

        var btnCancel = new Button
        {
            Text = "Cancel",
            Size = new Size(120, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(80, 100, 130),
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Name = "btnCancel"
        };
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = Color.LightGray;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        var btnCreate = new Button
        {
            Text = _editingStaff is null ? "Create account" : "Save changes",
            Size = new Size(140, 40),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(255, 132, 35),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Name = "btnCreate"
        };
        btnCreate.FlatAppearance.BorderSize = 0;
        btnCreate.Click += BtnCreate_Click;
        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 0, 0, 0),
            Margin = new Padding(0)
        };
        buttonPanel.Controls.Add(btnCreate);
        buttonPanel.Controls.Add(btnCancel);
        layout.Controls.Add(buttonPanel, 0, 3);

        _contentPanel.Controls.Add(layout);
        this.Controls.Add(_contentPanel);

        SetTextBoxValue("txtFirstName", _editingStaff?.FirstName);
        SetTextBoxValue("txtMiddleName", _editingStaff?.MiddleName);
        SetTextBoxValue("txtLastName", _editingStaff?.LastName);
        SetTextBoxValue("txtEmail", _editingStaff?.Email);
        SetTextBoxValue("txtPhone", _editingStaff?.PhoneNumber);
        SetTextBoxValue("txtAddress", _editingStaff?.HomeAddress);
    }

    private void BtnCreate_Click(object? sender, EventArgs e)
    {
        var firstName = GetTextBoxValue("txtFirstName");
        var middleName = GetTextBoxValue("txtMiddleName");
        var lastName = GetTextBoxValue("txtLastName");
        var email = GetTextBoxValue("txtEmail");
        var phone = GetTextBoxValue("txtPhone");
        var birthdayPicker = FindControlRecursively<DateTimePicker>(this, "txtBirthday");
        var birthday = birthdayPicker?.Value.Date ?? DateTime.MinValue;
        var address = GetTextBoxValue("txtAddress");
        var password = _editingStaff is null ? GenerateTemporaryPassword() : _editingStaff.Password;

        if (string.IsNullOrWhiteSpace(firstName))
        {
            MessageBox.Show("Please enter the first name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            MessageBox.Show("Please enter the last name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            MessageBox.Show("Please enter a username using the @universalgym.ph domain.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var emailParts = email.Split('@');
        if (emailParts.Length != 2 || string.IsNullOrWhiteSpace(emailParts[0]) ||
            !emailParts[1].Equals("universalgym.ph", StringComparison.OrdinalIgnoreCase) ||
            email.Any(char.IsWhiteSpace))
        {
            MessageBox.Show("Username must end with @universalgym.ph.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (phone.Length != 11 || phone.Any(c => !char.IsDigit(c)))
        {
            MessageBox.Show("Phone number must contain exactly 11 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var birthDate = birthday;

        if (string.IsNullOrWhiteSpace(address))
        {
            MessageBox.Show("Please enter a home address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var username = emailParts[0].Trim();
        var fullName = string.Join(" ", new[] { firstName, middleName, lastName }.Where(part => !string.IsNullOrWhiteSpace(part)));

        _staff = new Staff
        {
            Id = _editingStaff?.Id ?? 0,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            FullName = fullName,
            Email = email,
            PhoneNumber = phone,
            Birthday = birthDate,
            Position = GetComboBoxValue("cmbAccountType"),
            HomeAddress = address,
            Username = _editingStaff?.Username ?? username,
            Password = password,
            PasswordHash = _editingStaff?.PasswordHash ?? "",
            JoinedDate = _editingStaff?.JoinedDate ?? DateTime.Today,
            Status = _editingStaff?.Status ?? "Active",
            Permissions = ReadPermissions()
        };

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void SetTextBoxValue(string name, string? value)
    {
        var control = FindControlRecursively<TextBox>(this, name);
        if (control is not null) control.Text = value ?? "";
    }

    private StaffPermissions ReadPermissions()
    {
        var permissions = StaffPermissions.CreateDefault();
        foreach (var check in _permissionChecks)
        {
            var property = typeof(StaffPermissions).GetProperty(check.Key);
            property?.SetValue(permissions, check.Value.Checked);
        }

        return permissions;
    }

    private static bool GetPermissionValue(StaffPermissions? permissions, string propertyName)
    {
        permissions ??= StaffPermissions.CreateDefault();
        return typeof(StaffPermissions).GetProperty(propertyName)?.GetValue(permissions) is bool value && value;
    }

    private static string GenerateTemporaryPassword()
    {
        const string uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lowercase = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        var password = new char[8];
        var all = uppercase + lowercase + digits;
        password[0] = uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)];
        password[1] = lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        for (var i = 3; i < password.Length; i++)
        {
            password[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        RandomNumberGenerator.Shuffle(password.AsSpan());
        return new string(password);
    }

    private string GetTextBoxValue(string name)
    {
        var control = FindControlRecursively<TextBox>(this, name);
        return control?.Text.Trim() ?? "";
    }

    private string GetComboBoxValue(string name)
    {
        var control = FindControlRecursively<ComboBox>(this, name);
        return control?.Text.Trim() ?? "";
    }

    private static T? FindControlRecursively<T>(Control root, string name) where T : Control
    {
        if (root is T typed && string.Equals(typed.Name, name, StringComparison.Ordinal))
            return typed;

        foreach (Control child in root.Controls)
        {
            var match = FindControlRecursively<T>(child, name);
            if (match is not null)
                return match;
        }

        return null;
    }
}
