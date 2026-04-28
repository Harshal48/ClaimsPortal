namespace EY.OnboardingLab.Core.Entities;

public class TaxpayerPii
{
    public Guid TaxpayerId { get; set; }

    // Store encrypted value later. For now keep as plain string to finish functionality.
    public string SsnEncrypted { get; set; } = "";

    public DateTime DateOfBirth { get; set; }

    public string? AddressStreet { get; set; }

    public string? AddressCity { get; set; }

    public string? AddressState { get; set; }

    public string? AddressZip { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

