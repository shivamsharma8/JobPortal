using Microsoft.EntityFrameworkCore;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Infrastructure.Data;

public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options)
{
    public DbSet<JobStat> JobStats => Set<JobStat>();
    public DbSet<PlatformStat> PlatformStats => Set<PlatformStat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("analytics");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnalyticsDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BuildingBlocks.Common.Domain.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdateTimestamp();

        return base.SaveChangesAsync(cancellationToken);
    }
}
