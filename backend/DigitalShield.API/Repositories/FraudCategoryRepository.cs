using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class FraudCategoryRepository : IFraudCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public FraudCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<FraudCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.FraudCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(fc => fc.Id == id, cancellationToken);
    }

    public async Task<List<FraudCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FraudCategories
            .AsNoTracking()
            .OrderBy(fc => fc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FraudCategory>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FraudCategories
            .AsNoTracking()
            .Where(fc => fc.IsActive)
            .OrderBy(fc => fc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FraudCategory category, CancellationToken cancellationToken = default)
    {
        await _context.FraudCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FraudCategory category, CancellationToken cancellationToken = default)
    {
        _context.FraudCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
