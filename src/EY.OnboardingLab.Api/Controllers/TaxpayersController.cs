using EY.OnboardingLab.Api.Dtos.Taxpayers;
using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Preparer,Reviewer")]
public class TaxpayersController : ControllerBase
{
    private readonly ITaxpayerService _taxpayerService;

    public TaxpayersController(ITaxpayerService taxpayerService)
    {
        _taxpayerService = taxpayerService;
    }

    // GET: /api/taxpayers
    [HttpGet]
    public async Task<ActionResult<List<Taxpayer>>> GetAll(CancellationToken cancellationToken)
    {
        var taxpayers = await _taxpayerService.GetAllAsync(cancellationToken);

        return Ok(taxpayers);
    }

    // GET: /api/taxpayers/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Taxpayer>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var taxpayer = await _taxpayerService.GetByIdAsync(id, cancellationToken);

        if (taxpayer is null)
            return NotFound();

        return Ok(taxpayer);
    }

    // POST: /api/taxpayers
    [HttpPost]
    public async Task<ActionResult<Taxpayer>> Create(CreateTaxpayerRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TaxpayerNumber))
            return BadRequest("TaxpayerNumber is required.");

        if (string.IsNullOrWhiteSpace(request.LegalName))
            return BadRequest("LegalName is required.");

        var taxpayer = await _taxpayerService.CreateAsync(
            new CreateTaxpayerRequest(request.TaxpayerNumber.Trim(), request.LegalName.Trim(), request.CreatedByUserId),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = taxpayer.Id }, taxpayer);
    }

    // PUT: /api/taxpayers/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Taxpayer>> Update(Guid id, UpdateTaxpayerRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TaxpayerNumber))
            return BadRequest("TaxpayerNumber is required.");

        if (string.IsNullOrWhiteSpace(request.LegalName))
            return BadRequest("LegalName is required.");

        var taxpayer = await _taxpayerService.UpdateAsync(
            id,
            new UpdateTaxpayerRequest(request.TaxpayerNumber.Trim(), request.LegalName.Trim()),
            cancellationToken);

        if (taxpayer is null)
            return NotFound();

        return Ok(taxpayer);
    }

    // DELETE: /api/taxpayers/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _taxpayerService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
