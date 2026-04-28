using InterviewService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewService.Infrastructure.Data;

public class InterviewDbContext(DbContextOptions<InterviewDbContext> options) : DbContext(options)
{
    public DbSet<Interview> Interviews => Set<Interview>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("interview");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InterviewDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BuildingBlocks.Common.Domain.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdateTimestamp();

        return base.SaveChangesAsync(cancellationToken);
    }
}
