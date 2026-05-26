using Claims.Application.Interfaces;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

/// <summary>
/// Implementación concreta de IClaimRepository usando Entity Framework Core.
/// 
/// ¿Por qué existe este archivo?
/// En la Fase 4 definimos la interfaz IClaimRepository (el contrato).
/// Aquí está la implementación real que sabe cómo hablar con la base de datos.
/// La lógica de negocio (ClaimService) solo conoce la interfaz, nunca esta clase directamente.
/// Esto permite cambiar la base de datos sin tocar la lógica de negocio.
/// </summary>
public class ClaimRepository : IClaimRepository
{
    private readonly ClaimsDbContext _context;

    public ClaimRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista paginada y filtrable de claims.
    /// 
    /// ¿Cómo funciona?
    /// 1. Empieza con todos los claims (IQueryable — aún no ejecuta SQL)
    /// 2. Si hay filtro de statusCode, agrega un WHERE
    /// 3. Si hay filtro de policyNumber, agrega otro WHERE
    /// 4. Cuenta el total ANTES de paginar (para el metadata de paginación)
    /// 5. Aplica Skip/Take para la paginación
    /// 6. Ejecuta el SQL con ToListAsync()
    /// </summary>
    public async Task<(List<Claim> Claims, int Total)> GetAllAsync(
        int page, int pageSize, string? statusCode, string? policyNumber)
    {

        var query = _context.Claims.AsQueryable();

        if (!string.IsNullOrWhiteSpace(statusCode))
            query = query.Where(c => c.StatusCode == statusCode);

        if (!string.IsNullOrWhiteSpace(policyNumber))
            query = query.Where(c => c.PolicyNumber.Contains(policyNumber));

        var total = await query.CountAsync();

        var claims = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (claims, total);
    }

    public async Task<Claim?> GetByIdAsync(int id)
    {
        return await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Claim?> GetByClaimNumberAsync(string claimNumber)
    {
        return await _context.Claims.FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
    }

    public async Task AddAsync(Claim claim)
    {
        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Claim claim)
    {
        _context.Claims.Update(claim);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Claim claim)
    {
        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtiene el siguiente número de secuencia para un año dado.
    /// 
    /// ¿Cómo funciona?
    /// Busca todos los claims cuyo ClaimNumber empieza con "CLM-{year}-",
    /// cuenta cuántos hay, y devuelve el siguiente número.
    /// Si no hay claims para ese año, devuelve 1.
    /// 
    /// Ejemplo: Si existen CLM-2026-00001, CLM-2026-00002, CLM-2026-00003
    ///          → Devuelve 4
    /// </summary>
    public async Task<int> GetNextSequenceNumberAsync(int year)
    {
        var prefix = $"CLM-{year}-";

        var count = await _context.Claims
            .CountAsync(c => c.ClaimNumber.StartsWith(prefix));

        return count + 1;
    }
}
