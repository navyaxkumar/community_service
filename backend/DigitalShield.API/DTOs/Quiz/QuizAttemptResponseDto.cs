namespace DigitalShield.API.DTOs.Quiz;

public class QuizAttemptResponseDto
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
