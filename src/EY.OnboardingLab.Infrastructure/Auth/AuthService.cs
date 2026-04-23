using System.IdentityModel.Tokens.Jwt;  // to crete token
using System.Security.Claims; // data inside token
using System.Text;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EY.OnboardingLab.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;   // to read users frm databse
    private readonly IConfiguration _configuration; // to read val from appsetting

    public AuthService(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<LoginResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var input = (request.UserNameOrEmail ?? "").Trim();  //clean the ipnut

        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.UserName == input || u.Email == input,   
            cancellationToken);

        if (user is null)
            return null;

        if (!user.IsActive)
            return null;

        // Beginner-friendly login (plain text password). Later we will replace with a secure hash.
        if (user.Password != request.Password)
            return null;

        var jwtKey = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresInSecondsText = _configuration["Jwt:ExpiresInSeconds"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("Missing configuration value: Jwt:Key");

        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException("Missing configuration value: Jwt:Issuer");

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Missing configuration value: Jwt:Audience");

        if (!int.TryParse(expiresInSecondsText, out var expiresInSeconds))
            throw new InvalidOperationException("Missing/invalid configuration value: Jwt:ExpiresInSeconds");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));   //same secret is used to sign in and varify
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddSeconds(expiresInSeconds);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);  //convert token object to string

        return new LoginResult(
            AccessToken: accessToken,
            ExpiresInSeconds: expiresInSeconds,
            UserId: user.Id,
            UserName: user.UserName,
            Email: user.Email,
            Role: user.Role);
    }
}
