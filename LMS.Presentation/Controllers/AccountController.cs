using LMS.Shared.DTOs.AuthDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
	private readonly IAuthService _authService;

	public AccountController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(UserRegistrationDto dto)
	{
		var result = await _authService.RegisterUserAsync(dto);
		return result.Succeeded ? StatusCode(201) : BadRequest(result.Errors);
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> Login(UserAuthDto dto)
	{
		if (!await _authService.ValidateUserAsync(dto))
			return Unauthorized();

		var token = await _authService.CreateTokenAsync(addTime: true);
		return Ok(token);
	}

	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh(TokenDto token)
	{
		try
		{
			var newToken = await _authService.RefreshTokenAsync(token);
			return Ok(newToken);
		}
		catch
		{
			return Unauthorized();
		}
	}
}