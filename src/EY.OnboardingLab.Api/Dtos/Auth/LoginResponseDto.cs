namespace EY.OnboardingLab.Api.Dtos.Auth;

public record LoginResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds,
    Guid UserId,
    string UserName,
    string Email,
    string Role);
