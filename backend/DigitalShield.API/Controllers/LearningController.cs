using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningController : ControllerBase
{
    private readonly ILearningModuleService _learningService;

    public LearningController(ILearningModuleService learningService)
    {
        _learningService = learningService;
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetPublishedModules(CancellationToken cancellationToken)
    {
        var modules = await _learningService.GetPublishedAsync(cancellationToken);
        return Ok(modules);
    }

    [HttpGet("modules/category/{fraudCategoryId:int}")]
    public async Task<IActionResult> GetModulesByCategory(int fraudCategoryId, CancellationToken cancellationToken)
    {
        var modules = await _learningService.GetByCategoryAsync(fraudCategoryId, cancellationToken);
        return Ok(modules);
    }

    [HttpGet("modules/{id:int}")]
    public async Task<IActionResult> GetModuleById(int id, CancellationToken cancellationToken)
    {
        var module = await _learningService.GetByIdAsync(id, cancellationToken);
        if (module is null)
        {
            return NotFound();
        }

        return Ok(module);
    }

    [Authorize]
    [HttpPost("modules")]
    public async Task<IActionResult> CreateModule([FromBody] CreateLearningModuleDto request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _learningService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetModuleById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("modules/{id:int}")]
    public async Task<IActionResult> UpdateModule(int id, [FromBody] UpdateLearningModuleDto request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _learningService.UpdateAsync(id, request, cancellationToken);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
