namespace EY.OnboardingLab.Api.Dtos.Returns;

public record CreateTaxReturnRequestDto(Guid TaxpayerId, int TaxYear, string FilingStatus);

