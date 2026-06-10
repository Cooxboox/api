using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Recipes;

namespace Cooxboox.Builders;

public interface IRecipeBuilder
{
  IRecipeBuilder WithId(RecipeId id);
  IRecipeBuilder WithKitchen(Kitchen? kitchen);
  IRecipeBuilder WithName(Name name);

  Recipe Build();
}

public class RecipeBuilder : IRecipeBuilder
{
  private readonly Faker _faker;

  private RecipeId? _recipeId;
  private Kitchen? _kitchen;
  private Name? _name;

  public RecipeBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IRecipeBuilder WithId(RecipeId id)
  {
    _recipeId = id;
    return this;
  }

  public IRecipeBuilder WithKitchen(Kitchen? kitchen)
  {
    _kitchen = kitchen;
    return this;
  }

  public IRecipeBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public Recipe Build()
  {
    Kitchen kitchen = _kitchen ?? new KitchenBuilder(_faker).Build();
    Name name = _name ?? new("Recipe");

    return _recipeId.HasValue
      ? new Recipe(_recipeId.Value, name)
      : new Recipe(kitchen, name, kitchen.OwnerId.ActorId);
  }
}
