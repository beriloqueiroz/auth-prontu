using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace identity.user;

public class TokenService
{
  private readonly IConfiguration Configuration;
  public TokenService(IConfiguration configuration)
  {
    Configuration = configuration;
  }
  public string Generate(User user)
  {

    var claims = new Claim[]
    {
        new("username", user.UserName ?? ""),
        new("id", user.Id),
        new("externalId", user.ExternalId)
    };

    var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SymmetricSecurityKey"] ?? "0xa3fa6d97AaAz7e145b37451fc344e58c"));

    var signingCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken
        (
        expires: DateTime.Now.AddDays(15),
        claims: claims,
        signingCredentials: signingCredentials
        );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}