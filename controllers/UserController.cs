using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity.user;

[ApiController]
[Route("[Controller]")]
public class UserController : ControllerBase
{

  private readonly UserService UserService;
  public UserController(UserService userService)
  {
    UserService = userService;
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser(RegisterUserControllerDto input)
  {
    User user = new()
    {
      ExternalId = input.ExternalId,
      UserName = input.ExternalId,
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

}

