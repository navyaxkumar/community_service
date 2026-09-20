namespace DigitalShield.API.DTOs.Progress;

public class CreateUserProgressDto
{
    public int LearningModuleId { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsCompleted { get; set; }
}
