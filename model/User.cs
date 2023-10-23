namespace identity.user;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Index(nameof(ExternalId))]
public class User : IdentityUser
{
  public User() : base()
  {
  }
  public required string ExternalId { get; set; }
}