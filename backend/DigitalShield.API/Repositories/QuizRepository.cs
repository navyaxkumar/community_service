using DigitalShield.API.Data;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;

    public QuizRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.FraudCategory)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<Quiz?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.FraudCategory)
            .Include(q => q.Questions)
                .ThenInclude(question => question.Options)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<List<Quiz>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.FraudCategory)
            .Include(q => q.Questions)
                .ThenInclude(question => question.Options)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Quiz>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.FraudCategory)
            .Where(q => q.IsPublished)
            .OrderBy(q => q.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Quiz>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.FraudCategory)
            .Where(q => q.FraudCategoryId == fraudCategoryId)
            .OrderBy(q => q.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasQuizAttemptsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizAttempts.AnyAsync(qa => qa.QuizId == id, cancellationToken);
    }

    public async Task AddAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        await _context.Quizzes.AddAsync(quiz, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        var tracked = _context.Quizzes.Local.FirstOrDefault(e => e.Id == quiz.Id);
        if (tracked != null && !ReferenceEquals(tracked, quiz))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Quiz quiz, CancellationToken cancellationToken = default)
    {
        var tracked = _context.Quizzes.Local.FirstOrDefault(e => e.Id == quiz.Id);
        if (tracked != null && !ReferenceEquals(tracked, quiz))
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
