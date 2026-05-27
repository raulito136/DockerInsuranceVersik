using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReferenceData.Domain;

namespace ReferenceData.Infrastructure.Persistence.Configurations
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(
                new Region { Id = 1, Code = "NSW", Name = "New South Wales", IsActive = true },
                new Region { Id = 2, Code = "VIC", Name = "Victoria", IsActive = true },
                new Region { Id = 3, Code = "QLD", Name = "Queensland", IsActive = true },
                new Region { Id = 4, Code = "WA", Name = "Western Australia", IsActive = true },
                new Region { Id = 5, Code = "SA", Name = "South Australia", IsActive = true },
                new Region { Id = 6, Code = "TAS", Name = "Tasmania", IsActive = true },
                new Region { Id = 7, Code = "ACT", Name = "Australian Capital Territory", IsActive = true },
                new Region { Id = 8, Code = "NT", Name = "Northern Territory", IsActive = true }
            );
        }
    }
}