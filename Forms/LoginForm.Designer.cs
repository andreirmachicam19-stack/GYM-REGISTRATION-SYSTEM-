#nullable enable

namespace GymRegistrationApp.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;
    private Panel panelBrand = null!; private Label lblBrand = null!; private Label lblTagline = null!; private Panel panelLogin = null!;
    private Label lblTitle = null!; private Label lblSubtitle = null!; private Label lblUser = null!; private Label lblPass = null!;
    private TextBox txtUsername = null!; private TextBox txtPassword = null!; private CheckBox chkShowPassword = null!; private Button btnLogin = null!; private Label lblError = null!;
    protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }
    private void InitializeComponent()
    {
        panelBrand = new Panel();
        lblBrand = new Label();
        lblTagline = new Label();
        panelLogin = new Panel();
        lblTitle = new Label();
        lblSubtitle = new Label();
        lblUser = new Label();
        txtUsername = new TextBox();
        lblPass = new Label();
        txtPassword = new TextBox();
        chkShowPassword = new CheckBox();
        btnLogin = new Button();
        lblError = new Label();
        panelBrand.SuspendLayout();
        panelLogin.SuspendLayout();
        SuspendLayout();
        // 
        // panelBrand
        // 
        panelBrand.BackColor = Color.FromArgb(18, 23, 38);
        panelBrand.Controls.Add(lblBrand);
        panelBrand.Controls.Add(lblTagline);
        panelBrand.Dock = DockStyle.Left;
        panelBrand.Location = new Point(0, 0);
        panelBrand.Name = "panelBrand";
        panelBrand.Size = new Size(430, 700);
        panelBrand.TabIndex = 1;
        // 
        // lblBrand
        // 
        lblBrand.Font = new Font("Segoe UI", 34F, FontStyle.Bold);
        lblBrand.ForeColor = Color.White;
        lblBrand.Location = new Point(25, 137);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(369, 210);
        lblBrand.TabIndex = 0;
        lblBrand.Text = "UNIVERSAL\nGYM";
        lblBrand.Click += lblBrand_Click;
        // 
        // lblTagline
        // 
        lblTagline.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTagline.ForeColor = Color.FromArgb(255, 132, 35);
        lblTagline.Location = new Point(56, 382);
        lblTagline.Name = "lblTagline";
        lblTagline.Size = new Size(300, 67);
        lblTagline.TabIndex = 1;
        lblTagline.Text = "STRONGER BUSINESS.\nSMARTER GYM.";
        // 
        // panelLogin
        // 
        panelLogin.BackColor = Color.FromArgb(246, 247, 250);
        panelLogin.Controls.Add(lblTitle);
        panelLogin.Controls.Add(lblSubtitle);
        panelLogin.Controls.Add(lblUser);
        panelLogin.Controls.Add(txtUsername);
        panelLogin.Controls.Add(lblPass);
        panelLogin.Controls.Add(txtPassword);
        panelLogin.Controls.Add(chkShowPassword);
        panelLogin.Controls.Add(btnLogin);
        panelLogin.Controls.Add(lblError);
        panelLogin.Dock = DockStyle.Fill;
        panelLogin.Location = new Point(430, 0);
        panelLogin.Name = "panelLogin";
        panelLogin.Size = new Size(570, 700);
        panelLogin.TabIndex = 0;
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(28, 35, 52);
        lblTitle.Location = new Point(58, 113);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(483, 54);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Sign in to your account";
        // 
        // lblSubtitle
        // 
        lblSubtitle.Font = new Font("Segoe UI", 10F);
        lblSubtitle.ForeColor = Color.Gray;
        lblSubtitle.Location = new Point(68, 180);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(380, 25);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Welcome back! Enter your details below.";
        // 
        // lblUser
        // 
        lblUser.AutoSize = true;
        lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUser.Location = new Point(70, 240);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(91, 20);
        lblUser.TabIndex = 2;
        lblUser.Text = "USERNAME";
        // 
        // txtUsername
        // 
        txtUsername.BorderStyle = BorderStyle.FixedSingle;
        txtUsername.Font = new Font("Segoe UI", 11F);
        txtUsername.Location = new Point(70, 265);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(365, 32);
        txtUsername.TabIndex = 3;
        txtUsername.Text = "admin";
        // 
        // lblPass
        // 
        lblPass.AutoSize = true;
        lblPass.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPass.Location = new Point(70, 320);
        lblPass.Name = "lblPass";
        lblPass.Size = new Size(91, 20);
        lblPass.TabIndex = 4;
        lblPass.Text = "PASSWORD";
        // 
        // txtPassword
        // 
        txtPassword.BorderStyle = BorderStyle.FixedSingle;
        txtPassword.Font = new Font("Segoe UI", 11F);
        txtPassword.Location = new Point(70, 345);
        txtPassword.Name = "txtPassword";
        txtPassword.Size = new Size(365, 32);
        txtPassword.TabIndex = 5;
        txtPassword.Text = "admin123";
        txtPassword.UseSystemPasswordChar = true;
        // 
        // chkShowPassword
        // 
        chkShowPassword.AutoSize = true;
        chkShowPassword.ForeColor = Color.DimGray;
        chkShowPassword.Location = new Point(70, 389);
        chkShowPassword.Name = "chkShowPassword";
        chkShowPassword.Size = new Size(134, 24);
        chkShowPassword.TabIndex = 6;
        chkShowPassword.Text = "Show password";
        chkShowPassword.CheckedChanged += chkShowPassword_CheckedChanged;
        // 
        // btnLogin
        // 
        btnLogin.BackColor = Color.FromArgb(255, 132, 35);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(70, 430);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(365, 46);
        btnLogin.TabIndex = 7;
        btnLogin.Text = "SIGN IN  →";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += btnLogin_Click;
        // 
        // lblError
        // 
        lblError.ForeColor = Color.Firebrick;
        lblError.Location = new Point(70, 484);
        lblError.Name = "lblError";
        lblError.Size = new Size(365, 25);
        lblError.TabIndex = 8;
        lblError.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // LoginForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(1000, 700);
        Controls.Add(panelLogin);
        Controls.Add(panelBrand);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "UniversalGym | Gym Management";
        panelBrand.ResumeLayout(false);
        panelLogin.ResumeLayout(false);
        panelLogin.PerformLayout();
        ResumeLayout(false);
    }
}
