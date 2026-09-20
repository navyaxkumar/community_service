using DigitalShield.API.Constants;
using DigitalShield.API.DTOs.FraudCategory;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/fraud-categories")]
public class FraudCategoriesController : ControllerBase
{
    private readonly IFraudCategoryService _service;

    public FraudCategoriesController(IFraudCategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _service.GetActiveAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _service.GetByIdAsync(id, cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateFraudCategoryDto request, CancellationToken cancellationToken)
    {
        var category = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFraudCategoryDto request, CancellationToken cancellationToken)
    {
        var category = await _service.UpdateAsync(id, request, cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }
}
