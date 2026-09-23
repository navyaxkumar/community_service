using DigitalShield.API.Authorization;
using DigitalShield.API.DTOs.User;
using DigitalShield.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserService _userService;

    public UsersController(ICurrentUserService currentUser, IUserService userService)
    {
        _currentUser = currentUser;
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var user = await _userService.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
        {
            return Unauthorized();
        }

        var user = await _userService.UpdateAsync(_currentUser.UserId.Value, request, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }
}
