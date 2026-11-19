namespace GymRegistrationApp.Forms;

public sealed class ReceiptPreviewForm : Form
{
    private readonly WebBrowser _browser = new() { Dock = DockStyle.Fill };
    private readonly Button _printButton = new() { Text = "Print receipt", AutoSize = true, Enabled = false };
    private readonly Label _status = new() { AutoSize = true, Text = "Loading receipt...", Padding = new Padding(8, 8, 0, 0) };

    public ReceiptPreviewForm(string receiptPath, bool allowPrinting = true)
    {
        Text = allowPrinting ? "Print receipt" : "Receipt preview";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(720, 640);
        Size = new Size(820, 760);

        var toolbar = new Panel { Dock = DockStyle.Bottom, Height = 48, Padding = new Padding(8) };
        _printButton.Visible = allowPrinting;
        _printButton.Click += (_, _) => PrintReceipt();
        var closeButton = new Button { Text = "Close", AutoSize = true, DialogResult = DialogResult.Cancel };
        closeButton.Left = _printButton.Right + 12;
        _status.Left = closeButton.Right + 16;
        toolbar.Controls.Add(_printButton);
        toolbar.Controls.Add(closeButton);
        toolbar.Controls.Add(_status);
        CancelButton = closeButton;
        Controls.Add(_browser);
        Controls.Add(toolbar);

        _browser.DocumentCompleted += (_, _) =>
        {
            _printButton.Enabled = allowPrinting;
            _status.Text = allowPrinting ? "Ready to print" : "Receipt preview";
        };
        _browser.Navigate(new Uri(receiptPath).AbsoluteUri);
    }

    private void PrintReceipt()
    {
        if (_browser.Document is null) return;
        try
        {
            // Uses the WinForms/Windows print pipeline and needs no .html association.
            _browser.Print();
            _status.Text = "Print command sent";
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(this, $"Unable to print the receipt: {ex.Message}", "Print receipt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
