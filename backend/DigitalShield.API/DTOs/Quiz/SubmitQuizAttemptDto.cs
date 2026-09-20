namespace DigitalShield.API.DTOs.Quiz;

public class SubmitQuizAttemptDto
{
    public int QuizId { get; set; }
    public List<QuizAnswerDto> Answers { get; set; } = new();
}

public class QuizAnswerDto
{
    public int QuestionId { get; set; }
    public int OptionId { get; set; }
}
