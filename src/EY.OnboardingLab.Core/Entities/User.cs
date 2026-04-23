namespace EY.OnboardingLab.Core.Entities;

public class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Password { get; set; } = "";

    // We keep Role as a simple string for now to stay beginner-friendly.
    // Later we can switch to an enum + authorization policies (Admin/Preparer/Reviewer).
    public string Role { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
