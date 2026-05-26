using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PoliciesService.Domain;

namespace PoliciesService.Infrastructure.Configuration
{
    public class PolicyHolderConfiguration : IEntityTypeConfiguration<PolicyHolder>
    {
        public void Configure(EntityTypeBuilder<PolicyHolder> builder)
        {
            builder.HasKey(ph => ph.Id);

            builder.Property(ph => ph.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ph => ph.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ph => ph.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(ph => ph.Email)
                .IsUnique();

            builder.Property(ph => ph.Phone)
                .HasMaxLength(20);

            builder.Property(ph => ph.RegionCode)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}
