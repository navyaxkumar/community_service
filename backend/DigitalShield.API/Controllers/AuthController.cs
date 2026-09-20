using DigitalShield.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var isValid = await _authService.ValidateCredentialsAsync(username, password);

        if (!isValid)
        {
            return BadRequest("Invalid credentials.");
        }

        return Ok(new { message = "Authentication placeholder ready." });
    }
}
