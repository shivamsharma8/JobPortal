using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionService.Domain.Entities;

namespace SubscriptionService.Infrastructure.Data;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
        builder.Property(x => x.Plan).HasColumnName("plan").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.StartsAt).HasColumnName("starts_at").IsRequired();
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();
        
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasIndex(x => x.RecruiterId).HasDatabaseName("ix_subscriptions_recruiter_id");
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id").IsRequired();
        builder.Property(x => x.RecruiterId).HasColumnName("recruiter_id").IsRequired();
        builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(10).IsRequired();
        builder.Property(x => x.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50).IsRequired();
        builder.Property(x => x.TransactionId).HasColumnName("transaction_id").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.PaidAt).HasColumnName("paid_at").IsRequired();
        
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired();

        builder.HasIndex(x => x.SubscriptionId).HasDatabaseName("ix_payments_subscription_id");
        builder.HasIndex(x => x.RecruiterId).HasDatabaseName("ix_payments_recruiter_id");
    }
}
