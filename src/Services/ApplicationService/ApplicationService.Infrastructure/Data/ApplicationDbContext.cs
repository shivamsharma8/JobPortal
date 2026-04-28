using Microsoft.EntityFrameworkCore;
using ApplicationEntity = ApplicationService.Domain.Entities.Application;

namespace ApplicationService.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationEntity> Applications => Set<ApplicationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("application");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BuildingBlocks.Common.Domain.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdateTimestamp();

        return base.SaveChangesAsync(cancellationToken);
    }
}
