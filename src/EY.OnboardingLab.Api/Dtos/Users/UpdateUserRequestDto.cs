namespace EY.OnboardingLab.Api.Dtos.Users;

public record UpdateUserRequestDto(string UserName, string Email, string Role, bool IsActive);
