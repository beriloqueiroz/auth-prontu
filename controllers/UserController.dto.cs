using System.ComponentModel.DataAnnotations;

namespace identity.user;

public class RegisterUserControllerDto
{
  [Required]
  public required string Email { get; set; }
  [Required]
  [DataType(DataType.Password)]
  public required string Password { get; set; }
  [Required]
  [Display(Name = "Re-type Password")]
  [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
  public required string RePassword { get; set; }

  [Required]
  public required string ExternalId { get; set; }
}

public record LoginUserControllerDto
(
  [Required]
  string Username,
  [Required]
  [DataType(DataType.Password)]
  string Password)
{ }

public record LoginEmailUserControllerDto
(
  [Required]
  string Email,
  [Required]
  [DataType(DataType.Password)]
  string Password)
{ }