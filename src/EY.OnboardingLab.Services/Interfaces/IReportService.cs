using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface IReportService
{
    Task<List<Report>> GetAllAsync(CancellationToken cancellationToken);

    Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Report> GenerateAsync(GenerateReportRequest request, CancellationToken cancellationToken);

    Task<List<Activity>> GetRecentDownloadsAsync(CancellationToken cancellationToken);
}

public record GenerateReportRequest(string ReportType);

