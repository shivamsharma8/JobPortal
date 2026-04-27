using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.Domain.Entities;

namespace ProfileService.Infrastructure.Configurations;

public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
{
    public void Configure(EntityTypeBuilder<CandidateProfile> builder)
    {
        builder.ToTable("candidate_profiles", "profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.Headline).HasColumnName("headline").HasMaxLength(256);
        builder.Property(x => x.Summary).HasColumnName("summary").HasMaxLength(4000);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(32);
        builder.Property(x => x.LinkedInUrl).HasColumnName("linkedin_url").HasMaxLength(512);
        builder.Property(x => x.GitHubUrl).HasColumnName("github_url").HasMaxLength(512);
        builder.Property(x => x.PortfolioUrl).HasColumnName("portfolio_url").HasMaxLength(512);
        builder.Property(x => x.ExperienceLevel).HasColumnName("experience_level").HasConversion<string>();
        builder.Property(x => x.YearsOfExperience).HasColumnName("years_of_experience");
        builder.Property(x => x.ExpectedSalary).HasColumnName("expected_salary").HasPrecision(18, 2);
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(8);
        builder.Property(x => x.IsOpenToWork).HasColumnName("is_open_to_work").HasDefaultValue(true);
        builder.Property(x => x.ProfileCompletionPercent).HasColumnName("profile_completion_percent");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("ix_candidate_profiles_user_id");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Ignore(x => x.DomainEvents);
        builder.HasMany(x => x.Skills).WithOne().HasForeignKey(s => s.CandidateProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Experiences).WithOne().HasForeignKey(e => e.CandidateProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Addresses).WithOne().HasForeignKey(a => a.CandidateProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Resumes).WithOne().HasForeignKey(r => r.CandidateProfileId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RecruiterProfileConfiguration : IEntityTypeConfiguration<RecruiterProfile>
{
    public void Configure(EntityTypeBuilder<RecruiterProfile> builder)
    {
        builder.ToTable("recruiter_profiles", "profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.CompanyName).HasColumnName("company_name").HasMaxLength(256);
        builder.Property(x => x.CompanyWebsite).HasColumnName("company_website").HasMaxLength(512);
        builder.Property(x => x.CompanyLogoUrl).HasColumnName("company_logo_url").HasMaxLength(1024);
        builder.Property(x => x.Industry).HasColumnName("industry").HasMaxLength(128);
        builder.Property(x => x.CompanySize).HasColumnName("company_size").HasMaxLength(64);
        builder.Property(x => x.CompanyDescription).HasColumnName("company_description").HasMaxLength(4000);
        builder.Property(x => x.ContactEmail).HasColumnName("contact_email").HasMaxLength(256);
        builder.Property(x => x.ContactPhone).HasColumnName("contact_phone").HasMaxLength(32);
        builder.Property(x => x.LinkedInUrl).HasColumnName("linkedin_url").HasMaxLength(512);
        builder.Property(x => x.ProfileCompletionPercent).HasColumnName("profile_completion_percent");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("ix_recruiter_profiles_user_id");
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Ignore(x => x.DomainEvents);
        builder.HasMany(x => x.Addresses).WithOne().HasForeignKey(a => a.RecruiterProfileId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills", "profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.CandidateProfileId).HasColumnName("candidate_profile_id");
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(128).IsRequired();
        builder.Property(x => x.ProficiencyLevel).HasColumnName("proficiency_level");
        builder.Property(x => x.YearsOfExperience).HasColumnName("years_of_experience");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Ignore(x => x.DomainEvents);
    }
}

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses", "profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.CandidateProfileId).HasColumnName("candidate_profile_id");
        builder.Property(x => x.RecruiterProfileId).HasColumnName("recruiter_profile_id");
        builder.Property(x => x.Street).HasColumnName("street").HasMaxLength(256);
        builder.Property(x => x.City).HasColumnName("city").HasMaxLength(128);
        builder.Property(x => x.State).HasColumnName("state").HasMaxLength(128);
        builder.Property(x => x.Country).HasColumnName("country").HasMaxLength(128);
        builder.Property(x => x.ZipCode).HasColumnName("zip_code").HasMaxLength(20);
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Ignore(x => x.DomainEvents);
    }
}

public class ResumeDocumentConfiguration : IEntityTypeConfiguration<ResumeDocument>
{
    public void Configure(EntityTypeBuilder<ResumeDocument> builder)
    {
        builder.ToTable("resume_documents", "profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.CandidateProfileId).HasColumnName("candidate_profile_id");
        builder.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(256).IsRequired();
        builder.Property(x => x.StorageKey).HasColumnName("storage_key").HasMaxLength(1024).IsRequired();
        builder.Property(x => x.ContentType).HasColumnName("content_type").HasMaxLength(128);
        builder.Property(x => x.FileSizeBytes).HasColumnName("file_size_bytes");
        builder.Property(x => x.IsPrimary).HasColumnName("is_primary").HasDefaultValue(false);
        builder.Property(x => x.PublicUrl).HasColumnName("public_url").HasMaxLength(2048);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Ignore(x => x.DomainEvents);
    }
}

public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.ToTable("work_experiences", "profile");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.CandidateProfileId).HasColumnName("candidate_profile_id");
        builder.Property(x => x.Title).HasColumnName("title").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Company).HasColumnName("company").HasMaxLength(256).IsRequired();
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(256);
        builder.Property(x => x.StartDate).HasColumnName("start_date");
        builder.Property(x => x.EndDate).HasColumnName("end_date");
        builder.Property(x => x.IsCurrentRole).HasColumnName("is_current_role").HasDefaultValue(false);
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(4000);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Ignore(x => x.DomainEvents);
    }
}
