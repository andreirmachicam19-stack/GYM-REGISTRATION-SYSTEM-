namespace GymRegistrationApp.Models;

public sealed class DailyGymVisit
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string MemberId { get; set; } = "";
    public bool IsGymMember { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.Today;
    public decimal Fee { get; set; } = 50m;
    public string PaymentMethod { get; set; } = "Cash";
    public string PaymentReference { get; set; } = "";
    public string RegisteredBy { get; set; } = "";
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";

    public string FullName => string.Join(" ", new[] { FirstName, MiddleName, LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));
}
