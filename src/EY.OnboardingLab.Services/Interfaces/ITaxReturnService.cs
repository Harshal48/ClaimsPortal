using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface ITaxReturnService
{
    Task<List<TaxReturn>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaxReturn?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TaxReturn> CreateAsync(CreateTaxReturnRequest request, CancellationToken cancellationToken);
    Task<TaxReturn?> UpdateAsync(Guid id, UpdateTaxReturnRequest request, CancellationToken cancellationToken);

    Task<TaxReturn?> AddIncomeAsync(Guid id, AddIncomeRequest request, CancellationToken cancellationToken);
    Task<TaxReturn?> AddDeductionAsync(Guid id, AddDeductionRequest request, CancellationToken cancellationToken);

    Task<TaxReturn?> SubmitAsync(Guid id, CancellationToken cancellationToken);
}

public record CreateTaxReturnRequest(
    Guid TaxpayerId,
    int TaxYear,
    string FilingStatus);

public record UpdateTaxReturnRequest(
    int TaxYear,
    string FilingStatus);

public record AddIncomeRequest(decimal Amount);

public record AddDeductionRequest(string DeductionName, string DeductionType, decimal Amount);

