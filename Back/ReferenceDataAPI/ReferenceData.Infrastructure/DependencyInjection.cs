using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReferenceData.Application.Interfaces;
using ReferenceData.Application.Services;
using ReferenceData.Infrastructure.Persistence.Repositories;
using ReferenceData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ReferenceData.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ReferenceDataDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ReferenceDataDbContext).Assembly.FullName)));

            services.AddScoped<IPolicyTypeRepository, PolicyTypeRepository>();
            services.AddScoped<ICoverageTypeRepository, CoverageTypeRepository>();
            services.AddScoped<IClaimStatusRepository, ClaimStatusRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();

            services.AddScoped<PolicyTypeService>();
            services.AddScoped<CoverageTypeService>();
            services.AddScoped<ClaimStatusService>();
            services.AddScoped<RegionService>();

            return services;
        }
    }
}
