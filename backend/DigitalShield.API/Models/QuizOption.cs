namespace DigitalShield.API.Models;

public class QuizOption
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int Order { get; set; }

    public int QuizQuestionId { get; set; }
    public QuizQuestion QuizQuestion { get; set; } = null!;
}
