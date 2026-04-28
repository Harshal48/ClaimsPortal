using EY.OnboardingLab.Api.Dtos.TaxpayerPii;
using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/taxpayers/{taxpayerId:guid}/pii")]
[Authorize(Roles = "Admin,Preparer")]
public class TaxpayerPiiController : ControllerBase
{
    private readonly ITaxpayerPiiService _taxpayerPiiService;

    public TaxpayerPiiController(ITaxpayerPiiService taxpayerPiiService)
    {
        _taxpayerPiiService = taxpayerPiiService;
    }

    // GET: /api/taxpayers/{taxpayerId}/pii
    [HttpGet]
    public async Task<ActionResult<TaxpayerPii>> Get(Guid taxpayerId, CancellationToken cancellationToken)
    {
        var pii = await _taxpayerPiiService.GetByTaxpayerIdAsync(taxpayerId, cancellationToken);

        if (pii is null)
            return NotFound();

        return Ok(pii);
    }

    // PUT: /api/taxpayers/{taxpayerId}/pii
    [HttpPut]
    public async Task<ActionResult<TaxpayerPii>> Upsert(
        Guid taxpayerId,
        UpsertTaxpayerPiiRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Ssn))
            return BadRequest("Ssn is required.");

        if (request.DateOfBirth == default)
            return BadRequest("DateOfBirth is required.");

        var saved = await _taxpayerPiiService.UpsertAsync(
            taxpayerId,
            new UpsertTaxpayerPiiRequest(
                request.Ssn,
                request.DateOfBirth,
                request.AddressStreet,
                request.AddressCity,
                request.AddressState,
                request.AddressZip),
            cancellationToken);

        return Ok(saved);
    }
}

