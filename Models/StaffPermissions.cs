namespace GymRegistrationApp.Models;

public sealed class StaffPermissions
{
    public bool CanViewMembers { get; set; } = true;
    public bool CanAddMembers { get; set; } = true;
    public bool CanEditMembers { get; set; } = true;
    public bool CanDeleteMembers { get; set; } = true;
    public bool CanManagePayments { get; set; } = true;
    public bool CanManageDailyGym { get; set; } = true;
    public bool CanViewOverviewMemberships { get; set; } = true;
    public bool CanViewOverviewDailyGym { get; set; } = true;
    public bool CanViewOverviewPayments { get; set; } = true;
    public bool CanViewOverviewRevenue { get; set; }
    public bool CanViewReports { get; set; }
    public bool CanManageExpenses { get; set; }
    public bool CanManageStaff { get; set; }
    public bool CanViewDeletedRecords { get; set; }

    public static StaffPermissions CreateDefault() => new();
}
