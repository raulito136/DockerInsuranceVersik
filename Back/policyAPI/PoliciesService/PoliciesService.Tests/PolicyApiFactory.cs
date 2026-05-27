// PoliciesService\PoliciesService.Tests\PolicyApiFactory.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PoliciesService.Api;
using PoliciesService.Application.Interfaces;
using PoliciesService.Infrastructure;

namespace PoliciesService.Tests
{
    public class PolicyApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();
        public Mock<IReferenceDataClient> ReferenceDataClientMock { get; } = new();
        private const string URL = "http://localhost";

        public PolicyApiFactory()
        {
            Environment.SetEnvironmentVariable("Services__ReferenceData", URL);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("Services:ReferenceData", URL);
            builder.ConfigureServices(services =>
            {
                // Remove existing AppDbContext options
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(DbContextOptions));
                services.RemoveAll(typeof(System.Data.Common.DbConnection));
                    
                // Add in-memory database
                services.AddScoped<DbContextOptions<AppDbContext>>(provider =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                    optionsBuilder.UseInMemoryDatabase(_dbName);
                    return optionsBuilder.Options;
                });

                // Remove existing Refit client and inject mock
                services.RemoveAll(typeof(IReferenceDataClient));
                services.AddSingleton(ReferenceDataClientMock.Object);

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure the database is created.
                db.Database.EnsureCreated();
            });
        }
    }
}
