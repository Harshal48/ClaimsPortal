using EY.OnboardingLab.Core.Entities;

namespace EY.OnboardingLab.Services.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
    Task<User?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public record CreateUserRequest(string UserName, string Email, string Password, string Role, bool IsActive);
public record UpdateUserRequest(string UserName, string Email, string Role, bool IsActive);
