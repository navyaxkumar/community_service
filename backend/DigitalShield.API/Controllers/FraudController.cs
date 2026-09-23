using DigitalShield.API.DTOs.Fraud;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/fraud")]
[Authorize]
public class FraudController : ControllerBase
{
    private readonly IFraudAnalysisService _service;

    public FraudController(IFraudAnalysisService service)
    {
        _service = service;
    }

    [HttpPost("check")]
    public IActionResult Check([FromBody] FraudCheckRequestDto request)
    {
        return Ok(_service.Analyze(request));
    }
}
