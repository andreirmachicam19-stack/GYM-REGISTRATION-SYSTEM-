#nullable enable

namespace GymRegistrationApp.Forms;

partial class PaymentDialog
{
    private System.ComponentModel.IContainer? components = null;
    private Label lblTitle = null!, lblMember = null!, lblAmount = null!, lblMethod = null!, lblPlan = null!, lblExpiry = null!;
    private ComboBox cmbMember = null!, cmbMethod = null!, cmbPlan = null!; private NumericUpDown numAmount = null!; private CheckBox chkRenew = null!; private DateTimePicker dtExpiry = null!; private Button btnSave = null!, btnCancel = null!;
    protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblMember = new Label();
        lblAmount = new Label();
        lblMethod = new Label();
        lblPlan = new Label();
        lblExpiry = new Label();
        cmbMember = new ComboBox();
        cmbMethod = new ComboBox();
        cmbPlan = new ComboBox();
        numAmount = new NumericUpDown();
        chkRenew = new CheckBox();
        dtExpiry = new DateTimePicker();
        btnSave = new Button();
        btnCancel = new Button();
        ((System.ComponentModel.ISupportInitialize)numAmount).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.Location = new Point(28, 22);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(400, 40);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Record payment";
        // 
        // lblMember
        // 
        lblMember.Location = new Point(0, 0);
        lblMember.Name = "lblMember";
        lblMember.Size = new Size(100, 23);
        lblMember.TabIndex = 1;
        // 
        // lblAmount
        // 
        lblAmount.Location = new Point(0, 0);
        lblAmount.Name = "lblAmount";
        lblAmount.Size = new Size(100, 23);
        lblAmount.TabIndex = 3;
        // 
        // lblMethod
        // 
        lblMethod.Location = new Point(0, 0);
        lblMethod.Name = "lblMethod";
        lblMethod.Size = new Size(100, 23);
        lblMethod.TabIndex = 5;
        // 
        // lblPlan
        // 
        lblPlan.Location = new Point(0, 0);
        lblPlan.Name = "lblPlan";
        lblPlan.Size = new Size(100, 23);
        lblPlan.TabIndex = 8;
        // 
        // lblExpiry
        // 
        lblExpiry.Location = new Point(0, 0);
        lblExpiry.Name = "lblExpiry";
        lblExpiry.Size = new Size(100, 23);
        lblExpiry.TabIndex = 10;
        // 
        // cmbMember
        // 
        cmbMember.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbMember.Location = new Point(28, 128);
        cmbMember.Name = "cmbMember";
        cmbMember.Size = new Size(390, 28);
        cmbMember.TabIndex = 2;
        // 
        // cmbMethod
        // 
        cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbMethod.Items.Add("Cash");
        cmbMethod.Location = new Point(30, 249);
        cmbMethod.Name = "cmbMethod";
        cmbMethod.Size = new Size(390, 28);
        cmbMethod.TabIndex = 6;
        // 
        // cmbPlan
        // 
        cmbPlan.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPlan.Items.AddRange(new object[] { "Monthly", "2-Month Plan" });
        cmbPlan.Location = new Point(30, 362);
        cmbPlan.Name = "cmbPlan";
        cmbPlan.Size = new Size(390, 28);
        cmbPlan.TabIndex = 9;
        cmbPlan.SelectedIndexChanged += cmbPlan_SelectedIndexChanged;
        // 
        // numAmount
        // 
        numAmount.DecimalPlaces = 2;
        numAmount.Location = new Point(30, 179);
        numAmount.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numAmount.Name = "numAmount";
        numAmount.Size = new Size(390, 27);
        numAmount.TabIndex = 4;
        // 
        // chkRenew
        // 
        chkRenew.Location = new Point(30, 300);
        chkRenew.Name = "chkRenew";
        chkRenew.Size = new Size(390, 28);
        chkRenew.TabIndex = 7;
        chkRenew.Text = "Renew / update membership with this payment";
        chkRenew.CheckedChanged += chkRenew_CheckedChanged;
        // 
        // dtExpiry
        // 
        dtExpiry.Format = DateTimePickerFormat.Short;
        dtExpiry.Location = new Point(30, 432);
        dtExpiry.Name = "dtExpiry";
        dtExpiry.Size = new Size(390, 27);
        dtExpiry.TabIndex = 11;
        dtExpiry.Value = new DateTime(2026, 11, 12, 0, 0, 0, 0);
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.FromArgb(255, 132, 35);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(180, 495);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(130, 40);
        btnSave.TabIndex = 12;
        btnSave.Text = "Save Payment";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(320, 495);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 40);
        btnCancel.TabIndex = 13;
        btnCancel.Text = "Cancel";
        // 
        // PaymentDialog
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(495, 565);
        Controls.Add(lblTitle);
        Controls.Add(lblMember);
        Controls.Add(cmbMember);
        Controls.Add(lblAmount);
        Controls.Add(numAmount);
        Controls.Add(lblMethod);
        Controls.Add(cmbMethod);
        Controls.Add(chkRenew);
        Controls.Add(lblPlan);
        Controls.Add(cmbPlan);
        Controls.Add(lblExpiry);
        Controls.Add(dtExpiry);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PaymentDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Payment Transaction";
        ((System.ComponentModel.ISupportInitialize)numAmount).EndInit();
        ResumeLayout(false);
    }
    private static void LabelAt(Label l, string text, int x, int y) { l.Text = text; l.Font = new Font("Segoe UI", 9, FontStyle.Bold); l.Location = new Point(x, y); l.Size = new Size(260, 22); l.ForeColor = Color.FromArgb(50, 57, 72); }
}
