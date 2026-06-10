using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Claims.Infrastructure;
using Claims.Application.Interfaces;
using Claims.Application.Services;

var builder = WebApplication.CreateBuilder(args);

Console.Title = "Claims API";

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddDbContext<ClaimsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClaimsDb")));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IClaimCommentService, ClaimCommentService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMFE", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowMFE");
app.MapControllers();

app.Run();
