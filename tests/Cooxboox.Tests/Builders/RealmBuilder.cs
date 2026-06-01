using Bogus;
using Krakenar.Contracts.Realms;

namespace Cooxboox.Builders;

public interface IRealmBuilder
{
  Realm Build();
}

public class RealmBuilder : IRealmBuilder
{
  private readonly Faker _faker;

  public RealmBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public Realm Build()
  {
    Realm realm = new()
    {
      Id = Guid.NewGuid(),
      Version = 1,
      UniqueSlug = "cooxboox",
      DisplayName = "Cooxboox",
      RequireUniqueEmail = true,
      RequireConfirmedAccount = true
    };
    realm.CreatedOn = realm.UpdatedOn = realm.SecretChangedOn = DateTime.UtcNow;
    return realm;
  }
}
