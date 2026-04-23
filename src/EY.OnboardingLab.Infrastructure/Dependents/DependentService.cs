using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EY.OnboardingLab.Infrastructure.Dependents;

public class DependentService : IDependentService
{
    private readonly AppDbContext _db;

    public DependentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Dependent>> GetByTaxpayerIdAsync(Guid taxpayerId, CancellationToken cancellationToken)
    {
        var dependents = await _db.Dependents
            .Where(d => d.TaxpayerId == taxpayerId)
            .OrderByDescending(d => d.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return dependents;
    }

    public async Task<Dependent> AddAsync(Guid taxpayerId, CreateDependentRequest request, CancellationToken cancellationToken)
    {
        var dependent = new Dependent
        {
            Id = Guid.NewGuid(),
            TaxpayerId = taxpayerId,
            Name = request.Name.Trim(),
            Relationship = request.Relationship.Trim(),
            DateOfBirth = request.DateOfBirth,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Dependents.Add(dependent);
        await _db.SaveChangesAsync(cancellationToken);

        return dependent;
    }
}

