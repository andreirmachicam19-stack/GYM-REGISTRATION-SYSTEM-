using GymRegistrationApp.Models;
using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public sealed class ExpensesForm : Form
{
    private readonly string _username;
    private readonly bool _canManageExpenses;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoGenerateColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White };
    private readonly TextBox _search = new() { Width = 220, PlaceholderText = "Search name, category, description" };
    private readonly ComboBox _category = new() { Width = 170, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _from = new() { Width = 120, Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker _to = new() { Width = 120, Format = DateTimePickerFormat.Short };
    private readonly Label _total = Metric("TOTAL EXPENSES");
    private readonly Label _today = Metric("TODAY");
    private readonly Label _month = Metric("THIS MONTH");
    private readonly Label _year = Metric("THIS YEAR");
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 30000 };

    public ExpensesForm(string username, bool canManageExpenses = false)
    {
        _username = username;
        _canManageExpenses = canManageExpenses;
        Text = "Expenses Management";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(1000, 650);
        Size = new Size(1200, 760);
        BackColor = Color.FromArgb(246, 247, 250);
        BuildLayout();
        _timer.Tick += (_, _) => { DataStore.Reload(); RefreshData(); };
        _timer.Start();
        FormClosed += (_, _) => _timer.Stop();
        RefreshData();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), RowCount = 4, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 88)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(new Label { Text = "Expenses Management", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(29,36,52), Dock = DockStyle.Fill }, 0, 0);
        var metrics = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = Color.White, Padding = new Padding(14) };
        foreach (var label in new[] { _total, _today, _month, _year }) metrics.Controls.Add(label);
        root.Controls.Add(metrics, 0, 1);

        var filters = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 8, 0, 0) };
        filters.Controls.Add(new Label { Text = "Search", AutoSize = true, Margin = new Padding(0, 7, 6, 0) }); filters.Controls.Add(_search);
        filters.Controls.Add(new Label { Text = "Category", AutoSize = true, Margin = new Padding(15, 7, 6, 0) }); filters.Controls.Add(_category);
        filters.Controls.Add(new Label { Text = "From", AutoSize = true, Margin = new Padding(15, 7, 6, 0) }); filters.Controls.Add(_from);
        filters.Controls.Add(new Label { Text = "To", AutoSize = true, Margin = new Padding(8, 7, 6, 0) }); filters.Controls.Add(_to);
        var add = Button("+ Add Expense"); add.Click += (_, _) => EditExpense(null); filters.Controls.Add(add);
        var edit = Button("Edit"); edit.Click += (_, _) => EditExpense(SelectedExpense()); filters.Controls.Add(edit);
        var voidButton = Button("Void", Color.FromArgb(190,54,54)); voidButton.Click += (_, _) => VoidExpense(); filters.Controls.Add(voidButton);
        _search.TextChanged += (_, _) => RefreshGrid(); _category.SelectedIndexChanged += (_, _) => RefreshGrid(); _from.ValueChanged += (_, _) => RefreshGrid(); _to.ValueChanged += (_, _) => RefreshGrid();
        root.Controls.Add(filters, 0, 2);

        _grid.Columns.AddRange(new DataGridViewColumn[] {
            TextColumn("Id", "EXPENSE ID", 155), TextColumn("Name", "EXPENSE NAME", 180), TextColumn("Category", "CATEGORY", 160),
            new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "AMOUNT", DataPropertyName = "Amount", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "₱#,##0.00" } },
            new DataGridViewTextBoxColumn { Name = "ExpenseDate", HeaderText = "DATE", DataPropertyName = "ExpenseDate", Width = 105, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } },
            TextColumn("PaymentMethod", "PAYMENT", 105), TextColumn("RecordedBy", "RECORDED BY", 120), TextColumn("Status", "STATUS", 90) });
        root.Controls.Add(_grid, 0, 3); Controls.Add(root);
    }

    private static DataGridViewTextBoxColumn TextColumn(string name, string header, int width) => new() { Name = name, HeaderText = header, DataPropertyName = name, Width = width };
    private static Label Metric(string title) => new() { Text = title + "\n₱0.00", AutoSize = false, Width = 210, Height = 58, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(29,36,52), Margin = new Padding(0, 0, 20, 0) };
    private static Button Button(string text, Color? color = null) => new() { Text = text, AutoSize = true, Height = 32, FlatStyle = FlatStyle.Flat, BackColor = color ?? Color.FromArgb(255,132,35), ForeColor = Color.White, FlatAppearance = { BorderSize = 0 }, Margin = new Padding(8, 0, 0, 0) };

    private void RefreshData()
    {
        var categories = new[] { "All Categories" }.Concat(DataStore.ExpenseCategories.OrderBy(c => c)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var selected = _category.SelectedItem?.ToString(); _category.DataSource = categories; if (selected is not null && categories.Contains(selected)) _category.SelectedItem = selected;
        var active = DataStore.ActiveExpenses.ToList(); var today = DateTime.Today; var month = new DateTime(today.Year, today.Month, 1); var year = new DateTime(today.Year, 1, 1);
        _total.Text = $"TOTAL EXPENSES\n₱{active.Sum(e => e.Amount):N2}"; _today.Text = $"EXPENSES TODAY\n₱{active.Where(e => e.ExpenseDate.Date == today).Sum(e => e.Amount):N2}"; _month.Text = $"THIS MONTH\n₱{active.Where(e => e.ExpenseDate.Date >= month).Sum(e => e.Amount):N2}"; _year.Text = $"THIS YEAR\n₱{active.Where(e => e.ExpenseDate.Date >= year).Sum(e => e.Amount):N2}";
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        var search = _search.Text.Trim(); var category = _category.SelectedItem?.ToString();
        var rows = DataStore.Expenses.Where(e => e.ExpenseDate.Date >= _from.Value.Date && e.ExpenseDate.Date <= _to.Value.Date && (category is null || category == "All Categories" || e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrWhiteSpace(search) || $"{e.Name} {e.Category} {e.Description} {e.Id}".Contains(search, StringComparison.OrdinalIgnoreCase))).OrderByDescending(e => e.ExpenseDate).ThenBy(e => e.Name).ToList();
        _grid.DataSource = null; _grid.DataSource = rows;
    }

    private Expense? SelectedExpense() => _grid.CurrentRow?.DataBoundItem as Expense;
    private void EditExpense(Expense? existing)
    {
        if (existing is null && !HasWriteAccess()) return;
        using var editor = new ExpenseEditor(existing, _username, DataStore.ExpenseCategories);
        if (editor.ShowDialog(this) != DialogResult.OK || editor.Value is null) return;
        var expense = editor.Value; if (existing is null) DataStore.Expenses.Add(expense); else { var index = DataStore.Expenses.FindIndex(e => e.Id == existing.Id); if (index >= 0) DataStore.Expenses[index] = expense; }
        if (!DataStore.ExpenseCategories.Contains(expense.Category, StringComparer.OrdinalIgnoreCase)) DataStore.ExpenseCategories.Add(expense.Category);
        DataStore.SaveExpenses(); DataStore.RecordStaffActivity(_username, existing is null ? "Expense added" : "Expense updated", $"{expense.Name}: ₱{expense.Amount:N2} ({expense.Category})"); RefreshData();
    }

    private void VoidExpense()
    {
        if (!HasWriteAccess()) return; var expense = SelectedExpense(); if (expense is null) { MessageBox.Show("Select an expense first."); return; }
        if (expense.IsDeleted || expense.Status.Equals("Voided", StringComparison.OrdinalIgnoreCase)) return;
        if (MessageBox.Show($"Void expense {expense.Id}? It will remain in the history.", "Confirm void", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        expense.IsDeleted = true; expense.Status = "Voided"; expense.DeletedAt = DateTime.Now; expense.DeletedBy = _username; DataStore.SaveExpenses(); DataStore.SaveArchiveRecord("Expense", expense.Id, expense.Name, expense.DeletedAt.Value, _username, $"Amount: {expense.Amount:N2}; Category: {expense.Category}"); DataStore.RecordStaffActivity(_username, "Expense voided", $"Voided expense {expense.Id}."); RefreshData();
    }

    private bool HasWriteAccess() => _canManageExpenses || DataStore.Users.FirstOrDefault(u => u.Username.Equals(_username, StringComparison.OrdinalIgnoreCase))?.Role.Equals("Owner", StringComparison.OrdinalIgnoreCase) == true;

    private sealed class ExpenseEditor : Form
    {
        private readonly TextBox _name = new(); private readonly ComboBox _category = new() { DropDownStyle = ComboBoxStyle.DropDown }; private readonly NumericUpDown _amount = new() { DecimalPlaces = 2, Maximum = 100000000, Minimum = 0.01m, Increment = 0.01m, Width = 150 }; private readonly DateTimePicker _date = new() { Format = DateTimePickerFormat.Short }; private readonly ComboBox _method = new() { DropDownStyle = ComboBoxStyle.DropDownList }; private readonly TextBox _description = new();
        private readonly string _username; private readonly Expense? _existing; public Expense? Value { get; private set; }
        public ExpenseEditor(Expense? existing, string username, IEnumerable<string> categories)
        {
            _existing = existing; _username = username; Text = existing is null ? "Add Expense" : "Edit Expense"; StartPosition = FormStartPosition.CenterParent; Size = new Size(510, 430); MinimumSize = new Size(510, 430); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false;
            _category.Items.AddRange(categories.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).Cast<object>().ToArray()); _method.Items.Add("Cash"); _method.SelectedIndex = 0; _description.Multiline = true; _description.Height = 75; _name.Width = 280; _category.Width = 280;
            var table = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(22), ColumnCount = 2, RowCount = 8 }; table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135)); table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Add(table, "Expense name *", _name, 0); Add(table, "Category *", _category, 1); Add(table, "Amount *", _amount, 2); Add(table, "Expense date *", _date, 3); Add(table, "Payment method", _method, 4); Add(table, "Description", _description, 5);
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft }; var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true }; var save = Button("Save"); save.Click += (_, _) => Save(); buttons.Controls.Add(cancel); buttons.Controls.Add(save); table.Controls.Add(buttons, 1, 6); Controls.Add(table); AcceptButton = save; CancelButton = cancel;
            if (existing is not null) { _name.Text = existing.Name; _category.Text = existing.Category; _amount.Value = Math.Max(_amount.Minimum, Math.Min(_amount.Maximum, existing.Amount)); _date.Value = existing.ExpenseDate; _method.SelectedIndex = 0; _description.Text = existing.Description; }
        }
        private static void Add(TableLayoutPanel t, string label, Control control, int row) { t.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 7, 0, 0) }, 0, row); t.Controls.Add(control, 1, row); }
        private static Button Button(string text) => new() { Text = text, AutoSize = true, BackColor = Color.FromArgb(255,132,35), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(_name.Text) || string.IsNullOrWhiteSpace(_category.Text)) { MessageBox.Show("Expense name and category are required."); return; }
            if (_amount.Value <= 0) { MessageBox.Show("Amount must be greater than zero."); return; }
            if (_date.Value.Date > DateTime.Today) { MessageBox.Show("Expense date cannot be in the future."); return; }
            Value = new Expense { Id = _existing?.Id ?? DataStore.NewExpenseId(), Name = _name.Text.Trim(), Category = _category.Text.Trim(), Amount = _amount.Value, ExpenseDate = _date.Value.Date, PaymentMethod = "Cash", Description = _description.Text.Trim(), RecordedBy = _existing?.RecordedBy ?? _username, Status = _existing?.Status ?? "Active", IsDeleted = _existing?.IsDeleted ?? false, DeletedAt = _existing?.DeletedAt, DeletedBy = _existing?.DeletedBy ?? "" }; DialogResult = DialogResult.OK; Close();
        }
    }
}
