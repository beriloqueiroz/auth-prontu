using System.Net;
using Microsoft.AspNetCore.Identity;

namespace identity.user;

public class UserService
{
  private readonly UserManager<User> UserManager;
  private readonly SignInManager<User> SignInManager;

  private readonly IEmailSender EmailSender;

  private readonly TokenService TokenService;

  private readonly string UrlEmailConfirmation = "http://localhost:5000/User/confirm";

  public UserService(UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService, IEmailSender emailSender)
  {
    UserManager = userManager;
    SignInManager = signInManager;
    TokenService = tokenService;
    EmailSender = emailSender;
  }

  public async Task Register(User user, string password)
  {
    var userResult = await UserManager.CreateAsync(user, password);

    if (!userResult.Succeeded)
    {
      throw new ApplicationException("Erro ao cadastrar usuário!" + userResult.Errors.Select(err => $"Code: {err.Code}, desctription: {err.Description}"));
    }

    string token = await UserManager.GenerateEmailConfirmationTokenAsync(user);

    if (user.Email == null || user.Id == null)
    {
      throw new ApplicationException("Erro ao enviar email, email nulo!");

    }
    await EmailSender.SendEmailAsync(user.Email, "Prontu - Email de confirmação", $"Confirme seu email no link {UrlEmailConfirmation}?token={WebUtility.UrlEncode(token)}&id={WebUtility.UrlEncode(user.Id)}");
  }

  public async Task Confirm(string id, string token)
  {
    User? user = await UserManager.FindByIdAsync(id);
    if (user == null)
    {
      throw new ApplicationException("Erro ao encontrar usuário!");
    }
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