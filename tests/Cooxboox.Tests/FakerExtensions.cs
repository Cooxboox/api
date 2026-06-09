using Bogus;
using Cooxboox.Core.Localization;

namespace Cooxboox;

public static class FakerExtensions
{
  public static Language Language(this Faker faker) => faker.PickRandom(Core.Localization.Language.All.ToArray());
}
