namespace EY.OnboardingLab.Core.Entities;

public class Taxpayer
{
    public Guid Id { get; set; }

    public string TaxpayerNumber { get; set; } = "";

    public string LegalName { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public Guid? CreatedByUserId { get; set; }
}
