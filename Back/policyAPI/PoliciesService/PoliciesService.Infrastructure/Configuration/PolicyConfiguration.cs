using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PoliciesService.Domain;

namespace PoliciesService.Infrastructure.Configuration
{
    public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.PolicyNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(e => e.PolicyNumber)
                .IsUnique();

            builder.Property(e => e.PolicyTypeCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.CoverageTypeCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.CoverageAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.PremiumAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(p => p.PolicyHolder)
                  .WithMany(ph => ph.Policies)
                  .HasForeignKey(p => p.PolicyHolderId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
