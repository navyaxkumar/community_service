namespace DigitalShield.API.Models;

public class QuizQuestion
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Order { get; set; }

    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();
}
