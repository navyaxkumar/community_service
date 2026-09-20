using DigitalShield.API.Constants;
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
        var module = await _service.GetByIdAsync(id, cancellationToken);
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
}
