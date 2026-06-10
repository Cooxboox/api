using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.RecipeTypes;
using Cooxboox.Core.Kitchens;

namespace Cooxboox.Builders;

public interface IRecipeTypeBuilder
{
  IRecipeTypeBuilder WithId(RecipeTypeId id);
  IRecipeTypeBuilder WithKitchen(Kitchen? kitchen);
  IRecipeTypeBuilder WithName(Name name);

  RecipeType Build();
}

public class RecipeTypeBuilder : IRecipeTypeBuilder
{
  private readonly Faker _faker;

  private RecipeTypeId? _ingredientTypeId;
  private Kitchen? _kitchen;
  private Name? _name;

  public RecipeTypeBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IRecipeTypeBuilder WithId(RecipeTypeId id)
  {
    _ingredientTypeId = id;
    return this;
  }

  public IRecipeTypeBuilder WithKitchen(Kitchen? kitchen)
  {
    _kitchen = kitchen;
    return this;
  }

  public IRecipeTypeBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public RecipeType Build()
  {
    Kitchen kitchen = _kitchen ?? new KitchenBuilder(_faker).Build();
    Name name = _name ?? new("RecipeType");

    return _ingredientTypeId.HasValue
      ? new RecipeType(_ingredientTypeId.Value, name)
      : new RecipeType(kitchen, name, kitchen.OwnerId.ActorId);
  }
}
