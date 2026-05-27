using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Claims.Domain;

namespace Claims.Infrastructure;

public class ClaimAuditConfiguration : IEntityTypeConfiguration<ClaimAudit>
{
    public void Configure(EntityTypeBuilder<ClaimAudit> builder)
    {
        builder.HasOne<Claim>()
            .WithMany()
            .HasForeignKey(ca => ca.ClaimId);

        builder.Property(ca => ca.ChangedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ca => ca.FieldChanged)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ca => ca.OldValue)
            .HasMaxLength(500);

        builder.Property(ca => ca.NewValue)
            .HasMaxLength(500);

        builder.Property(ca => ca.ChangedAt)
            .IsRequired();
    }
}
