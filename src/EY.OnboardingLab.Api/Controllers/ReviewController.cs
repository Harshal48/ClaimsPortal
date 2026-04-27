using EY.OnboardingLab.Api.Dtos.Review;
using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Reviewer")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: /api/review/pending
    [HttpGet("pending")]
    public async Task<ActionResult<List<TaxReturn>>> GetPending(CancellationToken cancellationToken)
    {
        var pending = await _reviewService.GetPendingAsync(cancellationToken);
        return Ok(pending);
    }

    // GET: /api/review/{returnId}
    [HttpGet("{returnId:guid}")]
    public async Task<ActionResult<List<TaxReview>>> GetReviewHistory(Guid returnId, CancellationToken cancellationToken)
    {
        var history = await _reviewService.GetReviewHistoryAsync(returnId, cancellationToken);
        return Ok(history);
    }

    // POST: /api/review/{returnId}/approve
    [HttpPost("{returnId:guid}/approve")]
    public async Task<ActionResult<TaxReturn>> Approve(Guid returnId, ReviewActionRequestDto request, CancellationToken cancellationToken)
    {
        var reviewerId = GetUserId();
        if (reviewerId is null)
            return Unauthorized();

        var updated = await _reviewService.ApproveAsync(
            returnId,
            reviewerId.Value,
            new ReviewActionRequest(request.Comments),
            cancellationToken);

        if (updated is null)
            return BadRequest("Approve failed. Return must be Submitted.");

        return Ok(updated);
    }

    // POST: /api/review/{returnId}/reject
    [HttpPost("{returnId:guid}/reject")]
    public async Task<ActionResult<TaxReturn>> Reject(Guid returnId, ReviewActionRequestDto request, CancellationToken cancellationToken)
    {
        var reviewerId = GetUserId();
        if (reviewerId is null)
            return Unauthorized();

        var updated = await _reviewService.RejectAsync(
            returnId,
            reviewerId.Value,
            new ReviewActionRequest(request.Comments),
            cancellationToken);

        if (updated is null)
            return BadRequest("Reject failed. Return must be Submitted.");

        return Ok(updated);
    }

    // POST: /api/review/{returnId}/request-changes
    [HttpPost("{returnId:guid}/request-changes")]
    public async Task<ActionResult<TaxReturn>> RequestChanges(Guid returnId, ReviewActionRequestDto request, CancellationToken cancellationToken)
    {
        var reviewerId = GetUserId();
        if (reviewerId is null)
            return Unauthorized();

        var updated = await _reviewService.RequestChangesAsync(
            returnId,
            reviewerId.Value,
            new ReviewActionRequest(request.Comments),
            cancellationToken);

        if (updated is null)
            return BadRequest("Request changes failed. Return must be Submitted.");

        return Ok(updated);
    }

    private Guid? GetUserId()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(userIdText, out var userId))
            return null;

        return userId;
    }
}

