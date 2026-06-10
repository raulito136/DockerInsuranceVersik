using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using ReferenceData.Infrastructure;
using ReferenceData.Infrastructure.Persistence;

namespace ReferenceData.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMFE", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddMvc();

            builder.Services.AddControllers();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.MapControllers();
            app.UseCors("AllowMFE");

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetRequiredService<ReferenceDataDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
public partial class Program { }