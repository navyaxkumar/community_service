namespace DigitalShield.API.DTOs.Quiz;

public class QuizQuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Order { get; set; }
    public List<QuizOptionDto> Options { get; set; } = new();
}
