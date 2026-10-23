using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Common;
using DigitalShield.API.DTOs.Learning;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/learning-modules")]
public class LearningModulesController : ControllerBase
{
    private readonly ILearningModuleService _service;

    public LearningModulesController(ILearningModuleService service)
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
        var module = await _service.GetByIdAsync(id, cancellationToken);
        return module is null ? NotFound() : Ok(module);
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
        var module = await _service.GetAdminByIdAsync(id, cancellationToken);
        return module is null ? NotFound() : Ok(module);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateLearningModuleDto request, CancellationToken cancellationToken)
    {
        var module = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = module.Id }, module);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLearningModuleDto request, CancellationToken cancellationToken)
    {
        var module = await _service.UpdateAsync(id, request, cancellationToken);
        return module is null ? NotFound() : Ok(module);
    }

    [HttpPatch("{id:int}/publish")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetPublishStatus(int id, [FromBody] PublishStatusRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var module = await _service.SetPublishStatusAsync(id, request.IsPublished, cancellationToken);
            return module is null ? NotFound() : Ok(module);
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
