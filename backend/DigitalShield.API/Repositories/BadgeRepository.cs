using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class BadgeRepository : IBadgeRepository
{
    private readonly ApplicationDbContext _context;

    public BadgeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Badge?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Badges
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<List<Badge>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Badges
            .AsNoTracking()
            .Where(b => b.IsActive)
            .OrderBy(b => b.RequiredPoints)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Badge badge, CancellationToken cancellationToken = default)
    {
        await _context.Badges.AddAsync(badge, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Badge badge, CancellationToken cancellationToken = default)
    {
        _context.Badges.Update(badge);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
