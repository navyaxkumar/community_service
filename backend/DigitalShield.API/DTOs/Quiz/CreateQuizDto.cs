namespace DigitalShield.API.DTOs.Quiz;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public int? FraudCategoryId { get; set; }
    public List<CreateQuizQuestionDto> Questions { get; set; } = new();
}
