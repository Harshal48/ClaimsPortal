using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EY.OnboardingLab.Infrastructure.Taxpayers;

public class TaxpayerService : ITaxpayerService
{
    private readonly AppDbContext _db;

    public TaxpayerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Taxpayer> CreateAsync(CreateTaxpayerRequest request, CancellationToken cancellationToken)
    {
        var taxpayer = new Taxpayer
        {
            Id = Guid.NewGuid(),
            TaxpayerNumber = request.TaxpayerNumber,
            LegalName = request.LegalName,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Taxpayers.Add(taxpayer);
        await _db.SaveChangesAsync(cancellationToken);

        return taxpayer;
    }

    public async Task<Taxpayer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var taxpayer = await _db.Taxpayers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return taxpayer;
    }

    public async Task<List<Taxpayer>> GetAllAsync(CancellationToken cancellationToken)
    {
        var taxpayers = await _db.Taxpayers
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return taxpayers;
    }

    public async Task<Taxpayer?> UpdateAsync(Guid id, UpdateTaxpayerRequest request, CancellationToken cancellationToken)
    {
        var taxpayer = await _db.Taxpayers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (taxpayer is null)
            return null;

        taxpayer.TaxpayerNumber = request.TaxpayerNumber;
        taxpayer.LegalName = request.LegalName;
        taxpayer.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return taxpayer;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var taxpayer = await _db.Taxpayers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (taxpayer is null)
            return false;

        _db.Taxpayers.Remove(taxpayer);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
