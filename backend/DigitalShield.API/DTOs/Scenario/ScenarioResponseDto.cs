namespace DigitalShield.API.DTOs.Scenario;

public class ScenarioResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Situation { get; set; } = string.Empty;
    public string CorrectAction { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public int? FraudCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
