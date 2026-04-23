namespace EY.OnboardingLab.Api.Dtos.Dependents;

public record DependentResponseDto(
    Guid Id,
    Guid TaxpayerId,
    string Name,
    string Relationship,
    DateTime DateOfBirth,
    DateTime CreatedAtUtc);

