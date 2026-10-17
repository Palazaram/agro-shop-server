using AgroShop.Domain.Abstractions;
using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace AgroShop.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    Task IUnitOfWork.SaveChangesAsync(CancellationToken ct)
        => SaveChangesAsync(ct);
}