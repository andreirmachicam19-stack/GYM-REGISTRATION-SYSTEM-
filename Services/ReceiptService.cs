using System.Net;
using GymRegistrationApp.Forms;
using GymRegistrationApp.Models;

namespace GymRegistrationApp.Services;

public static class ReceiptService
{
    public static string SaveReceipt(Member member, Payment payment)
    {
        var directory = Path.Combine(DataStore.DataFolder, "Receipts");
        Directory.CreateDirectory(directory);
        var fileName = $"Receipt-{SanitizeFileName(payment.Reference)}.html";
        var path = Path.Combine(directory, fileName);
        File.WriteAllText(path, BuildHtml(member, payment));
        return path;
    }

    public static void OpenReceipt(Member member, Payment payment)
    {
        var path = SaveReceipt(member, payment);
        using var preview = new ReceiptPreviewForm(path, allowPrinting: false);
        preview.ShowDialog();
    }

    public static void PrintReceipt(Member member, Payment payment)
    {
        var path = SaveReceipt(member, payment);
        // Shell "print" depends on an external .html file association.
        using var preview = new ReceiptPreviewForm(path);
        preview.ShowDialog();
    }

    private static string BuildHtml(Member member, Payment payment)
    {
        var paid = payment.AmountPaid > 0m ? payment.AmountPaid : payment.Amount;
        var change = payment.AmountPaid > 0m ? payment.Change : 0m;
        var date = payment.Date.ToString("MMM d, yyyy h:mm tt");
        return $"<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\"><title>Receipt {E(payment.Reference)}</title>"
            + "<style>*{box-sizing:border-box}body{font-family:Segoe UI,Arial,sans-serif;background:#eef2f7;padding:28px;color:#172033}.receipt{background:#fff;width:620px;margin:auto;padding:34px;border:1px solid #dbe3ee;border-radius:14px;box-shadow:0 8px 28px #d5dce5}.brand{width:100%;border-bottom:3px solid #ff8423;padding-bottom:20px;margin-bottom:22px}.brand td{vertical-align:top}.brand h1{font-size:30px;letter-spacing:.5px;margin:0;color:#e96812}.tag{text-align:right;font-size:11px;letter-spacing:1.5px;text-transform:uppercase;color:#64748b;font-weight:700;white-space:nowrap}.subtitle{margin:6px 0 0;color:#64748b}.section-title{font-size:16px;margin:0 0 10px}.details{width:100%;border-collapse:collapse}.details td{padding:10px 0;border-bottom:1px solid #edf0f4;font-size:14px}.details td:first-child{color:#526176;width:52%}.details td:last-child{text-align:right;font-weight:600}.details .total td{font-size:19px;font-weight:700;color:#172033;border-top:2px solid #172033;border-bottom:0;padding-top:17px}.muted{color:#64748b;font-size:12px;line-height:1.6}.footer{border-top:1px solid #edf0f4;margin-top:22px;padding-top:16px}@media print{body{background:#fff;padding:0}.receipt{width:100%;border:0;border-radius:0;box-shadow:none;padding:0}@page{margin:16mm}}</style></head><body><main class=\"receipt\"><table class=\"brand\"><tr><td><h1>UniversalGym</h1><p class=\"subtitle\">Official payment receipt</p></td><td class=\"tag\">Membership payment</td></tr></table>"
            + $"<h2 class=\"section-title\">Payment details</h2><table class=\"details\"><tr><td>Receipt reference</td><td>{E(payment.Reference)}</td></tr><tr><td>Date</td><td>{E(date)}</td></tr>"
            + $"<tr><td>Member</td><td>{E(member.FullName)}</td></tr><tr><td>Member ID</td><td>{E(member.Id)}</td></tr>"
            + $"<tr><td>Plan</td><td>{E(member.Plan)}</td></tr><tr><td>Membership fee</td><td>PHP {member.Fee:N2}</td></tr>"
            + $"<tr><td>Amount paid</td><td>PHP {paid:N2}</td></tr><tr><td>Change</td><td>PHP {change:N2}</td></tr>"
            + $"<tr class=\"total\"><td>Total amount</td><td>PHP {payment.Amount:N2}</td></tr></table><div class=\"footer\"><p class=\"muted\">Payment method: {E(payment.Method)}<br>Collected by: {E(payment.CollectedBy)}</p><p>Thank you for your payment.</p></div></main></body></html>";
    }

    private static string E(string value) => WebUtility.HtmlEncode(value);
    private static string SanitizeFileName(string value) => string.Concat(value.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_'));
}
