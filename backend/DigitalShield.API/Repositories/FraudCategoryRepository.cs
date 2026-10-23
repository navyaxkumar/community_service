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

    public async Task<bool> HasAssociatedContentAsync(int id, CancellationToken cancellationToken = default)
    {
        var hasModules = await _context.LearningModules.AnyAsync(lm => lm.FraudCategoryId == id, cancellationToken);
        if (hasModules) return true;

        var hasQuizzes = await _context.Quizzes.AnyAsync(q => q.FraudCategoryId == id, cancellationToken);
        if (hasQuizzes) return true;

        var hasScenarios = await _context.Scenarios.AnyAsync(s => s.FraudCategoryId == id, cancellationToken);
        return hasScenarios;
    }

    public async Task AddAsync(FraudCategory category, CancellationToken cancellationToken = default)
    {
        await _context.FraudCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FraudCategory category, CancellationToken cancellationToken = default)
    {
        var tracked = _context.FraudCategories.Local.FirstOrDefault(e => e.Id == category.Id);
        if (tracked != null && !ReferenceEquals(tracked, category))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.FraudCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(FraudCategory category, CancellationToken cancellationToken = default)
    {
        var tracked = _context.FraudCategories.Local.FirstOrDefault(e => e.Id == category.Id);
        if (tracked != null && !ReferenceEquals(tracked, category))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.FraudCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
