using EY.OnboardingLab.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EY.OnboardingLab.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Taxpayer> Taxpayers => Set<Taxpayer>();
    public DbSet<Dependent> Dependents => Set<Dependent>();
    public DbSet<TaxReturn> TaxReturns => Set<TaxReturn>();
    public DbSet<Deduction> Deductions => Set<Deduction>();
    public DbSet<TaxReview> TaxReviews => Set<TaxReview>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Report> Reports => Set<Report>();
}
