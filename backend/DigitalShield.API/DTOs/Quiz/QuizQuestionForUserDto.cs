namespace DigitalShield.API.DTOs.Quiz;

public class QuizQuestionForUserDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Order { get; set; }
    public List<QuizOptionForUserDto> Options { get; set; } = new();
}
