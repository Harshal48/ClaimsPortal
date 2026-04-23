namespace EY.OnboardingLab.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

public record LoginRequest(string UserNameOrEmail, string Password);

public record LoginResult(
    string AccessToken,
    int ExpiresInSeconds,
    Guid UserId,
    string UserName,
    string Email,
    string Role);
