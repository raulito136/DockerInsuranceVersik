using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReferenceData.Domain;

namespace ReferenceData.Infrastructure.Persistence.Configurations
{
    public class CoverageTypeConfiguration : IEntityTypeConfiguration<CoverageType>
    {
        public void Configure(EntityTypeBuilder<CoverageType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(
                new CoverageType { Id = 1, Code = "FULL", Name = "Full", Description = "All-risk coverage.", IsActive = true },
                new CoverageType { Id = 2, Code = "PARTIAL", Name = "Partial", Description = "Named-perils coverage.", IsActive = true },
                new CoverageType { Id = 3, Code = "THIRD_PARTY", Name = "Third Party", Description = "Covers legal liability to third parties.", IsActive = true },
                new CoverageType { Id = 4, Code = "CATASTROPHIC", Name = "Catastrophic", Description = "Covers only extreme, large-scale loss events.", IsActive = true }
            );
        }
    }
}