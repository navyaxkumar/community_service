namespace DigitalShield.API.Models;

public class LearningModule
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int? FraudCategoryId { get; set; }
    public FraudCategory? FraudCategory { get; set; }

    public ICollection<UserProgress> UserProgress { get; set; } = new List<UserProgress>();
}
