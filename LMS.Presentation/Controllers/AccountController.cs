using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LMS.Presentation;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{

	//private readonly SignInManager<ApplicationUser> _signInManager;
	//private readonly UserManager<ApplicationUser> _userManager;

	//public AccountController(
	//	SignInManager<ApplicationUser> signInManager,
	//	UserManager<ApplicationUser> userManager)
	//{
	//	_signInManager = signInManager;
	//	_userManager = userManager;
	//}

	//[HttpPost("login")] 
	//public async Task<IActionResult> Login(LoginModel model)
	//{
	//	var user = await _userManager.FindByEmailAsync(model.Email);
	//	if (user == null)
	//		return Unauthorized("Invalid login credentials");

	//	var result = await _signInManager.PasswordSignInAsync(
	//		user.UserName,
	//		model.Password,
	//		isPersistent: true,
	//		lockoutOnFailure: false);

	//	if (result.Succeeded)
	//		return Ok("Logged in");

	//	return Unauthorized("Invalid login credentials");
	//}

	//[HttpPost("logout")]
	//public async Task<IActionResult> Logout()
	//{
	//	await _signInManager.SignOutAsync();
	//	return Ok("Logged out");
	//}
}