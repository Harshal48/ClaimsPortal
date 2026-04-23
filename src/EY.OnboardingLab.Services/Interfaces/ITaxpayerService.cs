using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface ITaxpayerService
{
    Task<Taxpayer> CreateAsync(CreateTaxpayerRequest request, CancellationToken cancellationToken);
    Task<Taxpayer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Taxpayer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Taxpayer?> UpdateAsync(Guid id, UpdateTaxpayerRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public record CreateTaxpayerRequest(string TaxpayerNumber, string LegalName, Guid? CreatedByUserId);
public record UpdateTaxpayerRequest(string TaxpayerNumber, string LegalName);
