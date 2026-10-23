namespace DigitalShield.API.DTOs.Learning;

public class LearningModuleResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public int? FraudCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string TargetAudience { get; set; } = "All";
    public string DifficultyLevel { get; set; } = "Beginner";
    public int EstimatedMinutes { get; set; } = 5;
    public string? LearningObjectives { get; set; }
    public string? SafetyTips { get; set; }
}
