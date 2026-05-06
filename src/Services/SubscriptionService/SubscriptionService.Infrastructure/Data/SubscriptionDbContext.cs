using Microsoft.EntityFrameworkCore;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Infrastructure.Data;

public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : DbContext(options)
{
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("subscription");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubscriptionDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BuildingBlocks.Common.Domain.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdateTimestamp();

        return base.SaveChangesAsync(cancellationToken);
    }
}
