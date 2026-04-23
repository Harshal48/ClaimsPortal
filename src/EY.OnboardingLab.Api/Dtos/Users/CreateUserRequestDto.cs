namespace EY.OnboardingLab.Api.Dtos.Users;

public record CreateUserRequestDto(string UserName, string Email, string Password, string Role, bool IsActive);
