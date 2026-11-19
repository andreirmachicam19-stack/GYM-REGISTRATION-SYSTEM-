using GymRegistrationApp.Models;
using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public sealed class DailyGymForm : Form
{
    private readonly string _username;
    private readonly DateTimePicker _datePicker = new() { Format = DateTimePickerFormat.Long, Width = 270 };
    private readonly ComboBox _memberSearch = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDown, DisplayMember = "DisplayName" };
    private readonly Label _selectedSummary = new() { AutoSize = true };
    private readonly Label _monthSummary = new() { AutoSize = true };
    private readonly Label _memberType = new() { AutoSize = true, Text = "Walk-in visitor", ForeColor = Color.FromArgb(100, 108, 122) };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = false, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
    private readonly TextBox _firstName = new();
    private readonly TextBox _middleName = new();
    private readonly TextBox _lastName = new();
    private Button _addButton = null!;
    private Button _editButton = null!;
    private Button _archiveButton = null!;
    private MemberOption? _selectedMember;
    private bool _updatingMemberFields;
    private bool _refreshingMemberSearch;
    private decimal DailyFee => DataStore.Plans.FirstOrDefault(p => p.IsActive && p.Name.Equals("Daily", StringComparison.OrdinalIgnoreCase))?.Price ?? 50m;
    private DailyGymVisit? Selected => (_grid.CurrentRow?.DataBoundItem as DailyGymVisitRow)?.Visit;
    private bool IsToday => _datePicker.Value.Date == AttendanceService.ManilaNow().Date;

    public DailyGymForm(string username, DateTime? selectedDate = null)
    {
        _username = username;
        _datePicker.Value = selectedDate?.Date ?? DateTime.Today;
        Text = "Daily Gym";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(850, 520);
        Size = new Size(1050, 680);
        BackColor = Color.FromArgb(245, 246, 249);
        BuildLayout();
        _datePicker.ValueChanged += (_, _) => RefreshView();
        _memberSearch.TextChanged += (_, _) =>
        {
            if (!_refreshingMemberSearch && _memberSearch.SelectedItem is not MemberOption) RefreshMemberSearch();
        };
        _memberSearch.SelectedIndexChanged += (_, _) => SelectMemberFromSearch();
        _firstName.TextChanged += (_, _) => { if (!_updatingMemberFields) ClearSelectedMember(); };
        _lastName.TextChanged += (_, _) => { if (!_updatingMemberFields) ClearSelectedMember(); };
        RefreshView();
        DataStore.DataReloaded += DataStore_DataReloaded;
        FormClosed += (_, _) => DataStore.DataReloaded -= DataStore_DataReloaded;
    }

    private void DataStore_DataReloaded(object? sender, EventArgs e)
    {
        if (!IsDisposed && IsHandleCreated) RefreshView();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), RowCount = 6, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var title = new Label { Text = "Daily Gym", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(29, 36, 52), Dock = DockStyle.Fill };
        root.Controls.Add(title, 0, 0);
        var calendar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        var previous = Button("Previous"); previous.Click += (_, _) => _datePicker.Value = _datePicker.Value.Date.AddDays(-1);
        var today = Button("Today"); today.Click += (_, _) => _datePicker.Value = DateTime.Today;
        var next = Button("Next"); next.Click += (_, _) => _datePicker.Value = _datePicker.Value.Date.AddDays(1);
        calendar.Controls.AddRange(new Control[] { previous, _datePicker, today, next });
        root.Controls.Add(calendar, 0, 1);
        var metrics = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = Color.White, Padding = new Padding(14) };
        metrics.Controls.Add(_selectedSummary); metrics.Controls.Add(new Label { Width = 28 }); metrics.Controls.Add(_monthSummary);
        root.Controls.Add(metrics, 0, 2);
        var searchPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        searchPanel.Controls.Add(new Label { Text = "Search member", AutoSize = true, Margin = new Padding(0, 8, 8, 0) });
        _memberSearch.Width = 300;
        searchPanel.Controls.Add(_memberSearch);
        searchPanel.Controls.Add(_memberType);
        root.Controls.Add(searchPanel, 0, 3);

        var editor = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        editor.Controls.Add(new Label { Text = "First Name *", AutoSize = true, Margin = new Padding(0, 8, 6, 0) });
        _firstName.Width = 150; editor.Controls.Add(_firstName);
        editor.Controls.Add(new Label { Text = "Middle Name", AutoSize = true, Margin = new Padding(14, 8, 6, 0) });
        _middleName.Width = 130; editor.Controls.Add(_middleName);
        editor.Controls.Add(new Label { Text = "Last Name", AutoSize = true, Margin = new Padding(14, 8, 6, 0) });
        _lastName.Width = 150; editor.Controls.Add(_lastName);
         _addButton = Button("Add visitor"); _addButton.Click += (_, _) => SaveVisit(false);
         _editButton = Button("Edit selected"); _editButton.Click += (_, _) => SaveVisit(true);
         _archiveButton = Button("Move to archive"); _archiveButton.Click += (_, _) => ArchiveSelectedVisit();
         editor.Controls.AddRange(new Control[] { _addButton, _editButton, _archiveButton, new Label { Text = $"Cash • ₱{DailyFee:N2}", AutoSize = true, Margin = new Padding(14, 8, 0, 0), ForeColor = Color.FromArgb(100, 108, 122) } });
        root.Controls.Add(editor, 0, 4);
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "VISITOR", DataPropertyName = "FullName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "TYPE", DataPropertyName = "VisitorType", Width = 125 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "VISIT DATE", DataPropertyName = "VisitDate", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "MMM d, yyyy" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "FEE", DataPropertyName = "Fee", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "₱#,##0.00" } });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "PAYMENT", DataPropertyName = "PaymentMethod", Width = 100 });
        _grid.SelectionChanged += (_, _) =>
        {
            if (Selected is not { } v) return;
            _updatingMemberFields = true;
            _firstName.Text = v.FirstName;
            _middleName.Text = v.MiddleName;
            _lastName.Text = v.LastName;
            _updatingMemberFields = false;
            _selectedMember = string.IsNullOrWhiteSpace(v.MemberId) ? null : FindMember(v.MemberId);
            UpdateMemberType();
        };
        root.Controls.Add(_grid, 0, 5);
        Controls.Add(root);
    }

    private static Button Button(string text) => new() { Text = text, AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 132, 35), ForeColor = Color.White, Margin = new Padding(4, 0, 4, 0) };

    private void RefreshView()
    {
        var date = _datePicker.Value.Date;
        var monthStart = new DateTime(date.Year, date.Month, 1);
        var visits = DataStore.DailyGymVisits.Where(v => !v.IsDeleted && v.VisitDate.Date == date).OrderBy(v => v.FullName).Select(v => new DailyGymVisitRow(v)).ToList();
        var monthVisits = DataStore.DailyGymVisits.Where(v => v.VisitDate >= monthStart && v.VisitDate < monthStart.AddMonths(1) && !v.IsDeleted).ToList();
        _selectedSummary.Text = $"{visits.Count(v => !v.IsDeleted)} visitors  •  Walk-in Revenue ₱{visits.Where(v => !v.IsDeleted).Sum(v => v.Fee):N2}";
        _monthSummary.Text = $"This month: {monthVisits.Count} visits  •  ₱{monthVisits.Sum(v => v.Fee):N2}";
        _grid.DataSource = null; _grid.DataSource = visits;
        _grid.RowsDefaultCellStyle.BackColor = Color.White;
        UpdateInputState();
        RefreshMemberSearch();
    }

    private void UpdateInputState()
    {
        var canEdit = IsToday;
        _memberSearch.Enabled = canEdit;
        _firstName.Enabled = canEdit;
        _middleName.Enabled = canEdit;
        _lastName.Enabled = canEdit;
        _addButton.Enabled = canEdit;
        _editButton.Enabled = canEdit;
        _archiveButton.Enabled = canEdit;
    }

    private void RefreshMemberSearch()
    {
        var text = _memberSearch.Text;
        var caret = _memberSearch.SelectionStart;
        var selectedDate = _datePicker.Value.Date;
        var attendedMemberIds = DataStore.DailyGymVisits
            .Where(v => !v.IsDeleted && v.VisitDate.Date == selectedDate && !string.IsNullOrWhiteSpace(v.MemberId))
            .Select(v => v.MemberId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var members = DataStore.Members
            .Where(m => !m.IsDeleted &&
                        m.ExpiryDate.Date >= selectedDate &&
                        string.Equals(m.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                        !attendedMemberIds.Contains(m.Id))
            .Select(m => new MemberOption(m))
            .Where(m => string.IsNullOrWhiteSpace(text) || m.DisplayName.Contains(text, StringComparison.OrdinalIgnoreCase) || m.Id.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.DisplayName)
            .ToList();
        _refreshingMemberSearch = true;
        _memberSearch.BeginUpdate();
        _memberSearch.Items.Clear();
        foreach (var member in members) _memberSearch.Items.Add(member);
        _memberSearch.Text = text;
        _memberSearch.SelectionStart = Math.Min(caret, _memberSearch.Text.Length);
        _memberSearch.EndUpdate();
        _refreshingMemberSearch = false;
        if (_memberSearch.Focused && members.Count > 0) _memberSearch.DroppedDown = true;
    }

    private void SelectMemberFromSearch()
    {
        if (_memberSearch.SelectedItem is not MemberOption member) return;
        _selectedMember = member;
        _updatingMemberFields = true;
        _firstName.Text = member.Member.FirstName;
        _middleName.Text = member.Member.MiddleName;
        _lastName.Text = member.Member.LastName;
        _updatingMemberFields = false;
        UpdateMemberType();
    }

    private void ClearSelectedMember()
    {
        _selectedMember = null;
        UpdateMemberType();
    }

    private void UpdateMemberType()
    {
        _memberType.Text = _selectedMember is null ? "Walk-in visitor" : $"Gym member • {_selectedMember.Id}";
        _memberType.ForeColor = _selectedMember is null ? Color.FromArgb(100, 108, 122) : Color.FromArgb(43, 157, 105);
    }

    private MemberOption? FindMember(string id)
    {
        var member = DataStore.Members.FirstOrDefault(m => !m.IsDeleted && m.Id == id);
        return member is null ? null : new MemberOption(member);
    }

    private void SaveVisit(bool editing)
    {
        if (!IsToday)
        {
            MessageBox.Show("Daily Gym input is allowed for today only.", "Today only", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var first = _firstName.Text.Trim();
        if (string.IsNullOrWhiteSpace(first)) { MessageBox.Show("First Name is required."); return; }
        var visit = editing ? Selected : null;
        if (editing && visit is null) { MessageBox.Show("Select a visitor to edit."); return; }
        var date = _datePicker.Value.Date;
        var middle = _middleName.Text.Trim();
        var last = _lastName.Text.Trim();
        var duplicate = DataStore.DailyGymVisits.Any(v =>
            !v.IsDeleted &&
            v.VisitDate.Date == date &&
            (visit is null || v.Id != visit.Id) &&
            string.Equals(v.FirstName.Trim(), first, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(v.LastName.Trim(), last, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            MessageBox.Show($"{first} {last}".Trim() + $" is already registered for {date:MMM d, yyyy}.", "Duplicate visitor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        var isGymMember = _selectedMember is not null;
        if (isGymMember && date == AttendanceService.ManilaNow().Date && !editing)
        {
            var result = AttendanceService.Record(
                new AttendanceSubmission(first, middle, last, _selectedMember!.Id),
                _username);
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Attendance", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataStore.Reload();
            _firstName.Clear();
            _middleName.Clear();
            _lastName.Clear();
            RefreshView();
            return;
        }
        if (visit is null)
        {
            visit = new DailyGymVisit { Id = DataStore.NewDailyGymVisitId(), VisitDate = date, RegisteredBy = _username };
            DataStore.DailyGymVisits.Add(visit);
        }
        var dailyFee = DailyFee;
        visit.FirstName = first; visit.MiddleName = middle; visit.LastName = last; visit.MemberId = _selectedMember?.Id ?? ""; visit.IsGymMember = isGymMember; visit.Fee = isGymMember ? 0m : dailyFee; visit.PaymentMethod = isGymMember ? "Included" : "Cash"; visit.VisitDate = date; visit.IsDeleted = false;

        if (isGymMember)
        {
            DataStore.Payments.RemoveAll(p =>
                (!string.IsNullOrWhiteSpace(visit.PaymentReference) && p.Reference == visit.PaymentReference) ||
                (p.MemberId == visit.Id && string.Equals(p.PaymentType, "Daily Gym Pass", StringComparison.OrdinalIgnoreCase)));
            visit.PaymentReference = "";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(visit.PaymentReference)) visit.PaymentReference = DataStore.NewPaymentReference();
            var payment = DataStore.Payments.FirstOrDefault(p => p.Reference == visit.PaymentReference);
            if (payment is null)
            {
                DataStore.Payments.Add(new Payment { Reference = visit.PaymentReference, MemberId = "", MemberName = visit.FullName, Amount = dailyFee, AmountPaid = dailyFee, Method = "Cash", PaymentType = "Daily Gym Pass", CollectedBy = _username, Date = visit.VisitDate, Status = "Paid" });
            }
            else
            {
                payment.MemberId = ""; payment.MemberName = visit.FullName; payment.Date = visit.VisitDate; payment.Amount = dailyFee; payment.AmountPaid = dailyFee; payment.Change = 0m; payment.Method = "Cash"; payment.IsDeleted = false; payment.Status = "Paid";
            }
        }
        DataStore.AddOrUpdateAttendanceFromMemberVisit(visit, _username);
        DataStore.SaveDailyGymVisits(); DataStore.SavePayments(); _firstName.Clear(); _middleName.Clear(); _lastName.Clear(); RefreshView();
    }

    private void ArchiveSelectedVisit()
    {
        if (!IsToday)
        {
            MessageBox.Show("Daily Gym changes are allowed for today only.", "Today only", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (Selected is not { } visit)
        {
            MessageBox.Show("Select a visitor to move to Archive.");
            return;
        }

        if (MessageBox.Show($"Move {visit.FullName} to Archive? It can be restored later.", "Confirm archive", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

        visit.IsDeleted = true;
        visit.DeletedAt = DateTime.Now;
        visit.DeletedBy = _username;
        // Archiving the visit removes it from the active Daily Gym list, but must
        // not archive the payment: the payment is the historical source of truth
        // for walk-in revenue and should remain recognized.

        DataStore.SaveDailyGymVisits();
        DataStore.SaveArchiveRecord("DailyGymVisit", visit.Id, visit.FullName, visit.DeletedAt.Value, visit.DeletedBy, visit.VisitDate.ToString("yyyy-MM-dd"));
        _firstName.Clear();
        _lastName.Clear();
        RefreshView();
    }

    private sealed class MemberOption
    {
        public Member Member { get; }
        public string Id => Member.Id;
        public string DisplayName => string.IsNullOrWhiteSpace(Member.FullName)
            ? string.Join(" ", new[] { Member.FirstName, Member.MiddleName, Member.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)))
            : Member.FullName;

        public MemberOption(Member member) => Member = member;

        public override string ToString() => $"{DisplayName} ({Id})";
    }

    private sealed class DailyGymVisitRow
    {
        public DailyGymVisit Visit { get; }
        public string FullName { get; }
        public string VisitorType { get; }
        public DateTime VisitDate { get; }
        public decimal Fee { get; }
        public string PaymentMethod { get; }
        public bool IsDeleted { get; }

        public DailyGymVisitRow(DailyGymVisit visit)
        {
            Visit = visit;
            FullName = visit.FullName;
            VisitorType = visit.IsGymMember ? "Gym member" : "Walk-in";
            VisitDate = visit.VisitDate;
            Fee = visit.Fee;
            PaymentMethod = visit.PaymentMethod;
            IsDeleted = visit.IsDeleted;
        }
    }

}
