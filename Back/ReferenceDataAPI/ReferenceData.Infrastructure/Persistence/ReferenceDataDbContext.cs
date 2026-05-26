using Microsoft.EntityFrameworkCore;
using ReferenceData.Domain;
using ReferenceData.Infrastructure.Persistence.Configurations;


namespace ReferenceData.Infrastructure.Persistence
{
    public class ReferenceDataDbContext(DbContextOptions<ReferenceDataDbContext> options) : DbContext(options)
    {
        public DbSet<PolicyType> PolicyTypes => Set<PolicyType>();
        public DbSet<CoverageType> CoverageTypes => Set<CoverageType>();
        public DbSet<ClaimStatus> ClaimStatuses => Set<ClaimStatus>();
        public DbSet<Region> Regions => Set<Region>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("reference-data");

            modelBuilder.Entity<ClaimStatus>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<CoverageType>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<PolicyType>().HasQueryFilter(x => x.IsActive);
            modelBuilder.Entity<Region>().HasQueryFilter(x => x.IsActive);
            modelBuilder.ApplyConfiguration(new PolicyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CoverageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClaimStatusConfiguration());
            modelBuilder.ApplyConfiguration(new RegionConfiguration());
        }
    }
}