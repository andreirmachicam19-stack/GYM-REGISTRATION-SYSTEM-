namespace GymRegistrationApp.Models;

public sealed class AppUser
{
    public string Username { get; set; } = "";
    // Kept for reading existing users.json files; new and migrated credentials use PasswordHash.
    public string Password { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool RequiresPasswordChange { get; set; }
    public string Role { get; set; } = "Staff";
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";
}
