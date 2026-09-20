namespace DigitalShield.API.DTOs.Scenario;

public class UpdateScenarioDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Situation { get; set; } = string.Empty;
    public string CorrectAction { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public int? FraudCategoryId { get; set; }
}
