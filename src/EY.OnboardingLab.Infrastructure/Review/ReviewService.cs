using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EY.OnboardingLab.Infrastructure.Review;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;

    public ReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaxReturn>> GetPendingAsync(CancellationToken cancellationToken)
    {
        var pending = await _db.TaxReturns
            .Where(r => r.ReturnStatus == "Submitted")
            .OrderBy(r => r.UpdatedAtUtc)
            .ToListAsync(cancellationToken);

        return pending;
    }

    public async Task<List<TaxReview>> GetReviewHistoryAsync(Guid taxReturnId, CancellationToken cancellationToken)
    {
        var history = await _db.TaxReviews
            .Where(r => r.TaxReturnId == taxReturnId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return history;
    }

    public Task<TaxReturn?> ApproveAsync(Guid taxReturnId, Guid reviewerId, ReviewActionRequest request, CancellationToken cancellationToken)
        => ChangeStatusAsync(taxReturnId, reviewerId, "Approved", "ReturnApproved", request, cancellationToken);

    public Task<TaxReturn?> RejectAsync(Guid taxReturnId, Guid reviewerId, ReviewActionRequest request, CancellationToken cancellationToken)
        => ChangeStatusAsync(taxReturnId, reviewerId, "Rejected", "ReturnRejected", request, cancellationToken);

    public Task<TaxReturn?> RequestChangesAsync(Guid taxReturnId, Guid reviewerId, ReviewActionRequest request, CancellationToken cancellationToken)
        => ChangeStatusAsync(taxReturnId, reviewerId, "NeedsChanges", "ReturnChangesRequested", request, cancellationToken);

    private async Task<TaxReturn?> ChangeStatusAsync(
        Guid taxReturnId,
        Guid reviewerId,
        string newStatus,
        string activityType,
        ReviewActionRequest request,
        CancellationToken cancellationToken)
    {
        var taxReturn = await _db.TaxReturns.FirstOrDefaultAsync(r => r.Id == taxReturnId, cancellationToken);
        if (taxReturn is null)
            return null;

        if (!string.Equals(taxReturn.ReturnStatus, "Submitted", StringComparison.OrdinalIgnoreCase))
            return null;

        var oldStatus = taxReturn.ReturnStatus;

        taxReturn.ReturnStatus = newStatus;
        taxReturn.ReviewerId = reviewerId;
        taxReturn.UpdatedAtUtc = DateTime.UtcNow;

        var review = new TaxReview
        {
            Id = Guid.NewGuid(),
            TaxReturnId = taxReturn.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ReviewerId = reviewerId,
            Comments = string.IsNullOrWhiteSpace(request.Comments) ? null : request.Comments.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            Type = activityType,
            Message = $"Tax return {taxReturn.Id} changed from {oldStatus} to {newStatus}.",
            UserId = reviewerId,
            TimestampUtc = DateTime.UtcNow
        };

        _db.TaxReviews.Add(review);
        _db.Activities.Add(activity);

        await _db.SaveChangesAsync(cancellationToken);
        return taxReturn;
    }
}

