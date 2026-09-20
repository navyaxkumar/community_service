using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.Interfaces.Repositories;
using DigitalShield.API.Interfaces.Services;
using DigitalShield.API.Models;

namespace DigitalShield.API.Services;

public class UserProgressService : IUserProgressService
{
    private readonly ILearningModuleRepository _moduleRepository;
    private readonly IUserProgressRepository _repository;

    public UserProgressService(ILearningModuleRepository moduleRepository, IUserProgressRepository repository)
    {
        _moduleRepository = moduleRepository;
        _repository = repository;
    }

    public async Task<List<UserProgressResponseDto>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var progress = await _repository.GetByUserAsync(userId, cancellationToken);
        return progress.Select(MapToResponse).ToList();
    }

    public async Task<UserProgressResponseDto?> GetByUserAndModuleAsync(int userId, int learningModuleId, CancellationToken cancellationToken = default)
    {
        var progress = await _repository.GetByUserAndModuleAsync(userId, learningModuleId, cancellationToken);
        return progress is null ? null : MapToResponse(progress);
    }

    public async Task<UserProgressResponseDto> CreateAsync(int userId, CreateUserProgressDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _moduleRepository.GetByIdAsync(request.LearningModuleId, cancellationToken) is null)
        {
            throw new InvalidOperationException("Learning module was not found.");
        }

        if (await _repository.ExistsAsync(userId, request.LearningModuleId, cancellationToken))
        {
            throw new InvalidOperationException("Progress for this user and learning module already exists.");
        }

        var progress = new UserProgress
        {
            UserId = userId,
            LearningModuleId = request.LearningModuleId,
            ProgressPercentage = request.ProgressPercentage,
            IsCompleted = request.ProgressPercentage == 100,
            StartedAt = DateTime.UtcNow,
            CompletedAt = request.ProgressPercentage == 100 ? DateTime.UtcNow : null
        };

        await _repository.AddAsync(progress, cancellationToken);
        return MapToResponse(progress);
    }

    public async Task<UserProgressResponseDto?> UpdateAsync(int userId, int learningModuleId, UpdateUserProgressDto request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var progress = await _repository.GetByUserAndModuleAsync(userId, learningModuleId, cancellationToken);
        if (progress is null)
        {
            return null;
        }

        progress.ProgressPercentage = request.ProgressPercentage;
        progress.IsCompleted = request.ProgressPercentage == 100;
        progress.CompletedAt = progress.IsCompleted ? progress.CompletedAt ?? DateTime.UtcNow : null;

        await _repository.UpdateAsync(progress, cancellationToken);
        return MapToResponse(progress);
    }

    private static UserProgressResponseDto MapToResponse(UserProgress progress)
    {
        return new UserProgressResponseDto
        {
            Id = progress.Id,
            LearningModuleId = progress.LearningModuleId,
            IsCompleted = progress.IsCompleted,
            ProgressPercentage = progress.ProgressPercentage,
            StartedAt = progress.StartedAt,
            CompletedAt = progress.CompletedAt
        };
    }
}
