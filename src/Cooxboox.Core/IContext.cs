using Krakenar.Contracts;

namespace Cooxboox.Core;

public interface IContext
{
  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();
}
