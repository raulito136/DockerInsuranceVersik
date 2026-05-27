using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReferenceData.Domain;

namespace ReferenceData.Infrastructure.Persistence.Configurations
{
    public class ClaimStatusConfiguration : IEntityTypeConfiguration<ClaimStatus>
    {
        public void Configure(EntityTypeBuilder<ClaimStatus> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(
                new ClaimStatus { Id = 1, Code = "SUBMITTED", Name = "Submitted", Description = "Claim has been filed and is awaiting review.", IsActive = true },
                new ClaimStatus { Id = 2, Code = "UNDER_REVIEW", Name = "Under Review", Description = "A reviewer is actively assessing the claim.", IsActive = true },
                new ClaimStatus { Id = 3, Code = "APPROVED", Name = "Approved", Description = "The claim has been accepted; payment will be processed.", IsActive = true },
                new ClaimStatus { Id = 4, Code = "REJECTED", Name = "Rejected", Description = "The claim has been denied; no payment will be made.", IsActive = true },
                new ClaimStatus { Id = 5, Code = "PAID", Name = "Paid", Description = "Payment has been issued to the policy holder.", IsActive = true }
            );
        }
    }
}