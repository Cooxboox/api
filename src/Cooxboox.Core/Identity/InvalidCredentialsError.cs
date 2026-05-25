using Krakenar.Contracts;

namespace Cooxboox.Core.Identity;

public record InvalidCredentialsError : Error
{
  public InvalidCredentialsError() : base("InvalidCredentials", "The specified credentials did not match.")
  {
  }
}
