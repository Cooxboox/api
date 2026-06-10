using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.Kitchens;

namespace Cooxboox.Builders;

public interface IIngredientCategoryBuilder
{
  IIngredientCategoryBuilder WithId(IngredientCategoryId id);
  IIngredientCategoryBuilder WithKitchen(Kitchen? kitchen);
  IIngredientCategoryBuilder WithName(Name name);

  IngredientCategory Build();
}

public class IngredientCategoryBuilder : IIngredientCategoryBuilder
{
  private readonly Faker _faker;

  private IngredientCategoryId? _ingredientCategoryId;
  private Kitchen? _kitchen;
  private Name? _name;

  public IngredientCategoryBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IIngredientCategoryBuilder WithId(IngredientCategoryId id)
  {
    _ingredientCategoryId = id;
    return this;
  }

  public IIngredientCategoryBuilder WithKitchen(Kitchen? kitchen)
  {
    _kitchen = kitchen;
    return this;
  }

  public IIngredientCategoryBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public IngredientCategory Build()
  {
    Kitchen kitchen = _kitchen ?? new KitchenBuilder(_faker).Build();
    Name name = _name ?? new("IngredientCategory");

    return _ingredientCategoryId.HasValue
      ? new IngredientCategory(_ingredientCategoryId.Value, name)
      : new IngredientCategory(kitchen, name, kitchen.OwnerId.ActorId);
  }
}
