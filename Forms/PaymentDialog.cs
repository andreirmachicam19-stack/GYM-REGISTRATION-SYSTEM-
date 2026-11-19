using GymRegistrationApp.Models;
using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public partial class PaymentDialog : Form
{
    public Member? SelectedMember => cmbMember.SelectedItem as Member;
    public decimal Amount => numAmount.Value;
    public string Method => cmbMethod.SelectedItem?.ToString() ?? "Cash";
    public bool IsRenewal => false;
    public string Plan => cmbPlan.SelectedItem?.ToString() ?? "Monthly";
    public DateTime ExpiryDate => dtExpiry.Value.Date;

    public PaymentDialog()
    {
        InitializeComponent(); 
        chkRenew.Visible = false;
        lblPlan.Visible = false;
        cmbPlan.Visible = false;
        lblExpiry.Visible = false;
        dtExpiry.Visible = false;
        cmbMethod.SelectedIndex = 0;
        cmbMethod.Enabled = false;
        cmbMember.DisplayMember = "FullName"; 
        cmbMember.ValueMember = "Id";
        cmbMember.DataSource = DataStore.Members.OrderBy(m => m.FullName).ToList();
        cmbMember.SelectedIndexChanged += cmbMember_SelectedIndexChanged;
        UpdateAmountLimit();

        if (cmbMember.SelectedItem is Member m) 
            numAmount.Value = Math.Min(numAmount.Maximum, DataStore.GetPlanFee(m.Plan));

        // Test database connection on form load
        TestDatabaseConnection();

        chkRenew_CheckedChanged(this, EventArgs.Empty);
    }

    public void SelectMember(Member member)
    {
        var index = cmbMember.FindStringExact(member.FullName);
        if (index >= 0)
        {
            cmbMember.SelectedIndex = index;
        }
        else
        {
            cmbMember.SelectedItem = member;
        }

        UpdateAmountLimit();
    }

    private void cmbMember_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateAmountLimit();
    }

    private void UpdateAmountLimit()
    {
        var max = SelectedMember is null ? 0m : DataStore.GetPlanFee(SelectedMember.Plan);
        numAmount.Maximum = max;
        if (Amount > numAmount.Maximum)
        {
            numAmount.Value = numAmount.Maximum;
        }
    }

    private void TestDatabaseConnection()
    {
        // Silently test connection; the form continues regardless
        try
        {
            _ = DatabaseConnection.TestConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection test in PaymentDialog: {ex.Message}");
        }
    }
    private void chkRenew_CheckedChanged(object? sender, EventArgs e) { cmbPlan.Enabled = dtExpiry.Enabled = chkRenew.Checked; lblPlan.Enabled = lblExpiry.Enabled = chkRenew.Checked; }
    private void cmbPlan_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbPlan.SelectedItem is null) return;
        var m = SelectedMember; var start = m is not null && m.ExpiryDate.Date >= DateTime.Today ? m.ExpiryDate.Date : DateTime.Today;
        dtExpiry.Value = DataStore.GetPlanExpiry(cmbPlan.SelectedItem.ToString() ?? "Monthly", start);
    }
    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (SelectedMember is null) { MessageBox.Show("Select a member."); return; }
        if (Amount <= 0) { MessageBox.Show("Payment amount must be greater than zero."); numAmount.Focus(); return; }
        var maxAllowed = DataStore.GetPlanFee(SelectedMember.Plan);
        if (Amount > maxAllowed) { MessageBox.Show($"Payment amount cannot exceed the remaining balance of ₱{maxAllowed:N2}.", "Invalid payment", MessageBoxButtons.OK, MessageBoxIcon.Warning); numAmount.Focus(); return; }
        if (IsRenewal && dtExpiry.Value.Date < DateTime.Today) { MessageBox.Show("Expiration date cannot be in the past."); return; }
        DialogResult = DialogResult.OK; Close();
    }
}
