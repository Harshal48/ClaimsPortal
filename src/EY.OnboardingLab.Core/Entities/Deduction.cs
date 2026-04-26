namespace EY.OnboardingLab.Core.Entities;

public class Deduction
{
    public Guid Id { get; set; }

    public Guid TaxReturnId { get; set; }

    public string DeductionName { get; set; } = "";

    public string DeductionType { get; set; } = "";

    public decimal Amount { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

