using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EY.OnboardingLab.Infrastructure.Returns;

public class TaxReturnService : ITaxReturnService
{
    private readonly AppDbContext _db;

    public TaxReturnService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaxReturn>> GetAllAsync(CancellationToken cancellationToken)
    {
        var returns = await _db.TaxReturns
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return returns;
    }

    public async Task<TaxReturn?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var taxReturn = await _db.TaxReturns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return taxReturn;
    }

    public async Task<TaxReturn> CreateAsync(CreateTaxReturnRequest request, CancellationToken cancellationToken)
    {
        var taxReturn = new TaxReturn
        {
            Id = Guid.NewGuid(),
            TaxpayerId = request.TaxpayerId,
            TaxYear = request.TaxYear,
            FilingStatus = request.FilingStatus.Trim(),
            ReturnStatus = "Draft",
            TotalIncome = 0,
            TotalWithheld = 0,
            TotalDeduction = 0,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.TaxReturns.Add(taxReturn);
        await _db.SaveChangesAsync(cancellationToken);

        return taxReturn;
    }

    public async Task<TaxReturn?> UpdateAsync(Guid id, UpdateTaxReturnRequest request, CancellationToken cancellationToken)
    {
        var taxReturn = await _db.TaxReturns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (taxReturn is null)
            return null;

        if (!string.Equals(taxReturn.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return null;

        taxReturn.TaxYear = request.TaxYear;
        taxReturn.FilingStatus = request.FilingStatus.Trim();
        taxReturn.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return taxReturn;
    }

    public async Task<TaxReturn?> AddIncomeAsync(Guid id, AddIncomeRequest request, CancellationToken cancellationToken)
    {
        var taxReturn = await _db.TaxReturns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (taxReturn is null)
            return null;

        if (!string.Equals(taxReturn.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return null;

        taxReturn.TotalIncome += request.Amount;
        taxReturn.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return taxReturn;
    }

    public async Task<TaxReturn?> AddDeductionAsync(Guid id, AddDeductionRequest request, CancellationToken cancellationToken)
    {
        var taxReturn = await _db.TaxReturns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (taxReturn is null)
            return null;

        if (!string.Equals(taxReturn.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return null;

        var deduction = new Deduction
        {
            Id = Guid.NewGuid(),
            TaxReturnId = taxReturn.Id,
            DeductionName = request.DeductionName.Trim(),
            DeductionType = request.DeductionType.Trim(),
            Amount = request.Amount,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Deductions.Add(deduction);

        taxReturn.TotalDeduction += request.Amount;
        taxReturn.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return taxReturn;
    }

    public async Task<TaxReturn?> SubmitAsync(Guid id, CancellationToken cancellationToken)
    {
        var taxReturn = await _db.TaxReturns.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (taxReturn is null)
            return null;

        if (!string.Equals(taxReturn.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return null;

        taxReturn.ReturnStatus = "Submitted";
        taxReturn.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return taxReturn;
    }
}

