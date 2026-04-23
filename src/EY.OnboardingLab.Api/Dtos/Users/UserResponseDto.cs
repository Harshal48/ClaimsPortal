namespace EY.OnboardingLab.Api.Dtos.Users;

public record UserResponseDto(
    Guid Id,
    string UserName,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAtUtc);
