using LMS.Shared;
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var user = await _serviceManager.UserService.GetCurrentUserAsync(userId, ct);

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
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<UserReadDto>> UpdateUser(
        string id,
        [FromBody] UserUpdateDto dto,
        CancellationToken ct)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
            return Unauthorized();

        var request = new UpdateUserContext
        {
            CurrentUserId = userId,
            IsTeacher = User.IsInRole("Teacher")
        };

        try
        {
            var updated = await _serviceManager.UserService.UpdateUserAsync(request, id, dto, ct);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("Not allowed to update this user");
        }
    }

    [HttpPost]
    [Authorize(Roles = Roles.Teacher)]
    public async Task<ActionResult<UserReadDto>> CreateUser(
    [FromBody] UserCreateDto dto,
    CancellationToken ct)
    {
        try
        {
            var created = await _serviceManager.UserService.CreateUserAsync(dto, ct);

            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created); ;
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteUserById(string id, CancellationToken ct)
    {
        var deleted = await _serviceManager.UserService.DeleteUserByIdAsync(id, ct);

        if (!deleted) return NotFound();

        return NoContent();
    }
}