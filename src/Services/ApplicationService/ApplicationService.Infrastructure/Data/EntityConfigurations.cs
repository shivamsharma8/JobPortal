using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = ApplicationService.Domain.Entities.Application;

namespace ApplicationService.Infrastructure.Data;

public class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.ToTable("applications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobId).HasColumnName("job_id").IsRequired();
        builder.Property(x => x.CandidateId).HasColumnName("candidate_id").IsRequired();
        builder.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
        builder.Property(x => x.ResumeUrl).HasColumnName("resume_url").HasMaxLength(500).IsRequired();
        builder.Property(x => x.CoverLetter).HasColumnName("cover_letter").HasMaxLength(4000);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.AppliedAt).HasColumnName("applied_at").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasIndex(x => new { x.JobId, x.CandidateId }).IsUnique().HasDatabaseName("ux_applications_job_candidate");
        builder.HasIndex(x => x.CandidateId).HasDatabaseName("ix_applications_candidate_id");
        builder.HasIndex(x => x.JobId).HasDatabaseName("ix_applications_job_id");
        builder.HasIndex(x => x.RecruiterId).HasDatabaseName("ix_applications_recruiter_id");
    }
}
