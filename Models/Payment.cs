namespace GymRegistrationApp.Models;

public sealed class Payment
{
    public string Reference { get; set; } = "";
    public string MemberId { get; set; } = "";
    public string MemberName { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Change { get; set; }
    public string Method { get; set; } = "Cash";
    public string PaymentType { get; set; } = "Membership Payment";
    public string CollectedBy { get; set; } = "";
    public DateTime Date { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Paid";
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";
}
