using Microsoft.AspNetCore.Identity;

namespace identity.user;

public class UserService
{
  private readonly UserManager<User> UserManager;
  private readonly SignInManager<User> SignInManager;

  private readonly TokenService TokenService;

  public UserService(UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService)
  {
    UserManager = userManager;
    SignInManager = signInManager;
    TokenService = tokenService;
  }

  public async Task Register(User user, string password)
  {
    var userResult = await UserManager.CreateAsync(user, password);

    if (!userResult.Succeeded)
    {
      throw new ApplicationException("Erro ao cadastrar usuário!" + userResult.Errors.Select(err => $"Code: {err.Code}, desctription: {err.Description}"));
    }

    string token = await UserManager.GenerateEmailConfirmationTokenAsync(user);

    //TODO send email confirmation
  }

  public async Task Confirm(User user, string token)
  {
    var userResult = await UserManager.ConfirmEmailAsync(user, token);

    if (!userResult.Succeeded)
    {
      throw new ApplicationException("Erro ao confirmar!");
    }
  }

  public async Task<string> Login(string username, string password)
  {
    var userResult = await SignInManager.PasswordSignInAsync(username, password, false, false);

    if (!userResult.Succeeded)
    {
      throw new ApplicationException("Erro ao entrar!");
    }

    User? user = SignInManager.UserManager.Users.FirstOrDefault(u => u.NormalizedUserName == username.ToUpper());

    if (user == null)
    {
      throw new ApplicationException("Erro ao obter token!");
    }

    return TokenService.Generate(user);
  }

  public async Task<string> LoginWithEmail(string email, string password)
  {
    var user = await SignInManager.UserManager.FindByEmailAsync(email);

    if (user == null)
    {
      throw new ApplicationException("Erro ao validar/autenticar!");
    }

    var userResult = await SignInManager.PasswordSignInAsync(user, password, false, false);

    if (!userResult.Succeeded)
    {
      throw new ApplicationException("Erro ao validar/autenticar!");
    }

    return TokenService.Generate(user);
  }

  public Task Logout(string[] authTokens)
  {
    TokenService.RemoveAuthTokens(authTokens);
    return Task.CompletedTask;
  }

  public async Task ChangePassword(string id, string newPassword, string currentPassword)
  {
    User? user = await UserManager.FindByIdAsync(id);
    if (user == null)
    {
      throw new ApplicationException("Erro ao encontrar usuário!");
    }
    await UserManager.ChangePasswordAsync(user, currentPassword, newPassword);

    string[] authTokens = user.AuthTokens.Select(at => at.Value).ToArray();
    TokenService.RemoveAuthTokens(authTokens);
  }

}