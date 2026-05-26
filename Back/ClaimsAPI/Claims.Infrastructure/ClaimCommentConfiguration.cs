using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Claims.Domain;

namespace Claims.Infrastructure;

public class ClaimCommentConfiguration : IEntityTypeConfiguration<ClaimComment>
{
    public void Configure(EntityTypeBuilder<ClaimComment> builder)
    {
        builder.HasOne<Claim>()
            .WithMany()
            .HasForeignKey(cc => cc.ClaimId);

        builder.Property(cc => cc.AuthorName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(cc => cc.Comment)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(cc => cc.CreatedAt)
            .IsRequired();
    }
}
