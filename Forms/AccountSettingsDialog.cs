using GymRegistrationApp.Models;
using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public sealed class AccountSettingsDialog : Form
{
    private readonly string _username;
    private readonly TextBox _currentPassword = PasswordBox();
    private readonly TextBox _newPassword = PasswordBox();
    private readonly TextBox _confirmPassword = PasswordBox();
    private readonly DataGridView _positionsGrid = Grid();
    private readonly DataGridView _plansGrid = Grid();

    public AccountSettingsDialog(string username)
    {
        _username = username;
        Text = "Account Settings";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 520);
        Size = new Size(860, 600);
        BackColor = Color.FromArgb(246, 247, 250);

        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(14, 8) };
        tabs.TabPages.Add(CreatePasswordTab());
        var user = DataStore.Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user?.Role.Equals("Owner", StringComparison.OrdinalIgnoreCase) == true
            || user?.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
        {
            tabs.TabPages.Add(CreatePositionsTab());
            tabs.TabPages.Add(CreatePlansTab());
        }
        Controls.Add(tabs);
        LoadPositions();
        LoadPlans();
    }

    private TabPage CreatePasswordTab()
    {
        var page = new TabPage("Change password") { BackColor = Color.White, Padding = new Padding(30) };
        var title = Title("Protect your account", "Update your password regularly to keep your gym records secure.");
        page.Controls.Add(title);
        AddField(page, "Current password", _currentPassword, 100);
        AddField(page, "New password", _newPassword, 170);
        AddField(page, "Confirm new password", _confirmPassword, 240);
        var save = Button("Save password", 30, 330, 140);
        save.Click += SavePassword_Click;
        page.Controls.Add(save);
        return page;
    }

    private TabPage CreatePositionsTab()
    {
        var page = new TabPage("Positions") { BackColor = Color.White, Padding = new Padding(20) };
        page.Controls.Add(Title("Account positions", "Create account types that can be assigned to staff accounts."));
        ConfigureGrid(_positionsGrid, "Position");
        _positionsGrid.Location = new Point(20, 85); _positionsGrid.Size = new Size(780, 360);
        page.Controls.Add(_positionsGrid);
        var add = Button("+ Create position", 20, 465, 145); add.Click += (_, _) => AddPosition();
        var edit = Button("Edit", 175, 465, 90); edit.Click += (_, _) => EditPosition();
        var delete = Button("Delete", 275, 465, 90); delete.Click += (_, _) => DeletePosition();
        page.Controls.AddRange(new Control[] { add, edit, delete });
        return page;
    }

    private TabPage CreatePlansTab()
    {
        var page = new TabPage("Membership plans") { BackColor = Color.White, Padding = new Padding(20) };
        page.Controls.Add(Title("Membership plans", "Set the duration and price used when registering or renewing members."));
        ConfigureGrid(_plansGrid, "Plan", "Months", "Price");
        _plansGrid.Location = new Point(20, 85); _plansGrid.Size = new Size(780, 360);
        page.Controls.Add(_plansGrid);
        var add = Button("+ Create plan", 20, 465, 130); add.Click += (_, _) => AddPlan();
        var edit = Button("Edit", 160, 465, 90); edit.Click += (_, _) => EditPlan();
        var delete = Button("Delete", 260, 465, 90); delete.Click += (_, _) => DeletePlan();
        page.Controls.AddRange(new Control[] { add, edit, delete });
        return page;
    }

    private void LoadPositions() => _positionsGrid.DataSource = DatabaseConnection.GetStaffPositions().Select(p => new { Position = p }).ToList();
    private void LoadPlans() => _plansGrid.DataSource = DataStore.GetPlans().Select(p => new { p.Id, Plan = p.Name, p.Months, Price = $"₱{p.Price:N2}" }).ToList();

    private void AddPosition()
    {
        var name = Ask("Create position", "Position name:");
        if (string.IsNullOrWhiteSpace(name)) return;
        if (!DatabaseConnection.InsertStaffPosition(name)) MessageBox.Show(this, "Position already exists or could not be saved.", "Positions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        LoadPositions();
    }

    private void EditPosition()
    {
        var oldName = Selected(_positionsGrid, "Position"); if (oldName is null) return;
        var name = Ask("Edit position", "Position name:", oldName); if (string.IsNullOrWhiteSpace(name)) return;
        if (!DatabaseConnection.UpdateStaffPosition(oldName, name)) MessageBox.Show(this, "This position cannot be edited.", "Positions", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        LoadPositions();
    }

    private void DeletePosition()
    {
        var name = Selected(_positionsGrid, "Position"); if (name is null) return;
        if (name is "Admin" or "Front Desk Staff") { MessageBox.Show(this, "Default positions cannot be deleted.", "Positions", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        if (MessageBox.Show(this, $"Delete {name}? Existing accounts keep their current position.", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        { DatabaseConnection.DeleteStaffPosition(name); LoadPositions(); }
    }

    private void AddPlan()
    {
        var plan = PlanEditor(null); if (plan is null) return;
        plan.Id = 0; DataStore.Plans.Add(plan); DataStore.SavePlans(); LoadPlans();
    }

    private void EditPlan()
    {
        if (_plansGrid.CurrentRow?.Cells["Id"].Value is not int id) return;
        var existing = DataStore.Plans.FirstOrDefault(p => p.Id == id); if (existing is null) return;
        var updated = PlanEditor(existing); if (updated is null) return;
        existing.Name = updated.Name; existing.Months = updated.Months; existing.Price = updated.Price;
        DataStore.SavePlans(); LoadPlans();
    }

    private void DeletePlan()
    {
        if (_plansGrid.CurrentRow?.Cells["Id"].Value is not int id) return;
        var plan = DataStore.Plans.FirstOrDefault(p => p.Id == id); if (plan is null) return;
        if (MessageBox.Show(this, $"Delete {plan.Name}? Existing members keep their saved plan and price.", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        plan.IsActive = false; DataStore.SavePlans(); LoadPlans();
    }

    private static GymPlan? PlanEditor(GymPlan? source)
    {
        using var form = new Form { Text = source is null ? "Create plan" : "Edit plan", ClientSize = new Size(360, 220), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false };
        var name = new TextBox { Text = source?.Name ?? "", Location = new Point(25, 35), Width = 300 };
        var months = new NumericUpDown { Minimum = 1, Maximum = 120, Value = source?.Months ?? 1, Location = new Point(25, 95), Width = 140 };
        var price = new NumericUpDown { Minimum = 0, Maximum = 100000000, DecimalPlaces = 2, ThousandsSeparator = true, Value = source?.Price ?? 0, Location = new Point(185, 95), Width = 140 };
        form.Controls.AddRange(new Control[] { new Label { Text = "Plan name", Location = new Point(25, 15), AutoSize = true }, name, new Label { Text = "Months", Location = new Point(25, 75), AutoSize = true }, months, new Label { Text = "Price (₱)", Location = new Point(185, 75), AutoSize = true }, price });
        var ok = Button("Save", 225, 155, 100); ok.DialogResult = DialogResult.OK; form.Controls.Add(ok); form.AcceptButton = ok;
        return form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(name.Text) ? new GymPlan { Name = name.Text.Trim(), Months = (int)months.Value, Price = price.Value } : null;
    }

    private void SavePassword_Click(object? sender, EventArgs e)
    {
        var user = DataStore.Users.FirstOrDefault(u => u.Username.Equals(_username, StringComparison.OrdinalIgnoreCase));
        if (user is null || !user.IsActive) { MessageBox.Show(this, "This account is no longer active.", "Account Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (!PasswordHasher.Verify(_currentPassword.Text, user.PasswordHash, user.Password)) { MessageBox.Show(this, "The current password is incorrect.", "Account Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (!PasswordHasher.IsValidPassword(_newPassword.Text, out var error)) { MessageBox.Show(this, error, "Account Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (_newPassword.Text != _confirmPassword.Text) { MessageBox.Show(this, "New password and confirmation do not match.", "Account Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        user.PasswordHash = PasswordHasher.Hash(_newPassword.Text); user.Password = ""; DataStore.SaveUsers();
        MessageBox.Show(this, "Your password was changed successfully.", "Account Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _currentPassword.Clear(); _newPassword.Clear(); _confirmPassword.Clear();
    }

    private static Panel Title(string heading, string subtitle) { var panel = new Panel { Location = new Point(20, 15), Size = new Size(760, 60) }; panel.Controls.Add(new Label { Text = heading, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(29, 36, 52), AutoSize = true }); panel.Controls.Add(new Label { Text = subtitle, Location = new Point(0, 35), AutoSize = true, ForeColor = Color.Gray }); return panel; }
    private static Button Button(string text, int x, int y, int width) => new() { Text = text, Location = new Point(x, y), Size = new Size(width, 36), BackColor = Color.FromArgb(255, 132, 35), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
    private static TextBox PasswordBox() => new() { UseSystemPasswordChar = true, Width = 420, Height = 30 };
    private static void AddField(Control parent, string label, TextBox field, int top) { parent.Controls.Add(new Label { Text = label, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(30, top), AutoSize = true }); field.Location = new Point(30, top + 22); parent.Controls.Add(field); }
    private static DataGridView Grid() => new() { AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoGenerateColumns = true, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.FixedSingle, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private static void ConfigureGrid(DataGridView grid, params string[] visible) { grid.DataBindingComplete += (_, _) => { foreach (DataGridViewColumn column in grid.Columns) column.Visible = visible.Contains(column.HeaderText); }; }
    private static string? Selected(DataGridView grid, string column) => grid.CurrentRow?.Cells[column].Value?.ToString();
    private static string? Ask(string title, string prompt, string value = "") { using var form = new Form { Text = title, ClientSize = new Size(360, 145), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false }; form.Controls.Add(new Label { Text = prompt, Location = new Point(20, 20), AutoSize = true }); var input = new TextBox { Text = value, Location = new Point(20, 48), Width = 315 }; form.Controls.Add(input); var ok = Button("Save", 235, 90, 100); ok.DialogResult = DialogResult.OK; form.Controls.Add(ok); form.AcceptButton = ok; return form.ShowDialog() == DialogResult.OK ? input.Text.Trim() : null; }
}