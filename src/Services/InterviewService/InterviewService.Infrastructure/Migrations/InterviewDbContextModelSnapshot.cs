using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using InterviewService.Infrastructure.Data;

[DbContext(typeof(InterviewDbContext))]
partial class InterviewDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("interview");

        modelBuilder.Entity("InterviewService.Domain.Entities.Interview", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnName("id");
            b.Property<Guid>("ApplicationId").HasColumnName("application_id");
            b.Property<Guid>("CandidateId").HasColumnName("candidate_id");
            b.Property<Guid>("RecruiterId").HasColumnName("recruiter_id");
            b.Property<DateTime>("ScheduledAt").HasColumnName("scheduled_at");
            b.Property<string>("Mode").HasColumnName("mode").HasMaxLength(32);
            b.Property<string>("MeetingLink").HasColumnName("meeting_link").HasMaxLength(500);
            b.Property<string>("Location").HasColumnName("location").HasMaxLength(500);
            b.Property<string>("Notes").HasColumnName("notes").HasMaxLength(2000);
            b.Property<string>("Status").HasColumnName("status").HasMaxLength(32);
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at");
            b.Property<DateTime>("UpdatedAt").HasColumnName("updated_at");
            b.Property<bool>("IsDeleted").HasColumnName("is_deleted");
            b.HasKey("Id");
            b.ToTable("interviews");
            b.HasIndex("ApplicationId").IsUnique().HasDatabaseName("ux_interviews_application_id");
            b.HasIndex("CandidateId").HasDatabaseName("ix_interviews_candidate_id");
            b.HasIndex("RecruiterId").HasDatabaseName("ix_interviews_recruiter_id");
            b.HasIndex("ScheduledAt").HasDatabaseName("ix_interviews_scheduled_at");
        });
    }
}
