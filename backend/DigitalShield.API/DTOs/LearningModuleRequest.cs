namespace DigitalShield.API.DTOs;

public class LearningModuleRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public int? FraudCategoryId { get; set; }
}
