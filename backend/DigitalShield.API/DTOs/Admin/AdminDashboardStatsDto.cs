namespace DigitalShield.API.DTOs.Admin;

public class AdminDashboardStatsDto
{
    // Learning Modules
    public int TotalModules { get; set; }
    public int PublishedModules { get; set; }
    public int DraftModules { get; set; }

    // Quizzes
    public int TotalQuizzes { get; set; }
    public int PublishedQuizzes { get; set; }
    public int DraftQuizzes { get; set; }
    public int TotalQuizQuestions { get; set; }

    // Scenarios
    public int TotalScenarios { get; set; }
    public int PublishedScenarios { get; set; }
    public int DraftScenarios { get; set; }

    // Threat Categories
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
    public int InactiveCategories { get; set; }

    // Badges
    public int TotalBadges { get; set; }
    public int ActiveBadges { get; set; }
    public int InactiveBadges { get; set; }

    // User Activity
    public int TotalUsers { get; set; }
    public int TotalQuizAttempts { get; set; }
}
