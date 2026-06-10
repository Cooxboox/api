using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Kitchens;

namespace Cooxboox.Builders;

public interface IIngredientBuilder
{
  IIngredientBuilder WithId(IngredientId id);
  IIngredientBuilder WithKitchen(Kitchen? kitchen);
  IIngredientBuilder WithName(Name name);

  Ingredient Build();
}

public class IngredientBuilder : IIngredientBuilder
{
  private readonly Faker _faker;

  private IngredientId? _ingredientId;
  private Kitchen? _kitchen;
  private Name? _name;

  public IngredientBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IIngredientBuilder WithId(IngredientId id)
  {
    _ingredientId = id;
    return this;
  }

  public IIngredientBuilder WithKitchen(Kitchen? kitchen)
  {
    _kitchen = kitchen;
    return this;
  }

  public IIngredientBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public Ingredient Build()
  {
    Kitchen kitchen = _kitchen ?? new KitchenBuilder(_faker).Build();
    Name name = _name ?? new("Ingredient");

    return _ingredientId.HasValue
      ? new Ingredient(_ingredientId.Value, name)
      : new Ingredient(kitchen, name, kitchen.OwnerId.ActorId);
  }
}
