using Claims.Application.Interfaces;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Repositories;

/// <summary>
/// Implementación de IClaimCommentRepository usando EF Core.
/// CRUD básico de comentarios vinculados a un claim.
/// </summary>
public class ClaimCommentRepository : IClaimCommentRepository
{
    private readonly ClaimsDbContext _context;

    public ClaimCommentRepository(ClaimsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos los comentarios de un claim, ordenados del más reciente al más antiguo.
    /// </summary>
    public async Task<List<ClaimComment>> GetByClaimIdAsync(int claimId)
    {
        return await _context.ClaimComments
            .Where(cc => cc.ClaimId == claimId)
            .OrderByDescending(cc => cc.CreatedAt)
            .ToListAsync();
    }

    public async Task<ClaimComment?> GetByIdAsync(int id)
    {
        return await _context.ClaimComments.FirstOrDefaultAsync(cc => cc.Id == id);
    }

    public async Task AddAsync(ClaimComment comment)
    {
        _context.ClaimComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ClaimComment comment)
    {
        _context.ClaimComments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}
