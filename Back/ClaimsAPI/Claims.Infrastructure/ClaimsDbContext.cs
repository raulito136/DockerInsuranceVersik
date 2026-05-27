using Microsoft.EntityFrameworkCore;
using Claims.Domain;

namespace Claims.Infrastructure;

public class ClaimsDbContext : DbContext
{
    public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Claim> Claims { get; set; }
    public DbSet<ClaimComment> ClaimComments { get; set; }
    public DbSet<ClaimAudit> ClaimAudits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("claims");

        modelBuilder.ApplyConfiguration(new ClaimConfiguration());
        modelBuilder.ApplyConfiguration(new ClaimCommentConfiguration());
        modelBuilder.ApplyConfiguration(new ClaimAuditConfiguration());
    }
}
