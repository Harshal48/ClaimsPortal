namespace EY.OnboardingLab.Api.Dtos.TaxpayerPii;

public record UpsertTaxpayerPiiRequestDto(
    string Ssn,
    DateTime DateOfBirth,
    string? AddressStreet,
    string? AddressCity,
    string? AddressState,
    string? AddressZip);

