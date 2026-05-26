using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Claims.Domain;

namespace Claims.Infrastructure;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.Property(c => c.ClaimNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(c => c.ClaimNumber).IsUnique();

        builder.Property(c => c.PolicyNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.ClaimDate)
            .IsRequired();

        builder.Property(c => c.StatusCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired();
    }
}
