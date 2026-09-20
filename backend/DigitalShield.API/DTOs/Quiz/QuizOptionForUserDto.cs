namespace DigitalShield.API.DTOs.Quiz;

public class QuizOptionForUserDto
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int Order { get; set; }
}
