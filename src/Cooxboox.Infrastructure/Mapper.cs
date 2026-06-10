using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.RecipeCategories.Models;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Core.RecipeTypes.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public IngredientModel ToIngredient(IngredientEntity source)
  {
    IngredientModel destination = new()
    {
      Id = source.EntityId,
      Name = source.Name,
      Notes = source.Notes,
      Status = source.Status,
      PublishedVersion = source.PublishedVersion,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    foreach (IngredientLocaleEntity locale in source.Locales)
    {
      destination.Locales.Add(ToIngredientLocale(locale));
    }

    MapAggregate(source, destination);

    return destination;
  }
  private IngredientLocaleModel ToIngredientLocale(IngredientLocaleEntity source) => new()
  {
    Language = new Locale(source.Language),
    Name = source.Name,
    Slug = source.Slug,
    MetaDescription = source.MetaDescription,
    HtmlContent = source.HtmlContent,
    Notes = source.Notes,
    Version = source.Version,
    CreatedBy = FindActor(source.CreatedBy),
    CreatedOn = source.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(source.UpdatedBy),
    UpdatedOn = source.UpdatedOn.AsUniversalTime(),
    Status = source.Status,
    PublishedVersion = source.PublishedVersion,
    PublishedBy = TryGetActor(source.PublishedBy),
    PublishedOn = source.PublishedOn?.AsUniversalTime()
  };

  public IngredientCategoryModel ToIngredientCategory(IngredientCategoryEntity source)
  {
    IngredientCategoryModel destination = new()
    {
      Id = source.EntityId,
      Name = source.Name,
      Notes = source.Notes,
      Status = source.Status,
      PublishedVersion = source.PublishedVersion,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    foreach (IngredientCategoryLocaleEntity locale in source.Locales)
    {
      destination.Locales.Add(ToIngredientCategoryLocale(locale));
    }

    MapAggregate(source, destination);

    return destination;
  }
  private IngredientCategoryLocaleModel ToIngredientCategoryLocale(IngredientCategoryLocaleEntity source) => new()
  {
    Language = new Locale(source.Language),
    Name = source.Name,
    Slug = source.Slug,
    MetaDescription = source.MetaDescription,
    HtmlContent = source.HtmlContent,
    Notes = source.Notes,
    Version = source.Version,
    CreatedBy = FindActor(source.CreatedBy),
    CreatedOn = source.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(source.UpdatedBy),
    UpdatedOn = source.UpdatedOn.AsUniversalTime(),
    Status = source.Status,
    PublishedVersion = source.PublishedVersion,
    PublishedBy = TryGetActor(source.PublishedBy),
    PublishedOn = source.PublishedOn?.AsUniversalTime()
  };

  public IngredientTypeModel ToIngredientType(IngredientTypeEntity source)
  {
    IngredientTypeModel destination = new()
    {
      Id = source.EntityId,
      Name = source.Name,
      Notes = source.Notes,
      Status = source.Status,
      PublishedVersion = source.PublishedVersion,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    foreach (IngredientTypeLocaleEntity locale in source.Locales)
    {
      destination.Locales.Add(ToIngredientTypeLocale(locale));
    }

    MapAggregate(source, destination);

    return destination;
  }
  private IngredientTypeLocaleModel ToIngredientTypeLocale(IngredientTypeLocaleEntity source) => new()
  {
    Language = new Locale(source.Language),
    Name = source.Name,
    Slug = source.Slug,
    MetaDescription = source.MetaDescription,
    HtmlContent = source.HtmlContent,
    Notes = source.Notes,
    Version = source.Version,
    CreatedBy = FindActor(source.CreatedBy),
    CreatedOn = source.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(source.UpdatedBy),
    UpdatedOn = source.UpdatedOn.AsUniversalTime(),
    Status = source.Status,
    PublishedVersion = source.PublishedVersion,
    PublishedBy = TryGetActor(source.PublishedBy),
    PublishedOn = source.PublishedOn?.AsUniversalTime()
  };

  public RecipeModel ToRecipe(RecipeEntity source)
  {
    RecipeModel destination = new()
    {
      Id = source.EntityId,
      Name = source.Name,
      Notes = source.Notes,
      Status = source.Status,
      PublishedVersion = source.PublishedVersion,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    foreach (RecipeLocaleEntity locale in source.Locales)
    {
      destination.Locales.Add(ToRecipeLocale(locale));
    }

    MapAggregate(source, destination);

    return destination;
  }
  private RecipeLocaleModel ToRecipeLocale(RecipeLocaleEntity source) => new()
  {
    Language = new Locale(source.Language),
    Name = source.Name,
    Slug = source.Slug,
    MetaDescription = source.MetaDescription,
    HtmlContent = source.HtmlContent,
    Notes = source.Notes,
    Version = source.Version,
    CreatedBy = FindActor(source.CreatedBy),
    CreatedOn = source.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(source.UpdatedBy),
    UpdatedOn = source.UpdatedOn.AsUniversalTime(),
    Status = source.Status,
    PublishedVersion = source.PublishedVersion,
    PublishedBy = TryGetActor(source.PublishedBy),
    PublishedOn = source.PublishedOn?.AsUniversalTime()
  };

  public RecipeCategoryModel ToRecipeCategory(RecipeCategoryEntity source)
  {
    RecipeCategoryModel destination = new()
    {
      Id = source.EntityId,
      Name = source.Name,
      Notes = source.Notes,
      Status = source.Status,
      PublishedVersion = source.PublishedVersion,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    foreach (RecipeCategoryLocaleEntity locale in source.Locales)
    {
      destination.Locales.Add(ToRecipeCategoryLocale(locale));
    }

    MapAggregate(source, destination);

    return destination;
  }
  private RecipeCategoryLocaleModel ToRecipeCategoryLocale(RecipeCategoryLocaleEntity source) => new()
  {
    Language = new Locale(source.Language),
    Name = source.Name,
    Slug = source.Slug,
    MetaDescription = source.MetaDescription,
    HtmlContent = source.HtmlContent,
    Notes = source.Notes,
    Version = source.Version,
    CreatedBy = FindActor(source.CreatedBy),
    CreatedOn = source.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(source.UpdatedBy),
    UpdatedOn = source.UpdatedOn.AsUniversalTime(),
    Status = source.Status,
    PublishedVersion = source.PublishedVersion,
    PublishedBy = TryGetActor(source.PublishedBy),
    PublishedOn = source.PublishedOn?.AsUniversalTime()
  };

  public RecipeTypeModel ToRecipeType(RecipeTypeEntity source)
  {
    RecipeTypeModel destination = new()
    {
      Id = source.EntityId,
      Name = source.Name,
      Notes = source.Notes,
      Status = source.Status,
      PublishedVersion = source.PublishedVersion,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    foreach (RecipeTypeLocaleEntity locale in source.Locales)
    {
      destination.Locales.Add(ToRecipeTypeLocale(locale));
    }

    MapAggregate(source, destination);

    return destination;
  }
  private RecipeTypeLocaleModel ToRecipeTypeLocale(RecipeTypeLocaleEntity source) => new()
  {
    Language = new Locale(source.Language),
    Name = source.Name,
    Slug = source.Slug,
    MetaDescription = source.MetaDescription,
    HtmlContent = source.HtmlContent,
    Notes = source.Notes,
    Version = source.Version,
    CreatedBy = FindActor(source.CreatedBy),
    CreatedOn = source.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(source.UpdatedBy),
    UpdatedOn = source.UpdatedOn.AsUniversalTime(),
    Status = source.Status,
    PublishedVersion = source.PublishedVersion,
    PublishedBy = TryGetActor(source.PublishedBy),
    PublishedOn = source.PublishedOn?.AsUniversalTime()
  };

  public KitchenModel ToKitchen(KitchenEntity source)
  {
    KitchenModel destination = new()
    {
      Id = source.EntityId,
      Owner = FindActor(source.OwnerId),
      Confidentiality = source.Confidentiality,
      Name = source.Name,
      Slug = source.Slug,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? actorId) => FindActor(actorId is null ? null : new ActorId(actorId));
  private Actor FindActor(ActorId? actorId) => TryGetActor(actorId) ?? _system;
  private Actor? TryGetActor(string? actorId) => TryGetActor(actorId is null ? null : new ActorId(actorId));
  private Actor? TryGetActor(ActorId? actorId) => actorId.HasValue && _actors.TryGetValue(actorId.Value, out Actor? actor) ? actor : null;
}
