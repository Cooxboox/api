using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes;
using Cooxboox.Core.Recipes.Models;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Recipes;

[Trait(Traits.Category, Categories.Integration)]
public class RecipeIntegrationTests : IntegrationTests
{
  private readonly IRecipeRepository _recipeRepository;
  private readonly IRecipeService _recipeService;

  private Recipe _recipe = null!;

  public RecipeIntegrationTests() : base()
  {
    _recipeRepository = ServiceProvider.GetRequiredService<IRecipeRepository>();
    _recipeService = ServiceProvider.GetRequiredService<IRecipeService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _recipe = new RecipeBuilder(Faker).WithKitchen(Context.Kitchen).Build();
    await _recipeRepository.SaveAsync(_recipe);
  }

  [Theory(DisplayName = "It should create a new recipe.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceRecipePayload payload = new()
    {
      Name = " Brochettes de poulet citronnées au BBQ ",
      Notes = "  Les brochettes de poulet citronnées au BBQ sont un plat principal savoureux, parfait pour un repas d’été au barbecue.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceRecipeResult result = await _recipeService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    RecipeModel recipe = result.Recipe;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, recipe.Id);
    }
    Assert.Equal(2, recipe.Version);
    Assert.Equal(Actor, recipe.CreatedBy);
    Assert.Equal(DateTime.UtcNow, recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(recipe.CreatedBy, recipe.UpdatedBy);
    Assert.True(recipe.CreatedOn < recipe.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), recipe.Name);
    Assert.Equal(payload.Notes.Trim(), recipe.Notes);
  }

  [Theory(DisplayName = "It should publish a recipe.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_Recipe_When_Publish_Then_Published(bool publishInvariant, bool publishLocale)
  {
    Language language = Faker.Language();
    _recipe.SetLocale(language, new RecipeLocale(_recipe.Name, null, null, null, null), Actor.ToActorId());
    await _recipeRepository.SaveAsync(_recipe);

    RecipeModel? recipe = null;
    long version = _recipe.Version;
    if (publishInvariant && publishLocale)
    {
      recipe = await _recipeService.PublishAllAsync(_recipe.Entity.Id);
      version += 2;
    }
    else if (publishInvariant)
    {
      recipe = await _recipeService.PublishAsync(_recipe.Entity.Id);
      version++;
    }
    else if (publishLocale)
    {
      recipe = await _recipeService.PublishAsync(_recipe.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(recipe);

    Assert.Equal(_recipe.Entity.Id, recipe.Id);
    Assert.Equal(version, recipe.Version);
    Assert.Equal(_recipe.CreatedBy, recipe.CreatedBy.ToActorId());
    Assert.Equal(_recipe.CreatedOn.AsUniversalTime(), recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipe.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipe.UpdatedOn, TimeSpan.FromSeconds(10));

    if (publishInvariant)
    {
      Assert.Equal(ContentStatus.Latest, recipe.Status);
      Assert.Equal(recipe.Version - (publishLocale ? 1 : 0), recipe.PublishedVersion);
      Assert.Equal(Actor, recipe.PublishedBy);
      Assert.True(recipe.PublishedOn.HasValue);
      Assert.Equal(DateTime.UtcNow, recipe.PublishedOn.Value, TimeSpan.FromSeconds(10));
    }
    else
    {
      Assert.Equal(ContentStatus.Unpublished, recipe.Status);
      Assert.Null(recipe.PublishedVersion);
      Assert.Null(recipe.PublishedBy);
      Assert.Null(recipe.PublishedOn);
    }

    RecipeLocaleModel locale = Assert.Single(recipe.Locales);
    if (publishLocale)
    {
      Assert.Equal(ContentStatus.Latest, locale.Status);
      Assert.Equal(locale.Version, locale.PublishedVersion);
      Assert.Equal(Actor, locale.PublishedBy);
      Assert.True(locale.PublishedOn.HasValue);
      Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
    }
    else
    {
      Assert.Equal(ContentStatus.Unpublished, locale.Status);
      Assert.Null(locale.PublishedVersion);
      Assert.Null(locale.PublishedBy);
      Assert.Null(locale.PublishedOn);
    }
  }

  [Theory(DisplayName = "It should unpublish a recipe.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_Recipe_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _recipe.SetLocale(language, new RecipeLocale(_recipe.Name, null, null, null, null), Actor.ToActorId());
    _recipe.Publish(Actor.ToActorId());
    await _recipeRepository.SaveAsync(_recipe);

    RecipeModel? recipe = null;
    long version = _recipe.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      recipe = await _recipeService.UnpublishAllAsync(_recipe.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      recipe = await _recipeService.UnpublishAsync(_recipe.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      recipe = await _recipeService.UnpublishAsync(_recipe.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(recipe);

    Assert.Equal(_recipe.Entity.Id, recipe.Id);
    Assert.Equal(version, recipe.Version);
    Assert.Equal(_recipe.CreatedBy, recipe.CreatedBy.ToActorId());
    Assert.Equal(_recipe.CreatedOn.AsUniversalTime(), recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipe.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipe.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, recipe.Status);
      Assert.Null(recipe.PublishedVersion);
      Assert.Null(recipe.PublishedBy);
      Assert.Null(recipe.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, recipe.Status);
      Assert.Equal(Actor, recipe.PublishedBy);
      Assert.True(recipe.PublishedOn.HasValue);
    }

    RecipeLocaleModel locale = Assert.Single(recipe.Locales);
    if (unpublishLocale)
    {
      Assert.Equal(ContentStatus.Unpublished, locale.Status);
      Assert.Null(locale.PublishedVersion);
      Assert.Null(locale.PublishedBy);
      Assert.Null(locale.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, locale.Status);
      Assert.Equal(Actor, locale.PublishedBy);
      Assert.True(locale.PublishedOn.HasValue);
    }
  }

  [Fact(DisplayName = "It should read a recipe by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    RecipeModel? recipe = await _recipeService.ReadAsync(_recipe.Entity.Id);
    Assert.NotNull(recipe);
    Assert.Equal(_recipe.Entity.Id, recipe.Id);
  }

  [Fact(DisplayName = "It should replace an existing recipe.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceRecipePayload payload = new()
    {
      Name = " Brochettes de poulet citronnées au BBQ ",
      Notes = "  Les brochettes de poulet citronnées au BBQ sont un plat principal savoureux, parfait pour un repas d’été au barbecue.  "
    };

    CreateOrReplaceRecipeResult result = await _recipeService.CreateOrReplaceAsync(payload, _recipe.Entity.Id);
    Assert.False(result.Created);
    RecipeModel recipe = result.Recipe;

    Assert.Equal(_recipe.Entity.Id, recipe.Id);
    Assert.Equal(_recipe.Version + 1, recipe.Version);
    Assert.Equal(_recipe.CreatedBy, recipe.CreatedBy.ToActorId());
    Assert.Equal(_recipe.CreatedOn.AsUniversalTime(), recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipe.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipe.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), recipe.Name);
    Assert.Equal(payload.Notes.Trim(), recipe.Notes);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    SearchRecipesPayload payload = new();
    SearchResults<RecipeModel> results = await _recipeService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when the recipe does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _recipeService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the recipe does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveRecipeLocalePayload payload = new("Brochettes de poulet citronnées au BBQ");
    Assert.Null(await _recipeService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the recipe does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _recipeService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the recipe does not exist (Update).")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateRecipePayload payload = new();
    Assert.Null(await _recipeService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the recipe does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateRecipeLocalePayload payload = new();
    Assert.Null(await _recipeService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    Assert.Null(await _recipeService.ReadAsync(_recipe.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchRecipesPayload payload = new()
    {
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_recipe.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_recipe.Name.Value[1..^1]}%"));

    SearchResults<RecipeModel> results = await _recipeService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    RecipeModel recipe = Assert.Single(results.Items);
    Assert.Equal(_recipe.Entity.Id, recipe.Id);
  }

  [Theory(DisplayName = "It should save a recipe locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_Recipe_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _recipe.SetLocale(language, new RecipeLocale(_recipe.Name, null, null, null, null), Actor.ToActorId());
      await _recipeRepository.SaveAsync(_recipe);
    }

    SaveRecipeLocalePayload payload = new()
    {
      Name = " Brochettes de poulet citronnées au BBQ ",
      Slug = "brochettes-poulet-citron-bbq",
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "  Les brochettes de poulet citronnées au BBQ sont un plat principal savoureux, parfait pour un repas d’été au barbecue.  ",
      Notes = "    "
    };

    RecipeModel? recipe = await _recipeService.SaveLocaleAsync(_recipe.Entity.Id, language.Code, payload);
    Assert.NotNull(recipe);

    Assert.Equal(_recipe.Entity.Id, recipe.Id);
    Assert.Equal(_recipe.Version + 1, recipe.Version);
    Assert.Equal(_recipe.CreatedOn.AsUniversalTime(), recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_recipe.CreatedBy, recipe.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, recipe.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipe.UpdatedBy);

    RecipeLocaleModel locale = Assert.Single(recipe.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.Name.Trim(), locale.Name);
    Assert.Equal(payload.Slug, locale.Slug);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(recipe.Version, locale.Version);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(locale.CreatedBy, locale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, locale.CreatedOn, TimeSpan.FromSeconds(10));
    if (exists)
    {
      Assert.True(locale.CreatedOn < locale.UpdatedOn);
    }
    else
    {
      Assert.Equal(locale.CreatedOn, locale.UpdatedOn);
    }
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new recipe.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceRecipePayload payload = new("Brochettes de poulet citronnées au BBQ");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateRecipe, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing a recipe.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.PublishAsync(_recipe.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(Recipe.EntityKind, _recipe.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing recipe.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceRecipePayload payload = new("Brochettes de poulet citronnées au BBQ");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.CreateOrReplaceAsync(payload, _recipe.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Recipe.EntityKind, _recipe.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving a recipe locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveRecipeLocalePayload payload = new("Brochettes de poulet citronnées au BBQ");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.SaveLocaleAsync(_recipe.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Recipe.EntityKind, _recipe.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing a recipe.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.UnpublishAsync(_recipe.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(Recipe.EntityKind, _recipe.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing recipe.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateRecipePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.UpdateAsync(_recipe.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Recipe.EntityKind, _recipe.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating a recipe locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateRecipeLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeService.UpdateLocaleAsync(_recipe.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Recipe.EntityKind, _recipe.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the recipe locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateRecipeLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _recipeService.UpdateLocaleAsync(_recipe.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_recipe.Entity, entity);
  }

  [Fact(DisplayName = "It should update an existing recipe.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateRecipePayload payload = new()
    {
      Notes = new Optional<string>("  Les brochettes de poulet citronnées au BBQ sont un plat principal savoureux, parfait pour un repas d’été au barbecue.  ")
    };

    RecipeModel? recipe = await _recipeService.UpdateAsync(_recipe.Entity.Id, payload);
    Assert.NotNull(recipe);

    Assert.Equal(_recipe.Entity.Id, recipe.Id);
    Assert.Equal(_recipe.Version + 1, recipe.Version);
    Assert.Equal(_recipe.CreatedBy, recipe.CreatedBy.ToActorId());
    Assert.Equal(_recipe.CreatedOn.AsUniversalTime(), recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipe.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipe.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_recipe.Name.Value, recipe.Name);
    Assert.Equal(payload.Notes.Value?.Trim(), recipe.Notes);
  }

  [Fact(DisplayName = "It should update a recipe locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _recipe.SetLocale(language, new RecipeLocale(_recipe.Name, null, null, null, null), Actor.ToActorId());
    await _recipeRepository.SaveAsync(_recipe);

    UpdateRecipeLocalePayload payload = new()
    {
      Slug = new Optional<string>("brochettes-poulet-citron-bbq"),
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("  Les brochettes de poulet citronnées au BBQ sont un plat principal savoureux, parfait pour un repas d’été au barbecue.  "),
      Notes = new Optional<string>("    ")
    };

    RecipeModel? recipe = await _recipeService.UpdateLocaleAsync(_recipe.Entity.Id, language.Code, payload);
    Assert.NotNull(recipe);

    Assert.Equal(_recipe.Entity.Id, recipe.Id);
    Assert.Equal(_recipe.Version + 1, recipe.Version);
    Assert.Equal(_recipe.CreatedOn.AsUniversalTime(), recipe.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_recipe.CreatedBy, recipe.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, recipe.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipe.UpdatedBy);

    RecipeLocaleModel locale = Assert.Single(recipe.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(_recipe.Name.Value, locale.Name);
    Assert.Equal(payload.Slug.Value, locale.Slug);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(recipe.Version, locale.Version);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(locale.CreatedBy, locale.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, locale.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.True(locale.CreatedOn < locale.UpdatedOn);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }
}
