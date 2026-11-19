#nullable enable

namespace GymRegistrationApp.Forms;

partial class MemberDialog
{
    private System.ComponentModel.IContainer? components = null;
    private Label lblTitle = null!, lblName = null!, lblPhone = null!, lblEmail = null!, lblPlan = null!, lblStart = null!, lblFee = null!;
    private Label lblFirst = null!, lblMiddle = null!, lblLast = null!, lblMethod = null!, lblAmountPaid = null!, lblChange = null!;
    private TextBox txtFirstName = null!, txtMiddleName = null!, txtLastName = null!, txtPhone = null!, txtEmail = null!;
    private ComboBox cmbPlan = null!, cmbMethod = null!; private DateTimePicker dtStart = null!; private NumericUpDown numFee = null!, numAmountPaid = null!; private Button btnSave = null!, btnCancel = null!, btnPrintReceipt = null!;
    protected override void Dispose(bool disposing) { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblName = new Label();
        lblFirst = new Label();
        lblMiddle = new Label();
        lblLast = new Label();
        lblPhone = new Label();
        lblEmail = new Label();
        lblPlan = new Label();
        lblStart = new Label();
        lblFee = new Label();
        lblAmountPaid = new Label();
        lblChange = new Label();
        txtFirstName = new TextBox();
        txtMiddleName = new TextBox();
        txtLastName = new TextBox();
        txtPhone = new TextBox();
        txtEmail = new TextBox();
        cmbPlan = new ComboBox();
        cmbMethod = new ComboBox();
        dtStart = new DateTimePicker();
        numFee = new NumericUpDown();
        numAmountPaid = new NumericUpDown();
        btnSave = new Button();
        btnCancel = new Button();
        btnPrintReceipt = new Button();
        lblMethod = new Label();
        ((System.ComponentModel.ISupportInitialize)numFee).BeginInit();
        SuspendLayout();
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 37, 54);
        lblTitle.Location = new Point(20, 18);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(460, 52);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Register new member";
        // 
        // lblName
        // 
        // header label (kept for compatibility)
        lblName.Location = new Point(30, 48);
        lblName.Name = "lblName";
        lblName.Size = new Size(120, 20);
        lblName.TabIndex = 1;
        lblName.Text = "Name";
        // first/middle/last labels
        lblFirst.Location = new Point(30, 70);
        lblFirst.Name = "lblFirst";
        lblFirst.Size = new Size(80, 20);
        lblFirst.TabIndex = 2;
        lblFirst.Text = "First name";
        lblMiddle.Location = new Point(185, 70);
        lblMiddle.Name = "lblMiddle";
        lblMiddle.Size = new Size(80, 20);
        lblMiddle.TabIndex = 3;
        lblMiddle.Text = "Middle name";
        lblLast.Location = new Point(330, 70);
        lblLast.Name = "lblLast";
        lblLast.Size = new Size(80, 20);
        lblLast.TabIndex = 4;
        lblLast.Text = "Last name";
        // 
        // lblPhone
        // 
        lblPhone.Location = new Point(30, 130);
        lblPhone.Name = "lblPhone";
        lblPhone.Size = new Size(120, 20);
        lblPhone.TabIndex = 3;
        lblPhone.Text = "Phone";
        // 
        // lblEmail
        // 
        lblEmail.Location = new Point(270, 130);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(120, 20);
        lblEmail.TabIndex = 5;
        lblEmail.Text = "Email";
        // 
        // lblPlan
        // 
        lblPlan.Location = new Point(30, 190);
        lblPlan.Name = "lblPlan";
        lblPlan.Size = new Size(120, 20);
        lblPlan.TabIndex = 7;
        lblPlan.Text = "Plan";
        // 
        // lblStart
        // 
        lblStart.Location = new Point(270, 190);
        lblStart.Name = "lblStart";
        lblStart.Size = new Size(120, 20);
        lblStart.TabIndex = 9;
        lblStart.Text = "Start date";
        // 
        // lblFee
        // 
        lblFee.Location = new Point(30, 250);
        lblFee.Name = "lblFee";
        lblFee.Size = new Size(120, 20);
        lblFee.TabIndex = 11;
        lblFee.Text = "Fee";
        // 
        // txtFirstName
        // 
        txtFirstName.Location = new Point(30, 92);
        txtFirstName.Name = "txtFirstName";
        txtFirstName.Size = new Size(140, 27);
        txtFirstName.TabIndex = 2;
        // txtMiddleName
        // 
        txtMiddleName.Location = new Point(185, 92);
        txtMiddleName.Name = "txtMiddleName";
        txtMiddleName.Size = new Size(140, 27);
        txtMiddleName.TabIndex = 3;
        // txtLastName
        // 
        txtLastName.Location = new Point(340, 92);
        txtLastName.Name = "txtLastName";
        txtLastName.Size = new Size(110, 27);
        txtLastName.TabIndex = 4;
        // 
        // txtPhone
        // 
        txtPhone.Location = new Point(30, 152);
        txtPhone.Name = "txtPhone";
        txtPhone.Size = new Size(220, 27);
        txtPhone.MaxLength = 11;
        txtPhone.TabIndex = 5;
        // 
        // txtEmail
        // 
        txtEmail.Location = new Point(270, 152);
        txtEmail.Name = "txtEmail";
        txtEmail.Size = new Size(180, 27);
        txtEmail.TabIndex = 6;
        // 
        // cmbPlan
        // 
        cmbPlan.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPlan.Location = new Point(30, 212);
        cmbPlan.Name = "cmbPlan";
        cmbPlan.Size = new Size(220, 28);
        cmbPlan.TabIndex = 8;
        cmbPlan.SelectedIndexChanged += cmbPlan_SelectedIndexChanged;
        // 
        // cmbMethod
        // 
        cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbMethod.Items.Add("Cash");
        cmbMethod.Enabled = false;
        cmbMethod.Location = new Point(270, 278);
        cmbMethod.Name = "cmbMethod";
        cmbMethod.Size = new Size(180, 28);
        cmbMethod.TabIndex = 14;
        //
        // lblAmountPaid
        //
        lblAmountPaid.Location = new Point(30, 310);
        lblAmountPaid.Name = "lblAmountPaid";
        lblAmountPaid.Size = new Size(120, 20);
        lblAmountPaid.TabIndex = 15;
        lblAmountPaid.Text = "Amount paid";
        //
        // numAmountPaid
        //
        numAmountPaid.DecimalPlaces = 2;
        numAmountPaid.Location = new Point(30, 332);
        numAmountPaid.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numAmountPaid.Name = "numAmountPaid";
        numAmountPaid.Size = new Size(220, 27);
        numAmountPaid.TabIndex = 16;
        numAmountPaid.ValueChanged += numAmountPaid_ValueChanged;
        //
        // lblChange
        //
        lblChange.Location = new Point(270, 310);
        lblChange.Name = "lblChange";
        lblChange.Size = new Size(180, 48);
        lblChange.TabIndex = 17;
        lblChange.Text = "Change\r\n₱0.00";
        // 
        // dtStart
        // 
        dtStart.Format = DateTimePickerFormat.Short;
        dtStart.Location = new Point(270, 212);
        dtStart.Name = "dtStart";
        dtStart.Size = new Size(180, 27);
        dtStart.TabIndex = 10;
        // 
        // numFee
        // 
        numFee.DecimalPlaces = 2;
        numFee.Location = new Point(30, 272);
        numFee.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numFee.Name = "numFee";
        numFee.Enabled = false;
        numFee.Size = new Size(220, 27);
        numFee.TabIndex = 12;
        numFee.Value = new decimal(new int[] { 600, 0, 0, 0 });
        // 
        // btnSave
        // 
        btnSave.BackColor = Color.FromArgb(255, 132, 35);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(90, 390);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(120, 36);
        btnSave.TabIndex = 18;
        btnSave.Text = "Save Member";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(230, 390);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 19;
        btnCancel.Text = "Cancel";
        //
        // btnPrintReceipt
        //
        btnPrintReceipt.Location = new Point(340, 390);
        btnPrintReceipt.Name = "btnPrintReceipt";
        btnPrintReceipt.Size = new Size(120, 36);
        btnPrintReceipt.TabIndex = 20;
        btnPrintReceipt.Text = "Print Receipt";
        btnPrintReceipt.UseVisualStyleBackColor = true;
        btnPrintReceipt.Visible = false;
        btnPrintReceipt.Click += btnPrintReceipt_Click;
        // 
        // lblMethod
        // 
        lblMethod.Location = new Point(270, 250);
        lblMethod.Name = "lblMethod";
        lblMethod.Size = new Size(140, 20);
        lblMethod.TabIndex = 13;
        lblMethod.Text = "Payment method";
        // 
        // MemberDialog
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(500, 450);
        Controls.Add(lblTitle);
        Controls.Add(lblName);
        Controls.Add(lblFirst);
        Controls.Add(txtFirstName);
        Controls.Add(lblMiddle);
        Controls.Add(txtMiddleName);
        Controls.Add(lblLast);
        Controls.Add(txtLastName);
        Controls.Add(lblPhone);
        Controls.Add(txtPhone);
        Controls.Add(lblEmail);
        Controls.Add(txtEmail);
        Controls.Add(lblPlan);
        Controls.Add(cmbPlan);
        Controls.Add(lblStart);
        Controls.Add(dtStart);
        Controls.Add(lblMethod);
        Controls.Add(cmbMethod);
        Controls.Add(lblFee);
        Controls.Add(numFee);
        Controls.Add(lblAmountPaid);
        Controls.Add(numAmountPaid);
        Controls.Add(lblChange);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        Controls.Add(btnPrintReceipt);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MemberDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Member Registration";
        Load += MemberDialog_Load;
        ((System.ComponentModel.ISupportInitialize)numFee).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private static void ConfigureLabel(Label l, string text, int x, int y) { l.Text = text; l.Font = new Font("Segoe UI", 9, FontStyle.Bold); l.Location = new Point(x, y); l.Size = new Size(250, 22); l.ForeColor = Color.FromArgb(50, 57, 72); }
    private static void ConfigureText(TextBox t, int x, int y) { t.Location = new Point(x, y); t.Size = new Size(390, 30); t.Font = new Font("Segoe UI", 10); t.BorderStyle = BorderStyle.FixedSingle; }


}
