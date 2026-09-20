using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public QuizAttemptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAttempt?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Include(qa => qa.Quiz)
            .Include(qa => qa.User)
            .FirstOrDefaultAsync(qa => qa.Id == id, cancellationToken);
    }

    public async Task<List<QuizAttempt>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<QuizAttempt>> GetByQuizAsync(int quizId, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Include(qa => qa.User)
            .Where(qa => qa.QuizId == quizId)
            .OrderByDescending(qa => qa.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(QuizAttempt attempt, CancellationToken cancellationToken = default)
    {
        await _context.QuizAttempts.AddAsync(attempt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
