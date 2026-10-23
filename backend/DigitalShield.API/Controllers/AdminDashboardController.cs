using DigitalShield.API.Constants;
using DigitalShield.API.Data;
using DigitalShield.API.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Admin)]
public class AdminDashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken)
    {
        var totalModules = await _context.LearningModules.CountAsync(cancellationToken);
        var publishedModules = await _context.LearningModules.CountAsync(m => m.IsPublished, cancellationToken);
        var draftModules = totalModules - publishedModules;

        var totalQuizzes = await _context.Quizzes.CountAsync(cancellationToken);
        var publishedQuizzes = await _context.Quizzes.CountAsync(q => q.IsPublished, cancellationToken);
        var draftQuizzes = totalQuizzes - publishedQuizzes;
        var totalQuizQuestions = await _context.QuizQuestions.CountAsync(cancellationToken);

        var totalScenarios = await _context.Scenarios.CountAsync(cancellationToken);
        var publishedScenarios = await _context.Scenarios.CountAsync(s => s.IsPublished, cancellationToken);
        var draftScenarios = totalScenarios - publishedScenarios;

        var totalCategories = await _context.FraudCategories.CountAsync(cancellationToken);
        var activeCategories = await _context.FraudCategories.CountAsync(c => c.IsActive, cancellationToken);
        var inactiveCategories = totalCategories - activeCategories;

        var totalBadges = await _context.Badges.CountAsync(cancellationToken);
        var activeBadges = await _context.Badges.CountAsync(b => b.IsActive, cancellationToken);
        var inactiveBadges = totalBadges - activeBadges;

        var totalUsers = await _context.Users.CountAsync(cancellationToken);
        var totalQuizAttempts = await _context.QuizAttempts.CountAsync(cancellationToken);

        var stats = new AdminDashboardStatsDto
        {
            TotalModules = totalModules,
            PublishedModules = publishedModules,
            DraftModules = draftModules,

            TotalQuizzes = totalQuizzes,
            PublishedQuizzes = publishedQuizzes,
            DraftQuizzes = draftQuizzes,
            TotalQuizQuestions = totalQuizQuestions,

            TotalScenarios = totalScenarios,
            PublishedScenarios = publishedScenarios,
            DraftScenarios = draftScenarios,

            TotalCategories = totalCategories,
            ActiveCategories = activeCategories,
            InactiveCategories = inactiveCategories,

            TotalBadges = totalBadges,
            ActiveBadges = activeBadges,
            InactiveBadges = inactiveBadges,

            TotalUsers = totalUsers,
            TotalQuizAttempts = totalQuizAttempts
        };

        return Ok(stats);
    }
}
