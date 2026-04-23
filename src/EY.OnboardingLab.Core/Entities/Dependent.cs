namespace EY.OnboardingLab.Core.Entities;

public class Dependent
{
    public Guid Id { get; set; }

    public Guid TaxpayerId { get; set; }

    public string Name { get; set; } = "";

    public string Relationship { get; set; } = "";

    public DateTime DateOfBirth { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }
}

