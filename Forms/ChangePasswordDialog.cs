using GymRegistrationApp.Services;

namespace GymRegistrationApp.Forms;

public sealed class ChangePasswordDialog : Form
{
    private readonly string _temporaryPassword;
    private readonly TextBox _newPassword = PasswordBox();
    private readonly TextBox _repeatPassword = PasswordBox();
    private readonly Label _error = new() { ForeColor = Color.Firebrick, AutoSize = false, Size = new Size(420, 42) };

    public ChangePasswordDialog(string temporaryPassword)
    {
        _temporaryPassword = temporaryPassword;
        Text = "Change temporary password";
        ClientSize = new Size(500, 300);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;

        Controls.Add(new Label
        {
            Text = "For your security, create a new password before continuing.",
            Location = new Point(35, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        });
        AddField("New Password", _newPassword, 75);
        AddField("Repeat Password", _repeatPassword, 130);
        _error.Location = new Point(35, 180);
        Controls.Add(_error);

        var confirm = new Button
        {
            Text = "Confirm Password",
            Location = new Point(305, 235),
            Size = new Size(160, 36),
            BackColor = Color.FromArgb(255, 132, 35),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        confirm.Click += Confirm_Click;
        Controls.Add(confirm);
        AcceptButton = confirm;
        FormClosing += (_, e) =>
        {
            if (DialogResult != DialogResult.OK)
            {
                DialogResult = DialogResult.Cancel;
            }
        };
    }

    public string NewPassword => _newPassword.Text;

    private void Confirm_Click(object? sender, EventArgs e)
    {
        var password = _newPassword.Text;
        if (!PasswordHasher.IsValidPassword(password, out var error))
        {
            ShowError(error);
            return;
        }

        if (!string.Equals(password, _repeatPassword.Text, StringComparison.Ordinal))
        {
            ShowError("New Password and Repeat Password must match.");
            return;
        }

        if (string.Equals(password, _temporaryPassword, StringComparison.Ordinal))
        {
            ShowError("The new password must be different from the temporary password.");
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void ShowError(string message)
    {
        _error.Text = message;
        _newPassword.Focus();
    }

    private void AddField(string label, TextBox field, int top)
    {
        Controls.Add(new Label { Text = label, Location = new Point(35, top), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
        field.Location = new Point(180, top - 4);
        field.Size = new Size(285, 30);
        Controls.Add(field);
    }

    private static TextBox PasswordBox() => new() { UseSystemPasswordChar = true };
}
