using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Infrastructure.Data;

public class JobStatConfiguration : IEntityTypeConfiguration<JobStat>
{
    public void Configure(EntityTypeBuilder<JobStat> builder)
    {
        builder.ToTable("job_stats");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobId).HasColumnName("job_id").IsRequired();
        builder.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
        builder.Property(x => x.TotalApplications).HasColumnName("total_applications").IsRequired();
        builder.Property(x => x.Shortlisted).HasColumnName("shortlisted").IsRequired();
        builder.Property(x => x.InterviewsScheduled).HasColumnName("interviews_scheduled").IsRequired();
        builder.Property(x => x.Offered).HasColumnName("offered").IsRequired();
        builder.Property(x => x.Rejected).HasColumnName("rejected").IsRequired();
        builder.Property(x => x.Withdrawn).HasColumnName("withdrawn").IsRequired();
        builder.Property(x => x.FirstApplicationAt).HasColumnName("first_application_at");
        builder.Property(x => x.LatestApplicationAt).HasColumnName("latest_application_at");
        
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasIndex(x => x.JobId).IsUnique().HasDatabaseName("ux_job_stats_job_id");
        builder.HasIndex(x => x.RecruiterId).HasDatabaseName("ix_job_stats_recruiter_id");
    }
}

public class PlatformStatConfiguration : IEntityTypeConfiguration<PlatformStat>
{
    public void Configure(EntityTypeBuilder<PlatformStat> builder)
    {
        builder.ToTable("platform_stats");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date).HasColumnName("date").IsRequired();
        builder.Property(x => x.TotalJobs).HasColumnName("total_jobs").IsRequired();
        builder.Property(x => x.TotalApplications).HasColumnName("total_applications").IsRequired();
        builder.Property(x => x.TotalInterviews).HasColumnName("total_interviews").IsRequired();
        builder.Property(x => x.TotalOffers).HasColumnName("total_offers").IsRequired();
        
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasIndex(x => x.Date).IsUnique().HasDatabaseName("ux_platform_stats_date");
    }
}
