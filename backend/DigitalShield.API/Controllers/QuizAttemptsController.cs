using DigitalShield.API.Authorization;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/quiz-attempts")]
[Authorize]
public class QuizAttemptsController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IQuizAttemptService _service;

    public QuizAttemptsController(ICurrentUserService currentUser, IQuizAttemptService service)
    {
        _currentUser = currentUser;
        _service = service;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserAttempts(CancellationToken cancellationToken)
    {
        return !_currentUser.UserId.HasValue
            ? Unauthorized()
            : Ok(await _service.GetByUserAsync(_currentUser.UserId.Value, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var attempt = await _service.GetByIdAsync(id, cancellationToken);
        return attempt is null ? NotFound() : Ok(attempt);
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitQuizAttemptDto request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var attempt = await _service.SubmitAttemptAsync(_currentUser.UserId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = attempt.Id }, attempt);
    }
}
