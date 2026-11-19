namespace GymRegistrationApp.Models;

public sealed class AttendanceRecord
{
    public string Id { get; set; } = "";
    public string MemberId { get; set; } = "";
    public string MemberName { get; set; } = "";
    public DateTime AttendanceDate { get; set; } = DateTime.Today;
    public DateTime CheckInTime { get; set; } = DateTime.Now;
    public string RecordedBy { get; set; } = "";
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = "";
}
