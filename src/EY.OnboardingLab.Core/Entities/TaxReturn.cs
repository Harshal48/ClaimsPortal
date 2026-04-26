namespace EY.OnboardingLab.Core.Entities;

public class TaxReturn
{
    public Guid Id { get; set; }

    public Guid TaxpayerId { get; set; }

    public int TaxYear { get; set; }

    public string ReturnStatus { get; set; } = "Draft";

    public string FilingStatus { get; set; } = "";

    public Guid? ReviewerId { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal TotalWithheld { get; set; }

    public decimal TotalDeduction { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

