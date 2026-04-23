using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface IDependentService
{
    Task<List<Dependent>> GetByTaxpayerIdAsync(Guid taxpayerId, CancellationToken cancellationToken);

    Task<Dependent> AddAsync(Guid taxpayerId, CreateDependentRequest request, CancellationToken cancellationToken);
}

public record CreateDependentRequest(string Name, string Relationship, DateTime DateOfBirth);

