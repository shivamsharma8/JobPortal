using JobService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobService.Infrastructure.Data;

public class JobDbContext(DbContextOptions<JobDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSkill> JobSkills => Set<JobSkill>();
    public DbSet<JobCategory> JobCategories => Set<JobCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("job");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobDbContext).Assembly);
        SeedCategories(modelBuilder);
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Software Engineering", Slug = "software-engineering", Description = "Software development roles", JobCount = 0, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), IsDeleted = false },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Data Science", Slug = "data-science", Description = "Data and ML roles", JobCount = 0, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), IsDeleted = false },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Product Management", Slug = "product-management", Description = "PM roles", JobCount = 0, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), IsDeleted = false },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Design", Slug = "design", Description = "UX/UI design roles", JobCount = 0, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), IsDeleted = false },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "DevOps & Infrastructure", Slug = "devops", Description = "DevOps and cloud roles", JobCount = 0, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), IsDeleted = false },
        };
        modelBuilder.Entity<JobCategory>().HasData(categories);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BuildingBlocks.Common.Domain.BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdateTimestamp();
        return base.SaveChangesAsync(cancellationToken);
    }
}
