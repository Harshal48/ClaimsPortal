namespace EY.OnboardingLab.Core.Entities;

public class TaxReview
{
    public Guid Id { get; set; }

    public Guid TaxReturnId { get; set; }

    public string OldStatus { get; set; } = "";

    public string NewStatus { get; set; } = "";

    public Guid ReviewerId { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

