using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.Badge;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/badges")]
[Authorize]
public class BadgesController : ControllerBase
{
    private readonly IBadgeService _service;

    public BadgesController(IBadgeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetActiveAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var badge = await _service.GetByIdAsync(id, cancellationToken);
        return badge is null ? NotFound() : Ok(badge);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateBadgeDto request, CancellationToken cancellationToken)
    {
        var badge = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = badge.Id }, badge);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBadgeDto request, CancellationToken cancellationToken)
    {
        var badge = await _service.UpdateAsync(id, request, cancellationToken);
        return badge is null ? NotFound() : Ok(badge);
    }
}
