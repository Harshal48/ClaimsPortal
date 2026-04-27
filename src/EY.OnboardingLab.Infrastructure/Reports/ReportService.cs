using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EY.OnboardingLab.Infrastructure.Reports;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Report>> GetAllAsync(CancellationToken cancellationToken)
    {
        var reports = await _db.Reports
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return reports;
    }

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var report = await _db.Reports.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return report;
    }

    public async Task<Report> GenerateAsync(GenerateReportRequest request, CancellationToken cancellationToken)
    {
        var reportType = (request.ReportType ?? "").Trim();
        if (string.IsNullOrWhiteSpace(reportType))
            throw new InvalidOperationException("ReportType is required.");

        // Simple generator: just store some JSON content. Later we can generate PDFs.
        var payload = new
        {
            reportType,
            generatedAtUtc = DateTime.UtcNow
        };

        var report = new Report
        {
            Id = Guid.NewGuid(),
            ReportType = reportType,
            Status = "Generated",
            FileName = $"{reportType}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json",
            ContentType = "application/json",
            Content = JsonSerializer.Serialize(payload),
            GeneratedByUserId = Guid.Empty,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Reports.Add(report);

        // Log an activity entry so "recent downloads" can be shown later.
        _db.Activities.Add(new Activity
        {
            Id = Guid.NewGuid(),
            Type = "ReportGenerated",
            Message = $"Report {report.ReportType} generated (ReportId={report.Id}).",
            UserId = report.GeneratedByUserId,
            TimestampUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<List<Activity>> GetRecentDownloadsAsync(CancellationToken cancellationToken)
    {
        // For now, we return recent report-related activities.
        var activities = await _db.Activities
            .Where(a => a.Type == "ReportGenerated" || a.Type == "ReportDownloaded")
            .OrderByDescending(a => a.TimestampUtc)
            .Take(25)
            .ToListAsync(cancellationToken);

        return activities;
    }
}

