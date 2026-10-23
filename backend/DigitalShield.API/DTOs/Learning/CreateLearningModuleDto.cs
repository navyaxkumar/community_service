namespace DigitalShield.API.DTOs.Learning;

public class CreateLearningModuleDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public int? FraudCategoryId { get; set; }

    public string? TargetAudience { get; set; }
    public string? DifficultyLevel { get; set; }
    public int? EstimatedMinutes { get; set; }
    public string? LearningObjectives { get; set; }
    public string? SafetyTips { get; set; }
}
