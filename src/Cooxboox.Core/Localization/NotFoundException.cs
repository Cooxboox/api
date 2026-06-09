using Krakenar.Contracts;

namespace Cooxboox.Core.Localization;

public abstract class NotFoundException : ErrorException
{
  protected NotFoundException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
