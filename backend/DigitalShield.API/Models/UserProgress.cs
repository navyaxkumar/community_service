namespace DigitalShield.API.Models;

public class UserProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LearningModuleId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public LearningModule LearningModule { get; set; } = null!;
}
