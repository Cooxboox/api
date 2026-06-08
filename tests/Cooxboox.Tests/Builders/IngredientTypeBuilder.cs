using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.Kitchens;

namespace Cooxboox.Builders;

public interface IIngredientTypeBuilder
{
  IIngredientTypeBuilder WithId(IngredientTypeId id);
  IIngredientTypeBuilder WithKitchen(Kitchen? kitchen);
  IIngredientTypeBuilder WithName(Name name);

  IngredientType Build();
}

public class IngredientTypeBuilder : IIngredientTypeBuilder
{
  private readonly Faker _faker;

  private IngredientTypeId? _ingredientTypeId;
  private Kitchen? _kitchen;
  private Name? _name;

  public IngredientTypeBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IIngredientTypeBuilder WithId(IngredientTypeId id)
  {
    _ingredientTypeId = id;
    return this;
  }

  public IIngredientTypeBuilder WithKitchen(Kitchen? kitchen)
  {
    _kitchen = kitchen;
    return this;
  }

  public IIngredientTypeBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public IngredientType Build()
  {
    Kitchen kitchen = _kitchen ?? new KitchenBuilder(_faker).Build();
    Name name = _name ?? new("IngredientType");

    return _ingredientTypeId.HasValue
      ? new IngredientType(_ingredientTypeId.Value, name)
      : new IngredientType(kitchen, name, kitchen.OwnerId.ActorId);
  }
}
