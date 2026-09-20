namespace DigitalShield.API.DTOs.Quiz;

public class CreateQuizOptionDto
{
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
}
