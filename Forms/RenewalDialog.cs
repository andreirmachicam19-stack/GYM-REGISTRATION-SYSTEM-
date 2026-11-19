using GymRegistrationApp.Models;

namespace GymRegistrationApp.Forms;

public partial class RenewalDialog : Form
{
    private readonly Member _member;
    private readonly Dictionary<string, decimal> _planFees = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Monthly"] = Member.MonthlyFee,
        ["2-Month Plan"] = Member.TwoMonthFee
    };

    public string SelectedPlan => cmbPlan.SelectedItem?.ToString() ?? "Monthly";
    public DateTime StartDate => dtStart.Value.Date;
    public DateTime ExpiryDate => dtExpiry.Value.Date;
    public decimal RenewalFee => numRenewalFee.Value;
    public decimal AmountPaid => numAmountPaid.Value;
    public decimal Change => Math.Max(0m, AmountPaid - RenewalFee);
    public string PaymentMethod => cmbMethod.SelectedItem?.ToString() ?? "Cash";

    public RenewalDialog(Member member)
    {
        _member = member;
        InitializeComponent();
        LoadPlans();
        PopulateDefaults();
        UpdateFeePreview();
    }

    private void LoadPlans()
    {
        cmbPlan.Items.Clear();
        cmbPlan.Items.AddRange(new object[] { "Monthly", "2-Month Plan" });
        var selectedPlan = string.Equals(_member.Plan, "2M Plan", StringComparison.OrdinalIgnoreCase) ? "2-Month Plan" : _member.Plan;
        cmbPlan.SelectedIndex = cmbPlan.FindStringExact(selectedPlan) >= 0 ? cmbPlan.FindStringExact(selectedPlan) : 0;
    }

    private void PopulateDefaults()
    {
        var start = DateTime.Today;
        if (_member.ExpiryDate.Date > DateTime.Today)
        {
            start = _member.ExpiryDate.Date;
        }

        dtStart.Value = start;
        dtExpiry.Enabled = false;
        cmbMethod.SelectedIndex = 0;
        cmbMethod.Enabled = false;
        numRenewalFee.Value = Math.Min(decimal.MaxValue, GetTotalDueForSelectedPlan());
        numAmountPaid.Value = numRenewalFee.Value;
        UpdateExpiryDate();
    }

    private void UpdateFeePreview()
    {
        var fee = RenewalFee;
        var paid = AmountPaid;
        var remaining = Math.Max(0m, fee - paid);
        lblChangeValue.Text = $"₱{Change:N2}";
        lblAmountSummary.Text = $"Fixed fee: ₱{fee:N2}  |  Amount paid: ₱{paid:N2}  |  Change: ₱{Change:N2}  |  Balance: ₱{remaining:N2}";
    }

    private void cmbPlan_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateExpiryDate();
        numRenewalFee.Value = GetTotalDueForSelectedPlan();
        UpdateFeePreview();
    }

    private void dtStart_ValueChanged(object? sender, EventArgs e)
    {
        UpdateExpiryDate();
    }

    private void numRenewalFee_ValueChanged(object? sender, EventArgs e)
    {
        if (numAmountPaid.Value > numRenewalFee.Value)
        {
            numAmountPaid.Value = numRenewalFee.Value;
        }
        UpdateFeePreview();
    }

    private void numAmountPaid_ValueChanged(object? sender, EventArgs e)
    {
        UpdateFeePreview();
    }

    private void UpdateExpiryDate()
    {
        var start = dtStart.Value.Date;
        var plan = SelectedPlan;
        dtExpiry.Value = plan switch
        {
            "Monthly" => start.AddMonths(1),
            "2-Month Plan" => start.AddMonths(2),
            _ => start.AddMonths(1)
        };
    }

    private decimal GetDefaultFeeForPlan(string plan)
    {
        return _planFees.TryGetValue(plan, out var fee) ? fee : Member.MonthlyFee;
    }

    private decimal GetTotalDueForSelectedPlan()
    {
        return GetDefaultFeeForPlan(SelectedPlan);
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (RenewalFee <= 0m)
        {
            MessageBox.Show("Renewal fee must be greater than zero.", "Invalid fee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            numRenewalFee.Focus();
            return;
        }

        if (RenewalFee != GetDefaultFeeForPlan(SelectedPlan)
            || !string.Equals(PaymentMethod, "Cash", StringComparison.OrdinalIgnoreCase)
            || ExpiryDate != StartDate.AddMonths(SelectedPlan == "2-Month Plan" ? 2 : 1))
        {
            MessageBox.Show("Select a supported plan. The fee, expiry date, and payment method are fixed for renewals.", "Invalid renewal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (AmountPaid < 0m)
        {
            MessageBox.Show("Amount paid cannot be negative.", "Invalid payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            numAmountPaid.Focus();
            return;
        }

        if (ExpiryDate.Date < StartDate.Date)
        {
            MessageBox.Show("Membership expiry must be on or after the start date.", "Invalid date range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void InitializeComponent()
    {
        Text = "Renew Membership";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        Width = 470;
        Height = 690;
        BackColor = Color.White;

        var title = new Label
        {
            Text = "Renew membership",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(29, 36, 52),
            AutoSize = true,
            Location = new Point(24, 20)
        };

        var subtitle = new Label
        {
            Text = $"Member: {_member.FullName}",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(100, 108, 122),
            AutoSize = true,
            Location = new Point(24, 58)
        };

        var lblPlan = new Label { Text = "New membership plan", Location = new Point(24, 94), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        cmbPlan = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(24, 118), Width = 400, Font = new Font("Segoe UI", 10F) };
        cmbPlan.SelectedIndexChanged += cmbPlan_SelectedIndexChanged;

        var lblStart = new Label { Text = "Start date", Location = new Point(24, 162), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        dtStart = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(24, 186), Width = 400, Value = DateTime.Today };
        dtStart.ValueChanged += dtStart_ValueChanged;

        var lblExpiry = new Label { Text = "Expiry date", Location = new Point(24, 230), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        dtExpiry = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(24, 254), Width = 400, Value = DateTime.Today.AddMonths(1) };

        var lblFee = new Label { Text = "Total amount due (fixed by plan)", Location = new Point(24, 298), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        numRenewalFee = new NumericUpDown { DecimalPlaces = 2, Minimum = 0, Maximum = 9999999, Location = new Point(24, 322), Width = 400, Value = Member.MonthlyFee, ReadOnly = true, Enabled = false };
        numRenewalFee.ValueChanged += numRenewalFee_ValueChanged;

        var lblAmount = new Label { Text = "Amount paid", Location = new Point(24, 366), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        numAmountPaid = new NumericUpDown { DecimalPlaces = 2, Minimum = 0, Maximum = 9999999, Location = new Point(24, 390), Width = 400, Value = Member.MonthlyFee };
        numAmountPaid.ValueChanged += numAmountPaid_ValueChanged;

        var lblChange = new Label { Text = "Change", Location = new Point(24, 434), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        lblChangeValue = new Label { Text = "₱0.00", Location = new Point(24, 458), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(32, 128, 80) };

        var lblMethod = new Label { Text = "Payment method", Location = new Point(220, 434), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        cmbMethod = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(220, 458), Width = 204, Font = new Font("Segoe UI", 10F) };
        cmbMethod.Items.Add("Cash");
        cmbMethod.SelectedIndex = 0;
        cmbMethod.Enabled = false;

        lblAmountSummary = new Label
        {
            Text = "Fixed fee: ₱0.00 | Amount paid: ₱0.00 | Change: ₱0.00 | Balance: ₱0.00",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(32, 128, 80),
            Location = new Point(24, 510),
            AutoSize = true
        };

        btnSave = new Button
        {
            Text = "Confirm renewal",
            BackColor = Color.FromArgb(255, 132, 35),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Width = 150,
            Height = 40,
            Location = new Point(180, 570),
        };
        btnSave.Click += btnSave_Click;

        Controls.Add(title);
        Controls.Add(subtitle);
        Controls.Add(lblPlan);
        Controls.Add(cmbPlan);
        Controls.Add(lblStart);
        Controls.Add(dtStart);
        Controls.Add(lblExpiry);
        Controls.Add(dtExpiry);
        Controls.Add(lblFee);
        Controls.Add(numRenewalFee);
        Controls.Add(lblAmount);
        Controls.Add(numAmountPaid);
        Controls.Add(lblChange);
        Controls.Add(lblChangeValue);
        Controls.Add(lblMethod);
        Controls.Add(cmbMethod);
        Controls.Add(lblAmountSummary);
        Controls.Add(btnSave);
    }

    private ComboBox cmbPlan = null!;
    private DateTimePicker dtStart = null!;
    private DateTimePicker dtExpiry = null!;
    private NumericUpDown numRenewalFee = null!;
    private NumericUpDown numAmountPaid = null!;
    private ComboBox cmbMethod = null!;
    private Label lblAmountSummary = null!;
    private Label lblChangeValue = null!;
    private Button btnSave = null!;
}
