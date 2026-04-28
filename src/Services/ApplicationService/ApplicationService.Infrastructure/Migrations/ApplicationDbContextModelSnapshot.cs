using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationService.Infrastructure.Data;

[DbContext(typeof(ApplicationDbContext))]
partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("application");

        modelBuilder.Entity("ApplicationService.Domain.Entities.Application", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnName("id");
            b.Property<Guid>("JobId").HasColumnName("job_id");
            b.Property<Guid>("CandidateId").HasColumnName("candidate_id");
            b.Property<Guid>("RecruiterId").HasColumnName("recruiter_id");
            b.Property<string>("ResumeUrl").HasColumnName("resume_url").HasMaxLength(500);
            b.Property<string>("CoverLetter").HasColumnName("cover_letter").HasMaxLength(4000);
            b.Property<string>("Status").HasColumnName("status").HasMaxLength(32);
            b.Property<DateTime>("AppliedAt").HasColumnName("applied_at");
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at");
            b.Property<DateTime>("UpdatedAt").HasColumnName("updated_at");
            b.Property<bool>("IsDeleted").HasColumnName("is_deleted");
            b.HasKey("Id");
            b.ToTable("applications");
            b.HasIndex("JobId").HasDatabaseName("ix_applications_job_id");
            b.HasIndex("CandidateId").HasDatabaseName("ix_applications_candidate_id");
            b.HasIndex("RecruiterId").HasDatabaseName("ix_applications_recruiter_id");
            b.HasIndex("JobId", "CandidateId").IsUnique().HasDatabaseName("ux_applications_job_candidate");
        });
    }
}
