namespace DigitalShield.API.DTOs.Quiz;

public class QuizManagementDetailDto : QuizResponseDto
{
    public List<QuizQuestionDto> Questions { get; set; } = new();
}
