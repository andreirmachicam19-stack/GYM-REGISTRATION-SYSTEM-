namespace GymRegistrationApp.Models;

public sealed class GymPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Months { get; set; } = 1;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}
