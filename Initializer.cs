
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace identity.user;

public static class Initializer
{
  public static void InjectIdentity(this IServiceCollection services, IConfiguration Configuration)
  {
    services.AddDbContext<UserDbContext>(options => options.UseNpgsql(Configuration["ConnectionStrings:UserConnection"]));

    services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new()
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SymmetricSecurityKey"] ?? "0xa3fa6d97AaAz7e145b37451fc344e58c")),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
      };
    });

    services.AddScoped<UserService>();
    services.AddScoped<TokenService>();


    var db = services.BuildServiceProvider().GetRequiredService<UserDbContext>();
    db.Database.CanConnectAsync().ContinueWith(_ => db.Database.Migrate());

  }
}