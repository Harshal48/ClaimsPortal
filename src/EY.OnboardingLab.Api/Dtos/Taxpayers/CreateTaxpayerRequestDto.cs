namespace EY.OnboardingLab.Api.Dtos.Taxpayers;

public record CreateTaxpayerRequestDto(string TaxpayerNumber, string LegalName, Guid? CreatedByUserId);
