using EY.OnboardingLab.Infrastructure.Auth;
using EY.OnboardingLab.Infrastructure.Data;
using EY.OnboardingLab.Infrastructure.Dependents;
using EY.OnboardingLab.Infrastructure.Reports;
using EY.OnboardingLab.Infrastructure.Review;
using EY.OnboardingLab.Infrastructure.Returns;
using EY.OnboardingLab.Infrastructure.Taxpayers;
using EY.OnboardingLab.Infrastructure.Users;
using EY.OnboardingLab.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EY.OnboardingLab.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IDependentService, DependentService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ITaxReturnService, TaxReturnService>();
        services.AddScoped<ITaxpayerService, TaxpayerService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
