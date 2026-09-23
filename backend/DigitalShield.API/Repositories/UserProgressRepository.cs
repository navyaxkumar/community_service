using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class UserProgressRepository : IUserProgressRepository
{
    private readonly ApplicationDbContext _context;

    public UserProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProgress?> GetByUserAndModuleAsync(int userId, int learningModuleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProgress
            .AsNoTracking()
            .Include(up => up.LearningModule)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.LearningModuleId == learningModuleId, cancellationToken);
    }

    public async Task<List<UserProgress>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProgress
            .AsNoTracking()
            .Include(up => up.LearningModule)
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int userId, int learningModuleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProgress
            .AsNoTracking()
            .AnyAsync(up => up.UserId == userId && up.LearningModuleId == learningModuleId, cancellationToken);
    }

    public async Task AddAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        await _context.UserProgress.AddAsync(progress, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        _context.UserProgress.Update(progress);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
