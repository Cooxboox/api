using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.Kitchens;

namespace Cooxboox.Builders;

public interface IRecipeCategoryBuilder
{
  IRecipeCategoryBuilder WithId(RecipeCategoryId id);
  IRecipeCategoryBuilder WithKitchen(Kitchen? kitchen);
  IRecipeCategoryBuilder WithName(Name name);

  RecipeCategory Build();
}

public class RecipeCategoryBuilder : IRecipeCategoryBuilder
{
  private readonly Faker _faker;

  private RecipeCategoryId? _recipeCategoryId;
  private Kitchen? _kitchen;
  private Name? _name;

  public RecipeCategoryBuilder(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public IRecipeCategoryBuilder WithId(RecipeCategoryId id)
  {
    _recipeCategoryId = id;
    return this;
  }

  public IRecipeCategoryBuilder WithKitchen(Kitchen? kitchen)
  {
    _kitchen = kitchen;
    return this;
  }

  public IRecipeCategoryBuilder WithName(Name name)
  {
    _name = name;
    return this;
  }

  public RecipeCategory Build()
  {
    Kitchen kitchen = _kitchen ?? new KitchenBuilder(_faker).Build();
    Name name = _name ?? new("RecipeCategory");

    return _recipeCategoryId.HasValue
      ? new RecipeCategory(_recipeCategoryId.Value, name)
      : new RecipeCategory(kitchen, name, kitchen.OwnerId.ActorId);
  }
}
