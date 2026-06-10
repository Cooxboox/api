using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Search;
using Logitar;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Ingredients;

[Trait(Traits.Category, Categories.Integration)]
public class IngredientIntegrationTests : IntegrationTests
{
  private readonly IIngredientRepository _ingredientRepository;
  private readonly IIngredientService _ingredientService;

  private Ingredient _ingredient = null!;

  public IngredientIntegrationTests() : base()
  {
    _ingredientRepository = ServiceProvider.GetRequiredService<IIngredientRepository>();
    _ingredientService = ServiceProvider.GetRequiredService<IIngredientService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _ingredient = new IngredientBuilder(Faker).WithKitchen(Context.Kitchen).Build();
    await _ingredientRepository.SaveAsync(_ingredient);
  }

  [Theory(DisplayName = "It should create a new ingredient.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_NotExists_When_CreateOrReplace_Then_Created(bool withId)
  {
    CreateOrReplaceIngredientPayload payload = new()
    {
      Name = " Citron ",
      Notes = "  Le citron est un agrume acide, très utilisé en cuisine pour son jus et son zeste.  "
    };
    Guid? id = withId ? Guid.NewGuid() : null;

    CreateOrReplaceIngredientResult result = await _ingredientService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);
    IngredientModel ingredient = result.Ingredient;

    if (id.HasValue)
    {
      Assert.Equal(id.Value, ingredient.Id);
    }
    Assert.Equal(2, ingredient.Version);
    Assert.Equal(Actor, ingredient.CreatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(ingredient.CreatedBy, ingredient.UpdatedBy);
    Assert.True(ingredient.CreatedOn < ingredient.UpdatedOn);

    Assert.Equal(payload.Name.Trim(), ingredient.Name);
    Assert.Equal(payload.Notes.Trim(), ingredient.Notes);
  }

  [Fact(DisplayName = "It should publish an ingredient.")]
  public async Task Given_Ingredient_When_PublishAll_Then_AllPublished()
  {
    Language language = Faker.Language();
    _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientRepository.SaveAsync(_ingredient);

    IngredientModel? ingredient = await _ingredientService.PublishAllAsync(_ingredient.Entity.Id);
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 2, ingredient.Version);
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredient.Status);
    Assert.Equal(ingredient.Version - 1, ingredient.PublishedVersion);
    Assert.Equal(Actor, ingredient.PublishedBy);
    Assert.True(ingredient.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredient.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientLocaleModel locale = Assert.Single(ingredient.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should publish an ingredient invariant.")]
  public async Task Given_Ingredient_When_PublishInvariant_Then_InvariantPublished()
  {
    Language language = Faker.Language();
    _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientRepository.SaveAsync(_ingredient);

    IngredientModel? ingredient = await _ingredientService.PublishAsync(_ingredient.Entity.Id);
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 1, ingredient.Version);
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredient.Status);
    Assert.Equal(ingredient.Version, ingredient.PublishedVersion);
    Assert.Equal(Actor, ingredient.PublishedBy);
    Assert.True(ingredient.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredient.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientLocaleModel locale = Assert.Single(ingredient.Locales);
    Assert.Equal(ContentStatus.Unpublished, locale.Status);
    Assert.Null(locale.PublishedVersion);
    Assert.Null(locale.PublishedBy);
    Assert.Null(locale.PublishedOn);
  }

  [Fact(DisplayName = "It should publish an ingredient locale.")]
  public async Task Given_Ingredient_When_PublishLocale_Then_LocalePublished()
  {
    Language language = Faker.Language();
    _ingredient.PublishInvariant(Actor.ToActorId());
    _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientRepository.SaveAsync(_ingredient);

    IngredientModel? ingredient = await _ingredientService.PublishAsync(_ingredient.Entity.Id, language.Code);
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 1, ingredient.Version);
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(ContentStatus.Latest, ingredient.Status);
    Assert.Equal(ingredient.Version - 2, ingredient.PublishedVersion);
    Assert.Equal(Actor, ingredient.PublishedBy);
    Assert.True(ingredient.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, ingredient.PublishedOn.Value, TimeSpan.FromSeconds(10));

    IngredientLocaleModel locale = Assert.Single(ingredient.Locales);
    Assert.Equal(ContentStatus.Latest, locale.Status);
    Assert.Equal(locale.Version, locale.PublishedVersion);
    Assert.Equal(Actor, locale.PublishedBy);
    Assert.True(locale.PublishedOn.HasValue);
    Assert.Equal(DateTime.UtcNow, locale.PublishedOn.Value, TimeSpan.FromSeconds(10));
  }

  [Theory(DisplayName = "It should unpublish an ingredient.")]
  [InlineData(true, false)]
  [InlineData(false, true)]
  [InlineData(true, true)]
  public async Task Given_Ingredient_When_Unpublish_Then_Unpublished(bool unpublishInvariant, bool unpublishLocale)
  {
    Language language = Faker.Language();
    _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
    _ingredient.Publish(Actor.ToActorId());
    await _ingredientRepository.SaveAsync(_ingredient);

    IngredientModel? ingredient = null;
    long version = _ingredient.Version;
    if (unpublishInvariant && unpublishLocale)
    {
      ingredient = await _ingredientService.UnpublishAllAsync(_ingredient.Entity.Id);
      version += 2;
    }
    else if (unpublishInvariant)
    {
      ingredient = await _ingredientService.UnpublishAsync(_ingredient.Entity.Id);
      version++;
    }
    else if (unpublishLocale)
    {
      ingredient = await _ingredientService.UnpublishAsync(_ingredient.Entity.Id, language.Code);
      version++;
    }
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(version, ingredient.Version);
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));

    if (unpublishInvariant)
    {
      Assert.Equal(ContentStatus.Unpublished, ingredient.Status);
      Assert.Null(ingredient.PublishedVersion);
      Assert.Null(ingredient.PublishedBy);
      Assert.Null(ingredient.PublishedOn);
    }
    else
    {
      Assert.Equal(ContentStatus.Latest, ingredient.Status);
      Assert.Equal(Actor, ingredient.PublishedBy);
      Assert.True(ingredient.PublishedOn.HasValue);
    }

    IngredientLocaleModel locale = Assert.Single(ingredient.Locales);
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

  [Fact(DisplayName = "It should read an ingredient by ID.")]
  public async Task Given_Id_When_Read_Then_Read()
  {
    IngredientModel? ingredient = await _ingredientService.ReadAsync(_ingredient.Entity.Id);
    Assert.NotNull(ingredient);
    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
  }

  [Fact(DisplayName = "It should replace an existing ingredient.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceIngredientPayload payload = new()
    {
      Name = " Citron ",
      Notes = "  Le citron est un agrume acide, très utilisé en cuisine pour son jus et son zeste.  "
    };

    CreateOrReplaceIngredientResult result = await _ingredientService.CreateOrReplaceAsync(payload, _ingredient.Entity.Id);
    Assert.False(result.Created);
    IngredientModel ingredient = result.Ingredient;

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 1, ingredient.Version);
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Name.Trim(), ingredient.Name);
    Assert.Equal(payload.Notes.Trim(), ingredient.Notes);
  }

  [Fact(DisplayName = "It should return empty search results.")]
  public async Task Given_NotMatching_When_Search_Then_EmptyResults()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    SearchIngredientsPayload payload = new();
    SearchResults<IngredientModel> results = await _ingredientService.SearchAsync(payload);
    Assert.Equal(0, results.Total);
    Assert.Empty(results.Items);
  }

  [Fact(DisplayName = "It should return null when the ingredient does not exist (Publish).")]
  public async Task Given_NotExist_When_Publish_Then_NullReturned()
  {
    Assert.Null(await _ingredientService.PublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the ingredient does not exist (SaveLocale).")]
  public async Task Given_NotExist_When_SaveLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    SaveIngredientLocalePayload payload = new("Citron");
    Assert.Null(await _ingredientService.SaveLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the ingredient does not exist (Unpublish).")]
  public async Task Given_NotExist_When_Unpublish_Then_NullReturned()
  {
    Assert.Null(await _ingredientService.UnpublishAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return null when the ingredient does not exist (Update).")]
  public async Task Given_NotExist_When_Update_Then_NullReturned()
  {
    UpdateIngredientPayload payload = new();
    Assert.Null(await _ingredientService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "It should return null when the ingredient does not exist (UpdateLocale).")]
  public async Task Given_NotExist_When_UpdateLocale_Then_NullReturned()
  {
    Language language = Faker.Language();
    UpdateIngredientLocalePayload payload = new();
    Assert.Null(await _ingredientService.UpdateLocaleAsync(Guid.Empty, language.Code, payload));
  }

  [Fact(DisplayName = "It should return null when the user does not own the kitchen.")]
  public async Task Given_NotOwner_When_Read_Then_NullReturned()
  {
    Context.Kitchen = new KitchenBuilder().Build();

    Assert.Null(await _ingredientService.ReadAsync(_ingredient.Entity.Id));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Matching_When_Search_Then_CorrectResults()
  {
    SearchIngredientsPayload payload = new()
    {
      Skip = 0,
      Limit = 1
    };
    payload.Ids.Add(_ingredient.Entity.Id);
    payload.Search.Terms.Add(new SearchTerm($"%{_ingredient.Name.Value[1..^1]}%"));

    SearchResults<IngredientModel> results = await _ingredientService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    IngredientModel ingredient = Assert.Single(results.Items);
    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
  }

  [Theory(DisplayName = "It should save an ingredient locale.")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_Ingredient_When_SaveLocale_Then_Saved(bool exists)
  {
    Language language = Faker.Language();
    if (exists)
    {
      _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
      await _ingredientRepository.SaveAsync(_ingredient);
    }

    SaveIngredientLocalePayload payload = new()
    {
      Name = " Citron ",
      Slug = "citron",
      MetaDescription = "   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   ",
      HtmlContent = "  Le citron est un agrume acide, très utilisé en cuisine pour son jus et son zeste.  ",
      Notes = "    "
    };

    IngredientModel? ingredient = await _ingredientService.SaveLocaleAsync(_ingredient.Entity.Id, language.Code, payload);
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 1, ingredient.Version);
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);

    IngredientLocaleModel locale = Assert.Single(ingredient.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(payload.Name.Trim(), locale.Name);
    Assert.Equal(payload.Slug, locale.Slug);
    Assert.Equal(payload.MetaDescription.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(ingredient.Version, locale.Version);
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

  [Fact(DisplayName = "It should throw InvariantNotPublishedException when the invariant is not published.")]
  public async Task Given_UnpublishedInvariant_When_PublishLocale_Then_InvariantNotPublishedException()
  {
    Language language = Faker.Language();
    _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientRepository.SaveAsync(_ingredient);

    var exception = await Assert.ThrowsAsync<InvariantNotPublishedException>(async () => await _ingredientService.PublishAsync(_ingredient.Entity.Id, language.Code));
    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_ingredient.Entity, entity);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when creating a new ingredient.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientPayload payload = new("Citron");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.CreateOrReplaceAsync(payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.CreateIngredient, exception.Action);
    Assert.Null(exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when publishing an ingredient.")]
  public async Task Given_Exists_When_Publishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.PublishAsync(_ingredient.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Publish, exception.Action);
    Assert.Equal(new Entity(Ingredient.EntityKind, _ingredient.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when replacing an existing ingredient.")]
  public async Task Given_Exists_When_CreateOrReplace_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    CreateOrReplaceIngredientPayload payload = new("Citron");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.CreateOrReplaceAsync(payload, _ingredient.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Ingredient.EntityKind, _ingredient.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when saving an ingredient locale.")]
  public async Task Given_Exists_When_SaveLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    SaveIngredientLocalePayload payload = new("Citron");

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.SaveLocaleAsync(_ingredient.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Ingredient.EntityKind, _ingredient.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when unpublishing an ingredient.")]
  public async Task Given_Exists_When_Unpublishing_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.UnpublishAsync(_ingredient.Entity.Id));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Unpublish, exception.Action);
    Assert.Equal(new Entity(Ingredient.EntityKind, _ingredient.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an existing ingredient.")]
  public async Task Given_Exists_When_Update_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    UpdateIngredientPayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.UpdateAsync(_ingredient.Entity.Id, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Ingredient.EntityKind, _ingredient.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw PermissionDeniedException when updating an ingredient locale.")]
  public async Task Given_Exists_When_UpdateLocale_Then_PermissionDeniedException()
  {
    Context.User = new UserBuilder().Build();

    Language language = Faker.Language();
    UpdateIngredientLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _ingredientService.UpdateLocaleAsync(_ingredient.Entity.Id, language.Code, payload));
    Assert.Equal(Actor.ToActorId().Value, exception.ActorId);
    Assert.Equal(Actions.Update, exception.Action);
    Assert.Equal(new Entity(Ingredient.EntityKind, _ingredient.Entity.Id, Context.Kitchen?.Id).ToString(), exception.Resource);
  }

  [Fact(DisplayName = "It should throw LocaleNotFoundException when the ingredient locale was not found.")]
  public async Task Given_LocaleNotFound_When_UpdateLocale_Then_LocaleNotFoundException()
  {
    Language language = Faker.Language();
    UpdateIngredientLocalePayload payload = new();

    var exception = await Assert.ThrowsAsync<LocaleNotFoundException>(async () => await _ingredientService.UpdateLocaleAsync(_ingredient.Entity.Id, language.Code, payload));
    Assert.Equal(language.ToString(), exception.Language);

    Entity entity = new(exception.EntityKind, exception.EntityId, exception.KitchenId.HasValue ? new KitchenId(exception.KitchenId.Value) : null);
    Assert.Equal(_ingredient.Entity, entity);
  }

  [Fact(DisplayName = "It should update an existing ingredient.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateIngredientPayload payload = new()
    {
      Notes = new Optional<string>("  Le citron est un agrume acide, très utilisé en cuisine pour son jus et son zeste.  ")
    };

    IngredientModel? ingredient = await _ingredientService.UpdateAsync(_ingredient.Entity.Id, payload);
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 1, ingredient.Version);
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(_ingredient.Name.Value, ingredient.Name);
    Assert.Equal(payload.Notes.Value?.Trim(), ingredient.Notes);
  }

  [Fact(DisplayName = "It should update an ingredient locale.")]
  public async Task Given_LocaleFound_When_UpdateLocale_Then_Updated()
  {
    Language language = Faker.Language();
    _ingredient.SetLocale(language, new IngredientLocale(_ingredient.Name, null, null, null, null), Actor.ToActorId());
    await _ingredientRepository.SaveAsync(_ingredient);

    UpdateIngredientLocalePayload payload = new()
    {
      Slug = new Optional<string>("citron"),
      MetaDescription = new Optional<string>("   Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse sit amet velit venenatis, placerat lacus non, scelerisque metus. Vestibulum a ut.   "),
      HtmlContent = new Optional<string>("  Le citron est un agrume acide, très utilisé en cuisine pour son jus et son zeste.  "),
      Notes = new Optional<string>("    ")
    };

    IngredientModel? ingredient = await _ingredientService.UpdateLocaleAsync(_ingredient.Entity.Id, language.Code, payload);
    Assert.NotNull(ingredient);

    Assert.Equal(_ingredient.Entity.Id, ingredient.Id);
    Assert.Equal(_ingredient.Version + 1, ingredient.Version);
    Assert.Equal(_ingredient.CreatedOn.AsUniversalTime(), ingredient.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(_ingredient.CreatedBy, ingredient.CreatedBy.ToActorId());
    Assert.Equal(DateTime.UtcNow, ingredient.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, ingredient.UpdatedBy);

    IngredientLocaleModel locale = Assert.Single(ingredient.Locales);
    Assert.Equal(language.Code, locale.Language.Code);
    Assert.Equal(_ingredient.Name.Value, locale.Name);
    Assert.Equal(payload.Slug.Value, locale.Slug);
    Assert.Equal(payload.MetaDescription.Value?.Trim(), locale.MetaDescription);
    Assert.Equal(payload.HtmlContent.Value?.Trim(), locale.HtmlContent);
    Assert.Null(locale.Notes);

    Assert.Equal(ingredient.Version, locale.Version);
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
