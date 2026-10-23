using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Common;
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
    public async Task<IActionResult> GetPublished([FromQuery] string? audience, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPublishedAsync(audience, cancellationToken));
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

    [HttpGet("admin/all")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAllAdmin(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(cancellationToken));
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetByIdAdmin(int id, CancellationToken cancellationToken)
    {
        var quiz = await _service.GetManagementDetailsAsync(id, cancellationToken);
        return quiz is null ? NotFound() : Ok(quiz);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateQuizDto request, CancellationToken cancellationToken)
    {
        try
        {
            var quiz = await _service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto request, CancellationToken cancellationToken)
    {
        try
        {
            var quiz = await _service.UpdateAsync(id, request, cancellationToken);
            return quiz is null ? NotFound() : Ok(quiz);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/publish")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetPublishStatus(int id, [FromBody] PublishStatusRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var quiz = await _service.SetPublishStatusAsync(id, request.IsPublished, cancellationToken);
            return quiz is null ? NotFound() : Ok(quiz);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
