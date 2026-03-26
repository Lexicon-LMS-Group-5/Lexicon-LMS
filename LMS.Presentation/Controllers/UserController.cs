using LMS.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace LMS.Presentation.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public UserController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var user = await serviceManager.UserService.GetCurrentUserAsync(User, ct);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var users = await serviceManager.UserService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    [HttpPut("{id:string}")]
    public async Task<IActionResult> UpdateUser(
        string id,
        [FromBody] UserUpsertDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        //Add checks to see if user is you or you are a teacher

        var updated = await serviceManager.UserService.UpdateUserAsync(id, dto, ct);

        return Ok(updated);
    }
}