using Bogus;
using Cooxboox.Core.Kitchens;
using Krakenar.Contracts.Users;

namespace Cooxboox.Builders;

public interface IKitchenBuilder
{
  IKitchenBuilder WithEntityId(Guid id);
  IKitchenBuilder WithOwner(User? owner);
  IKitchenBuilder WithConfidentiality(Confidentiality confidentiality);
  IKitchenBuilder WithName(string name);
  IKitchenBuilder WithSlug(string? slug);
  IKitchenBuilder WithNotes(string? notes);

  Kitchen Build();
}

public class KitchenBuilder : IKitchenBuilder
{
  private readonly Faker _faker;

  private Confidentiality _confidentiality = default;
  private Guid _entityId = Guid.NewGuid();
  private string? _name = null;
  private string? _notes = null;
  private User? _owner = null;
  private string? _slug = null;

  public KitchenBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IKitchenBuilder WithEntityId(Guid entityId)
  {
    _entityId = entityId;
    return this;
  }

  public IKitchenBuilder WithOwner(User? owner)
  {
    _owner = owner;
    return this;
  }

  public IKitchenBuilder WithConfidentiality(Confidentiality confidentiality)
  {
    _confidentiality = confidentiality;
    return this;
  }

  public IKitchenBuilder WithName(string name)
  {
    _name = name;
    return this;
  }

  public IKitchenBuilder WithSlug(string? slug)
  {
    _slug = slug;
    return this;
  }

  public IKitchenBuilder WithNotes(string? notes)
  {
    _notes = notes;
    return this;
  }

  public Kitchen Build()
  {
    User owner = _owner ?? new UserBuilder(_faker).Build();
    string name = _name ?? _faker.Company.CompanyName();

    return new Kitchen(owner.Id, name, _entityId, _confidentiality, _slug, _notes);
  }
}
