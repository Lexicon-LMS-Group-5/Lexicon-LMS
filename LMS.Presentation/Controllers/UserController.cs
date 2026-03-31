using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System.Security.Claims;

namespace LMS.Presentation.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController(IServiceManager serviceManager) : ControllerBase
{
    private readonly IServiceManager _serviceManager = serviceManager;

    [HttpGet("me")]
    public async Task<ActionResult<UserReadDto>> GetCurrentUser(CancellationToken ct)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            return Unauthorized();

        var currentUserId = claim.Value;

        var user = await _serviceManager.UserService.GetCurrentUserAsync(currentUserId, ct);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers(CancellationToken ct)
    {
        var users = await _serviceManager.UserService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDto>> GetUserById(string id, CancellationToken ct)
    {
        var user = await _serviceManager.UserService.GetUserbyIdAsync(id, ct);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("edit/{id}")]
    public async Task<ActionResult<UserReadDto>> UpdateUser(
        string id,
        [FromBody] UserUpsertDto dto,
        CancellationToken ct)
    {

        var request = new UpdateUserRequest
        {
            CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
            IsTeacher = User.IsInRole("Teacher")
        };

        try
        {
            var updated = await _serviceManager.UserService.UpdateUserAsync(request, id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}