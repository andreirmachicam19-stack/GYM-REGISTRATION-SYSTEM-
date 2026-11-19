namespace GymRegistrationApp.Models;

public sealed class MembershipRenewal
{
    public string Reference { get; set; } = "";
    public string MemberId { get; set; } = "";
    public string MemberName { get; set; } = "";
    public string PreviousPlan { get; set; } = "";
    public string NewPlan { get; set; } = "";
    public string PreviousStatus { get; set; } = "";
    public DateTime PreviousStartDate { get; set; } = DateTime.Today;
    public DateTime PreviousExpiryDate { get; set; } = DateTime.Today;
    public decimal PreviousFee { get; set; }
    public DateTime NewStartDate { get; set; } = DateTime.Today;
    public DateTime NewExpiryDate { get; set; } = DateTime.Today;
    public decimal NewFee { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OutstandingAmount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string PaymentReference { get; set; } = "";
    public DateTime RenewalDate { get; set; } = DateTime.Now;
    public string ProcessedBy { get; set; } = "";
    public string Status { get; set; } = "Completed";
}
