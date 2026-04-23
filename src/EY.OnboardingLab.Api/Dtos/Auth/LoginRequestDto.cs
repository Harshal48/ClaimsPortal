namespace EY.OnboardingLab.Api.Dtos.Auth;

public record LoginRequestDto(string UserNameOrEmail, string Password);
