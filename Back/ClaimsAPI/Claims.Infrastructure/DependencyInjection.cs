using Claims.Application.Interfaces;
using Claims.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Claims.Infrastructure;

/// <summary>
/// Método de extensión que centraliza TODO el registro de dependencias de la capa Infrastructure.
/// 
/// ¿Por qué un archivo separado?
/// En vez de llenar Program.cs con decenas de líneas de registro, creamos un método de extensión
/// que encapsula toda la configuración. Así Program.cs queda limpio:
///     builder.Services.AddInfrastructure(builder.Configuration);
/// 
/// ¿Qué registra?
/// 1. Repositorios — Las implementaciones concretas de las interfaces de datos
/// 2. Clientes Refit — Los clientes HTTP tipados para hablar con otros microservicios
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<IClaimCommentRepository, ClaimCommentRepository>();
        services.AddScoped<IClaimAuditRepository, ClaimAuditRepository>();

        services
            .AddRefitClient<IPoliciesClient>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration["Services:Policies"]!));

        services
            .AddRefitClient<IReferenceDataClient>()
            .ConfigureHttpClient(c =>
                c.BaseAddress = new Uri(configuration["Services:ReferenceData"]!));

        return services;
    }
}
