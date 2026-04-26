using EY.OnboardingLab.Api.Dtos.Returns;
using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/returns")]
[Authorize(Roles = "Admin,Preparer,Reviewer")]
public class TaxReturnsController : ControllerBase
{
    private readonly ITaxReturnService _taxReturnService;

    public TaxReturnsController(ITaxReturnService taxReturnService)
    {
        _taxReturnService = taxReturnService;
    }

    // GET: /api/returns
    [HttpGet]
    public async Task<ActionResult<List<TaxReturnResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var returns = await _taxReturnService.GetAllAsync(cancellationToken);
        return Ok(returns.Select(ToDto).ToList());
    }

    // GET: /api/returns/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaxReturnResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var taxReturn = await _taxReturnService.GetByIdAsync(id, cancellationToken);

        if (taxReturn is null)
            return NotFound();

        return Ok(ToDto(taxReturn));
    }

    // POST: /api/returns
    [HttpPost]
    [Authorize(Roles = "Admin,Preparer")]
    public async Task<ActionResult<TaxReturnResponseDto>> Create(CreateTaxReturnRequestDto request, CancellationToken cancellationToken)
    {
        if (request.TaxpayerId == Guid.Empty)
            return BadRequest("TaxpayerId is required.");

        if (request.TaxYear <= 0)
            return BadRequest("TaxYear is required.");

        if (string.IsNullOrWhiteSpace(request.FilingStatus))
            return BadRequest("FilingStatus is required.");

        var created = await _taxReturnService.CreateAsync(
            new CreateTaxReturnRequest(
                request.TaxpayerId,
                request.TaxYear,
                request.FilingStatus),
            cancellationToken);

        return Ok(ToDto(created));
    }

    // PUT: /api/returns/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Preparer")]
    public async Task<ActionResult<TaxReturnResponseDto>> Update(
        Guid id,
        UpdateTaxReturnRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.TaxYear <= 0)
            return BadRequest("TaxYear is required.");

        if (string.IsNullOrWhiteSpace(request.FilingStatus))
            return BadRequest("FilingStatus is required.");

        var existing = await _taxReturnService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        if (!string.Equals(existing.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only Draft returns can be updated.");

        var updated = await _taxReturnService.UpdateAsync(
            id,
            new UpdateTaxReturnRequest(request.TaxYear, request.FilingStatus),
            cancellationToken);

        if (updated is null)
            return BadRequest("Update failed.");

        return Ok(ToDto(updated));
    }

    // POST: /api/returns/{id}/income
    [HttpPost("{id:guid}/income")]
    [Authorize(Roles = "Admin,Preparer")]
    public async Task<ActionResult<TaxReturnResponseDto>> AddIncome(
        Guid id,
        AddIncomeRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var existing = await _taxReturnService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        if (!string.Equals(existing.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only Draft returns can be modified.");

        var updated = await _taxReturnService.AddIncomeAsync(
            id,
            new AddIncomeRequest(request.Amount),
            cancellationToken);

        if (updated is null)
            return BadRequest("Add income failed.");

        return Ok(ToDto(updated));
    }

    // POST: /api/returns/{id}/deductions
    [HttpPost("{id:guid}/deductions")]
    [Authorize(Roles = "Admin,Preparer")]
    public async Task<ActionResult<TaxReturnResponseDto>> AddDeduction(
        Guid id,
        AddDeductionRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DeductionName))
            return BadRequest("DeductionName is required.");

        if (string.IsNullOrWhiteSpace(request.DeductionType))
            return BadRequest("DeductionType is required.");

        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than 0.");

        var existing = await _taxReturnService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        if (!string.Equals(existing.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only Draft returns can be modified.");

        var updated = await _taxReturnService.AddDeductionAsync(
            id,
            new AddDeductionRequest(
                request.DeductionName.Trim(),
                request.DeductionType.Trim(),
                request.Amount),
            cancellationToken);

        if (updated is null)
            return BadRequest("Add deduction failed.");

        return Ok(ToDto(updated));
    }

    // POST: /api/returns/{id}/submit
    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "Admin,Preparer")]
    public async Task<ActionResult<TaxReturnResponseDto>> Submit(Guid id, CancellationToken cancellationToken)
    {
        var existing = await _taxReturnService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        if (!string.Equals(existing.ReturnStatus, "Draft", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only Draft returns can be submitted.");

        var updated = await _taxReturnService.SubmitAsync(id, cancellationToken);
        if (updated is null)
            return BadRequest("Submit failed.");

        return Ok(ToDto(updated));
    }

    private static TaxReturnResponseDto ToDto(TaxReturn taxReturn)
    {
        return new TaxReturnResponseDto(
            Id: taxReturn.Id,
            TaxpayerId: taxReturn.TaxpayerId,
            TaxYear: taxReturn.TaxYear,
            ReturnStatus: taxReturn.ReturnStatus,
            FilingStatus: taxReturn.FilingStatus,
            ReviewerId: taxReturn.ReviewerId,
            TotalIncome: taxReturn.TotalIncome,
            TotalWithheld: taxReturn.TotalWithheld,
            TotalDeduction: taxReturn.TotalDeduction,
            CreatedAtUtc: taxReturn.CreatedAtUtc,
            UpdatedAtUtc: taxReturn.UpdatedAtUtc);
    }
}

