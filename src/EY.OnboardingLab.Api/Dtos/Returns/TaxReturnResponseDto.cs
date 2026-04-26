namespace EY.OnboardingLab.Api.Dtos.Returns;

public record TaxReturnResponseDto(
    Guid Id,
    Guid TaxpayerId,
    int TaxYear,
    string ReturnStatus,
    string FilingStatus,
    Guid? ReviewerId,
    decimal TotalIncome,
    decimal TotalWithheld,
    decimal TotalDeduction,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

