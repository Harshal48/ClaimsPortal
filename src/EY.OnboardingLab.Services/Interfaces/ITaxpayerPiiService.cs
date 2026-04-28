using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface ITaxpayerPiiService
{
    Task<TaxpayerPii?> GetByTaxpayerIdAsync(Guid taxpayerId, CancellationToken cancellationToken);

    Task<TaxpayerPii> UpsertAsync(Guid taxpayerId, UpsertTaxpayerPiiRequest request, CancellationToken cancellationToken);
}

public record UpsertTaxpayerPiiRequest(
    string Ssn,
    DateTime DateOfBirth,
    string? AddressStreet,
    string? AddressCity,
    string? AddressState,
    string? AddressZip);

