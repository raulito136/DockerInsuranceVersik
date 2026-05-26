using Claims.Application.Interfaces;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

/// <summary>
/// Implementación de IClaimAuditRepository usando EF Core.
/// Solo lectura + inserción — los registros de auditoría NUNCA se modifican ni eliminan.
/// 
/// ¿Para qué sirve la auditoría?
/// Cada vez que alguien cambia un campo de un claim (status, amount, etc.),
/// se registra quién lo hizo, qué campo cambió, el valor anterior y el nuevo.
/// Es un historial inmutable de todos los cambios.
/// </summary>
public class ClaimAuditRepository : IClaimAuditRepository
{
    private readonly ClaimsDbContext _context;

    public ClaimAuditRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos los registros de auditoría de un claim, del más reciente al más antiguo.
    /// </summary>
    public async Task<List<ClaimAudit>> GetByClaimIdAsync(int claimId)
    {
        return await _context.ClaimAudits
            .Where(ca => ca.ClaimId == claimId)
            .OrderByDescending(ca => ca.ChangedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ClaimAudit audit)
    {
        _context.ClaimAudits.Add(audit);
        await _context.SaveChangesAsync();
    }
}
