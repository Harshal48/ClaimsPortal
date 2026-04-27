namespace EY.OnboardingLab.Core.Entities;

public class Activity
{
    public Guid Id { get; set; }

    public string Message { get; set; } = "";

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }

    public string Type { get; set; } = "";
}

