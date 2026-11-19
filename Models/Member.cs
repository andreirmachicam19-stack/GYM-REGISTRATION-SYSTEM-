namespace GymRegistrationApp.Models;

public sealed class Member
{
    public const decimal MonthlyFee = 600m;
    public const decimal TwoMonthFee = 1200m;
    public string Id { get; set; } = "";
    // Name parts
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string LastName { get; set; } = "";
    // FullName kept for display/compatibility
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Plan { get; set; } = "Monthly";
    public decimal Fee { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal RemainingBalance { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime ExpiryDate { get; set; } = DateTime.Today.AddMonths(1);
    public string Status { get; set; } = "Active";
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";
}
