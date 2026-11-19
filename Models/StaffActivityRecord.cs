namespace GymRegistrationApp.Models;

public sealed class StaffActivityRecord
{
    public string Id { get; set; } = "";
    public string Username { get; set; } = "";
    public string StaffName { get; set; } = "";
    public string AccountType { get; set; } = "";
    public string Action { get; set; } = "";
    public string Details { get; set; } = "";
    public string MemberId { get; set; } = "";
    public DateTime OccurredAt { get; set; }
}
