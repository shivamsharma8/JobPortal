using JobService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobService.Infrastructure.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs", "job");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(16000).IsRequired();
        builder.Property(x => x.Company).HasColumnName("company").HasMaxLength(256).IsRequired();
        builder.Property(x => x.CompanyLogoUrl).HasColumnName("company_logo_url").HasMaxLength(1024);
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(256);
        builder.Property(x => x.IsRemote).HasColumnName("is_remote").HasDefaultValue(false);
        builder.Property(x => x.JobType).HasColumnName("job_type").HasConversion<string>();
        builder.Property(x => x.ExperienceLevel).HasColumnName("experience_level").HasConversion<string>();
        builder.Property(x => x.SalaryMin).HasColumnName("salary_min").HasMaxLength(64);
        builder.Property(x => x.SalaryMax).HasColumnName("salary_max").HasMaxLength(64);
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(8).HasDefaultValue("USD");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(x => x.PublishedAt).HasColumnName("published_at");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.CategoryId).HasColumnName("category_id");
        builder.Property(x => x.ViewCount).HasColumnName("view_count").HasDefaultValue(0);
        builder.Property(x => x.ApplicationCount).HasColumnName("application_count").HasDefaultValue(0);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.HasIndex(x => x.RecruiterId).HasDatabaseName("ix_jobs_recruiter_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_jobs_status");
        builder.HasIndex(x => x.CategoryId).HasDatabaseName("ix_jobs_category_id");
        builder.HasIndex(x => x.Title).HasDatabaseName("ix_jobs_title");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Ignore(x => x.DomainEvents);
        builder.HasMany(x => x.Skills).WithOne().HasForeignKey(s => s.JobId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.ToTable("job_skills", "job");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.JobId).HasColumnName("job_id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(128).IsRequired();
        builder.Property(x => x.IsRequired).HasColumnName("is_required").HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Ignore(x => x.DomainEvents);
    }
}

public class JobCategoryConfiguration : IEntityTypeConfiguration<JobCategory>
{
    public void Configure(EntityTypeBuilder<JobCategory> builder)
    {
        builder.ToTable("job_categories", "job");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(128).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(512);
        builder.Property(x => x.JobCount).HasColumnName("job_count").HasDefaultValue(0);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.HasIndex(x => x.Slug).IsUnique().HasDatabaseName("ix_job_categories_slug");
        builder.Ignore(x => x.DomainEvents);
    }
}
