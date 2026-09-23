namespace DigitalShield.API.DTOs.Progress;

public class UserProgressResponseDto
{
    public int Id { get; set; }
    public int LearningModuleId { get; set; }
    public bool IsCompleted { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
