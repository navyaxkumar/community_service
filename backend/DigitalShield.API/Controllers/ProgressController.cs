using DigitalShield.API.Authorization;
using DigitalShield.API.DTOs.Progress;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProgressService _service;

    public ProgressController(ICurrentUserService currentUser, IUserProgressService service)
    {
        _currentUser = currentUser;
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserProgress(CancellationToken cancellationToken)
    {
        return !TryGetCurrentUserId(out var userId)
            ? Unauthorized()
            : Ok(await _service.GetByUserAsync(userId, cancellationToken));
    }

    [HttpGet("me/{learningModuleId:int}")]
    public async Task<IActionResult> GetCurrentModuleProgress(int learningModuleId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var progress = await _service.GetByUserAndModuleAsync(userId, learningModuleId, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserProgressDto request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var progress = await _service.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetCurrentModuleProgress), new { learningModuleId = progress.LearningModuleId }, progress);
    }

    [HttpPut("me/{learningModuleId:int}")]
    public async Task<IActionResult> Update(int learningModuleId, [FromBody] UpdateUserProgressDto request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var progress = await _service.UpdateAsync(userId, learningModuleId, request, cancellationToken);
        return progress is null ? NotFound() : Ok(progress);
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        userId = _currentUser.UserId ?? 0;
        return userId > 0;
    }
}
