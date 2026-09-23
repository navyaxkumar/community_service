using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Scenario;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/scenarios")]
public class ScenariosController : ControllerBase
{
    private readonly IScenarioService _service;

    public ScenariosController(IScenarioService service)
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
        var scenario = await _service.GetByIdAsync(id, cancellationToken);
        return scenario is null ? NotFound() : Ok(scenario);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateScenarioDto request, CancellationToken cancellationToken)
    {
        var scenario = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = scenario.Id }, scenario);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScenarioDto request, CancellationToken cancellationToken)
    {
        var scenario = await _service.UpdateAsync(id, request, cancellationToken);
        return scenario is null ? NotFound() : Ok(scenario);
    }
}
