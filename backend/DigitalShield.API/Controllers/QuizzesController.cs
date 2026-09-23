using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Quiz;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/quizzes")]
public class QuizzesController : ControllerBase
{
    private readonly IQuizService _service;

    public QuizzesController(IQuizService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPublished(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPublishedAsync(cancellationToken));
    }

    [HttpGet("category/{categoryId:int}")]
    [Authorize]
    public async Task<IActionResult> GetByCategory(int categoryId, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetByCategoryAsync(categoryId, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var quiz = await _service.GetByIdAsync(id, cancellationToken);
        return quiz is null ? NotFound() : Ok(quiz);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateQuizDto request, CancellationToken cancellationToken)
    {
        var quiz = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto request, CancellationToken cancellationToken)
    {
        var quiz = await _service.UpdateAsync(id, request, cancellationToken);
        return quiz is null ? NotFound() : Ok(quiz);
    }
}
