using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface IReviewService
{
    Task<List<TaxReturn>> GetPendingAsync(CancellationToken cancellationToken);

    Task<List<TaxReview>> GetReviewHistoryAsync(Guid taxReturnId, CancellationToken cancellationToken);

    Task<TaxReturn?> ApproveAsync(Guid taxReturnId, Guid reviewerId, ReviewActionRequest request, CancellationToken cancellationToken);

    Task<TaxReturn?> RejectAsync(Guid taxReturnId, Guid reviewerId, ReviewActionRequest request, CancellationToken cancellationToken);

    Task<TaxReturn?> RequestChangesAsync(Guid taxReturnId, Guid reviewerId, ReviewActionRequest request, CancellationToken cancellationToken);
}

public record ReviewActionRequest(string? Comments);

