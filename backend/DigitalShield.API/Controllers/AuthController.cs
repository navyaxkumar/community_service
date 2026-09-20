using DigitalShield.API.DTOs;
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
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage ?? "Invalid credentials." });
        }

        return Ok(new
        {
            token = result.Token,
            userName = result.UserName,
            message = "Login successful."
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage ?? "Registration failed." });
        }

        return Ok(new
        {
            token = result.Token,
            userName = result.UserName,
            message = "Registration successful."
        });
    }
}
