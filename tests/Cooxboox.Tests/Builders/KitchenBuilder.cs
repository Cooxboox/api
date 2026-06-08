using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.Identity;
using Cooxboox.Core.Kitchens;
using Krakenar.Contracts.Users;

namespace Cooxboox.Builders;

public interface IKitchenBuilder
{
  IKitchenBuilder WithId(KitchenId id);
  IKitchenBuilder WithName(Name name);
  IKitchenBuilder WithOwner(User user);

  Kitchen Build();
}

public class KitchenBuilder : IKitchenBuilder
{
  private readonly Faker _faker;

  private KitchenId? _kitchenId = null;
  private Name? _name = null;
  private UserId? _ownerId = null;

  public KitchenBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IKitchenBuilder WithId(KitchenId id)
  {
    _kitchenId = id;
    return this;
  }

  public IKitchenBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public IKitchenBuilder WithOwner(User user)
  {
    _ownerId = new UserId(user);
    return this;
  }

  public Kitchen Build()
  {
    KitchenId kitchenId = _kitchenId ?? KitchenId.NewId();
    Name name = _name ?? new(_faker.Company.CompanyName());
    UserId ownerId = _ownerId ?? UserId.NewId();

    return new Kitchen(ownerId, name, kitchenId);
  }
}
