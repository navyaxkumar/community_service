namespace DigitalShield.API.Models;

public class FraudCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LearningModule> LearningModules { get; set; } = new List<LearningModule>();
    public ICollection<Scenario> Scenarios { get; set; } = new List<Scenario>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
