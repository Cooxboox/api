using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeCategories.Models;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.RecipeCategories;

[Trait(Traits.Category, Categories.Integration)]
public class RecipeCategoryIntegrationTests : IntegrationTests
{
  private readonly IRecipeCategoryRepository _recipeCategoryRepository;
  private readonly IRecipeCategoryService _recipeCategoryService;

  private RecipeCategory _recipeCategory = null!;

  public RecipeCategoryIntegrationTests() : base()
  {
    _recipeCategoryRepository = ServiceProvider.GetRequiredService<IRecipeCategoryRepository>();
    _recipeCategoryService = ServiceProvider.GetRequiredService<IRecipeCategoryService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _recipeCategory = new RecipeCategoryBuilder(Faker).WithKitchen(Context.Kitchen).Build();
    await _recipeCategoryRepository.SaveAsync(_recipeCategory);
  }

  [Theory(DisplayName = "It should create a new recipe category.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceRecipeCategoryPayload payload = new()
    {
      Name = " BBQ ",
      Notes = "Le BBQ regroupe les recettes de cuisine au barbecue, où les aliments sont cuits à la chaleur directe ou indirecte."
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceRecipeCategoryResult result = await _recipeCategoryService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    RecipeCategoryModel recipeCategory = result.RecipeCategory;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, recipeCategory.Id);
    }
    Assert.Equal(2, recipeCategory.Version);
    Assert.Equal(Actor, recipeCategory.CreatedBy);
    Assert.Equal(DateTime.UtcNow, recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(recipeCategory.CreatedBy, recipeCategory.UpdatedBy);
    Assert.True(recipeCategory.CreatedOn < recipeCategory.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), recipeCategory.Name);
    Assert.Equal(payload.Notes.Trim(), recipeCategory.Notes);
  }

  [Theory(DisplayName = "It should publish an recipe category.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_RecipeCategory_When_Publish_Then_Published(bool publishInvariant, bool publishLocale)
  {
    Language language = Faker.Language();
    _recipeCategory.SetLocale(language, new RecipeCategoryLocale(_recipeCategory.Name, null, null, null, null), Actor.ToActorId());
    await _recipeCategoryRepository.SaveAsync(_recipeCategory);

    RecipeCategoryModel? recipeCategory = null;
    long version = _recipeCategory.Version;
    if (publishInvariant && publishLocale)
    {
      recipeCategory = await _recipeCategoryService.PublishAllAsync(_recipeCategory.Entity.Id);
      version += 2;
    }
    else if (publishInvariant)
    {
      recipeCategory = await _recipeCategoryService.PublishAsync(_recipeCategory.Entity.Id);
      version++;
    }
    else if (publishLocale)
    {
      recipeCategory = await _recipeCategoryService.PublishAsync(_recipeCategory.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(recipeCategory);

    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
    Assert.Equal(version, recipeCategory.Version);
    Assert.Equal(_recipeCategory.CreatedBy, recipeCategory.CreatedBy.ToActorId());
    Assert.Equal(_recipeCategory.CreatedOn.AsUniversalTime(), recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    if (publishInvariant)
    {
      Assert.Equal(ContentStatus.Latest, recipeCategory.Status);
      Assert.Equal(recipeCategory.Version - (publishLocale ? 1 : 0), recipeCategory.PublishedVersion);
      Assert.Equal(Actor, recipeCategory.PublishedBy);
      Assert.True(recipeCategory.PublishedOn.HasValue);
      Assert.Equal(DateTime.UtcNow, recipeCategory.PublishedOn.Value, TimeSpan.FromSeconds(10));
    }
    else
    {
      Assert.Equal(ContentStatus.Unpublished, recipeCategory.Status);
      Assert.Null(recipeCategory.PublishedVersion);
      Assert.Null(recipeCategory.PublishedBy);
      Assert.Null(recipeCategory.PublishedOn);
    }

    RecipeCategoryLocaleModel locale = Assert.Single(recipeCategory.Locales);
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

  [Theory(DisplayName = "It should unpublish an recipe category.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_RecipeCategory_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _recipeCategory.SetLocale(language, new RecipeCategoryLocale(_recipeCategory.Name, null, null, null, null), Actor.ToActorId());
    _recipeCategory.Publish(Actor.ToActorId());
    await _recipeCategoryRepository.SaveAsync(_recipeCategory);

    RecipeCategoryModel? recipeCategory = null;
    long version = _recipeCategory.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      recipeCategory = await _recipeCategoryService.UnpublishAllAsync(_recipeCategory.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      recipeCategory = await _recipeCategoryService.UnpublishAsync(_recipeCategory.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      recipeCategory = await _recipeCategoryService.UnpublishAsync(_recipeCategory.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(recipeCategory);

    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
    Assert.Equal(version, recipeCategory.Version);
    Assert.Equal(_recipeCategory.CreatedBy, recipeCategory.CreatedBy.ToActorId());
    Assert.Equal(_recipeCategory.CreatedOn.AsUniversalTime(), recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, recipeCategory.Status);
      Assert.Null(recipeCategory.PublishedVersion);
      Assert.Null(recipeCategory.PublishedBy);
      Assert.Null(recipeCategory.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, recipeCategory.Status);
      Assert.Equal(Actor, recipeCategory.PublishedBy);
      Assert.True(recipeCategory.PublishedOn.HasValue);
    }

    RecipeCategoryLocaleModel locale = Assert.Single(recipeCategory.Locales);
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

  [Fact(DisplayName = "It should read an recipe category by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.ReadAsync(_recipeCategory.Entity.Id);
    Assert.NotNull(recipeCategory);
    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
  }

  [Fact(DisplayName = "It should replace an existing recipe category.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceRecipeCategoryPayload payload = new()
    {
      Name = " BBQ ",
      Notes = "Le BBQ regroupe les recettes de cuisine au barbecue, où les aliments sont cuits à la chaleur directe ou indirecte."
    };

    CreateOrReplaceRecipeCategoryResult result = await _recipeCategoryService.CreateOrReplaceAsync(payload, _recipeCategory.Entity.Id);
    Assert.False(result.Created);
    RecipeCategoryModel recipeCategory = result.RecipeCategory;

    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
    Assert.Equal(_recipeCategory.Version + 1, recipeCategory.Version);
    Assert.Equal(_recipeCategory.CreatedBy, recipeCategory.CreatedBy.ToActorId());
    Assert.Equal(_recipeCategory.CreatedOn.AsUniversalTime(), recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), recipeCategory.Name);
    Assert.Equal(payload.Notes.Trim(), recipeCategory.Notes);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    SearchRecipeCategoriesPayload payload = new();
    SearchResults<RecipeCategoryModel> results = await _recipeCategoryService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when the recipe category does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _recipeCategoryService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the recipe category does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveRecipeCategoryLocalePayload payload = new("BBQ");
    Assert.Null(await _recipeCategoryService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the recipe category does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _recipeCategoryService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the recipe category does not exist (Update).")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateRecipeCategoryPayload payload = new();
    Assert.Null(await _recipeCategoryService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the recipe category does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateRecipeCategoryLocalePayload payload = new();
    Assert.Null(await _recipeCategoryService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    Assert.Null(await _recipeCategoryService.ReadAsync(_recipeCategory.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchRecipeCategoriesPayload payload = new()
    {
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_recipeCategory.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_recipeCategory.Name.Value[1..^1]}%"));

    SearchResults<RecipeCategoryModel> results = await _recipeCategoryService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    RecipeCategoryModel recipeCategory = Assert.Single(results.Items);
    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
  }

  [Theory(DisplayName = "It should save an recipe category locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_RecipeCategory_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _recipeCategory.SetLocale(language, new RecipeCategoryLocale(_recipeCategory.Name, null, null, null, null), Actor.ToActorId());
      await _recipeCategoryRepository.SaveAsync(_recipeCategory);
    }

    SaveRecipeCategoryLocalePayload payload = new()
    {
      Name = " BBQ ",
      Slug = "bbq",
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "Le BBQ regroupe les recettes de cuisine au barbecue, où les aliments sont cuits à la chaleur directe ou indirecte.",
      Notes = "    "
    };

    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.SaveLocaleAsync(_recipeCategory.Entity.Id, language.Code, payload);
    Assert.NotNull(recipeCategory);

    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
    Assert.Equal(_recipeCategory.Version + 1, recipeCategory.Version);
    Assert.Equal(_recipeCategory.CreatedOn.AsUniversalTime(), recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_recipeCategory.CreatedBy, recipeCategory.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, recipeCategory.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeCategory.UpdatedBy);

    RecipeCategoryLocaleModel locale = Assert.Single(recipeCategory.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.Name.Trim(), locale.Name);
    Assert.Equal(payload.Slug, locale.Slug);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(recipeCategory.Version, locale.Version);
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

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new recipe category.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceRecipeCategoryPayload payload = new("BBQ");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateRecipeCategory, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing an recipe category.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.PublishAsync(_recipeCategory.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(RecipeCategory.EntityKind, _recipeCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing recipe category.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceRecipeCategoryPayload payload = new("BBQ");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.CreateOrReplaceAsync(payload, _recipeCategory.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeCategory.EntityKind, _recipeCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving an recipe category locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveRecipeCategoryLocalePayload payload = new("BBQ");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.SaveLocaleAsync(_recipeCategory.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeCategory.EntityKind, _recipeCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing an recipe category.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.UnpublishAsync(_recipeCategory.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(RecipeCategory.EntityKind, _recipeCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing recipe category.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateRecipeCategoryPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.UpdateAsync(_recipeCategory.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeCategory.EntityKind, _recipeCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an recipe category locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateRecipeCategoryLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _recipeCategoryService.UpdateLocaleAsync(_recipeCategory.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(RecipeCategory.EntityKind, _recipeCategory.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the recipe category locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateRecipeCategoryLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _recipeCategoryService.UpdateLocaleAsync(_recipeCategory.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_recipeCategory.Entity, entity);
  }

  [Fact(DisplayName = "It should update an existing recipe category.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateRecipeCategoryPayload payload = new()
    {
      Notes = new Optional<string>("Le BBQ regroupe les recettes de cuisine au barbecue, où les aliments sont cuits à la chaleur directe ou indirecte.")
    };

    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.UpdateAsync(_recipeCategory.Entity.Id, payload);
    Assert.NotNull(recipeCategory);

    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
    Assert.Equal(_recipeCategory.Version + 1, recipeCategory.Version);
    Assert.Equal(_recipeCategory.CreatedBy, recipeCategory.CreatedBy.ToActorId());
    Assert.Equal(_recipeCategory.CreatedOn.AsUniversalTime(), recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeCategory.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, recipeCategory.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_recipeCategory.Name.Value, recipeCategory.Name);
    Assert.Equal(payload.Notes.Value?.Trim(), recipeCategory.Notes);
  }

  [Fact(DisplayName = "It should update an recipe category locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _recipeCategory.SetLocale(language, new RecipeCategoryLocale(_recipeCategory.Name, null, null, null, null), Actor.ToActorId());
    await _recipeCategoryRepository.SaveAsync(_recipeCategory);

    UpdateRecipeCategoryLocalePayload payload = new()
    {
      Slug = new Optional<string>("bbq"),
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("Le BBQ regroupe les recettes de cuisine au barbecue, où les aliments sont cuits à la chaleur directe ou indirecte."),
      Notes = new Optional<string>("    ")
    };

    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.UpdateLocaleAsync(_recipeCategory.Entity.Id, language.Code, payload);
    Assert.NotNull(recipeCategory);

    Assert.Equal(_recipeCategory.Entity.Id, recipeCategory.Id);
    Assert.Equal(_recipeCategory.Version + 1, recipeCategory.Version);
    Assert.Equal(_recipeCategory.CreatedOn.AsUniversalTime(), recipeCategory.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_recipeCategory.CreatedBy, recipeCategory.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, recipeCategory.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, recipeCategory.UpdatedBy);

    RecipeCategoryLocaleModel locale = Assert.Single(recipeCategory.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(_recipeCategory.Name.Value, locale.Name);
    Assert.Equal(payload.Slug.Value, locale.Slug);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(recipeCategory.Version, locale.Version);
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
