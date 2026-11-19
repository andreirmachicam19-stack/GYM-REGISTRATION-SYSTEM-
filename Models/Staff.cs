namespace GymRegistrationApp.Models;

public sealed class Staff
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string MiddleName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime Birthday { get; set; }
    public string Position { get; set; } = "";
    public string HomeAddress { get; set; } = "";
    public string Username { get; set; } = "";
    // This field is only retained for compatibility with existing staff.json files.
    public string Password { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool RequiresPasswordChange { get; set; }
    public DateTime JoinedDate { get; set; }
    public string Status { get; set; } = "Active";
    public StaffPermissions Permissions { get; set; } = StaffPermissions.CreateDefault();
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";
}
