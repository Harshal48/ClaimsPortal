using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EY.OnboardingLab.Infrastructure.TaxpayerPii;

public class TaxpayerPiiService : ITaxpayerPiiService
{
    private readonly AppDbContext _db;

    public TaxpayerPiiService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TaxpayerPii?> GetByTaxpayerIdAsync(Guid taxpayerId, CancellationToken cancellationToken)
    {
        var pii = await _db.TaxpayerPiis.FirstOrDefaultAsync(p => p.TaxpayerId == taxpayerId, cancellationToken);
        return pii;
    }

    public async Task<TaxpayerPii> UpsertAsync(Guid taxpayerId, UpsertTaxpayerPiiRequest request, CancellationToken cancellationToken)
    {
        var pii = await _db.TaxpayerPiis.FirstOrDefaultAsync(p => p.TaxpayerId == taxpayerId, cancellationToken);

        if (pii is null)
        {
            pii = new TaxpayerPii
            {
                TaxpayerId = taxpayerId,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.TaxpayerPiis.Add(pii);
        }

        // For now we store SSN as-is in SsnEncrypted field.
        // Later we will encrypt it and keep only encrypted data in DB.
        pii.SsnEncrypted = request.Ssn.Trim();
        pii.DateOfBirth = request.DateOfBirth;
        pii.AddressStreet = request.AddressStreet?.Trim();
        pii.AddressCity = request.AddressCity?.Trim();
        pii.AddressState = request.AddressState?.Trim();
        pii.AddressZip = request.AddressZip?.Trim();
        pii.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return pii;
    }
}

