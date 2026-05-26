using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PoliciesService.Domain;

namespace PoliciesService.Infrastructure
{
    public class AppDbContext : DbContext
    {
        #region Attributes
        public DbSet<PolicyHolder> PolicyHolders { get; set; }
        public DbSet<Policy> Policies { get; set; }
        #endregion

        #region Constructors
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        #endregion

        #region Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("policy");

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        #endregion
    }
}
