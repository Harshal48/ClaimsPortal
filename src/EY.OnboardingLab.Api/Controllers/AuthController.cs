using EY.OnboardingLab.Api.Dtos.Auth;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserNameOrEmail))
            return BadRequest("UserNameOrEmail is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required.");

        var result = await _authService.LoginAsync(
            new LoginRequest(request.UserNameOrEmail.Trim(), request.Password),
            cancellationToken);

        if (result is null)
            return Unauthorized();

        var response = new LoginResponseDto(
            AccessToken: result.AccessToken,
            TokenType: "Bearer",
            ExpiresInSeconds: result.ExpiresInSeconds,
            UserId: result.UserId,
            UserName: result.UserName,
            Email: result.Email,
            Role: result.Role);

        return Ok(response);
    }

    // GET: /api/auth/me
    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var userName = User.Identity?.Name
                       ?? User.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            userId,
            userName,
            email,
            role
        });
    }
}
