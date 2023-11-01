using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity.user;

[ApiController]
[Route("[Controller]")]
public class UserController : ControllerBase
{

  private readonly UserService UserService;
  private readonly IAuthorizationService AuthorizationService;
  public UserController(UserService userService, IAuthorizationService authorizationService)
  {
    AuthorizationService = authorizationService;
    UserService = userService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser(RegisterUserControllerDto input)
  {
    User user = new()
    {
      UserName = input.Username,
      Email = input.Email
    };

    await UserService.Register(user, input.Password);

    return Ok("Usuário criado com sucesso!");
  }

  [HttpPost("login")]
  public async Task<IActionResult> LoginUser(LoginUserControllerDto input)
  {
    var token = await UserService.Login(input.Username, input.Password);

    return Ok(token);
  }

  [HttpPost("login/email")]
  public async Task<IActionResult> LoginUserWithEmail(LoginEmailUserControllerDto input)
  {
    var token = await UserService.LoginWithEmail(input.Email, input.Password);

    return Ok(token);
  }

  [HttpGet("authorization")]
  [Authorize]
  public IActionResult IsAuthorized()
  {
    return Ok("Acesso permitido!");
  }

  [HttpGet("confirm")]
  public async Task<IActionResult> EmailConfirmationAsync(string id, string token)
  {
    await UserService.Confirm(id, token);
    return Ok("Usuário confirmado!");
  }

  [HttpDelete("logout")]
  [Authorize]
  public async Task<IActionResult> Logout()
  {
    var authTokenClaim = User.FindFirst(claim => claim.Type == "authToken")?.Value;
    if (authTokenClaim == null)
    {
      return Ok();
    }
    await UserService.Logout(new[] { authTokenClaim });
    return Ok();
  }

  [HttpPut("change-password")]
  [Authorize]
  public async Task<IActionResult> ChangePassword(ChangePasswordControllerDto input)
  {
    var id = User.FindFirst(claim => claim.Type == "id")?.Value;
    if (id == null)
    {
      return NotFound();
    }
    var username = User.FindFirst(claim => claim.Type == "username")?.Value;
    if (username == null)
    {
      return NotFound();
    }
    await UserService.ChangePassword(id, input.NewPassword, input.OldPassword);
    return Ok();
  }

}

