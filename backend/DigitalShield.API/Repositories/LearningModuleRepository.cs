using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class LearningModuleRepository : ILearningModuleRepository
{
    private readonly ApplicationDbContext _context;

    public LearningModuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LearningModule?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LearningModules
            .AsNoTracking()
            .Include(lm => lm.FraudCategory)
            .FirstOrDefaultAsync(lm => lm.Id == id, cancellationToken);
    }

    public async Task<List<LearningModule>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LearningModules
            .AsNoTracking()
            .Include(lm => lm.FraudCategory)
            .Where(lm => lm.IsPublished)
            .OrderBy(lm => lm.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LearningModule>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default)
    {
        return await _context.LearningModules
            .AsNoTracking()
            .Include(lm => lm.FraudCategory)
            .Where(lm => lm.FraudCategoryId == fraudCategoryId)
            .OrderBy(lm => lm.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(LearningModule learningModule, CancellationToken cancellationToken = default)
    {
        await _context.LearningModules.AddAsync(learningModule, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(LearningModule learningModule, CancellationToken cancellationToken = default)
    {
        _context.LearningModules.Update(learningModule);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
