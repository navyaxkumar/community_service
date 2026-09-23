namespace DigitalShield.API.DTOs.Quiz;

public class UpdateQuizQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Order { get; set; }
    public List<CreateQuizOptionDto> Options { get; set; } = new();
}
