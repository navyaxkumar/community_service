namespace DigitalShield.API.DTOs.Quiz;

public class QuizDetailDto : QuizResponseDto
{
    public List<QuizQuestionForUserDto> Questions { get; set; } = new();
}
