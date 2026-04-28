using InterviewService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterviewService.Infrastructure.Data;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("interviews");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApplicationId).HasColumnName("application_id").IsRequired();
        builder.Property(x => x.CandidateId).HasColumnName("candidate_id").IsRequired();
        builder.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
        builder.Property(x => x.ScheduledAt).HasColumnName("scheduled_at").IsRequired();
        builder.Property(x => x.Mode).HasColumnName("mode").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.MeetingLink).HasColumnName("meeting_link").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(500).IsRequired();
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasIndex(x => x.ApplicationId).IsUnique().HasDatabaseName("ux_interviews_application_id");
        builder.HasIndex(x => x.CandidateId).HasDatabaseName("ix_interviews_candidate_id");
        builder.HasIndex(x => x.RecruiterId).HasDatabaseName("ix_interviews_recruiter_id");
        builder.HasIndex(x => x.ScheduledAt).HasDatabaseName("ix_interviews_scheduled_at");
    }
}
