using DigitalShield.API.DTOs.Quiz;

namespace DigitalShield.API.Interfaces.Services;

public interface IQuizService
{
    Task<QuizDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<QuizResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<QuizResponseDto>> GetPublishedAsync(string? audience = null, CancellationToken cancellationToken = default);
    Task<List<QuizResponseDto>> GetByCategoryAsync(int fraudCategoryId, CancellationToken cancellationToken = default);
    Task<QuizResponseDto> CreateAsync(CreateQuizDto request, CancellationToken cancellationToken = default);
    Task<QuizResponseDto?> UpdateAsync(int id, UpdateQuizDto request, CancellationToken cancellationToken = default);
    Task<QuizManagementDetailDto?> GetManagementDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<QuizResponseDto?> SetPublishStatusAsync(int id, bool isPublished, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
