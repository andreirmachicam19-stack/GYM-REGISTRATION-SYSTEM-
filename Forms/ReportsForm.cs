using System.Data;
using System.Text;
using GymRegistrationApp.Models;
using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public sealed class ReportsForm : Form
{
    private readonly ComboBox _report = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 210 };
    private readonly ComboBox _preset = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
    private readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, Width = 120 };
    private readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, Width = 120 };
    private readonly Label _summary = new() { Dock = DockStyle.Fill, AutoSize = false, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(29,36,52), Padding = new Padding(12) };
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White };
    private DataTable _table = new(); private DateTime _start; private DateTime _end;

    public ReportsForm()
    {
        Text = "Reports Management"; StartPosition = FormStartPosition.CenterParent; MinimumSize = new Size(1000, 650); Size = new Size(1200, 760); BackColor = Color.FromArgb(246,247,250);
        _report.Items.AddRange(new object[] { "Member Registration Report", "Payment & Revenue Report", "Attendance Report", "Membership Status Report", "Expenses Report", "Financial Summary Report" }); _report.SelectedIndex = 0;
        _preset.Items.AddRange(new object[] { "Today", "Yesterday", "This Week", "This Month", "This Year", "Custom Date Range" }); _preset.SelectedIndex = 0;
        _report.SelectedIndexChanged += (_, _) => RefreshReport(); _preset.SelectedIndexChanged += (_, _) => ApplyPreset(); _from.ValueChanged += (_, _) => { if (_preset.SelectedItem?.ToString() == "Custom Date Range") RefreshReport(); }; _to.ValueChanged += (_, _) => { if (_preset.SelectedItem?.ToString() == "Custom Date Range") RefreshReport(); };
        BuildLayout(); ApplyPreset();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), RowCount = 4, ColumnCount = 1 }; root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(new Label { Text = "Reports Management", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(29,36,52), Dock = DockStyle.Fill }, 0, 0);
        var filters = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 8, 0, 0) }; filters.Controls.Add(new Label { Text = "Report", AutoSize = true, Margin = new Padding(0, 7, 6, 0) }); filters.Controls.Add(_report); filters.Controls.Add(new Label { Text = "Period", AutoSize = true, Margin = new Padding(15, 7, 6, 0) }); filters.Controls.Add(_preset); filters.Controls.Add(new Label { Text = "From", AutoSize = true, Margin = new Padding(15, 7, 6, 0) }); filters.Controls.Add(_from); filters.Controls.Add(new Label { Text = "To", AutoSize = true, Margin = new Padding(8, 7, 6, 0) }); filters.Controls.Add(_to); var export = Button("Export PDF"); export.Click += (_, _) => ExportPdf(); filters.Controls.Add(export); root.Controls.Add(filters, 0, 1);
        root.Controls.Add(_summary, 0, 2); root.Controls.Add(_grid, 0, 3); Controls.Add(root);
    }
    private static Button Button(string text) => new() { Text = text, AutoSize = true, Height = 32, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255,132,35), ForeColor = Color.White, FlatAppearance = { BorderSize = 0 }, Margin = new Padding(12, 0, 0, 0) };

    private void ApplyPreset()
    {
        var today = DateTime.Today; var preset = _preset.SelectedItem?.ToString(); _from.Enabled = _to.Enabled = preset == "Custom Date Range";
        (_start, _end) = preset switch { "Yesterday" => (today.AddDays(-1), today.AddDays(-1)), "This Week" => (today.AddDays(-(int)today.DayOfWeek), today), "This Month" => (new DateTime(today.Year,today.Month,1), today), "This Year" => (new DateTime(today.Year,1,1), today), _ => (today,today) };
        if (preset == "Custom Date Range") { _start = _from.Value.Date; _end = _to.Value.Date; } else { _from.Value = _start; _to.Value = _end; } RefreshReport();
    }

    private void RefreshReport()
    {
        if (_end < _start) (_start, _end) = (_end, _start); _table = new DataTable(); var title = _report.SelectedItem?.ToString() ?? "Report";
        if (title.StartsWith("Member")) BuildMembers(); else if (title.StartsWith("Payment")) BuildPayments(); else if (title.StartsWith("Attendance")) BuildAttendance(); else if (title.StartsWith("Membership")) BuildStatus(); else if (title.StartsWith("Expenses")) BuildExpenses(); else BuildFinancial();
        _grid.DataSource = _table; _summary.Text = SummaryText(title);
    }

    private IEnumerable<Payment> Revenue() => DataStore.ActivePayments.Where(p => p.Status.Equals("Paid", StringComparison.OrdinalIgnoreCase) && p.Date.Date >= _start && p.Date.Date <= _end);
    private IEnumerable<Expense> Expenses() => DataStore.ActiveExpenses.Where(e => e.ExpenseDate.Date >= _start && e.ExpenseDate.Date <= _end);
    private void BuildMembers() { _table.Columns.Add("Member ID"); _table.Columns.Add("Full Name"); _table.Columns.Add("Membership Plan"); _table.Columns.Add("Registration Date"); _table.Columns.Add("Membership Status"); foreach (var m in DataStore.Members.Where(m => !m.IsDeleted && m.StartDate.Date >= _start && m.StartDate.Date <= _end).OrderBy(m => m.StartDate)) _table.Rows.Add(m.Id,m.FullName,m.Plan,m.StartDate.ToString("yyyy-MM-dd"),m.Status); }
    private void BuildPayments() { _table.Columns.Add("Payment ID"); _table.Columns.Add("Member Name"); _table.Columns.Add("Membership Plan"); _table.Columns.Add("Amount", typeof(decimal)); _table.Columns.Add("Payment Date"); _table.Columns.Add("Payment Method"); foreach (var p in Revenue().OrderByDescending(p => p.Date)) _table.Rows.Add(p.Reference,p.MemberName,DataStore.Members.FirstOrDefault(m => m.Id == p.MemberId)?.Plan ?? "",p.Amount,p.Date.ToString("yyyy-MM-dd HH:mm"),p.Method); }
    private void BuildAttendance() { _table.Columns.Add("Member ID"); _table.Columns.Add("Member Name"); _table.Columns.Add("Check-in Date"); _table.Columns.Add("Check-in Time"); foreach (var a in DataStore.AttendanceRecords.Where(a => !a.IsDeleted && a.AttendanceDate.Date >= _start && a.AttendanceDate.Date <= _end).OrderByDescending(a => a.CheckInTime)) _table.Rows.Add(a.MemberId,a.MemberName,a.AttendanceDate.ToString("yyyy-MM-dd"),a.CheckInTime.ToString("HH:mm:ss")); }
    private void BuildStatus() { _table.Columns.Add("Metric"); _table.Columns.Add("Count", typeof(int)); var soon = DateTime.Today.AddDays(30); _table.Rows.Add("Active Members",DataStore.Members.Count(m => !m.IsDeleted && m.Status.Equals("Active",StringComparison.OrdinalIgnoreCase) && m.ExpiryDate.Date >= DateTime.Today)); _table.Rows.Add("Expired Members",DataStore.Members.Count(m => !m.IsDeleted && (m.ExpiryDate.Date < DateTime.Today || m.Status.Equals("Expired",StringComparison.OrdinalIgnoreCase)))); _table.Rows.Add("Memberships Expiring Soon",DataStore.Members.Count(m => !m.IsDeleted && m.ExpiryDate.Date >= DateTime.Today && m.ExpiryDate.Date <= soon)); _table.Rows.Add("Renewed Memberships",DataStore.MembershipRenewals.Count(r => r.RenewalDate.Date >= _start && r.RenewalDate.Date <= _end && r.Status.Equals("Completed",StringComparison.OrdinalIgnoreCase))); }
    private void BuildExpenses() { _table.Columns.Add("Expense ID"); _table.Columns.Add("Expense Name"); _table.Columns.Add("Category"); _table.Columns.Add("Amount", typeof(decimal)); _table.Columns.Add("Expense Date"); _table.Columns.Add("Payment Method"); _table.Columns.Add("Recorded By"); foreach (var e in Expenses().OrderByDescending(e => e.ExpenseDate)) _table.Rows.Add(e.Id,e.Name,e.Category,e.Amount,e.ExpenseDate.ToString("yyyy-MM-dd"),e.PaymentMethod,e.RecordedBy); }
    private void BuildFinancial() { _table.Columns.Add("Metric"); _table.Columns.Add("Amount", typeof(decimal)); var revenue = Revenue().Sum(p => p.Amount); var expenses = Expenses().Sum(e => e.Amount); _table.Rows.Add("Successful Revenue",revenue); _table.Rows.Add("Active Expenses",expenses); _table.Rows.Add("Net Income",revenue-expenses); }
    private string SummaryText(string title) { var revenue = Revenue().Sum(p => p.Amount); var expenses = Expenses().Sum(e => e.Amount); return $"{title}   |   {_start:MMM d, yyyy} - {_end:MMM d, yyyy}   |   Records: {_table.Rows.Count}   |   Revenue: ₱{revenue:N2}   |   Expenses: ₱{expenses:N2}   |   Net Income: ₱{revenue-expenses:N2}"; }

    private void ExportPdf()
    {
        using var save = new SaveFileDialog { Filter = "PDF file (*.pdf)|*.pdf", FileName = $"{(_report.SelectedItem ?? "report").ToString()!.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmm}.pdf", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) }; if (save.ShowDialog(this) != DialogResult.OK) return;
        try { SimplePdf.Write(save.FileName, "UNIVERSAL GYM", _report.SelectedItem?.ToString() ?? "Report", _start, _end, _summary.Text, _table); MessageBox.Show("PDF export completed.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information); } catch (Exception ex) { MessageBox.Show("Could not export PDF: " + ex.Message, "Export error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private static class SimplePdf
    {
        public static void Write(string path, string gym, string title, DateTime from, DateTime to, string summary, DataTable table)
        {
            var lines = new List<string> { gym, title, $"Date range: {from:yyyy-MM-dd} to {to:yyyy-MM-dd}", $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", summary, "" }; lines.Add(string.Join(" | ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName))); foreach (DataRow row in table.Rows) lines.Add(string.Join(" | ", row.ItemArray.Select(v => Convert.ToString(v) ?? "")));
            var pages = lines.Chunk(46).ToList(); var objects = new List<string>(); objects.Add("<< /Type /Catalog /Pages 2 0 R >>"); objects.Add("<< /Type /Pages /Kids [" + string.Join(" ", Enumerable.Range(0,pages.Count).Select(i => $"{3+i*2} 0 R")) + $"] /Count {pages.Count} >>");
            for (var i=0;i<pages.Count;i++) { var content = new StringBuilder("BT /F1 9 Tf 36 770 Td "); foreach (var line in pages[i]) content.Append('(').Append(Escape(line.Length > 150 ? line[..150] : line)).Append(") Tj 0 -15 Td "); content.Append("ET"); objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 {3+pages.Count*2} 0 R >> >> /Contents {4+i*2} 0 R >>"); objects.Add($"<< /Length {Encoding.ASCII.GetByteCount(content.ToString())} >>\nstream\n{content}\nendstream"); }
            objects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"); var output = new StringBuilder("%PDF-1.4\n"); var offsets = new List<int>(); for (var i=0;i<objects.Count;i++) { offsets.Add(Encoding.ASCII.GetByteCount(output.ToString())); output.Append($"{i+1} 0 obj\n{objects[i]}\nendobj\n"); } var xref=Encoding.ASCII.GetByteCount(output.ToString()); output.Append($"xref\n0 {objects.Count+1}\n0000000000 65535 f \n"); foreach(var offset in offsets) output.Append($"{offset:0000000000} 00000 n \n"); output.Append($"trailer << /Size {objects.Count+1} /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF"); File.WriteAllBytes(path, Encoding.ASCII.GetBytes(output.ToString()));
        }
        private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)").Replace("\r", " ").Replace("\n", " ");
    }
}
