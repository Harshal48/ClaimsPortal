using EY.OnboardingLab.Api.Dtos.Users;
using EY.OnboardingLab.Core.Entities;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EY.OnboardingLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // POST: /api/users
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            return BadRequest("UserName is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required.");

        if (string.IsNullOrWhiteSpace(request.Role))
            return BadRequest("Role is required.");

        var user = await _userService.CreateAsync(
            new CreateUserRequest(
                request.UserName.Trim(),
                request.Email.Trim(),
                request.Password,
                request.Role.Trim(),
                request.IsActive),
            cancellationToken);

        return Ok(ToDto(user));
    }

    // GET: /api/users/{id}
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(ToDto(user));
    }

    // GET: /api/users
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users.Select(ToDto).ToList());
    }

    // PUT: /api/users/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> Update(Guid id, UpdateUserRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            return BadRequest("UserName is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required.");

        if (string.IsNullOrWhiteSpace(request.Role))
            return BadRequest("Role is required.");

        var user = await _userService.UpdateAsync(
            id,
            new UpdateUserRequest(
                request.UserName.Trim(),
                request.Email.Trim(),
                request.Role.Trim(),
                request.IsActive),
            cancellationToken);

        if (user is null)
            return NotFound();

        return Ok(ToDto(user));
    }

    // DELETE: /api/users/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _userService.DeleteAsync(id, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto(
            Id: user.Id,
            UserName: user.UserName,
            Email: user.Email,
            Role: user.Role,
            IsActive: user.IsActive,
            CreatedAtUtc: user.CreatedAtUtc);
    }
}
