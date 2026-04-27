using EY.OnboardingLab.Api.Dtos.Reports;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: /api/reports
    [HttpGet]
    public async Task<ActionResult<object>> GetAvailableReports()
    {
        // Simple list (matches your case study report types concept).
        return Ok(new[]
        {
            "ClaimantSummary",
            "ClaimPacketDetail",
            "ClaimAmountBreakdown",
            "AdjustmentSummary",
            "ActivityLog"
        });
    }

    // POST: /api/reports/generate
    [HttpPost("generate")]
    public async Task<ActionResult<object>> Generate(GenerateReportRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ReportType))
            return BadRequest("ReportType is required.");

        var report = await _reportService.GenerateAsync(
            new GenerateReportRequest(request.ReportType.Trim()),
            cancellationToken);

        return Ok(new
        {
            report.Id,
            report.ReportType,
            report.Status,
            report.FileName,
            report.ContentType,
            report.CreatedAtUtc
        });
    }

    // GET: /api/reports/{id}/download
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var report = await _reportService.GetByIdAsync(id, cancellationToken);
        if (report is null)
            return NotFound();

        var bytes = System.Text.Encoding.UTF8.GetBytes(report.Content);
        return File(bytes, report.ContentType, report.FileName);
    }

    // GET: /api/reports/recent-downloads
    [HttpGet("recent-downloads")]
    public async Task<IActionResult> RecentDownloads(CancellationToken cancellationToken)
    {
        var items = await _reportService.GetRecentDownloadsAsync(cancellationToken);
        return Ok(items);
    }
}

