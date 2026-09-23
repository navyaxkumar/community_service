namespace DigitalShield.API.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<UserProgress> UserProgress { get; set; } = new List<UserProgress>();

    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}
