using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReferenceData.Domain;

namespace ReferenceData.Infrastructure.Persistence.Configurations
{
    public class PolicyTypeConfiguration : IEntityTypeConfiguration<PolicyType>
    {
        public void Configure(EntityTypeBuilder<PolicyType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(
                new PolicyType { Id = 1, Code = "AVIATION", Name = "Aviation", Description = "Insurance for aircraft, airlines, airports, and aviation-related liability.", IsActive = true },
                new PolicyType { Id = 2, Code = "ENERGY", Name = "Energy", Description = "Insurance for oil rigs, wind farms, refineries, pipelines, etc.", IsActive = true },
                new PolicyType { Id = 3, Code = "MARINE", Name = "Marine", Description = "Insurance for ships, cargo, ports, and waterways.", IsActive = true },
                new PolicyType { Id = 4, Code = "CYBER", Name = "Cyber", Description = "Insurance against data breaches and ransomware attacks.", IsActive = true },
                new PolicyType { Id = 5, Code = "LIABILITY", Name = "Liability", Description = "General commercial liability insurance.", IsActive = true }
            );
        }
    }
}