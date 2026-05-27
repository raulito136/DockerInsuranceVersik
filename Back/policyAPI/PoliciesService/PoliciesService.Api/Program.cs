
using Microsoft.EntityFrameworkCore;
using PoliciesService.Application.Interfaces;
using PoliciesService.Application.Repositories;
using PoliciesService.Application.Services;
using PoliciesService.Infrastructure;
using PoliciesService.Infrastructure.Repositories;
using Refit;
using Scalar.AspNetCore;

namespace PoliciesService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.Title = "Policies API";

            // Policy name for CORS
            var CORSRules = "_corsRules";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: CORSRules,
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200", "http://localhost:4201", "http://localhost:4202", "http://localhost:4203")
                            .AllowAnyHeader()  // Allows any header (Authorization or Content-Type)
                            .AllowAnyMethod(); // Allows any API REST method (GET, POST, etc)
                    });
            });

            // Add services to the container.
            builder.Services.AddControllers();

            // Add API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
            }).AddMvc();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Get connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Register the DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            // Register Repositories
            builder.Services.AddScoped<IPolicyHolderRepository, PolicyHolderRepository>();
            builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();

            // Register Services
            builder.Services.AddScoped<IPolicyHolderService, PolicyHolderService>();
            builder.Services.AddScoped<IPolicyService, PolicyService>();

            // Get URL of Reference Data API
            var referenceDataUrl = builder.Configuration["Services:ReferenceData"];

            // Register Refit
            builder.Services.AddRefitClient<IReferenceDataClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(referenceDataUrl!));

            var app = builder.Build();

            // Apply database migrations automatically
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // Activate the CORS middleware
            app.UseCors(CORSRules);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
