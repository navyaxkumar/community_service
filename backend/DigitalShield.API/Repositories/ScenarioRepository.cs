using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class ScenarioRepository : IScenarioRepository
{
    private readonly ApplicationDbContext _context;

    public ScenarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Scenario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Scenarios
            .AsNoTracking()
            .Include(s => s.FraudCategory)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Scenario>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Scenarios
            .AsNoTracking()
            .Include(s => s.FraudCategory)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Scenario>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Scenarios
            .AsNoTracking()
            .Include(s => s.FraudCategory)
            .Where(s => s.IsPublished)
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Scenario>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Scenarios
            .AsNoTracking()
            .Include(s => s.FraudCategory)
            .Where(s => s.FraudCategoryId == fraudCategoryId)
            .OrderBy(s => s.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Scenario scenario, CancellationToken cancellationToken = default)
    {
        await _context.Scenarios.AddAsync(scenario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Scenario scenario, CancellationToken cancellationToken = default)
    {
        var tracked = _context.Scenarios.Local.FirstOrDefault(e => e.Id == scenario.Id);
        if (tracked != null && !ReferenceEquals(tracked, scenario))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Scenarios.Update(scenario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Scenario scenario, CancellationToken cancellationToken = default)
    {
        var tracked = _context.Scenarios.Local.FirstOrDefault(e => e.Id == scenario.Id);
        if (tracked != null && !ReferenceEquals(tracked, scenario))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Scenarios.Remove(scenario);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
