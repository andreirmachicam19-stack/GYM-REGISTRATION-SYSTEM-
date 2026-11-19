namespace GymRegistrationApp.Models;

public sealed class Expense
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.Today;
    public string PaymentMethod { get; set; } = "Cash";
    public string Description { get; set; } = "";
    public string RecordedBy { get; set; } = "";
    public string Status { get; set; } = "Active";
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";
}
