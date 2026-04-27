namespace EY.OnboardingLab.Core.Entities;

public class Report
{
    public Guid Id { get; set; }

    public string ReportType { get; set; } = "";

    public string Status { get; set; } = "Generated";

    public string FileName { get; set; } = "";

    public string ContentType { get; set; } = "application/json";

    public string Content { get; set; } = "";

    public Guid GeneratedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

